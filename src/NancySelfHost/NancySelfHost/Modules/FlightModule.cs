using FlightDataHandler;
using FlightDataHandler.Models;
using Microsoft.AspNet.SignalR;
using Nancy;
using Newtonsoft.Json;
using System;
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

            Get["/flights/{lat:decimal}/{lon:decimal}/{alt:decimal}", true] = async (parameters, ct) =>
            {
                var latitude = parameters["lat"];
                var longitude = parameters["lon"];
                var altitude = parameters["alt"];

                List<FlightInfo> flights = await dataHandler.RequestFlights(latitude, longitude, altitude);
                var json = JsonConvert.SerializeObject(flights.MapToDTO());
                return json;
            };

            dataHandler.UpdatedClientData += Value_UpdatedClientData;
        }

        private void Value_UpdatedClientData(object sender, Tuple<string, List<FlightInfo>> e)
        {
            var ctx = GlobalHost.ConnectionManager.GetHubContext("FlightsHub");
            var client = ctx.Clients.Client(e.Item1);
            client.newData(e.Item2.MapToDTO());
        }
    }
}