using System;

using GeographicLib;
using GeographicLib.Projections;

using Microsoft.Extensions.Logging;

using LowDistortionProjection;

namespace egbt22lib
{
    public static class Convert
    {
        private static ILogger? _logger;

        public static void InitializeLogger(ILogger logger)
        {
            _logger = logger;
        }

        public const double Bessel_a = 6377397.155;
        public const double Bessel_f = 1d / 299.1528128;

        public const double GRS80_a = 6378137.0;
        public const double GRS80_f = 1d / 298.257222101;

        public const double GK5_FE = 5500000.0;
        public const double GK5_Lon0 = 15.0;

        public const double EGBT22Local_FE = 10000.0;
        public const double EGBT22Local_FN = 50000.0;
        public const double EGBT22Local_Lat0 = 50.8247;
        public const double EGBT22Local_Lon0 = 13.9027;
        public const double EGBT22Local_k0 = 1.0000346;

        public static readonly double EGBT22Local_Mk0;
        public static readonly TransverseMercatorExact BesselGK = new TransverseMercatorExact(Bessel_a, Bessel_f, 1);
        public static readonly TransverseMercatorExact GRS80Local = new TransverseMercatorExact(GRS80_a, GRS80_f, EGBT22Local_k0);

        public static readonly Geocentric GRS80Geoc = new Geocentric(GRS80_a, GRS80_f);
        public static readonly Geocentric BesselGeoc = new Geocentric(Bessel_a, Bessel_f);

        static Convert()
        {
            (_, EGBT22Local_Mk0) = GRS80Local.Forward(0, EGBT22Local_Lat0, 0);
        }

        public static (double[] x, double[] y) ConvertArrays(Func<double, double, (double, double)> convert, double[] xin, double[] yin)
        {
            double[] x = new double[xin.Length];
            double[] y = new double[yin.Length];
            for (int i = 0; i < xin.Length; i++)
            {
                (x[i], y[i]) = convert(xin[i], yin[i]);
            }
            return (x, y);
        }

        public static (double[] x, double[] y, double[] z) ConvertArrays(Func<double, double, double, (double, double, double)> convert, double[] xin, double[] yin, double[] zin)
        {
            double[] x = new double[xin.Length];
            double[] y = new double[yin.Length];
            double[] z = new double[zin.Length];
            for (int i = 0; i < xin.Length; i++)
            {
                (x[i], y[i], z[i]) = convert(xin[i], yin[i], zin[i]);
            }
            return (x, y, z);
        }

        public static (double x, double y, double z) GRS80Geoc_Forward(double lat, double lon, double h) => GRS80Geoc.Forward(lat, lon, h);
        public static (double lat, double lon, double h) GRS80Geoc_Reverse(double x, double y, double z) => GRS80Geoc.Reverse(x, y, z);
        public static (double x, double y, double z) BesselGeoc_Forward(double lat, double lon, double h) => BesselGeoc.Forward(lat, lon, h);
        public static (double lat, double lon, double h) BesselGeoc_Reverse(double x, double y, double z) => BesselGeoc.Reverse(x, y, z);

        public static (double r, double h) BesselGK5_Forward(double lat, double lon)
        {
            var (x, y) = BesselGK.Forward(GK5_Lon0, lat, lon);
            return (x + GK5_FE, y);
        }

        public static (double lat, double lon) BesselGK5_Reverse(double r, double h)
        {
            return BesselGK.Reverse(GK5_Lon0, r - GK5_FE, h);
        }

        public static (double r, double h) BesselGK5_Forward(double lat, double lon, out double gamma, out double k)
        {
            var (x, y) = BesselGK.Forward(GK5_Lon0, lat, lon, out gamma, out k);
            return (x + GK5_FE, y);
        }

        public static (double lat, double lon) BesselGK5_Reverse(double r, double h, out double gamma, out double k)
        {
            return BesselGK.Reverse(GK5_Lon0, r - GK5_FE, h, out gamma, out k);
        }

        public static (double r, double h) EGBT22_Local_Forward(double lat, double lon)
        {
            var (x, y) = GRS80Local.Forward(EGBT22Local_Lon0, lat, lon);
            return (x + EGBT22Local_FE, y - EGBT22Local_Mk0 + EGBT22Local_FN);
        }

