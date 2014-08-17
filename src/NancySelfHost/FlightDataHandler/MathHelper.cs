using System;

namespace FlightDataHandler
{
    public static class MathHelper
    {
        public static double Distance(double lat1, double lon1, double lat2, double lon2, char unit)
        {
            double theta = lon1 - lon2;

            double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));

            dist = Math.Acos(dist);
            dist = rad2deg(dist);
            dist = dist * 60 * 1.1515;

            if (unit == 'K')
            {
                dist = dist * 1.609344;
            }
            else if (unit == 'N')
            {
                dist = dist * 0.8684;
            }

            return dist;
        }

        private static double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        private static double rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }

        #region cosinekitty.com/compass.html

        public static double EarthRadiusInMeters(double latitudeRadians)
        {
            // http://en.wikipedia.org/wiki/Earth_radius
            var a = 6378137.0;  // equatorial radius in meters
            var b = 6356752.3;  // polar radius in meters
            var cos = Math.Cos(latitudeRadians);
            var sin = Math.Sin(latitudeRadians);
            var t1 = a * a * cos;
            var t2 = b * b * sin;
            var t3 = a * cos;
            var t4 = b * sin;
            return Math.Sqrt((t1 * t1 + t2 * t2) / (t3 * t3 + t4 * t4));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="location">Tuple is latitude, longitude, elevation</param>
        /// <returns></returns>
        public static GeoLoc LocationToPoint(GeoPoint gp)
        {
            // Convert (lat, lon, elv) to (x, y, z).

            var lat = gp.Latitude * Math.PI / 180.0;
            var lon = gp.Longitude * Math.PI / 180.0;
            var radius = gp.ElevationInMeters + EarthRadiusInMeters(lat);
            var cosLon = Math.Cos(lon);
            var sinLon = Math.Sin(lon);
            var cosLat = Math.Cos(lat);
            var sinLat = Math.Sin(lat);
            var x = cosLon * cosLat * radius;
            var y = sinLon * cosLat * radius;
            var z = sinLat * radius;

            return new GeoLoc { Point = new Point3D { X = x, Y = y, Z = z }, Radius = radius };
        }

        public static double Distance(Point3D ap, Point3D bp)
        {
            var dx = ap.X - bp.X;
            var dy = ap.Y - bp.Y;
            var dz = ap.Z - bp.Z;
            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        public static Point3D RotateGlobe(GeoPoint b, GeoPoint a, double bradius, double aradius)
        {
            // Get modified coordinates of 'b' by rotating the globe so that 'a' is at lat=0, lon=0.
            var br = new GeoPoint { Latitude = b.Latitude, Longitude = (b.Longitude - a.Longitude), ElevationInMeters = b.ElevationInMeters };

            var brp = LocationToPoint(br);

            // scale all the coordinates based on the original, correct geoid radius...
            brp.Point.X *= (bradius / brp.Radius);
            brp.Point.Y *= (bradius / brp.Radius);
            brp.Point.Z *= (bradius / brp.Radius);
            brp.Radius = bradius;   // restore actual geoid-based radius calculation

            // Rotate brp cartesian coordinates around the z-axis by a.lon degrees,
            // then around the y-axis by a.lat degrees.
            // Though we are decreasing by a.lat degrees, as seen above the y-axis,
            // this is a positive (counterclockwise) rotation (if B's longitude is east of A's).
            // However, from this point of view the x-axis is pointing left.
            // So we will look the other way making the x-axis pointing right, the z-axis
            // pointing up, and the rotation treated as negative.

            var alat = -a.Latitude * Math.PI / 180.0;
            var acos = Math.Cos(alat);
            var asin = Math.Sin(alat);

            var bx = (brp.Point.X * acos) - (brp.Point.Z * asin);
            var by = brp.Point.Y;
            var bz = (brp.Point.X * asin) + (brp.Point.Z * acos);

            return new Point3D { X = bx, Y = by, Z = bz };
        }

        public static double CalculateDistanceInKm(GeoPoint a, GeoPoint b)
        {
            var ap = LocationToPoint(a);
            var bp = LocationToPoint(b);
            var distKm = 0.001 * Math.Round(Distance(ap.Point, bp.Point));

            return distKm;

            /*
            // Let's use a trick to calculate azimuth:
            // Rotate the globe so that point A looks like latitude 0, longitude 0.
            // We keep the actual radii calculated based on the oblate geoid,
            // but use angles based on subtraction.
            // Point A will be at x=radius, y=0, z=0.
            // Vector difference B-A will have dz = N/S component, dy = E/W component.

            var br = RotateGlobe(b, a, bp.Radius, ap.Radius);
            var theta = Math.Atan2(br.Z, br.Y) * 180.0 / Math.PI;
            var azimuth = 90.0 - theta;
            if (azimuth < 0.0)
            {
                azimuth += 360.0;
            }
            if (azimuth > 360.0)
            {
                azimuth -= 360.0;
            }

            var azRound = Math.Round(azimuth * 10) / 10;

            // Calculate altitude, which is the angle above the horizon of B as seen from A.
            // Almost always, B will actually be below the horizon, so the altitude will be negative.
            var shadow = Math.Sqrt((br.Y * br.Y) + (br.Z * br.Z));
            var altitude = Math.Atan2(br.X - ap.Radius, shadow) * 180.0 / Math.PI;
            var alRound = Math.Round(altitude * 100) / 100;
             */
        }

        #endregion cosinekitty.com/compass.html
    }

    public class GeoLoc
    {
        public Point3D Point { get; set; }

        public double Radius { get; set; }
    }

    public class GeoPoint
    {
        public GeoPoint()
        {}

        public GeoPoint(double lat, double lon, double el)
        {
            Latitude = lat;
            Longitude = lon;
            ElevationInMeters = el;
        }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public double ElevationInMeters { get; set; }
    }

    public class Point3D
    {
        public double X { get; set; }

        public double Y { get; set; }

        public double Z { get; set; }
    }
}