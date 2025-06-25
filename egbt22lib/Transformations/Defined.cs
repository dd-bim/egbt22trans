using egbt22lib.Conversions;

using System;
using System.Collections.Generic;
using System.Text;

using static egbt22lib.Conversions.Defined;

namespace egbt22lib.Transformations
{
    public static class Defined
    {
        public const double TOLRAD = 1e-13;
        public const double TOLM = 1e-7;

        public static readonly Transformation Trans_Datum_ETRS89_DREF91_to_DBRef = new Transformation(
                -584.9567,
                -107.7277,
                -413.8036,
                1.1155257601,
                0.2824170155,
                -3.1384505907,
                -7.992171
        );

        public static readonly Transformation Trans_Datum_DBRef_to_ETRS89_DREF91 = new Transformation(
                584.9636,
                107.7175,
                413.8067,
                -1.1155214628,
                -0.2824339890,
                3.1384490633,
                7.992235
        );


        public static readonly Transformation Trans_Datum_ETRS89_DREF91_to_EGBT22 = new Transformation(-0.0028, -0.0023, 0.0029);

        public static readonly Transformation Trans_Datum_ETRS89_CZ_to_EGBT22 = new Transformation(0.0028, 0.0023, -0.0029);



        public static (double lat, double lon, double ellH) DB_Ref_Geod_Normal_to_Ellipsoidal(double lat, double lon, double h)
        {
            double th = h;
            double diff = double.MaxValue;
            while (diff > TOLRAD)
            {
                var (x, y, z) = GC_Bessel.Forward(lat, lon, th);
                (x, y, z) = Trans_Datum_DBRef_to_ETRS89_DREF91.Forward(x, y, z);
                (double elat, double elon, _) = GC_GRS80.Reverse(x, y, z);
                double eh = h + Geoid.GetBKGBinaryGeoidHeight(elat, elon);
                (x, y, z) = GC_GRS80.Forward(elat, elon, eh);
                (x, y, z) = Trans_Datum_DBRef_to_ETRS89_DREF91
                    .Reverse(x, y, z); // same transformation reverse to avoid differences through different parameters
                double tlat, tlon;
                (tlat, tlon, th) = GC_Bessel.Reverse(x, y, z);
                diff = Math.Max(Math.Abs(tlat - lat), Math.Abs(tlon - lon));
            }

            return (lat, lon, th);
        }

        public static (double lat, double lon, double h) DB_Ref_Geod_Ellipsoidal_to_Normal(double lat, double lon, double ellH)
        {
            var (x, y, z) = GC_Bessel.Forward(lat, lon, ellH);
            (x, y, z) = Trans_Datum_DBRef_to_ETRS89_DREF91.Forward(x, y, z);
            (double elat, double elon, double eh) = GC_GRS80.Reverse(x, y, z);
            double h = eh - Geoid.GetBKGBinaryGeoidHeight(elat, elon);
            return (lat, lon, h);
        }

        public static (double lat, double lon, double ellH) ETRS89_Geod_Normal_to_Ellipsoidal(double lat, double lon, double h)
        {
            double eh = h + Geoid.GetBKGBinaryGeoidHeight(lat, lon);
            return (lat, lon, eh);
        }

        public static (double lat, double lon, double h) ETRS89_Geod_Ellipsoidal_to_Normal(double lat, double lon, double ellH)
        {
            double h = ellH - Geoid.GetBKGBinaryGeoidHeight(lat, lon);
            return (lat, lon, h);
        }

        public static (double lat, double lon, double ellH) EGBT22_Geod_Normal_to_Ellipsoidal(double lat, double lon, double h)
        {
            double th = h;
            double diff = double.MaxValue;
            while (diff > TOLRAD)
            {
                var (x, y, z) = GC_GRS80.Forward(lat, lon, th);
                (x, y, z) = Trans_Datum_ETRS89_DREF91_to_EGBT22.Reverse(x,y,z);
                (double elat, double elon, _) = GC_GRS80.Reverse(x,y,z);
                double eh = h + Geoid.GetBKGBinaryGeoidHeight(elat, elon);
                (x, y, z) = GC_GRS80.Forward(elat, elon, eh);
                (x, y, z) = Trans_Datum_ETRS89_DREF91_to_EGBT22.Forward(x,y,z);
                double tlat, tlon;
                (tlat, tlon, th) = GC_GRS80.Reverse(x,y,z);
                diff = Math.Max(Math.Abs(tlat - lat), Math.Abs(tlon - lon));
            }
//#if DEBUG
//            // Comparison of result to calculation without transformation
//            double hcheck = h + Geoid.GetBKGBinaryGeoidHeight(lat, lon);
//            Console.WriteLine($"EGBT22_Geod_Normal_to_Ellipsoidal Difference h: {th - hcheck} full:{th} short:{hcheck}");
//#endif
            return (lat, lon, th);
        }

        public static (double lat, double lon, double h) EGBT22_Geod_Ellipsoidal_to_Normal(double lat, double lon, double ellH)
        {
            var (x, y, z) = GC_GRS80.Forward(lat, lon, ellH);
            (x, y, z) = Trans_Datum_ETRS89_DREF91_to_EGBT22.Reverse(x,y,z);
            (double elat, double elon, double eh) = GC_GRS80.Reverse(x,y,z);
            double h = eh - Geoid.GetBKGBinaryGeoidHeight(elat, elon);
            return (lat, lon, h);
        }

    }

}