        public static (double lat, double lon) EGBT22_Local_Reverse(double r, double h)
        {
            return GRS80Local.Reverse(EGBT22Local_Lon0, r - EGBT22Local_FE, h + EGBT22Local_Mk0 - EGBT22Local_FN);
        }

        public static (double r, double h) EGBT22_Local_Forward(double lat, double lon, out double gamma, out double k)
        {
            var (x, y) = GRS80Local.Forward(EGBT22Local_Lon0, lat, lon, out gamma, out k);
            return (x + EGBT22Local_FE, y - EGBT22Local_Mk0 + EGBT22Local_FN);
        }

        public static (double lat, double lon) EGBT22_Local_Reverse(double r, double h, out double gamma, out double k)
        {
            return GRS80Local.Reverse(EGBT22Local_Lon0, r - EGBT22Local_FE, h + EGBT22Local_Mk0 - EGBT22Local_FN, out gamma, out k);
        }

        public static bool DBRef_GK5_to_EGBT22_Local(double[] gk5Rechts, double[] gk5Hoch, double[] h, out double[] localRechts, out double[] localHoch)
        {
            if (gk5Rechts.Length != gk5Hoch.Length || gk5Rechts.Length != h.Length)
            {
                _logger?.LogWarning("Input arrays have different lengths.");
                localRechts = Array.Empty<double>();
                localHoch = Array.Empty<double>();
                return false;
            }
            // GK5 to Geodetic conversion
            var (gk5Lat, gk5Lon) = ConvertArrays(BesselGK5_Reverse, gk5Rechts, gk5Hoch);
            // Geodetic to geocentric conversion
            var (dbrefX, dbrefY, dbrefZ) = ConvertArrays(BesselGeoc_Forward, gk5Lat, gk5Lon, h);
            // Geocentric DBREF to geocentric ETRS89 transformation
            var (etrs89X, etrs89Y, etrs89Z) = ConvertArrays(Transformation.DbrefToEtrs89, dbrefX, dbrefY, dbrefZ);
            // Geocentric ETRS89 to geodetic conversion
            var (etrs89Lat, etrs89Lon, _) = ConvertArrays(GRS80Geoc_Reverse, etrs89X, etrs89Y, etrs89Z);
            // GCG2016 geoid Heights
            var geoid = LowDistortionProjection.Geoid.GetBKGBinaryGeoidHeights("GCG2016v2023", etrs89Lat, etrs89Lon);
            // ETRS89 ellipsoid heights
            var hell = new double[geoid.Length];
            for (int i = 0; i < hell.Length; i++)
                hell[i] = geoid[i] + h[i];
            // ETRS89 geodetic to geocentric conversion, recalculation with adjusted heights
            (etrs89X, etrs89Y, etrs89Z) = ConvertArrays(GRS80Geoc_Forward, etrs89Lat, etrs89Lon, hell);
            // ETRS89 geocentric to DBREF geocentric transformation, recalculation with adjusted heights
            (dbrefX, dbrefY, dbrefZ) = ConvertArrays(Transformation.Etrs89ToDbref, etrs89X, etrs89Y, etrs89Z);
            // Corrected DBREF ellipsoid heights from geocentric to geodetic conversion
            var (_, _, hcorr) = ConvertArrays(BesselGeoc_Reverse, dbrefX, dbrefY, dbrefZ);
            // Geodetic to geocentric conversion, with corrected heights
            (dbrefX, dbrefY, dbrefZ) = ConvertArrays(BesselGeoc_Forward, gk5Lat, gk5Lon, hcorr);
            // Geocentric DBREF to geocentric ETRS89 transformation, with corrected heights
            (etrs89X, etrs89Y, etrs89Z) = ConvertArrays(Transformation.DbrefToEtrs89, dbrefX, dbrefY, dbrefZ);
            // Geocentric ETRS89 to geodetic conversion, with corrected heights
            (etrs89Lat, etrs89Lon, _) = ConvertArrays(GRS80Geoc_Reverse, etrs89X, etrs89Y, etrs89Z);
            // Geodetic to EGBT22_Local conversion
            (localRechts, localHoch) = ConvertArrays(EGBT22_Local_Forward, etrs89Lat, etrs89Lon);
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
            // Local to Geodetic conversion
            var (etrs89Lat, etrs89Lon) = ConvertArrays(EGBT22_Local_Reverse, localRechts, localHoch);
            // GCG2016 geoid Heights
            var geoid = LowDistortionProjection.Geoid.GetBKGBinaryGeoidHeights("GCG2016v2023", etrs89Lat, etrs89Lon);
            // ETRS89 ellipsoid heights
            var hell = new double[geoid.Length];
            for (int i = 0; i < hell.Length; i++)
                hell[i] = geoid[i] + h[i];
            // ETRS89 geodetic to geocentric conversion
            var (etrs89X, etrs89Y, etrs89Z) = ConvertArrays(GRS80Geoc_Forward, etrs89Lat, etrs89Lon, hell);
            // ETRS89 geocentric to DBREF geocentric transformation
            var (dbrefX, dbrefY, dbrefZ) = ConvertArrays(Transformation.Etrs89ToDbref2, etrs89X, etrs89Y, etrs89Z);
            // Geocentric to geodetic conversion
            var (dbrefLat, dbrefLon, _) = ConvertArrays(BesselGeoc_Reverse, dbrefX, dbrefY, dbrefZ);
            // Geodetic to GK5 conversion
            (gk5Rechts, gk5Hoch) = ConvertArrays(BesselGK5_Forward, dbrefLat, dbrefLon);
            _logger?.LogInformation("Conversion from EGBT22_Local to DBRef_GK5 successful.");
            return true;
        }

