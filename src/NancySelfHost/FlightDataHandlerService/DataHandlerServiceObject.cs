using FlightDataHandler;
using FlightDataHandler.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlightDataHandlerService
{
    public class DataHandlerServiceObject : IDataHandler
    {
        private Dealer<DataRequest, List<FlightInfo>> _req;
        private Pull<Tuple<string, List<FlightInfo>>> _pull;
        private Push<PushMessage> _push;

        public DataHandlerServiceObject()
        {
            ProtoBufHelper.BasicPrepare();

            _req = new Dealer<DataRequest, List<FlightInfo>>("tcp://127.0.0.1:5656");
            _pull = new Pull<Tuple<string, List<FlightInfo>>>("tcp://127.0.0.1:5657", PullAction);
            _push = new Push<PushMessage>("tcp://127.0.0.1:5658");

            Task.Factory.StartNew(_pull.Start);
        }

        public event EventHandler<Tuple<string, List<FlightInfo>>> UpdatedClientData;

        public Task<List<FlightInfo>> RequestFlights(double latitude, double longitude, double elevation)
        {
            return Task.Factory.StartNew<List<FlightInfo>>(() =>
                {
                    var flights = _req.SendRequest(new DataRequest { Method = 1, Value = Tuple.Create(latitude, longitude, elevation) });
                    return flights;
                });
        }

        public Task<List<FlightInfo>> RequestFlights()
        {
            return Task.Factory.StartNew<List<FlightInfo>>(() =>
                {
                    var allFlights = _req.SendRequest(new DataRequest { Method = 0 });
                    return allFlights;
                });
        }

        public void Subscribe(string clientId)
        {
            _push.Send(new PushMessage { Method = 0, ClientId = clientId });
        }

        public void UnSubscribe(string clientId)
        {
            _push.Send(new PushMessage { Method = 1, ClientId = clientId });
        }

        public void SetClientLocation(string clientId, double latitude, double longitude, double elevation)
        {
            _push.Send(new PushMessage { Method = 2, ClientId = clientId, Values = Tuple.Create(latitude, longitude, elevation) });
        }

        private void PullAction(Tuple<string, List<FlightInfo>> flights)
        {
            if (UpdatedClientData != null)
                UpdatedClientData(this, flights);
        }
    }
}