using FlightDataHandler.Models;
using Nancy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NancySelfHost
{
    public class FlightModule : NancyModule
    {
        public FlightModule()
        {
            Get["/Flights", true] = async (parameters, ct) =>
            {
                var flights = await Program.DataHandler.Value.RequestFlights();
                var json = JsonConvert.SerializeObject(flights.MapToDTO());
                return json;
            };
        }
    }
}