        // Using LowDistortionProjection
        public static readonly Srs DBRef_GK5 = Srs.Get(5685);
        public static readonly Srs DBRef_Geod_3D = Srs.Get(5830);
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
        public const string DBRef_Geod_3D_to_ETRS89_Geod_3D_options = "+proj=pipeline " +
            "+step +proj=axisswap +order=2,1 " +
            "+step +proj=unitconvert +xy_in=deg +z_in=m +xy_out=rad +z_out=m " +
            "+step +proj=cart +ellps=bessel " +
            "+step +proj=helmert +x=584.9636 +y=107.7175 +z=413.8067 +rx=-1.1155 +ry=-0.2824 +rz=3.1384 +s=7.9922 +exact +convention=coordinate_frame " +
            "+step +inv +proj=cart +ellps=GRS80 " +
            "+step +proj=unitconvert +xy_in=rad +z_in=m +xy_out=deg +z_out=m " +
            "+step +proj=axisswap +order=2,1";
        public const string DBRef_Geoc_to_ETRS89_Geoc_options =
            "+proj=helmert +x=584.9636 +y=107.7175 +z=413.8067 +rx=-1.1155 +ry=-0.2824 +rz=3.1384 +s=7.9922 +exact +convention=coordinate_frame";

        public const string EGBT22_Local_to_DBRef_GK5_options = "+proj=pipeline " +
            "+step +inv +proj=tmerc +lat_0=50.8247 +lon_0=13.9027 +k=1.0000346 +x_0=10000 +y_0=50000 +ellps=GRS80 " +
            "+step +proj=cart +ellps=GRS80 " +
            "+step +inv +proj=helmert +x=584.9636 +y=107.7175 +z=413.8067 +rx=-1.1155 +ry=-0.2824 +rz=3.1384 +s=7.9922 +exact +convention=coordinate_frame " +
            "+step +inv +proj=cart +ellps=bessel " +
            "+step +proj=tmerc +lat_0=0 +lon_0=15 +k=1 +x_0=5500000 +y_0=0 +ellps=bessel";

        public const string ETRS89_Geod_3D_to_DBRef_Geod_3D_options = "+proj=pipeline " +
            "+step +proj=axisswap +order=2,1 " +
            "+step +proj=unitconvert +xy_in=deg +z_in=m +xy_out=rad +z_out=m " +
            "+step +proj=cart +ellps=GRS80 " +
            "+step +inv +proj=helmert +x=584.9636 +y=107.7175 +z=413.8067 +rx=-1.1155 +ry=-0.2824 +rz=3.1384 +s=7.9922 +exact +convention=coordinate_frame " +
            "+step +inv +proj=cart +ellps=bessel " +
            "+step +proj=unitconvert +xy_in=rad +z_in=m +xy_out=deg +z_out=m " +
            "+step +proj=axisswap +order=2,1";

