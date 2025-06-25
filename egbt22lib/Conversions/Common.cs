using GeographicLib.Geocodes;

using System;
using System.Collections.Generic;
using System.Text;

namespace egbt22lib.Conversions
{
    public static class Common
    {
        // Parameter from EPSG 7004 definition
        public const double Bessel_a = 6377397.155;
        public const double Bessel_f = 1d / 299.1528128;
        public const double Bessel_e2 = (2 * Bessel_f) - (Bessel_f * Bessel_f);
        public const double Bessel_b = Bessel_a * (1 - Bessel_f);

        // Parameter from EPSG 7019 definition
        public const double GRS80_a = 6378137.0;
        public const double GRS80_f = 1d / 298.257222101;
        public const double GRS80_e2 = (2 * GRS80_f) - (GRS80_f * GRS80_f);
        public const double GRS80_b = GRS80_a * (1 - GRS80_f);

        public const double Max_Height_Difference = 100; // in meters, used for height difference checks

        public const double Normal_Height_Center = 176;

        public static bool IsInsideHeightRange_Normal(double height) => Max_Height_Difference <= Math.Abs(height - Normal_Height_Center);

        // DB_Ref Geod BBox
        public const double DB_Ref_Geod_Lat_Min = 50.641;
        public const double DB_Ref_Geod_Lat_Max = 51.019;
        public const double DB_Ref_Geod_Lon_Min = 13.833;
        public const double DB_Ref_Geod_Lon_Max = 13.976;
        public const double DB_Ref_Height_Center = 173;

        public static bool IsInsideBBox_DB_Ref(double lat, double lon)
        {
            return !(lat < DB_Ref_Geod_Lat_Min || lat > DB_Ref_Geod_Lat_Max
                  || lon < DB_Ref_Geod_Lon_Min || lon > DB_Ref_Geod_Lon_Max);
        }

        public static bool IsInsideHeightRange_DB_Ref(double height) => Max_Height_Difference <= Math.Abs(height - DB_Ref_Height_Center);

        // ETRS89 Geod BBox
        public const double ETRS89_Geod_Lat_Min = 50.640;
        public const double ETRS89_Geod_Lat_Max = 51.018;
        public const double ETRS89_Geod_Lon_Min = 13.831;
        public const double ETRS89_Geod_Lon_Max = 13.974;
        public const double ETRS89_Height_Center = 220;

        public static bool IsInsideBBox_ETRS89(double lat, double lon)
        {
            return !(lat < ETRS89_Geod_Lat_Min || lat > ETRS89_Geod_Lat_Max
                  || lon < ETRS89_Geod_Lon_Min || lon > ETRS89_Geod_Lon_Max);
        }

        public static bool IsInsideHeightRange_ETRS89(double height) => Max_Height_Difference <= Math.Abs(height - ETRS89_Height_Center);

        //// ETRS89/EGBT22 LDP BBox
        //public const double ETRS89_EGBT22_LDP_East_Min = 5000;
        //public const double ETRS89_EGBT22_LDP_East_Max = 15000;
        //public const double ETRS89_EGBT22_LDP_North_Min = 29500;
        //public const double ETRS89_EGBT22_LDP_North_Max = 715000;

        public static double GaussianRadius(in double a, in double f, in double phi)
        {
            //GaussianRadius
            double sinPhi = Math.Sin(phi * 180d / Math.PI);
            // simplified from: a*sqrt(1-e^2)/(1-e^2*sin^2 phi), where e^2= 2f - f^2
            return (a - (a * f)) / (((f - 2) * f * sinPhi * sinPhi) + 1);
        }

        //public static double LocalScale(in double a, in double f, in double b0, in double h0, out double radius)
        //{
        //    radius = GaussianRadius(in a, in f, in b0);
        //    return LocalScale(h0, radius);
        //}

        //public static double LocalScale(in double h0, in double radius) => 1 + (h0 / radius);

        public static double HeightOfScale(in double k0, in double radius) => (k0 - 1) * radius;

        //public static double PointScaleAtHeight(in double k, in double radius, in double ellipsoidalHeight) => k * (radius / (radius + ellipsoidalHeight));

        public static double PointScaleAtHeight(in double k, in double lat, in double ellipsoidalHeight, in double a, in double f)
        {
            double radius = GaussianRadius(a, f, lat);
            return k * (radius / (radius + ellipsoidalHeight));
        }

        public static double PointScaleAtHeight(in double lat, in double ellipsoidalHeight, in double a, in double f)
        {
            double radius = GaussianRadius(a, f, lat);
            return radius / (radius + ellipsoidalHeight);
        }

    }
}
