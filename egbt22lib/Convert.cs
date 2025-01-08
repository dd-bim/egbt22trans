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

        public static readonly Srs ETRS89_Geod_3D = Srs.Get(4937);

        public static readonly Srs EGBT22_Local = Srs.GetLocalTMSystemWithScale(4258, 50.8247, 13.9027, 1.0000346, 10000, 50000); // EGBT_LDP

        public static bool DBRef_GK5_to_EGBT22_Local(double[] gk5Rechts, double[] gk5Hoch, double[] h, out double[] localRechts, out double[] localHoch)
        {
            if (gk5Rechts.Length != gk5Hoch.Length || gk5Rechts.Length != h.Length)
            {
                _logger?.LogWarning("Input arrays have different lengths.");
                localRechts = Array.Empty<double>();
                localHoch = Array.Empty<double>();
                return false;
            }

            (localRechts, localHoch, _) = Srs.Convert(gk5Rechts, gk5Hoch, h, DBRef_GK5, EGBT22_Local);
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
            var (etrs89Lat, etrs89Lon) = Srs.Convert(localRechts, localHoch, EGBT22_Local, ETRS89_Geod_3D);
            var geoid = Geoid.GetBKGBinaryGeoidHeights("GCG2016v2023_NO", etrs89Lat, etrs89Lon);
            var hell = new double[geoid.Length];
            for (int i = 0; i < hell.Length; i++)
                hell[i] = geoid[i] + h[i];
            (gk5Rechts, gk5Hoch, _) = Srs.Convert(etrs89Lat, etrs89Lon, hell, ETRS89_Geod_3D, DBRef_GK5);
            _logger?.LogInformation("Conversion from EGBT22_Local to DBRef_GK5 successful.");
            return true;
        }
    }
}
