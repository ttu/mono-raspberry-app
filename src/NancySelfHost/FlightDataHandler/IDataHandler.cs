using FlightDataHandler.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlightDataHandler
{
    public interface IDataHandler
    {
        Task<List<FlightInfo>> RequestFlights(double latitude, double longitude, double elevation);

        Task<List<FlightInfo>> RequestFlights();

        void Subscribe(string clientId);

        void UnSubscribe(string clientId);

        void SetClientLocation(string clientId, double latitude, double longitude, double elevation);

        event EventHandler<Tuple<string, List<FlightInfo>>> UpdatedClientData;
    }
}