        public const string ETRS89_Geoc_to_DBRef_Geoc_options = "+proj=pipeline " +
            "+step +inv +proj=helmert +x=584.9636 +y=107.7175 +z=413.8067 +rx=-1.1155 +ry=-0.2824 +rz=3.1384 +s=7.9922 +exact +convention=coordinate_frame";

        public static bool DBRef_GK5_to_EGBT22_Local2(double[] gk5Rechts, double[] gk5Hoch, double[] h, out double[] localRechts, out double[] localHoch)
        {
            if (gk5Rechts.Length != gk5Hoch.Length || gk5Rechts.Length != h.Length)
            {
                _logger?.LogWarning("Input arrays have different lengths.");
                localRechts = Array.Empty<double>();
                localHoch = Array.Empty<double>();
                return false;
            }
            // GK5 to Geodetic conversion
            var (dbrefLat, dbrefLon) = Srs.Convert(gk5Rechts, gk5Hoch, DBRef_GK5, DBRef_Geod_3D);
            // DBREF to ETRS89 geodetic conversion and transformation 
            var (etrs89Lat, etrs89Lon, etrs89Hell) = Srs.Convert(dbrefLat, dbrefLon, h, DBRef_Geod_3D, ETRS89_Geod_3D, DBRef_Geod_3D_to_ETRS89_Geod_3D_options);
            // GCG2016 geoid Heights
            var geoid = LowDistortionProjection.Geoid.GetBKGBinaryGeoidHeights("GCG2016v2023", etrs89Lat, etrs89Lon);
            // ETRS89 ellipsoid heights (exact)
            for (int i = 0; i < etrs89Hell.Length; i++)
                etrs89Hell[i] = geoid[i] + h[i];
            // DBREF ellipsoidal heights through ETRS89 to DBREF geodetic conversion and transformation 
            var (_, _, dbRefHell) = Srs.Convert(etrs89Lat, etrs89Lon, etrs89Hell, ETRS89_Geod_3D, DBRef_Geod_3D, ETRS89_Geod_3D_to_DBRef_Geod_3D_options);
            // DBREF GK5 to EGBT22 (ETRS89) Local conversion and transformation
            (localRechts, localHoch, _) = Srs.Convert(gk5Rechts, gk5Hoch, dbRefHell, DBRef_GK5, EGBT22_Local, DBRef_GK5_to_EGBT22_Local_options);

            _logger?.LogInformation("Conversion from DBRef_GK5 to EGBT22_Local successful.");
            return true;
        }

        public static bool EGBT22_Local_to_DBRef_GK52(double[] localRechts, double[] localHoch, double[] h, out double[] gk5Rechts, out double[] gk5Hoch)
        {
            if (localRechts.Length != localHoch.Length || localRechts.Length != h.Length)
            {
                _logger?.LogWarning("Input arrays have different lengths.");
                gk5Rechts = Array.Empty<double>();
                gk5Hoch = Array.Empty<double>();
                return false;
            }
            // Local to Geodetic conversion
            var (etrs89Lat, etrs89Lon) = Srs.Convert(localRechts, localHoch, EGBT22_Local, ETRS89_Geod_3D);
            // GCG2016 geoid Heights
            var geoid = LowDistortionProjection.Geoid.GetBKGBinaryGeoidHeights("GCG2016v2023", etrs89Lat, etrs89Lon);
            // ETRS89 ellipsoid heights
            var hell = new double[geoid.Length];
            for (int i = 0; i < hell.Length; i++)
                hell[i] = geoid[i] + h[i];
            // EGBT22(ETRS89) Local to DBREF GK5 conversion and transformation
            (gk5Rechts, gk5Hoch, _) = Srs.Convert(localRechts, localHoch, hell, EGBT22_Local, DBRef_GK5, EGBT22_Local_to_DBRef_GK5_options);

            _logger?.LogInformation("Conversion from EGBT22_Local to DBRef_GK5 successful.");
            return true;
        }

    }
}
