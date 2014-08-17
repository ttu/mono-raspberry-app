using FlightDataHandler;
using FlightDataHandler.Models;
using Nancy;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace NancySelfHost
{
    public class FlightModule : NancyModule
    {
        public FlightModule(IDataHandler dataHandler)
        {
            Get["/flights"] = _ => View["flights"];

            Get["/allFlights", true] = async (parameters, ct) =>
            {
                List<FlightInfo> flights = await dataHandler.RequestFlights();
                var json = JsonConvert.SerializeObject(flights.MapToDTO());
                return json;
            };

            Get["/flights/{lat}/{lon}/{alt}", true] = async (parameters, ct) =>
            {
                var latitude = parameters["lat"];
                var longitude = parameters["lon"];
                var altitude = parameters["alt"];

                List<FlightInfo> flights = await dataHandler.RequestFlights(latitude, longitude, altitude);
                var json = JsonConvert.SerializeObject(flights.MapToDTO());
                return json;
            };
        }
    }
}