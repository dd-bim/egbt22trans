using System;

using GeographicLib;
using GeographicLib.Projections;

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
        
        public const double Bessel_a = 6377397.155;
        public const double Bessel_f = 1d / 299.1528128;

        public const double GRS80_a = 6378137.0;
        public const double GRS80_f = 1d / 298.257222101;

        public const double GK5_FE = 5500000.0;
        public const double GK5_Lon0 = 15.0;

        public const double UTM_FE = 500000.0;
        public const double UTM_k0 = 0.9996;
        public const double UTM33_Lon0 = 15.0;

        public const double EGBT22Local_FE = 10000.0;
        public const double EGBT22Local_FN = 50000.0;
        public const double EGBT22Local_Lat0 = 50.8247;
        public const double EGBT22Local_Lon0 = 13.9027;
        public const double EGBT22Local_k0 = 1.0000346;

        public static readonly double EGBT22Local_Mk0;
        public static readonly TransverseMercatorExact BesselGK = new TransverseMercatorExact(Bessel_a, Bessel_f, 1);
        public static readonly TransverseMercatorExact GRS80Local = new TransverseMercatorExact(GRS80_a, GRS80_f, EGBT22Local_k0);
        public static readonly TransverseMercatorExact GRS80UTM = new TransverseMercatorExact(GRS80_a, GRS80_f, UTM_k0);

        public static readonly Geocentric GRS80Geoc = new Geocentric(GRS80_a, GRS80_f);
        public static readonly Geocentric BesselGeoc = new Geocentric(Bessel_a, Bessel_f);

        static Convert()
        {
            (_, EGBT22Local_Mk0) = GRS80Local.Forward(0, EGBT22Local_Lat0, 0);
        }

        #region Conversions
        
        public static (double[] x, double[] y) ConvertArrays(Func<double, double, (double, double)> convert, double[] xin, double[] yin)
        {
            var x = new double[xin.Length];
            var y = new double[yin.Length];
            for (var i = 0; i < xin.Length; i++)
            {
                (x[i], y[i]) = convert(xin[i], yin[i]);
            }
            return (x, y);
        }
        public static (double[] x, double[] y, double[] z) ConvertArrays(Func<double, double, double, (double, double, double)> convert, double[] xin, double[] yin, double[] zin)
        {
            var x = new double[xin.Length];
            var y = new double[yin.Length];
            var z = new double[zin.Length];
            for (var i = 0; i < xin.Length; i++)
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
        public static (double gamma, double k) BesselGK5_Gamma_k(double r, double h)
        {
            _ = BesselGK.Reverse(GK5_Lon0, r - GK5_FE, h, out var gamma, out var k);
            return (gamma, k);
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
        public static (double gamma, double k) EGBT22_Local_Gamma_k(double r, double h)
        {
            _ = GRS80Local.Reverse(EGBT22Local_Lon0, r - EGBT22Local_FE, h + EGBT22Local_Mk0 - EGBT22Local_FN, out var gamma, out var k);
            return (gamma, k);
        }

        public static (double r, double h) GRS80UTM33_Forward(double lat, double lon)
        {
            var (x, y) = GRS80UTM.Forward(UTM33_Lon0, lat, lon);
            return (x + UTM_FE, y);
        }
        public static (double lat, double lon) GRS80UTM33_Reverse(double r, double h)
        {
            return GRS80UTM.Reverse(UTM33_Lon0, r - UTM_FE, h);
        }
        public static (double gamma, double k) GRS80UTM33_Gamma_k(double r, double h)
        {
            _ = GRS80UTM.Reverse(UTM33_Lon0, r - UTM_FE, h, out var gamma, out var k);
            return (gamma, k);
        }


        #endregion

        #region EGBT22_Local
        public static bool EGBT22_Local_Gamma_k(double[] localRechts, double[] localHoch, out double[] gamma, out double[] k)
        {
            if (localRechts.Length != localHoch.Length)
            {
                _logger?.LogError("Input arrays have different lengths.");
                gamma = Array.Empty<double>();
                k = Array.Empty<double>();
                return false;
            }
            (gamma, k) = ConvertArrays(EGBT22_Local_Gamma_k, localRechts, localHoch);
            return true;
        }
        public static bool EGBT22_Local_to_ETRS89_UTM33(double[] localRechts, double[] localHoch, out double[] easting, out double[] northing)
        {
            if (localRechts.Length != localHoch.Length)
            {
                _logger?.LogError("Input arrays have different lengths.");
                easting = Array.Empty<double>();
                northing = Array.Empty<double>();
                return false;
            }
            // Local to Geodetic conversion
            var (etrs89Lat, etrs89Lon) = ConvertArrays(EGBT22_Local_Reverse, localRechts, localHoch);
            // Geodetic to UTM33 conversion
            (easting, northing) = ConvertArrays(GRS80UTM33_Forward, etrs89Lat, etrs89Lon);
            //_logger?.LogInformation("Conversion from EGBT22_Local to ETRS89_UTM33 successful.");
            return true;
        }
        public static bool EGBT22_Local_to_DBRef_GK5(double[] localRechts, double[] localHoch, double[] h, out double[] gk5Rechts, out double[] gk5Hoch)
        {
            if (localRechts.Length != localHoch.Length || localRechts.Length != h.Length)
            {
                _logger?.LogError("Input arrays have different lengths.");
                gk5Rechts = Array.Empty<double>();
                gk5Hoch = Array.Empty<double>();
                return false;
            }
            // Local to Geodetic conversion
            var (etrs89Lat, etrs89Lon) = ConvertArrays(EGBT22_Local_Reverse, localRechts, localHoch);
            // GCG2016 geoid Heights
            var geoid = Geoid.GetBKGBinaryGeoidHeights(etrs89Lat, etrs89Lon);
            // ETRS89 ellipsoid heights
            var hell = new double[geoid.Length];
            for (var i = 0; i < hell.Length; i++)
                hell[i] = geoid[i] + h[i];
            // ETRS89 geodetic to geocentric conversion
            var (etrs89X, etrs89Y, etrs89Z) = ConvertArrays(GRS80Geoc_Forward, etrs89Lat, etrs89Lon, hell);
            // ETRS89 geocentric to DBREF geocentric transformation
            var (dbrefX, dbrefY, dbrefZ) = ConvertArrays(Transformation.DbrefToEtrs89Inv, etrs89X, etrs89Y, etrs89Z);
            // Geocentric to geodetic conversion
            var (dbrefLat, dbrefLon, _) = ConvertArrays(BesselGeoc_Reverse, dbrefX, dbrefY, dbrefZ);
            // Geodetic to GK5 conversion
            (gk5Rechts, gk5Hoch) = ConvertArrays(BesselGK5_Forward, dbrefLat, dbrefLon);
            //_logger?.LogInformation("Conversion from EGBT22_Local to DBRef_GK5 successful.");
            return true;
        }
        public static bool EGBT22_Local_to_DBRef_GK5_Ell(double[] localRechts, double[] localHoch, double[] localEllH, out double[] gk5Rechts, out double[] gk5Hoch, out double[] gk5EllH)
        {
            if (localRechts.Length != localHoch.Length || localRechts.Length != localEllH.Length)
            {
                _logger?.LogError("Input arrays have different lengths.");
                gk5Rechts = Array.Empty<double>();
                gk5Hoch = Array.Empty<double>();
                gk5EllH = Array.Empty<double>();
                return false;
            }
            // Local to Geodetic conversion
            var (etrs89Lat, etrs89Lon) = ConvertArrays(EGBT22_Local_Reverse, localRechts, localHoch);
            // ETRS89 geodetic to geocentric conversion
            var (etrs89X, etrs89Y, etrs89Z) = ConvertArrays(GRS80Geoc_Forward, etrs89Lat, etrs89Lon, localEllH);
            // ETRS89 geocentric to DBREF geocentric transformation
            var (dbrefX, dbrefY, dbrefZ) = ConvertArrays(Transformation.DbrefToEtrs89Inv, etrs89X, etrs89Y, etrs89Z);
            // Geocentric to geodetic conversion
            var (dbrefLat, dbrefLon, ellH) = ConvertArrays(BesselGeoc_Reverse, dbrefX, dbrefY, dbrefZ);
            // Geodetic to GK5 conversion
            (gk5Rechts, gk5Hoch) = ConvertArrays(BesselGK5_Forward, dbrefLat, dbrefLon);
            gk5EllH = ellH;
            //_logger?.LogInformation("Conversion from EGBT22_Local to DBRef_GK5 successful.");
            return true;
        }
        public static bool EGBT22_Local_to_ETRS89_Geoc(double[] localRechts, double[] localHoch, double[] h, out double[] etrs89X, out double[] etrs89Y, out double[] etrs89Z)
        {
            if (localRechts.Length != localHoch.Length || localRechts.Length != h.Length)
            {
                _logger?.LogError("Input arrays have different lengths.");
                etrs89X = Array.Empty<double>();
                etrs89Y = Array.Empty<double>();
                etrs89Z = Array.Empty<double>();
                return false;
            }
            // Local to Geodetic conversion
            var (etrs89Lat, etrs89Lon) = ConvertArrays(EGBT22_Local_Reverse, localRechts, localHoch);
            // GCG2016 geoid Heights
            var geoid = Geoid.GetBKGBinaryGeoidHeights(etrs89Lat, etrs89Lon);
            // ETRS89 ellipsoid heights
            var hEll = new double[geoid.Length];
            for (var i = 0; i < hEll.Length; i++)
                hEll[i] = geoid[i] + h[i];
            // ETRS89 geodetic to geocentric conversion
            (etrs89X, etrs89Y, etrs89Z) = ConvertArrays(GRS80Geoc_Forward, etrs89Lat, etrs89Lon, hEll);

            //_logger?.LogInformation("Conversion from EGBT22_Local to ETRS89_Geoc successful.");
            return true;
        }
        public static bool EGBT22_Local_to_ETRS89_Geod_3D(double[] localRechts, double[] localHoch, double[] h, out double[] etrs89Lat, out double[] etrs89Lon, out double[] etrs89Hell)
        {
            if (localRechts.Length != localHoch.Length || localRechts.Length != h.Length)
            {
                _logger?.LogError("Input arrays have different lengths.");
                etrs89Lat = Array.Empty<double>();
                etrs89Lon = Array.Empty<double>();
                etrs89Hell = Array.Empty<double>();
                return false;
            }
            // Local to Geodetic conversion
            (etrs89Lat, etrs89Lon) = ConvertArrays(EGBT22_Local_Reverse, localRechts, localHoch);
            // GCG2016 geoid Heights
            var geoid = Geoid.GetBKGBinaryGeoidHeights(etrs89Lat, etrs89Lon);
            // ETRS89 ellipsoid heights
            etrs89Hell = new double[geoid.Length];
            for (var i = 0; i < etrs89Hell.Length; i++)
                etrs89Hell[i] = geoid[i] + h[i];

            //_logger?.LogInformation("Conversion from EGBT22_Local to ETRS89_Geod_3D successful.");
            return true;
        }

        public static (double easting, double northing) EGBT22_Local_to_ETRS89_UTM33(double localRechts, double localHoch)
        {
            // Local to Geodetic conversion
            var (etrs89Lat, etrs89Lon) = EGBT22_Local_Reverse(localRechts, localHoch);
            // Geodetic to UTM33 conversion
            return GRS80UTM33_Forward(etrs89Lat, etrs89Lon);
        }
        public static (double gk5Rechts, double gk5Hoch) EGBT22_Local_to_DBRef_GK5(double localRechts, double localHoch, double h)
        {
            // Local to Geodetic conversion
            var (etrs89Lat, etrs89Lon) = EGBT22_Local_Reverse(localRechts, localHoch);
            // GCG2016 geoid Height
            var geoid = Geoid.GetBKGBinaryGeoidHeight(etrs89Lat, etrs89Lon);
            // ETRS89 ellipsoid height
            var hell = geoid + h;
            // ETRS89 geodetic to geocentric conversion
            var (etrs89X, etrs89Y, etrs89Z) = GRS80Geoc_Forward(etrs89Lat, etrs89Lon, hell);
            // ETRS89 geocentric to DBREF geocentric transformation
            var (dbrefX, dbrefY, dbrefZ) = Transformation.DbrefToEtrs89Inv(etrs89X, etrs89Y, etrs89Z);
            // Geocentric to geodetic conversion
            var (dbrefLat, dbrefLon, _) = BesselGeoc_Reverse(dbrefX, dbrefY, dbrefZ);
            // Geodetic to GK5 conversion
            return BesselGK5_Forward(dbrefLat, dbrefLon);
        }
        public static (double gk5Rechts, double gk5Hoch, double gk5EllH) EGBT22_Local_to_DBRef_GK5_Ell(double localRechts, double localHoch, double localEllH)
        {
            // Local to Geodetic conversion
            var (etrs89Lat, etrs89Lon) = EGBT22_Local_Reverse(localRechts, localHoch);
            // ETRS89 geodetic to geocentric conversion
            var (etrs89X, etrs89Y, etrs89Z) = GRS80Geoc_Forward(etrs89Lat, etrs89Lon, localEllH);
            // ETRS89 geocentric to DBREF geocentric transformation
            var (dbrefX, dbrefY, dbrefZ) = Transformation.DbrefToEtrs89Inv(etrs89X, etrs89Y, etrs89Z);
            // Geocentric to geodetic conversion
            var (dbrefLat, dbrefLon, ellH) = BesselGeoc_Reverse(dbrefX, dbrefY, dbrefZ);
            // Geodetic to GK5 conversion
            var (gk5Rechts, gk5Hoch) = BesselGK5_Forward(dbrefLat, dbrefLon);
            return (gk5Rechts, gk5Hoch, ellH);
        }
        public static (double etrs89X, double etrs89Y, double etrs89Z) EGBT22_Local_to_ETRS89_Geoc(double localRechts, double localHoch, double h)
        {
            // Local to Geodetic conversion
            var (etrs89Lat, etrs89Lon) = EGBT22_Local_Reverse(localRechts, localHoch);
            // GCG2016 geoid Heights
            var geoid = Geoid.GetBKGBinaryGeoidHeight(etrs89Lat, etrs89Lon);
            // ETRS89 ellipsoid heights
            var hEll = geoid + h;
            // ETRS89 geodetic to geocentric conversion
            return GRS80Geoc_Forward(etrs89Lat, etrs89Lon, hEll);
        }
        public static (double etrs89Lat, double etrs89Lon, double etrs89Hell) EGBT22_Local_to_ETRS89_Geod_3D(double localRechts, double localHoch, double h)
        {
            // Local to Geodetic conversion
            var (etrs89Lat, etrs89Lon) = EGBT22_Local_Reverse(localRechts, localHoch);
            // GCG2016 geoid Heights
            var geoid = Geoid.GetBKGBinaryGeoidHeight(etrs89Lat, etrs89Lon);
            // ETRS89 ellipsoid heights
            var etrs89Hell = geoid + h;

            //_logger?.LogInformation("Conversion from EGBT22_Local to ETRS89_Geod_3D successful.");
            return (etrs89Lat, etrs89Lon, etrs89Hell);
        }

        #endregion

        #region  ETRS89_UTM33
        public static bool ETRS89_UTM33_Gamma_k(double[] easting, double[] northing, out double[] gamma, out double[] k)
        {
            if (easting.Length != northing.Length)
            {
                _logger?.LogError("Input arrays have different lengths.");
                gamma = Array.Empty<double>();
                k = Array.Empty<double>();
                return false;
            }
            (gamma, k) = ConvertArrays(GRS80UTM33_Gamma_k, easting, northing);
            return true;
        }
        public static bool ETRS89_UTM33_to_EGBT22_Local(double[] easting, double[] northing, out double[] localRechts, out double[] localHoch)
        {
            if (easting.Length != northing.Length)
            {
                _logger?.LogError("Input arrays have different lengths.");
                localRechts = Array.Empty<double>();
                localHoch = Array.Empty<double>();
                return false;
            }
            // UTM33 to Geodetic conversion
            var (etrs89Lat, etrs89Lon) = ConvertArrays(GRS80UTM33_Reverse, easting, northing);
            // Geodetic to Local conversion
            (localRechts, localHoch) = ConvertArrays(EGBT22_Local_Forward, etrs89Lat, etrs89Lon);
            //_logger?.LogInformation("Conversion from ETRS89_UTM33 to EGBT22_Local successful.");
            return true;
        }
        public static bool ETRS89_UTM33_to_DBRef_GK5(double[] easting, double[] northing, double[] h, out double[] gk5Rechts, out double[] gk5Hoch)
        {
            if (easting.Length != northing.Length)
            {
                _logger?.LogError("Input arrays have different lengths.");
                gk5Rechts = Array.Empty<double>();
                gk5Hoch = Array.Empty<double>();
                return false;
            }
            // UTM33 to Geodetic conversion
            var (etrs89Lat, etrs89Lon) = ConvertArrays(GRS80UTM33_Reverse, easting, northing);
            // GCG2016 geoid Heights
            var geoid = Geoid.GetBKGBinaryGeoidHeights(etrs89Lat, etrs89Lon);
            // ETRS89 ellipsoid heights
            var hell = new double[geoid.Length];
            for (var i = 0; i < hell.Length; i++)
                hell[i] = geoid[i] + h[i];
            // ETRS89 geodetic to geocentric conversion
            var (etrs89X, etrs89Y, etrs89Z) = ConvertArrays(GRS80Geoc_Forward, etrs89Lat, etrs89Lon, hell);
            // ETRS89 geocentric to DBREF geocentric transformation
            var (dbrefX, dbrefY, dbrefZ) = ConvertArrays(Transformation.DbrefToEtrs89Inv, etrs89X, etrs89Y, etrs89Z);
            // Geocentric to geodetic conversion
            var (dbrefLat, dbrefLon, _) = ConvertArrays(BesselGeoc_Reverse, dbrefX, dbrefY, dbrefZ);
            // Geodetic to GK5 conversion
            (gk5Rechts, gk5Hoch) = ConvertArrays(BesselGK5_Forward, dbrefLat, dbrefLon);
            //_logger?.LogInformation("Conversion from ETRS89_UTM33 to DBRef_GK5 successful.");
            return true;
        }
        public static bool ETRS89_UTM33_to_DBRef_GK5_Ell(double[] easting, double[] northing, double[] etrs89EllH, out double[] gk5Rechts, out double[] gk5Hoch, out double[] gk5EllH)
        {
            if (easting.Length != northing.Length)
            {
                _logger?.LogError("Input arrays have different lengths.");
                gk5Rechts = Array.Empty<double>();
                gk5Hoch = Array.Empty<double>();
                gk5EllH = Array.Empty<double>();
                return false;
            }
            // UTM33 to Geodetic conversion
            var (etrs89Lat, etrs89Lon) = ConvertArrays(GRS80UTM33_Reverse, easting, northing);
            // ETRS89 geodetic to geocentric conversion
            var (etrs89X, etrs89Y, etrs89Z) = ConvertArrays(GRS80Geoc_Forward, etrs89Lat, etrs89Lon, etrs89EllH);
            // ETRS89 geocentric to DBREF geocentric transformation
            var (dbrefX, dbrefY, dbrefZ) = ConvertArrays(Transformation.DbrefToEtrs89Inv, etrs89X, etrs89Y, etrs89Z);
            // Geocentric to geodetic conversion
            var (dbrefLat, dbrefLon, ellH) = ConvertArrays(BesselGeoc_Reverse, dbrefX, dbrefY, dbrefZ);
            // Geodetic to GK5 conversion
            (gk5Rechts, gk5Hoch) = ConvertArrays(BesselGK5_Forward, dbrefLat, dbrefLon);
            gk5EllH = ellH;
            //_logger?.LogInformation("Conversion from ETRS89_UTM33 to DBRef_GK5 successful.");
            return true;
        }
        public static bool ETRS89_UTM33_to_ETRS89_Geoc(double[] easting, double[] northing, double[] h, out double[] etrs89X, out double[] etrs89Y, out double[] etrs89Z)
        {
            if (easting.Length != northing.Length)
            {
                _logger?.LogError("Input arrays have different lengths.");
                etrs89X = Array.Empty<double>();
                etrs89Y = Array.Empty<double>();
                etrs89Z = Array.Empty<double>();
                return false;
            }
            // UTM33 to Geodetic conversion
            var (etrs89Lat, etrs89Lon) = ConvertArrays(GRS80UTM33_Reverse, easting, northing);
            // GCG2016 geoid Heights
            var geoid = Geoid.GetBKGBinaryGeoidHeights(etrs89Lat, etrs89Lon);
            // ETRS89 ellipsoid heights
            var hell = new double[geoid.Length];
            for (var i = 0; i < hell.Length; i++)
                hell[i] = geoid[i] + h[i];
            // ETRS89 geodetic to geocentric conversion
            (etrs89X, etrs89Y, etrs89Z) = ConvertArrays(GRS80Geoc_Forward, etrs89Lat, etrs89Lon, hell);
            //_logger?.LogInformation("Conversion from ETRS89_UTM33 to ETRS89_Geoc successful.");
            return true;
        }
        public static bool ETRS89_UTM33_to_ETRS89_Geod_3D(double[] easting, double[] northing, double[] h, out double[] etrs89Lat, out double[] etrs89Lon, out double[] etrs89Hell)
        {
            if (easting.Length != northing.Length)
            {
                _logger?.LogError("Input arrays have different lengths.");
                etrs89Lat = Array.Empty<double>();
                etrs89Lon = Array.Empty<double>();
                etrs89Hell = Array.Empty<double>();
                return false;
            }
            // UTM33 to Geodetic conversion
            (etrs89Lat, etrs89Lon) = ConvertArrays(GRS80UTM33_Reverse, easting, northing);
            // GCG2016 geoid Heights
            var geoid = Geoid.GetBKGBinaryGeoidHeights(etrs89Lat, etrs89Lon);
            // ETRS89 ellipsoid heights
            etrs89Hell = new double[geoid.Length];
            for (var i = 0; i < etrs89Hell.Length; i++)
                etrs89Hell[i] = geoid[i] + h[i];
            //_logger?.LogInformation("Conversion from ETRS89_UTM33 to ETRS89_Geod_3D successful.");
            return true;
        }

        public static (double localRechts, double localHoch) ETRS89_UTM33_to_EGBT22_Local(double easting, double northing)
        {
            // UTM33 to Geodetic conversion
            var (etrs89Lat, etrs89Lon) = GRS80UTM33_Reverse(easting, northing);
            // Geodetic to Local conversion
            return EGBT22_Local_Forward(etrs89Lat, etrs89Lon);
        }
        public static (double gk5Rechts, double gk5Hoch) ETRS89_UTM33_to_DBRef_GK5(double easting, double northing, double h)
        {
            // UTM33 to Geodetic conversion
            var (etrs89Lat, etrs89Lon) = GRS80UTM33_Reverse(easting, northing);
            // GCG2016 geoid Heights
            var geoid = Geoid.GetBKGBinaryGeoidHeight(etrs89Lat, etrs89Lon);
            // ETRS89 ellipsoid heights
            var hell = geoid + h;
            // ETRS89 geodetic to geocentric conversion
            var (etrs89X, etrs89Y, etrs89Z) = GRS80Geoc_Forward(etrs89Lat, etrs89Lon, hell);
            // ETRS89 geocentric to DBREF geocentric transformation
            var (dbrefX, dbrefY, dbrefZ) = Transformation.DbrefToEtrs89Inv(etrs89X, etrs89Y, etrs89Z);
            // Geocentric to geodetic conversion
            var (dbrefLat, dbrefLon, _) = BesselGeoc_Reverse(dbrefX, dbrefY, dbrefZ);
            // Geodetic to GK5 conversion
            return BesselGK5_Forward(dbrefLat, dbrefLon);
        }
        public static (double etrs89X, double etrs89Y, double etrs89Z) ETRS89_UTM33_to_ETRS89_Geoc(double easting, double northing, double h)
        {
            // UTM33 to Geodetic conversion
            var (etrs89Lat, etrs89Lon) = GRS80UTM33_Reverse(easting, northing);
            // GCG2016 geoid Heights
            var geoid = Geoid.GetBKGBinaryGeoidHeight(etrs89Lat, etrs89Lon);
            // ETRS89 ellipsoid heights
            var hell = geoid + h;
            // ETRS89 geodetic to geocentric conversion
            return GRS80Geoc_Forward(etrs89Lat, etrs89Lon, hell);
        }
        public static (double etrs89Lat, double etrs89Lon, double etrs89Hell) ETRS89_UTM33_to_ETRS89_Geod_3D(double easting, double northing, double h)
        {
            // UTM33 to Geodetic conversion
            var (etrs89Lat, etrs89Lon) = GRS80UTM33_Reverse(easting, northing);
            // GCG2016 geoid Heights
            var geoid = Geoid.GetBKGBinaryGeoidHeight(etrs89Lat, etrs89Lon);
            // ETRS89 ellipsoid heights
            var etrs89Hell = geoid + h;
            //_logger?.LogInformation("Conversion from ETRS89_UTM33 to ETRS89_Geod_3D successful.");
            return (etrs89Lat, etrs89Lon, etrs89Hell);
        }

        #endregion

        #region DBRef_GK5
        public static bool DBRef_GK5_Gamma_k(double[] gk5Rechts, double[] gk5Hoch, out double[] gamma, out double[] k)
        {
            if (gk5Rechts.Length != gk5Hoch.Length)
            {
                _logger?.LogError("Input arrays have different lengths.");
                gamma = Array.Empty<double>();
                k = Array.Empty<double>();
                return false;
            }
            (gamma, k) = ConvertArrays(BesselGK5_Gamma_k, gk5Rechts, gk5Hoch);
            return true;
        }
        public static bool DBRef_GK5_to_EGBT22_Local(double[] gk5Rechts, double[] gk5Hoch, double[] h, out double[] localRechts, out double[] localHoch)
        {
            if (gk5Rechts.Length != gk5Hoch.Length || gk5Rechts.Length != h.Length)
            {
                _logger?.LogError("Input arrays have different lengths.");
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
            var geoid = Geoid.GetBKGBinaryGeoidHeights(etrs89Lat, etrs89Lon);
            // ETRS89 ellipsoid heights
            var hell = new double[geoid.Length];
            for (var i = 0; i < hell.Length; i++)
                hell[i] = geoid[i] + h[i];
            // ETRS89 geodetic to geocentric conversion, recalculation with adjusted heights
            (etrs89X, etrs89Y, etrs89Z) = ConvertArrays(GRS80Geoc_Forward, etrs89Lat, etrs89Lon, hell);
            // ETRS89 geocentric to DBREF geocentric transformation, recalculation with adjusted heights
            (dbrefX, dbrefY, dbrefZ) = ConvertArrays(Transformation.DbrefToEtrs89Inv, etrs89X, etrs89Y, etrs89Z);
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
            //_logger?.LogInformation("Conversion from DBRef_GK5 to EGBT22_Local successful.");
            return true;
        }
        public static bool DBRef_GK5_to_EGBT22_Local_Ell(double[] gk5Rechts, double[] gk5Hoch, double[] gk5EllH, out double[] localRechts, out double[] localHoch, out double[] localEllH)
        {
            if (gk5Rechts.Length != gk5Hoch.Length || gk5Rechts.Length != gk5EllH.Length)
            {
                _logger?.LogError("Input arrays have different lengths.");
                localRechts = Array.Empty<double>();
                localHoch = Array.Empty<double>();
                localEllH = Array.Empty<double>();
                return false;
            }
            // GK5 to Geodetic conversion
            var (gk5Lat, gk5Lon) = ConvertArrays(BesselGK5_Reverse, gk5Rechts, gk5Hoch);
            // Geodetic to geocentric conversion
            var (dbrefX, dbrefY, dbrefZ) = ConvertArrays(BesselGeoc_Forward, gk5Lat, gk5Lon, gk5EllH);
            // Geocentric DBREF to geocentric ETRS89 transformation
            var (etrs89X, etrs89Y, etrs89Z) = ConvertArrays(Transformation.DbrefToEtrs89, dbrefX, dbrefY, dbrefZ);
            // Geocentric ETRS89 to geodetic conversion
            var (etrs89Lat, etrs89Lon, ellH) = ConvertArrays(GRS80Geoc_Reverse, etrs89X, etrs89Y, etrs89Z);
            // Geodetic to EGBT22_Local conversion
            (localRechts, localHoch) = ConvertArrays(EGBT22_Local_Forward, etrs89Lat, etrs89Lon);
            localEllH = ellH;
            //_logger?.LogInformation("Conversion from DBRef_GK5 to EGBT22_Local successful.");
            return true;
        }
        public static bool DBRef_GK5_to_ETRS89_UTM33(double[] gk5Rechts, double[] gk5Hoch, double[] h, out double[] easting, out double[] northing)
        {
            if (gk5Rechts.Length != gk5Hoch.Length || gk5Rechts.Length != h.Length)
            {
                _logger?.LogError("Input arrays have different lengths.");
                easting = Array.Empty<double>();
                northing = Array.Empty<double>();
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
            var geoid = Geoid.GetBKGBinaryGeoidHeights(etrs89Lat, etrs89Lon);
            // ETRS89 ellipsoid heights
            var hell = new double[geoid.Length];
            for (var i = 0; i < hell.Length; i++)
                hell[i] = geoid[i] + h[i];
            // ETRS89 geodetic to geocentric conversion, recalculation with adjusted heights
            (etrs89X, etrs89Y, etrs89Z) = ConvertArrays(GRS80Geoc_Forward, etrs89Lat, etrs89Lon, hell);
            // ETRS89 geocentric to DBREF geocentric transformation, recalculation with adjusted heights
            (dbrefX, dbrefY, dbrefZ) = ConvertArrays(Transformation.DbrefToEtrs89Inv, etrs89X, etrs89Y, etrs89Z);
            // Corrected DBREF ellipsoid heights from geocentric to geodetic conversion
            var (_, _, hcorr) = ConvertArrays(BesselGeoc_Reverse, dbrefX, dbrefY, dbrefZ);
            // Geodetic to geocentric conversion, with corrected heights
            (dbrefX, dbrefY, dbrefZ) = ConvertArrays(BesselGeoc_Forward, gk5Lat, gk5Lon, hcorr);
            // Geocentric DBREF to geocentric ETRS89 transformation, with corrected heights
            (etrs89X, etrs89Y, etrs89Z) = ConvertArrays(Transformation.DbrefToEtrs89, dbrefX, dbrefY, dbrefZ);
            // Geocentric ETRS89 to geodetic conversion, with corrected heights
            (etrs89Lat, etrs89Lon, _) = ConvertArrays(GRS80Geoc_Reverse, etrs89X, etrs89Y, etrs89Z);
            // Geodetic to ETRS89_UTM33 conversion
            (easting, northing) = ConvertArrays(GRS80UTM33_Forward, etrs89Lat, etrs89Lon);
            //_logger?.LogInformation("Conversion from DBRef_GK5 to ETRS89_UTM33 successful.");
            return true;
        }
        public static bool DBRef_GK5_to_ETRS89_UTM33_Ell(double[] gk5Rechts, double[] gk5Hoch, double[] gk5EllH, out double[] easting, out double[] northing, out double[] etrs89EllH)
        {
            if (gk5Rechts.Length != gk5Hoch.Length || gk5Rechts.Length != gk5EllH.Length)
            {
                _logger?.LogError("Input arrays have different lengths.");
                easting = Array.Empty<double>();
                northing = Array.Empty<double>();
                etrs89EllH = Array.Empty<double>();
                return false;
            }
            // GK5 to Geodetic conversion
            var (gk5Lat, gk5Lon) = ConvertArrays(BesselGK5_Reverse, gk5Rechts, gk5Hoch);
            // Geodetic to geocentric conversion
            var (dbrefX, dbrefY, dbrefZ) = ConvertArrays(BesselGeoc_Forward, gk5Lat, gk5Lon, gk5EllH);
            // Geocentric DBREF to geocentric ETRS89 transformation
            var (etrs89X, etrs89Y, etrs89Z) = ConvertArrays(Transformation.DbrefToEtrs89, dbrefX, dbrefY, dbrefZ);
            // Geocentric ETRS89 to geodetic conversion
            var (etrs89Lat, etrs89Lon, ellH) = ConvertArrays(GRS80Geoc_Reverse, etrs89X, etrs89Y, etrs89Z);
            // Geodetic to ETRS89_UTM33 conversion
            (easting, northing) = ConvertArrays(GRS80UTM33_Forward, etrs89Lat, etrs89Lon);
            etrs89EllH = ellH;
            //_logger?.LogInformation("Conversion from DBRef_GK5 to ETRS89_UTM33 successful.");
            return true;
        }
        public static bool DBRef_GK5_to_ETRS89_Geoc(double[] gk5Rechts, double[] gk5Hoch, double[] h, out double[] etrs89X, out double[] etrs89Y, out double[] etrs89Z)
        {
            if (gk5Rechts.Length != gk5Hoch.Length || gk5Rechts.Length != h.Length)
            {
                _logger?.LogError("Input arrays have different lengths.");
                etrs89X = Array.Empty<double>();
                etrs89Y = Array.Empty<double>();
                etrs89Z = Array.Empty<double>();
                return false;
            }
            // GK5 to Geodetic conversion
            var (gk5Lat, gk5Lon) = ConvertArrays(BesselGK5_Reverse, gk5Rechts, gk5Hoch);
            // Geodetic to geocentric conversion
            var (dbrefX, dbrefY, dbrefZ) = ConvertArrays(BesselGeoc_Forward, gk5Lat, gk5Lon, h);
            // Geocentric DBREF to geocentric ETRS89 transformation
            (etrs89X, etrs89Y, etrs89Z) = ConvertArrays(Transformation.DbrefToEtrs89, dbrefX, dbrefY, dbrefZ);
            // Geocentric ETRS89 to geodetic conversion
            var (etrs89Lat, etrs89Lon, _) = ConvertArrays(GRS80Geoc_Reverse, etrs89X, etrs89Y, etrs89Z);
            // GCG2016 geoid Heights
            var geoid = Geoid.GetBKGBinaryGeoidHeights(etrs89Lat, etrs89Lon);
            // ETRS89 ellipsoid heights
            var hEll = new double[geoid.Length];
            for (var i = 0; i < hEll.Length; i++)
                hEll[i] = geoid[i] + h[i];
            // ETRS89 geodetic to geocentric conversion, recalculation with adjusted heights
            (etrs89X, etrs89Y, etrs89Z) = ConvertArrays(GRS80Geoc_Forward, etrs89Lat, etrs89Lon, hEll);
            // ETRS89 geocentric to DBREF geocentric transformation, recalculation with adjusted heights
            (dbrefX, dbrefY, dbrefZ) = ConvertArrays(Transformation.DbrefToEtrs89Inv, etrs89X, etrs89Y, etrs89Z);
            // Corrected DBREF ellipsoid heights from geocentric to geodetic conversion
            var (_, _, hcorr) = ConvertArrays(BesselGeoc_Reverse, dbrefX, dbrefY, dbrefZ);
            // Geodetic to geocentric conversion, with corrected heights
            (dbrefX, dbrefY, dbrefZ) = ConvertArrays(BesselGeoc_Forward, gk5Lat, gk5Lon, hcorr);
            // Geocentric DBREF to geocentric ETRS89 transformation, with corrected heights
            (etrs89X, etrs89Y, etrs89Z) = ConvertArrays(Transformation.DbrefToEtrs89, dbrefX, dbrefY, dbrefZ);
            //_logger?.LogInformation("Conversion from DBRef_GK5 to ETRS89_Geoc successful.");
            return true;
        }
        public static bool DBRef_GK5_to_ETRS89_Geod_3D(double[] gk5Rechts, double[] gk5Hoch, double[] h, out double[] etrs89Lat, out double[] etrs89Lon, out double[] etrs89Hell)
        {
            if (gk5Rechts.Length != gk5Hoch.Length || gk5Rechts.Length != h.Length)
            {
                _logger?.LogError("Input arrays have different lengths.");
                etrs89Lat = Array.Empty<double>();
                etrs89Lon = Array.Empty<double>();
                etrs89Hell = Array.Empty<double>();
                return false;
            }
            // GK5 to Geodetic conversion
            var (gk5Lat, gk5Lon) = ConvertArrays(BesselGK5_Reverse, gk5Rechts, gk5Hoch);
            // Geodetic to geocentric conversion
            var (dbrefX, dbrefY, dbrefZ) = ConvertArrays(BesselGeoc_Forward, gk5Lat, gk5Lon, h);
            // Geocentric DBREF to geocentric ETRS89 transformation
            var (etrs89X, etrs89Y, etrs89Z) = ConvertArrays(Transformation.DbrefToEtrs89, dbrefX, dbrefY, dbrefZ);
            // Geocentric ETRS89 to geodetic conversion
            (etrs89Lat, etrs89Lon, _) = ConvertArrays(GRS80Geoc_Reverse, etrs89X, etrs89Y, etrs89Z);
            // GCG2016 geoid Heights
            var geoid = Geoid.GetBKGBinaryGeoidHeights(etrs89Lat, etrs89Lon);
            // ETRS89 ellipsoid heights
            etrs89Hell = new double[geoid.Length];
            for (var i = 0; i < etrs89Hell.Length; i++)
                etrs89Hell[i] = geoid[i] + h[i];
            // ETRS89 geodetic to geocentric conversion, recalculation with adjusted heights
            (etrs89X, etrs89Y, etrs89Z) = ConvertArrays(GRS80Geoc_Forward, etrs89Lat, etrs89Lon, etrs89Hell);
            // ETRS89 geocentric to DBREF geocentric transformation, recalculation with adjusted heights
            (dbrefX, dbrefY, dbrefZ) = ConvertArrays(Transformation.DbrefToEtrs89Inv, etrs89X, etrs89Y, etrs89Z);
            // Corrected DBREF ellipsoid heights from geocentric to geodetic conversion
            var (_, _, hcorr) = ConvertArrays(BesselGeoc_Reverse, dbrefX, dbrefY, dbrefZ);
            // Geodetic to geocentric conversion, with corrected heights
            (dbrefX, dbrefY, dbrefZ) = ConvertArrays(BesselGeoc_Forward, gk5Lat, gk5Lon, hcorr);
            // Geocentric DBREF to geocentric ETRS89 transformation, with corrected heights
            (etrs89X, etrs89Y, etrs89Z) = ConvertArrays(Transformation.DbrefToEtrs89, dbrefX, dbrefY, dbrefZ);
            // Geocentric ETRS89 to geodetic conversion, with corrected heights
            (etrs89Lat, etrs89Lon, _) = ConvertArrays(GRS80Geoc_Reverse, etrs89X, etrs89Y, etrs89Z);

            //_logger?.LogInformation("Conversion from DBRef_GK5 to ETRS89_Geod_3D successful.");
            return true;
        }

        public static (double gamma, double k) DBRef_GK5_Gamma_k(double gk5Rechts, double gk5Hoch)=> BesselGK5_Gamma_k(gk5Rechts, gk5Hoch);
        public static (double localRechts, double localHoch) DBRef_GK5_to_EGBT22_Local(double gk5Rechts, double gk5Hoch, double h)
        {
            // GK5 to Geodetic conversion
            var (gk5Lat, gk5Lon) = BesselGK5_Reverse(gk5Rechts, gk5Hoch);
            // Geodetic to geocentric conversion
            var (dbrefX, dbrefY, dbrefZ) = BesselGeoc_Forward(gk5Lat, gk5Lon, h);
            // Geocentric DBREF to geocentric ETRS89 transformation
            var (etrs89X, etrs89Y, etrs89Z) = Transformation.DbrefToEtrs89(dbrefX, dbrefY, dbrefZ);
            // Geocentric ETRS89 to geodetic conversion
            var (etrs89Lat, etrs89Lon, _) = GRS80Geoc_Reverse(etrs89X, etrs89Y, etrs89Z);
            // GCG2016 geoid Height
            var geoid = Geoid.GetBKGBinaryGeoidHeight(etrs89Lat, etrs89Lon);
            // ETRS89 ellipsoid height
            var hell = geoid + h;
            // ETRS89 geodetic to geocentric conversion, recalculation with adjusted heights
            (etrs89X, etrs89Y, etrs89Z) = GRS80Geoc_Forward(etrs89Lat, etrs89Lon, hell);
            // ETRS89 geocentric to DBREF geocentric transformation, recalculation with adjusted heights
            (dbrefX, dbrefY, dbrefZ) = Transformation.DbrefToEtrs89Inv(etrs89X, etrs89Y, etrs89Z);
            // Corrected DBREF ellipsoid heights from geocentric to geodetic conversion
            var (_, _, hcorr) = BesselGeoc_Reverse(dbrefX, dbrefY, dbrefZ);
            // Geodetic to geocentric conversion, with corrected heights
            (dbrefX, dbrefY, dbrefZ) = BesselGeoc_Forward(gk5Lat, gk5Lon, hcorr);
            // Geocentric DBREF to geocentric ETRS89 transformation, with corrected heights
            (etrs89X, etrs89Y, etrs89Z) = Transformation.DbrefToEtrs89(dbrefX, dbrefY, dbrefZ);
            // Geocentric ETRS89 to geodetic conversion, with corrected heights
            (etrs89Lat, etrs89Lon, _) = GRS80Geoc_Reverse(etrs89X, etrs89Y, etrs89Z);
            // Geodetic to EGBT22_Local conversion
            return EGBT22_Local_Forward(etrs89Lat, etrs89Lon);
        }
        public static (double localRechts, double localHoch, double localEllH) DBRef_GK5_to_EGBT22_Local_Ell(double gk5Rechts, double gk5Hoch, double gk5EllH)
        {
            // GK5 to Geodetic conversion
            var (gk5Lat, gk5Lon) = BesselGK5_Reverse(gk5Rechts, gk5Hoch);
            // Geodetic to geocentric conversion
            var (dbrefX, dbrefY, dbrefZ) = BesselGeoc_Forward(gk5Lat, gk5Lon, gk5EllH);
            // Geocentric DBREF to geocentric ETRS89 transformation
            var (etrs89X, etrs89Y, etrs89Z) = Transformation.DbrefToEtrs89(dbrefX, dbrefY, dbrefZ);
            // Geocentric ETRS89 to geodetic conversion
            var (etrs89Lat, etrs89Lon, localEllH) = GRS80Geoc_Reverse(etrs89X, etrs89Y, etrs89Z);
            // Geodetic to EGBT22_Local conversion
            var (localRechts, localHoch) = EGBT22_Local_Forward(etrs89Lat, etrs89Lon);
            return (localRechts, localHoch, localEllH);
        }
        public static (double easting, double northing) DBRef_GK5_to_ETRS89_UTM33(double gk5Rechts, double gk5Hoch, double h)
        {
            // GK5 to Geodetic conversion
            var (gk5Lat, gk5Lon) = BesselGK5_Reverse(gk5Rechts, gk5Hoch);
            // Geodetic to geocentric conversion
            var (dbrefX, dbrefY, dbrefZ) = BesselGeoc_Forward(gk5Lat, gk5Lon, h);
            // Geocentric DBREF to geocentric ETRS89 transformation
            var (etrs89X, etrs89Y, etrs89Z) = Transformation.DbrefToEtrs89(dbrefX, dbrefY, dbrefZ);
            // Geocentric ETRS89 to geodetic conversion
            var (etrs89Lat, etrs89Lon, _) = GRS80Geoc_Reverse(etrs89X, etrs89Y, etrs89Z);
            // GCG2016 geoid Height
            var geoid = Geoid.GetBKGBinaryGeoidHeight(etrs89Lat, etrs89Lon);
            // ETRS89 ellipsoid height
            var hell = geoid + h;
            // ETRS89 geodetic to geocentric conversion, recalculation with adjusted heights
            (etrs89X, etrs89Y, etrs89Z) = GRS80Geoc_Forward(etrs89Lat, etrs89Lon, hell);
            // ETRS89 geocentric to DBREF geocentric transformation, recalculation with adjusted heights
            (dbrefX, dbrefY, dbrefZ) = Transformation.DbrefToEtrs89Inv(etrs89X, etrs89Y, etrs89Z);
            // Corrected DBREF ellipsoid heights from geocentric to geodetic conversion
            var (_, _, hcorr) = BesselGeoc_Reverse(dbrefX, dbrefY, dbrefZ);
            // Geodetic to geocentric conversion, with corrected heights
            (dbrefX, dbrefY, dbrefZ) = BesselGeoc_Forward(gk5Lat, gk5Lon, hcorr);
            // Geocentric DBREF to geocentric ETRS89 transformation, with corrected heights
            (etrs89X, etrs89Y, etrs89Z) = Transformation.DbrefToEtrs89(dbrefX, dbrefY, dbrefZ);
            // Geocentric ETRS89 to geodetic conversion, with corrected heights
            (etrs89Lat, etrs89Lon, _) = GRS80Geoc_Reverse(etrs89X, etrs89Y, etrs89Z);
            // Geodetic to ETRS89_UTM33 conversion
            return GRS80UTM33_Forward(etrs89Lat, etrs89Lon);
        }
        public static (double easting, double northing, double etrs89EllH) DBRef_GK5_to_ETRS89_UTM33_Ell(double gk5Rechts, double gk5Hoch, double gk5EllH)
        {
            // GK5 to Geodetic conversion
            var (gk5Lat, gk5Lon) = BesselGK5_Reverse(gk5Rechts, gk5Hoch);
            // Geodetic to geocentric conversion
            var (dbrefX, dbrefY, dbrefZ) = BesselGeoc_Forward(gk5Lat, gk5Lon, gk5EllH);
            // Geocentric DBREF to geocentric ETRS89 transformation
            var (etrs89X, etrs89Y, etrs89Z) = Transformation.DbrefToEtrs89(dbrefX, dbrefY, dbrefZ);
            // Geocentric ETRS89 to geodetic conversion
            var (etrs89Lat, etrs89Lon, etrs89EllH) = GRS80Geoc_Reverse(etrs89X, etrs89Y, etrs89Z);
            // Geodetic to ETRS89_UTM33 conversion
            var (easting, northing) = GRS80UTM33_Forward(etrs89Lat, etrs89Lon);
            return (easting, northing, etrs89EllH);
        }
        public static (double etrs89X, double etrs89Y, double etrs89Z) DBRef_GK5_to_ETRS89_Geoc(double gk5Rechts, double gk5Hoch, double h)
        {
            // GK5 to Geodetic conversion
            var (gk5Lat, gk5Lon) = BesselGK5_Reverse(gk5Rechts, gk5Hoch);
            // Geodetic to geocentric conversion
            var (dbrefX, dbrefY, dbrefZ) = BesselGeoc_Forward(gk5Lat, gk5Lon, h);
            // Geocentric DBREF to geocentric ETRS89 transformation
            var (etrs89X, etrs89Y, etrs89Z) = Transformation.DbrefToEtrs89(dbrefX, dbrefY, dbrefZ);
            // Geocentric ETRS89 to geodetic conversion
            var (etrs89Lat, etrs89Lon, _) = GRS80Geoc_Reverse(etrs89X, etrs89Y, etrs89Z);
            // GCG2016 geoid Heights
            var geoid = Geoid.GetBKGBinaryGeoidHeight(etrs89Lat, etrs89Lon);
            // ETRS89 ellipsoid heights
            var hEll = geoid + h;
            // ETRS89 geodetic to geocentric conversion, recalculation with adjusted heights
            (etrs89X, etrs89Y, etrs89Z) = GRS80Geoc_Forward(etrs89Lat, etrs89Lon, hEll);
            // ETRS89 geocentric to DBREF geocentric transformation, recalculation with adjusted heights
            (dbrefX, dbrefY, dbrefZ) = Transformation.DbrefToEtrs89Inv(etrs89X, etrs89Y, etrs89Z);
            // Corrected DBREF ellipsoid heights from geocentric to geodetic conversion
            var (_, _, hcorr) = BesselGeoc_Reverse(dbrefX, dbrefY, dbrefZ);
            // Geodetic to geocentric conversion, with corrected heights
            (dbrefX, dbrefY, dbrefZ) = BesselGeoc_Forward(gk5Lat, gk5Lon, hcorr);
            // Geocentric DBREF to geocentric ETRS89 transformation, with corrected heights
            return Transformation.DbrefToEtrs89(dbrefX, dbrefY, dbrefZ);
        }
        public static (double etrs89Lat, double etrs89Lon, double etrs89Hell) DBRef_GK5_to_ETRS89_Geod_3D(double gk5Rechts, double gk5Hoch, double h)
        {
            // GK5 to Geodetic conversion
            var (gk5Lat, gk5Lon) = BesselGK5_Reverse(gk5Rechts, gk5Hoch);
            // Geodetic to geocentric conversion
            var (dbrefX, dbrefY, dbrefZ) = BesselGeoc_Forward(gk5Lat, gk5Lon, h);
            // Geocentric DBREF to geocentric ETRS89 transformation
            var (etrs89X, etrs89Y, etrs89Z) = Transformation.DbrefToEtrs89(dbrefX, dbrefY, dbrefZ);
            // Geocentric ETRS89 to geodetic conversion
            var (etrs89Lat, etrs89Lon, _) = GRS80Geoc_Reverse(etrs89X, etrs89Y, etrs89Z);
            // GCG2016 geoid Heights
            var geoid = Geoid.GetBKGBinaryGeoidHeight(etrs89Lat, etrs89Lon);
            // ETRS89 ellipsoid heights
            var etrs89Hell = geoid + h;
            // ETRS89 geodetic to geocentric conversion, recalculation with adjusted heights
            (etrs89X, etrs89Y, etrs89Z) = GRS80Geoc_Forward(etrs89Lat, etrs89Lon, etrs89Hell);
            // ETRS89 geocentric to DBREF geocentric transformation, recalculation with adjusted heights
            (dbrefX, dbrefY, dbrefZ) = Transformation.DbrefToEtrs89Inv(etrs89X, etrs89Y, etrs89Z);
            // Corrected DBREF ellipsoid heights from geocentric to geodetic conversion
            var (_, _, hcorr) = BesselGeoc_Reverse(dbrefX, dbrefY, dbrefZ);
            // Geodetic to geocentric conversion, with corrected heights
            (dbrefX, dbrefY, dbrefZ) = BesselGeoc_Forward(gk5Lat, gk5Lon, hcorr);
            // Geocentric DBREF to geocentric ETRS89 transformation, with corrected heights
            (etrs89X, etrs89Y, etrs89Z) = Transformation.DbrefToEtrs89(dbrefX, dbrefY, dbrefZ);
            // Geocentric ETRS89 to geodetic conversion, with corrected heights
            (etrs89Lat, etrs89Lon, _) = GRS80Geoc_Reverse(etrs89X, etrs89Y, etrs89Z);

            //_logger?.LogInformation("Conversion from DBRef_GK5 to ETRS89_Geod_3D successful.");
            return (etrs89Lat, etrs89Lon, etrs89Hell);
        }

        #endregion

        #region ETRS89_Geoc
        public static bool ETRS89_Geoc_to_EGBT22_Local(double[] etrs89X, double[] etrs89Y, double[] etrs89Z, out double[] localRechts, out double[] localHoch, out double[] h)
        {
            if (etrs89X.Length != etrs89Y.Length || etrs89X.Length != etrs89Z.Length)
            {
                _logger?.LogError("Input arrays have different lengths.");
                localRechts = Array.Empty<double>();
                localHoch = Array.Empty<double>();
                h = Array.Empty<double>();
                return false;
            }
            // Geocentric to Geodetic conversion
            var (etrs89Lat, etrs89Lon, etrs89Hell) = ConvertArrays(GRS80Geoc_Reverse, etrs89X, etrs89Y, etrs89Z);
            // GCG2016 geoid Heights
            var geoid = Geoid.GetBKGBinaryGeoidHeights(etrs89Lat, etrs89Lon);
            // ETRS89 ellipsoid heights
            h = new double[geoid.Length];
            for (var i = 0; i < h.Length; i++)
                h[i] = etrs89Hell[i] - geoid[i];
            // Geodetic to EGBT22_Local conversion
            (localRechts, localHoch) = ConvertArrays(EGBT22_Local_Forward, etrs89Lat, etrs89Lon);

            //_logger?.LogInformation("Conversion from ETRS89_Geoc to EGBT22_Local successful.");
            return true;
        }
        public static bool ETRS89_Geoc_to_ETRS89_UTM33(double[] etrs89X, double[] etrs89Y, double[] etrs89Z, out double[] easting, out double[] northing, out double[] h)
        {
            if (etrs89X.Length != etrs89Y.Length || etrs89X.Length != etrs89Z.Length)
            {
                _logger?.LogError("Input arrays have different lengths.");
                easting = Array.Empty<double>();
                northing = Array.Empty<double>();
                h = Array.Empty<double>();
                return false;
            }
            // Geocentric to Geodetic conversion
            var (etrs89Lat, etrs89Lon, etrs89Hell) = ConvertArrays(GRS80Geoc_Reverse, etrs89X, etrs89Y, etrs89Z);
            // GCG2016 geoid Heights
            var geoid = Geoid.GetBKGBinaryGeoidHeights(etrs89Lat, etrs89Lon);
            // ETRS89 ellipsoid heights
            h = new double[geoid.Length];
            for (var i = 0; i < h.Length; i++)
                h[i] = etrs89Hell[i] - geoid[i];
            // Geodetic to EGBT22_Local conversion
            (easting, northing) = ConvertArrays(GRS80UTM33_Forward, etrs89Lat, etrs89Lon);

            //_logger?.LogInformation("Conversion from ETRS89_Geoc to ETRS89_UTM33 successful.");
            return true;
        }
        public static bool ETRS89_Geoc_to_DBRef_GK5(double[] etrs89X, double[] etrs89Y, double[] etrs89Z, out double[] gk5Rechts, out double[] gk5Hoch, out double[] h)
        {
            if (etrs89X.Length != etrs89Y.Length || etrs89X.Length != etrs89Z.Length)
            {
                _logger?.LogError("Input arrays have different lengths.");
                gk5Rechts = Array.Empty<double>();
                gk5Hoch = Array.Empty<double>();
                h = Array.Empty<double>();
                return false;
            }
            // Geocentric to Geodetic conversion
            var (etrs89Lat, etrs89Lon, etrs89Hell) = ConvertArrays(GRS80Geoc_Reverse, etrs89X, etrs89Y, etrs89Z);
            // GCG2016 geoid Heights
            var geoid = Geoid.GetBKGBinaryGeoidHeights(etrs89Lat, etrs89Lon);
            // ETRS89 ellipsoid heights
            h = new double[geoid.Length];
            for (var i = 0; i < h.Length; i++)
                h[i] = etrs89Hell[i] - geoid[i];
            // ETRS89 geocentric to DBREF geocentric transformation
            var (dbrefX, dbrefY, dbrefZ) = ConvertArrays(Transformation.DbrefToEtrs89Inv, etrs89X, etrs89Y, etrs89Z);
            // Geocentric to geodetic conversion
            var (dbrefLat, dbrefLon, _) = ConvertArrays(BesselGeoc_Reverse, dbrefX, dbrefY, dbrefZ);
            // Geodetic to GK5 conversion
            (gk5Rechts, gk5Hoch) = ConvertArrays(BesselGK5_Forward, dbrefLat, dbrefLon);

            //_logger?.LogInformation("Conversion from ETRS89_Geoc to DBRef_GK5 successful.");
            return true;
        }
        public static bool ETRS89_Geoc_to_ETRS89_Geod_3D(double[] etrs89X, double[] etrs89Y, double[] etrs89Z, out double[] etrs89Lat, out double[] etrs89Lon, out double[] etrs89Hell)
        {
            if (etrs89X.Length != etrs89Y.Length || etrs89X.Length != etrs89Z.Length)
            {
                _logger?.LogError("Input arrays have different lengths.");
                etrs89Lat = Array.Empty<double>();
                etrs89Lon = Array.Empty<double>();
                etrs89Hell = Array.Empty<double>();
                return false;
            }
            // Geocentric to Geodetic conversion
            (etrs89Lat, etrs89Lon, etrs89Hell) = ConvertArrays(GRS80Geoc_Reverse, etrs89X, etrs89Y, etrs89Z);

            //_logger?.LogInformation("Conversion from ETRS89_Geoc to ETRS89_Geod_3D successful.");
            return true;
        }

        public static (double localRechts, double localHoch, double h) ETRS89_Geoc_to_EGBT22_Local(double etrs89X, double etrs89Y, double etrs89Z)
        {
            // Geocentric to Geodetic conversion
            var (etrs89Lat, etrs89Lon, etrs89Hell) = GRS80Geoc_Reverse(etrs89X, etrs89Y, etrs89Z);
            // GCG2016 geoid Heights
            var geoid = Geoid.GetBKGBinaryGeoidHeight(etrs89Lat, etrs89Lon);
            // ETRS89 ellipsoid heights
            var h = etrs89Hell - geoid;
            // Geodetic to EGBT22_Local conversion
            var (localRechts, localHoch) = EGBT22_Local_Forward(etrs89Lat, etrs89Lon);
            return (localRechts, localHoch, h);
        }
        public static (double easting, double northing, double h) ETRS89_Geoc_to_ETRS89_UTM33(double etrs89X, double etrs89Y, double etrs89Z)
        {
            // Geocentric to Geodetic conversion
            var (etrs89Lat, etrs89Lon, etrs89Hell) = GRS80Geoc_Reverse(etrs89X, etrs89Y, etrs89Z);
            // GCG2016 geoid Heights
            var geoid = Geoid.GetBKGBinaryGeoidHeight(etrs89Lat, etrs89Lon);
            // ETRS89 ellipsoid heights
            var h = etrs89Hell - geoid;
            // Geodetic to EGBT22_Local conversion
            var (easting, northing) = GRS80UTM33_Forward(etrs89Lat, etrs89Lon);

            //_logger?.LogInformation("Conversion from ETRS89_Geoc to ETRS89_UTM33 successful.");
            return (easting, northing, h);
        }
        public static (double gk5Rechts, double gk5Hoch, double h) ETRS89_Geoc_to_DBRef_GK5(double etrs89X, double etrs89Y, double etrs89Z)
        {
            // Geocentric to Geodetic conversion
            var (etrs89Lat, etrs89Lon, etrs89Hell) = GRS80Geoc_Reverse(etrs89X, etrs89Y, etrs89Z);
            // GCG2016 geoid Heights
            var geoid = Geoid.GetBKGBinaryGeoidHeight(etrs89Lat, etrs89Lon);
            // ETRS89 ellipsoid heights
            var h = etrs89Hell - geoid;
            // ETRS89 geocentric to DBREF geocentric transformation
            var (dbrefX, dbrefY, dbrefZ) = Transformation.DbrefToEtrs89Inv(etrs89X, etrs89Y, etrs89Z);
            // Geocentric to geodetic conversion
            var (dbrefLat, dbrefLon, _) = BesselGeoc_Reverse(dbrefX, dbrefY, dbrefZ);
            // Geodetic to GK5 conversion
            var (gk5Rechts, gk5Hoch) = BesselGK5_Forward(dbrefLat, dbrefLon);

            //_logger?.LogInformation("Conversion from ETRS89_Geoc to DBRef_GK5 successful.");
            return (gk5Rechts, gk5Hoch, h);
        }
        public static (double etrs89Lat, double etrs89Lon, double etrs89Hell) ETRS89_Geoc_to_ETRS89_Geod_3D(double etrs89X, double etrs89Y, double etrs89Z)
        {
            // Geocentric to Geodetic conversion
            return GRS80Geoc_Reverse(etrs89X, etrs89Y, etrs89Z);
        }

        #endregion

        #region ETRS89_Geod_3D
        public static bool ETRS89_Geod_3D_to_EGBT22_Local(double[] etrs89Lat, double[] etrs89Lon, double[] etrs89Hell, out double[] localRechts, out double[] localHoch, out double[] h)
        {
            if (etrs89Lat.Length != etrs89Lon.Length || etrs89Lat.Length != etrs89Hell.Length)
            {
                _logger?.LogError("Input arrays have different lengths.");
                localRechts = Array.Empty<double>();
                localHoch = Array.Empty<double>();
                h = Array.Empty<double>();
                return false;
            }
            // GCG2016 geoid Heights
            var geoid = Geoid.GetBKGBinaryGeoidHeights(etrs89Lat, etrs89Lon);
            // ETRS89 ellipsoid heights
            h = new double[geoid.Length];
            for (var i = 0; i < h.Length; i++)
                h[i] = etrs89Hell[i] - geoid[i];
            // Geodetic to EGBT22_Local conversion
            (localRechts, localHoch) = ConvertArrays(EGBT22_Local_Forward, etrs89Lat, etrs89Lon);

            //_logger?.LogInformation("Conversion from ETRS89_Geod_3D to EGBT22_Local successful.");
            return true;
        }
        public static bool ETRS89_Geod_3D_to_ETRS89_UTM33(double[] etrs89Lat, double[] etrs89Lon, double[] etrs89Hell, out double[] easting, out double[] northing, out double[] h)
        {
            if (etrs89Lat.Length != etrs89Lon.Length || etrs89Lat.Length != etrs89Hell.Length)
            {
                _logger?.LogError("Input arrays have different lengths.");
                easting = Array.Empty<double>();
                northing = Array.Empty<double>();
                h = Array.Empty<double>();
                return false;
            }
            // GCG2016 geoid Heights
            var geoid = Geoid.GetBKGBinaryGeoidHeights(etrs89Lat, etrs89Lon);
            // ETRS89 ellipsoid heights
            h = new double[geoid.Length];
            for (var i = 0; i < h.Length; i++)
                h[i] = etrs89Hell[i] - geoid[i];
            // Geodetic to EGBT22_Local conversion
            (easting, northing) = ConvertArrays(GRS80UTM33_Forward, etrs89Lat, etrs89Lon);

            //_logger?.LogInformation("Conversion from ETRS89_Geod_3D to ETRS89_UTM33 successful.");
            return true;
        }
        public static bool ETRS89_Geod_3D_to_DBRef_GK5(double[] etrs89Lat, double[] etrs89Lon, double[] etrs89Hell, out double[] gk5Rechts, out double[] gk5Hoch, out double[] h)
        {
            if (etrs89Lat.Length != etrs89Lon.Length || etrs89Lat.Length != etrs89Hell.Length)
            {
                _logger?.LogError("Input arrays have different lengths.");
                gk5Rechts = Array.Empty<double>();
                gk5Hoch = Array.Empty<double>();
                h = Array.Empty<double>();
                return false;
            }
            // GCG2016 geoid Heights
            var geoid = Geoid.GetBKGBinaryGeoidHeights(etrs89Lat, etrs89Lon);
            // ETRS89 ellipsoid heights
            h = new double[geoid.Length];
            for (var i = 0; i < h.Length; i++)
                h[i] = etrs89Hell[i] - geoid[i];
            // ETRS89 geodetic to geocentric conversion
            var (etrs89X, etrs89Y, etrs89Z) = ConvertArrays(GRS80Geoc_Forward, etrs89Lat, etrs89Lon, etrs89Hell);
            // ETRS89 geocentric to DBREF geocentric transformation
            var (dbrefX, dbrefY, dbrefZ) = ConvertArrays(Transformation.DbrefToEtrs89Inv, etrs89X, etrs89Y, etrs89Z);
            // Geocentric to geodetic conversion
            var (dbrefLat, dbrefLon, _) = ConvertArrays(BesselGeoc_Reverse, dbrefX, dbrefY, dbrefZ);
            // Geodetic to GK5 conversion
            (gk5Rechts, gk5Hoch) = ConvertArrays(BesselGK5_Forward, dbrefLat, dbrefLon);
            //_logger?.LogInformation("Conversion from ETRS89_Geod_3D to DBRef_GK5 successful.");
            return true;
        }
        //public static bool ETRS89_Geod_3D_to_DBRef_GK52(double[] etrs89Lat, double[] etrs89Lon, double[] etrs89Hell, out double[] gk5Rechts, out double[] gk5Hoch, out double[] h)
        //{
        //    if (etrs89Lat.Length != etrs89Lon.Length || etrs89Lat.Length != etrs89Hell.Length)
        //    {
        //        _logger?.LogError("Input arrays have different lengths.");
        //        gk5Rechts = Array.Empty<double>();
        //        gk5Hoch = Array.Empty<double>();
        //        h = Array.Empty<double>();
        //        return false;
        //    }
        //    // GCG2016 geoid Heights
        //    var geoid = Geoid.GetBKGBinaryGeoidHeights(etrs89Lat, etrs89Lon);
        //    // ETRS89 ellipsoid heights
        //    h = new double[geoid.Length];
        //    for (var i = 0; i < h.Length; i++)
        //        h[i] = etrs89Hell[i] - geoid[i];
        //    // ETRS89 geodetic to geocentric conversion
        //    var (etrs89X, etrs89Y, etrs89Z) = ConvertArrays(GRS80Geoc_Forward, etrs89Lat, etrs89Lon, etrs89Hell);
        //    // ETRS89 geocentric to DBREF geocentric transformation
        //    var (dbrefX, dbrefY, dbrefZ) = ConvertArrays(Transformation.Etrs89ToDbref, etrs89X, etrs89Y, etrs89Z);
        //    // Geocentric to geodetic conversion
        //    var (dbrefLat, dbrefLon, _) = ConvertArrays(BesselGeoc_Reverse, dbrefX, dbrefY, dbrefZ);
        //    // Geodetic to GK5 conversion
        //    (gk5Rechts, gk5Hoch) = ConvertArrays(BesselGK5_Forward, dbrefLat, dbrefLon);
        //    //_logger?.LogInformation("Conversion from ETRS89_Geod_3D to DBRef_GK5 successful.");
        //    return true;
        //}
        public static bool ETRS89_Geod_3D_to_ETRS89_Geoc(double[] etrs89Lat, double[] etrs89Lon, double[] etrs89Hell, out double[] etrs89X, out double[] etrs89Y, out double[] etrs89Z)
        {
            if (etrs89Lat.Length != etrs89Lon.Length || etrs89Lat.Length != etrs89Hell.Length)
            {
                _logger?.LogError("Input arrays have different lengths.");
                etrs89X = Array.Empty<double>();
                etrs89Y = Array.Empty<double>();
                etrs89Z = Array.Empty<double>();
                return false;
            }
            // Geodetic to Geocentric conversion
            (etrs89X, etrs89Y, etrs89Z) = ConvertArrays(GRS80Geoc_Forward, etrs89Lat, etrs89Lon, etrs89Hell);

            //_logger?.LogInformation("Conversion from ETRS89_Geod_3D to ETRS89_Geoc successful.");
            return true;
        }

        public static (double localRechts, double localHoch, double h) ETRS89_Geod_3D_to_EGBT22_Local(double etrs89Lat, double etrs89Lon, double etrs89Hell)
        {
            // GCG2016 geoid Heights
            var geoid = Geoid.GetBKGBinaryGeoidHeight(etrs89Lat, etrs89Lon);
            // ETRS89 ellipsoid heights
            var h = etrs89Hell - geoid;
            // Geodetic to EGBT22_Local conversion
            var (localRechts, localHoch) = EGBT22_Local_Forward(etrs89Lat, etrs89Lon);

            //_logger?.LogInformation("Conversion from ETRS89_Geod_3D to EGBT22_Local successful.");
            return (localRechts, localHoch, h);
        }
        public static (double easting, double northing, double h) ETRS89_Geod_3D_to_ETRS89_UTM33(double etrs89Lat, double etrs89Lon, double etrs89Hell)
        {
            // GCG2016 geoid Heights
            var geoid = Geoid.GetBKGBinaryGeoidHeight(etrs89Lat, etrs89Lon);
            // ETRS89 ellipsoid heights
            var h = etrs89Hell - geoid;
            // Geodetic to EGBT22_Local conversion
            var (easting, northing) = GRS80UTM33_Forward(etrs89Lat, etrs89Lon);

            //_logger?.LogInformation("Conversion from ETRS89_Geod_3D to ETRS89_UTM33 successful.");
            return (easting, northing, h);
        }
        public static (double gk5Rechts, double gk5Hoch, double h) ETRS89_Geod_3D_to_DBRef_GK5(double etrs89Lat, double etrs89Lon, double etrs89Hell)
        {
            // GCG2016 geoid Heights
            var geoid = Geoid.GetBKGBinaryGeoidHeight(etrs89Lat, etrs89Lon);
            // ETRS89 ellipsoid heights
            var h = etrs89Hell - geoid;
            // ETRS89 geodetic to geocentric conversion
            var (etrs89X, etrs89Y, etrs89Z) = GRS80Geoc_Forward(etrs89Lat, etrs89Lon, etrs89Hell);
            // ETRS89 geocentric to DBREF geocentric transformation
            var (dbrefX, dbrefY, dbrefZ) = Transformation.DbrefToEtrs89Inv(etrs89X, etrs89Y, etrs89Z);
            // Geocentric to geodetic conversion
            var (dbrefLat, dbrefLon, _) = BesselGeoc_Reverse(dbrefX, dbrefY, dbrefZ);
            // Geodetic to GK5 conversion
            var (gk5Rechts, gk5Hoch) = BesselGK5_Forward(dbrefLat, dbrefLon);
            //_logger?.LogInformation("Conversion from ETRS89_Geod_3D to DBRef_GK5 successful.");
            return (gk5Rechts, gk5Hoch, h);
        }
        //public static (double gk5Rechts, double gk5Hoch, double h) ETRS89_Geod_3D_to_DBRef_GK52(double etrs89Lat, double etrs89Lon, double etrs89Hell)
        //{
        //    // GCG2016 geoid Heights
        //    var geoid = Geoid.GetBKGBinaryGeoidHeight(etrs89Lat, etrs89Lon);
        //    // ETRS89 ellipsoid heights
        //    var h = etrs89Hell - geoid;
        //    // ETRS89 geodetic to geocentric conversion
        //    var (etrs89X, etrs89Y, etrs89Z) = GRS80Geoc_Forward(etrs89Lat, etrs89Lon, etrs89Hell);
        //    // ETRS89 geocentric to DBREF geocentric transformation
        //    var (dbrefX, dbrefY, dbrefZ) = Transformation.DbrefToEtrs89Inv(etrs89X, etrs89Y, etrs89Z);
        //    // Geocentric to geodetic conversion
        //    var (dbrefLat, dbrefLon, _) = BesselGeoc_Reverse(dbrefX, dbrefY, dbrefZ);
        //    // Geodetic to GK5 conversion
        //    var (gk5Rechts, gk5Hoch) = BesselGK5_Forward(dbrefLat, dbrefLon);
        //    //_logger?.LogInformation("Conversion from ETRS89_Geod_3D to DBRef_GK5 successful.");
        //    return (gk5Rechts, gk5Hoch, h);
        //}
        public static (double etrs89X, double etrs89Y, double etrs89Z) ETRS89_Geod_3D_to_ETRS89_Geoc(double etrs89Lat, double etrs89Lon, double etrs89Hell)
        {
            // Geodetic to Geocentric conversion
            return GRS80Geoc_Forward(etrs89Lat, etrs89Lon, etrs89Hell);
        }

        #endregion

    }
}
