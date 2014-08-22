using FlightDataHandler.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

#pragma warning disable 4014

namespace FlightDataHandler
{
    public class DataHandler : IDataHandler
    {
        private const int MAX_DISTANCE_KM = 50;

        private System.Timers.Timer _stopTimer;
        private System.Timers.Timer _downloadTimer;

        private List<FlightInfo> _allFlights;
        private DataDownloader _loader;

        // TODO: Change to ConcurrentCollection
        private List<ClientInfo> _clients;

        private object _flightLock = new object();

        private AutoResetEvent _are = new AutoResetEvent(false);

        private Task _updateSender;
        private bool _sendingUpdates;

        public DataHandler()
        {
            _clients = new List<ClientInfo>();

            _loader = new DataDownloader();
            _stopTimer = new System.Timers.Timer();
            _downloadTimer = new System.Timers.Timer();

            _stopTimer.Interval = 30000;
            _stopTimer.Elapsed += _stopTimer_Elapsed;

            _downloadTimer.Interval = 10000;
            _downloadTimer.Elapsed += _fetchTimer_Elapsed;

            _updateSender = Task.Factory.StartNew(ClientUpdateAction);

            FetchActionAsync();
        }

        public event EventHandler<Tuple<string, List<FlightInfo>>> UpdatedClientData;

        public async Task<List<FlightInfo>> RequestFlights(double latitude, double longitude, double elevation)
        {
            var data = await GetFlightListWhenReady();

            CheckStopTimerReset();

            var flights = GetFlightsInsideDistance(data, latitude, longitude, MAX_DISTANCE_KM);

            AddUserDistanceToFlights(flights, latitude, longitude, elevation);

            return flights;
        }

        public async Task<List<FlightInfo>> RequestFlights()
        {
            var data = await GetFlightListWhenReady();

            CheckStopTimerReset();

            return data.ToList();
        }

        public void Subscribe(string clientId)
        {
            Log(string.Format("Client subscribed: {0}", clientId));

            if (_clients.Any(c => c.Id == clientId))
                return;

            _clients.Add(new ClientInfo { Id = clientId });

            lock (_stopTimer)
            {
                _stopTimer.Stop();
            }

            CheckDownloadTimer();
        }

        public void UnSubscribe(string clientId)
        {
            Log(string.Format("Client unsubscribed: {0}", clientId));

            if (!_clients.Any(c => c.Id == clientId))
                return;

            _clients.Remove(_clients.Single(c => c.Id == clientId));

            CheckStopTimerReset();
        }

        public void SetClientLocation(string clientId, double latitude, double longitude, double elevation)
        {
            var client = _clients.Single(c => c.Id == clientId);
            client.Latitude = latitude;
            client.Longitude = longitude;
            client.Elevation = elevation;
        }

        private async Task<List<FlightInfo>> GetFlightListWhenReady()
        {
            while (_allFlights == null)
            {
                CheckDownloadTimer();
                await Task.Delay(50);
            }

            return _allFlights;
        }

        private void CheckDownloadTimer()
        {
            lock (_downloadTimer)
            {
                if (!_downloadTimer.Enabled)
                    _downloadTimer.Start();
            }
        }

        private void CheckStopTimerReset()
        {
            if (_clients.Any())
                return;

            lock (_stopTimer)
            {
                _stopTimer.Stop();
                _stopTimer.Start();
            }
        }

        private async Task FetchActionAsync()
        {
            if (_sendingUpdates) // No need to process new ones as last ones are still on way
                return;

            var sw = Stopwatch.StartNew();

            var data = await _loader.GetData();
            var newData = DataParser.Parse(data);

            Log(string.Format("Data processed: {0}ms", sw.ElapsedMilliseconds.ToString()));

            lock (_flightLock)
                _allFlights = newData;

            _are.Set();
        }

        private void AddUserDistanceToFlights(List<FlightInfo> flights, double latitude, double longitude, double altitude)
        {
            var userPoint = new GeoPoint(latitude, longitude, altitude);

            foreach (var flight in flights)
            {
                var fp = new GeoPoint(flight.Latitude, flight.Longitude, flight.AltitudeM);

                flight.DistanceToUserInKm = MathHelper.CalculateDistanceInKm(userPoint, fp);
            }
        }

        private List<FlightInfo> GetFlightsInsideDistance(IList<FlightInfo> flights, double latitude, double longitude, int distanceKm)
        {
            var retVal = new List<FlightInfo>();

            flights.ToList().ForEach(f =>
            {
                if (MathHelper.Distance(f.Latitude, f.Longitude, latitude, longitude, 'K') < distanceKm)
                {
                    retVal.Add(f);
                }
            });

            return retVal;
        }

        private void ClientUpdateAction()
        {
            while (true)
            {
                _are.WaitOne();

                var sw = Stopwatch.StartNew();

                _sendingUpdates = true;

                // TODO: Make sure that no one changes flights after this
                var flightsToUse = _allFlights.ToList();

                Parallel.ForEach(_clients, c =>
                {
                    var flights = GetFlightsInsideDistance(flightsToUse, c.Latitude, c.Longitude, MAX_DISTANCE_KM);

                    AddUserDistanceToFlights(flights, c.Latitude, c.Longitude, c.Elevation);

                    if (UpdatedClientData != null)
                        UpdatedClientData(this, Tuple.Create(c.Id, flights));
                });

                _sendingUpdates = false;

                Log(string.Format("Data sent: {0}ms", sw.ElapsedMilliseconds.ToString()));
            }
        }

        private async void _fetchTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Debug.WriteLine("Download timer elapsed");

            if (_loader.IsBusy)
                return;

            await FetchActionAsync();
        }

        private void _stopTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Log("Download stopped");

            _stopTimer.Stop();
            _downloadTimer.Stop();

            lock (_flightLock)
                _allFlights = null;
        }

        private void Log(string message)
        {
            Trace.WriteLine(message);
        }

        private class ClientInfo
        {
            public string Id { get; set; }

            public double Latitude { get; set; }

            public double Longitude { get; set; }

            public double Elevation { get; set; }

            public ClientInfo Clone()
            {
                return new ClientInfo { Id = this.Id, Latitude = this.Latitude, Longitude = this.Longitude, Elevation = this.Elevation };
            }
        }
    }
}