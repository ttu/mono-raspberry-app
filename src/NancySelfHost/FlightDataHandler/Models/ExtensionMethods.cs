using System;
using System.Collections.Generic;
using System.Globalization;

namespace FlightDataHandler.Models
{
    public static class ExtensionMethods
    {
        public static readonly CultureInfo _ci = new CultureInfo("en-US");

        public static void Parse(this FlightInfo info, string[] data)
        {
            info.Registration = data[13];
            info.Latitude = System.Convert.ToDouble(data[1], _ci);
            info.Longitude = System.Convert.ToDouble(data[2], _ci);
            info.Track = System.Convert.ToInt32(data[3], _ci);
            info.AltitudeFt = System.Convert.ToDouble(data[4], _ci);
            info.SpeedKts = System.Convert.ToDouble(data[5], _ci);
            info.Source = data[11];
            info.Destination = data[12];
            info.Model = data[8];
        }

        public static FlightInfoDTO MapToDTO(this FlightInfo info)
        {
            return new FlightInfoDTO
            {
                Registration = info.Registration,
                AltitudeM = Math.Round(info.AltitudeM, 2),
                Latitude = info.Latitude,
                Longitude = info.Longitude,
                SpeedKmh = Math.Round(info.SpeedKmh, 0),
                Source = info.Source,
                Destination = info.Destination,
                DistanceToUserKm = Math.Round(info.DistanceToUserInKm, 3),
                Model = info.Model
            };
        }

        public static IList<FlightInfoDTO> MapToDTO(this List<FlightInfo> flights)
        {
            var dtos = new List<FlightInfoDTO>();
            flights.ForEach(i => dtos.Add(i.MapToDTO()));
            return dtos;
        }
    }
}