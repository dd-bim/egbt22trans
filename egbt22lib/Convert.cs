using System;

using LowDistortionProjection;

using Microsoft.Extensions.Logging;

namespace egbt22lib
{
    public static class Convert
    {
        private static ILogger? _logger;

        public static void InitializeLogger(ILogger logger)
        {
            _logger = logger;
        }


        public static readonly Srs DBRef_GK5 = Srs.Get(5685);
        public static readonly Srs DBRef_Geoc = Srs.Get(5828);

        public static readonly Srs ETRS89_Geod_3D = Srs.Get(4937);
        public static readonly Srs ETRS89_Geoc = Srs.Get(4936);

        public static readonly Srs EGBT22_Local = Srs.GetLocalTMSystemWithScale(4258, 50.8247, 13.9027, 1.0000346, 10000, 50000); // EGBT_LDP

        public const string DBRef_GK5_to_EGBT22_Local_options = "+proj=pipeline " +
            "+step +inv +proj=tmerc +lat_0=0 +lon_0=15 +k=1 +x_0=5500000 +y_0=0 +ellps=bessel " +
            "+step +proj=cart +ellps=bessel " +
            "+step +proj=helmert +x=584.9636 +y=107.7175 +z=413.8067 +rx=-1.1155 +ry=-0.2824 +rz=3.1384 +s=7.9922 +exact +convention=coordinate_frame " +
            "+step +inv +proj=cart +ellps=GRS80 " +
            "+step +proj=tmerc +lat_0=50.8247 +lon_0=13.9027 +k=1.0000346 +x_0=10000 +y_0=50000 +ellps=GRS80";

        public const string DBRef_GK5_to_ETRS89_Geod_3D_options = "+proj=pipeline " +
            "+step +inv +proj=tmerc +lat_0=0 +lon_0=15 +k=1 +x_0=5500000 +y_0=0 +ellps=bessel " +
            "+step +proj=cart +ellps=bessel " +
            "+step +proj=helmert +x=584.9636 +y=107.7175 +z=413.8067 +rx=-1.1155 +ry=-0.2824 +rz=3.1384 +s=7.9922 +exact +convention=coordinate_frame " +
            "+step +inv +proj=cart +ellps=GRS80";

        public const string EGBT22_Local_to_DBRef_GK5_options = "+proj=pipeline " +
            "+step +inv +proj=tmerc +lat_0=50.8247 +lon_0=13.9027 +k=1.0000346 +x_0=10000 +y_0=50000 +ellps=GRS80 " +
            "+step +proj=cart +ellps=GRS80 " +
            "+step +inv +proj=helmert +x=584.9636 +y=107.7175 +z=413.8067 +rx=-1.1155 +ry=-0.2824 +rz=3.1384 +s=7.9922 +exact +convention=coordinate_frame " +
            "+step +inv +proj=cart +ellps=bessel " +
            "+step +proj=tmerc +lat_0=0 +lon_0=15 +k=1 +x_0=5500000 +y_0=0 +ellps=bessel";
 
        public const string EGBT22_Local_to_ETRS89_Geod_3D_options = "+proj=pipeline " +
            "+step +inv +proj=tmerc +lat_0=50.8247 +lon_0=13.9027 +k=1.0000346 +x_0=10000 +y_0=50000 +ellps=GRS80 +xy_out=deg";
        
        public const string ETRS89_Geod_3D_to_DBRef_GK5_options = "+proj=pipeline " +
            "+step +proj=cart +ellps=GRS80 " +
            "+step +inv +proj=helmert +x=584.9636 +y=107.7175 +z=413.8067 +rx=-1.1155 +ry=-0.2824 +rz=3.1384 +s=7.9922 +exact +convention=coordinate_frame " +
            "+step +inv +proj=cart +ellps=bessel " +
            "+step +proj=tmerc +lat_0=0 +lon_0=15 +k=1 +x_0=5500000 +y_0=0 +ellps=bessel";


        public static bool DBRef_GK5_to_EGBT22_Local(double[] gk5Rechts, double[] gk5Hoch, double[] h, out double[] localRechts, out double[] localHoch)
        {
            if (gk5Rechts.Length != gk5Hoch.Length || gk5Rechts.Length != h.Length)
            {
                _logger?.LogWarning("Input arrays have different lengths.");
                localRechts = Array.Empty<double>();
                localHoch = Array.Empty<double>();
                return false;
            }
            var (etrs89Lat, etrs89Lon, hell_) = Srs.Convert(gk5Rechts, gk5Hoch, h, DBRef_GK5, ETRS89_Geod_3D, DBRef_GK5_to_ETRS89_Geod_3D_options);
            var geoid = Geoid.GetBKGBinaryGeoidHeights("GCG2016v2023", etrs89Lat, etrs89Lon);
            var hell = new double[geoid.Length];
            for (int i = 0; i < hell.Length; i++)
                hell[i] = geoid[i] + h[i];
            var (_, _, hellDBRef) = Srs.Convert(etrs89Lat, etrs89Lon, hell, ETRS89_Geod_3D, DBRef_GK5, ETRS89_Geod_3D_to_DBRef_GK5_options);
            (localRechts, localHoch, _) = Srs.Convert(gk5Rechts, gk5Hoch, hellDBRef, DBRef_GK5, EGBT22_Local, DBRef_GK5_to_EGBT22_Local_options);

            //(localRechts, localHoch, _) = Srs.Convert(gk5Rechts, gk5Hoch, h, DBRef_GK5, EGBT22_Local);

            _logger?.LogInformation("Conversion from DBRef_GK5 to EGBT22_Local successful.");
            return true;
        }

        public static bool EGBT22_Local_to_DBRef_GK5(double[] localRechts, double[] localHoch, double[] h, out double[] gk5Rechts, out double[] gk5Hoch)
        {
            if (localRechts.Length != localHoch.Length || localRechts.Length != h.Length)
            {
                _logger?.LogWarning("Input arrays have different lengths.");
                gk5Rechts = Array.Empty<double>();
                gk5Hoch = Array.Empty<double>();
                return false;
            }
            var (etrs89Lat, etrs89Lon, _) = Srs.Convert(localRechts, localHoch, h, EGBT22_Local, ETRS89_Geod_3D);
            var geoid = Geoid.GetBKGBinaryGeoidHeights("GCG2016v2023", etrs89Lat, etrs89Lon);
            var hell = new double[geoid.Length];
            for (int i = 0; i < hell.Length; i++)
                hell[i] = geoid[i] + h[i];
            (gk5Rechts, gk5Hoch, _) = Srs.Convert(localRechts, localHoch, hell, EGBT22_Local, DBRef_GK5, EGBT22_Local_to_DBRef_GK5_options);
            _logger?.LogInformation("Conversion from EGBT22_Local to DBRef_GK5 successful.");
            return true;
        }
    }
}
