using System;

namespace FlightDataHandler.Models
{
    public class FlightInfo
    {
        public string Id { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public double AltitudeFt { get; set; }

        public int Track { get; set; }

        public double SpeedKts { get; set; }

        public double Squawk { get; set; }

        public double Radar { get; set; }

        public string Registration { get; set; }

        public string Unk { get; set; }

        public string Source { get; set; }

        public string Destination { get; set; }

        public string Model { get; set; }

        public string Number { get; set; }

        public string X { get; set; }

        public string Y { get; set; }

        public string Num { get; set; }

        public double DistanceToUserInKm { get; set; }

        public double AltitudeM { get { return AltitudeFt * 0.3048; } }

        public double SpeedKmh { get { return SpeedKts * 1.852; } }
    }
}