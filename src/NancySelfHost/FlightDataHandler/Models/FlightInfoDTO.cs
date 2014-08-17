namespace FlightDataHandler.Models
{
    public class FlightInfoDTO
    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public double AltitudeM { get; set; }

        public double SpeedKmh { get; set; }

        public string Registration { get; set; }

        public string Source { get; set; }

        public string Destination { get; set; }

        public string Model { get; set; }

        public double DistanceToUserKm { get; set; }
    }
}