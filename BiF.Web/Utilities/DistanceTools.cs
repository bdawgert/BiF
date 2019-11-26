using System;

namespace BiF.Web.Utilities
{
    public class DistanceTools
    {
        public static double Haversine(decimal latitude1, decimal longitude1, decimal latitude2, decimal longitude2)
        {
            if (latitude1 == 0 || latitude2 == 0 || longitude1 == 0 || longitude2 == 0)
                return -1.0;

            double radiansLatitude = degreesToRadians(latitude1 - latitude2);
            double radiansLongitude = degreesToRadians(longitude1 - longitude2);

            double a = Math.Pow(Math.Sin(radiansLatitude / 2), 2) + Math.Pow(Math.Sin(radiansLongitude / 2), 2) * Math.Cos(degreesToRadians(latitude1)) * Math.Cos(degreesToRadians(latitude2));
            double c = 2d * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double d = 6378d * c * .621371;
            return d;
        }

        private static double degreesToRadians(decimal degrees) => Convert.ToDouble(degrees) * Math.PI / 180d;
    }

}