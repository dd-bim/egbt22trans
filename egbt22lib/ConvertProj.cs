//using System;

//using Microsoft.Extensions.Logging;

//using LowDistortionProjection;

//namespace egbt22lib
//{
//    public static class ConvertProj
//    {
//        private static ILogger? _logger;

//        public static void InitializeLogger(ILogger logger)
//        {
//            _logger = logger;
//        }

//        private static readonly Conversion 
//            dBRef_GK5_to_EGBT22_Local, 
//            dBRef_GK5_to_ETRS89_Geod_3D,
//            eGBT22_Local_to_ETRS89_Geod_3D,
//            eGBT22_Local_to_DBRef_GK5,
//            eTRS89_Geod_3D_to_DBRef_GK5,
//            eTRS89_Geod_3D_to_DBRef_Geod_3D;

//        static ConvertProj()
//        {
//            var DBRef_GK5 = Srs.Get(5685);
//            var DBRef_Geod_3D = Srs.Get(5830);
//            var ETRS89_Geod_3D = Srs.Get(4937);
//            var EGBT22_Local = Srs.GetLocalTMSystemWithScale(4258, 50.8247, 13.9027, 1.0000346, 10000, 50000); // EGBT_LDP

//            dBRef_GK5_to_EGBT22_Local = new Conversion(DBRef_GK5, EGBT22_Local, "+proj=pipeline " +
//            "+step +inv +proj=tmerc +lat_0=0 +lon_0=15 +k=1 +x_0=5500000 +y_0=0 +ellps=bessel " +
//            "+step +proj=cart +ellps=bessel " +
//            "+step +proj=helmert +x=584.9636 +y=107.7175 +z=413.8067 +rx=-1.1155 +ry=-0.2824 +rz=3.1384 +s=7.9922 +exact +convention=coordinate_frame " +
//            "+step +inv +proj=cart +ellps=GRS80 " +
//            "+step +proj=tmerc +lat_0=50.8247 +lon_0=13.9027 +k=1.0000346 +x_0=10000 +y_0=50000 +ellps=GRS80");

//            dBRef_GK5_to_ETRS89_Geod_3D = new Conversion(DBRef_GK5, ETRS89_Geod_3D, "+proj=pipeline " +
//            "+step +inv +proj=tmerc +lat_0=0 +lon_0=15 +k=1 +x_0=5500000 +y_0=0 +ellps=bessel " +
//            "+step +proj=cart +ellps=bessel " +
//            "+step +proj=helmert +x=584.9636 +y=107.7175 +z=413.8067 +rx=-1.1155 +ry=-0.2824 +rz=3.1384 +s=7.9922 +exact +convention=coordinate_frame " +
//            "+step +inv +proj=cart +ellps=GRS80 " +
//            "+step +proj=unitconvert +xy_in=rad +z_in=m +xy_out=deg +z_out=m " +
//            "+step +proj=axisswap +order=2,1");

//            eGBT22_Local_to_ETRS89_Geod_3D = new Conversion(EGBT22_Local, ETRS89_Geod_3D);

//            eGBT22_Local_to_DBRef_GK5 = new Conversion(EGBT22_Local, DBRef_GK5, "+proj=pipeline " +
//            "+step +inv +proj=tmerc +lat_0=50.8247 +lon_0=13.9027 +k=1.0000346 +x_0=10000 +y_0=50000 +ellps=GRS80 " +
//            "+step +proj=cart +ellps=GRS80 " +
//            "+step +inv +proj=helmert +x=584.9636 +y=107.7175 +z=413.8067 +rx=-1.1155 +ry=-0.2824 +rz=3.1384 +s=7.9922 +exact +convention=coordinate_frame " +
//            "+step +inv +proj=cart +ellps=bessel " +
//            "+step +proj=tmerc +lat_0=0 +lon_0=15 +k=1 +x_0=5500000 +y_0=0 +ellps=bessel");

//            eTRS89_Geod_3D_to_DBRef_GK5 = new Conversion(ETRS89_Geod_3D, DBRef_GK5, "+proj=pipeline " +
//            "+step +proj=axisswap +order=2,1 " +
//            "+step +proj=unitconvert +xy_in=deg +z_in=m +xy_out=rad +z_out=m " +
//            "+step +proj=cart +ellps=GRS80 " +
//            "+step +inv +proj=helmert +x=584.9636 +y=107.7175 +z=413.8067 +rx=-1.1155 +ry=-0.2824 +rz=3.1384 +s=7.9922 +exact +convention=coordinate_frame " +
//            "+step +inv +proj=cart +ellps=bessel " +
//            "+step +proj=tmerc +lat_0=0 +lon_0=15 +k=1 +x_0=5500000 +y_0=0 +ellps=bessel");

//            eTRS89_Geod_3D_to_DBRef_Geod_3D = new Conversion(ETRS89_Geod_3D, DBRef_Geod_3D, "+proj=pipeline " +
//            "+step +proj=axisswap +order=2,1 " +
//            "+step +proj=unitconvert +xy_in=deg +z_in=m +xy_out=rad +z_out=m " +
//            "+step +proj=cart +ellps=GRS80 " +
//            "+step +inv +proj=helmert +x=584.9636 +y=107.7175 +z=413.8067 +rx=-1.1155 +ry=-0.2824 +rz=3.1384 +s=7.9922 +exact +convention=coordinate_frame " +
//            "+step +inv +proj=cart +ellps=bessel " +
//            "+step +proj=unitconvert +xy_in=rad +z_in=m +xy_out=deg +z_out=m " +
//            "+step +proj=axisswap +order=2,1");
//        }

//        //public static readonly Srs DBRef_Geoc = Srs.Get(5828);

//        //public static readonly Srs ETRS89_Geoc = Srs.Get(4936);

//        //public const string DBRef_Geod_3D_to_ETRS89_Geod_3D_options = "+proj=pipeline " +
//        //    "+step +proj=axisswap +order=2,1 " +
//        //    "+step +proj=unitconvert +xy_in=deg +z_in=m +xy_out=rad +z_out=m " +
//        //    "+step +proj=cart +ellps=bessel " +
//        //    "+step +proj=helmert +x=584.9636 +y=107.7175 +z=413.8067 +rx=-1.1155 +ry=-0.2824 +rz=3.1384 +s=7.9922 +exact +convention=coordinate_frame " +
//        //    "+step +inv +proj=cart +ellps=GRS80 " +
//        //    "+step +proj=unitconvert +xy_in=rad +z_in=m +xy_out=deg +z_out=m " +
//        //    "+step +proj=axisswap +order=2,1";
//        //public const string DBRef_Geoc_to_ETRS89_Geoc_options =
//        //    "+proj=helmert +x=584.9636 +y=107.7175 +z=413.8067 +rx=-1.1155 +ry=-0.2824 +rz=3.1384 +s=7.9922 +exact +convention=coordinate_frame";

//        //public const string ETRS89_Geoc_to_DBRef_Geoc_options = "+proj=pipeline " +
//        //    "+step +inv +proj=helmert +x=584.9636 +y=107.7175 +z=413.8067 +rx=-1.1155 +ry=-0.2824 +rz=3.1384 +s=7.9922 +exact +convention=coordinate_frame";

//        public static bool DBRef_GK5_to_EGBT22_Local(double[] gk5Rechts, double[] gk5Hoch, double[] h, out double[] localRechts, out double[] localHoch)
//        {
//            if (gk5Rechts.Length != gk5Hoch.Length || gk5Rechts.Length != h.Length)
//            {
//                _logger?.LogWarning("Input arrays have different lengths.");
//                localRechts = Array.Empty<double>();
//                localHoch = Array.Empty<double>();
//                return false;
//            }
//            // GK5 to ETRS89 geodetic conversion and transformation 
//            var (etrs89Lat, etrs89Lon, etrs89Hell) = dBRef_GK5_to_ETRS89_Geod_3D.Convert(gk5Rechts, gk5Hoch, h);
//            // GCG2016 geoid Heights
//            var geoid = Geoid.GetBKGBinaryGeoidHeights(etrs89Lat, etrs89Lon);
//            // ETRS89 ellipsoid heights (exact)
//            for (int i = 0; i < etrs89Hell.Length; i++)
//                etrs89Hell[i] = geoid[i] + h[i];
//            // DBREF ellipsoidal heights through ETRS89 to DBREF geodetic conversion and transformation 
//            var (_, _, dbRefHell) = eTRS89_Geod_3D_to_DBRef_Geod_3D.Convert(etrs89Lat, etrs89Lon, etrs89Hell);
//            // DBREF GK5 to EGBT22 (ETRS89) Local conversion and transformation
//            (localRechts, localHoch, _) = dBRef_GK5_to_EGBT22_Local.Convert(gk5Rechts, gk5Hoch, dbRefHell);

//            _logger?.LogInformation("Conversion from DBRef_GK5 to EGBT22_Local successful.");
//            return true;
//        }

//        public static bool DBRef_GK5_to_ETRS89_Geod_3D(double[] gk5Rechts, double[] gk5Hoch, double[] h, out double[] etrs89Lat, out double[] etrs89Lon, out double[] hEll)
//        {
//            if (gk5Rechts.Length != gk5Hoch.Length || gk5Rechts.Length != h.Length)
//            {
//                _logger?.LogWarning("Input arrays have different lengths.");
//                etrs89Lat = Array.Empty<double>();
//                etrs89Lon = Array.Empty<double>();
//                hEll = Array.Empty<double>();
//                return false;
//            }
//            // GK5 to ETRS89 geodetic conversion and transformation 
//            (etrs89Lat, etrs89Lon, hEll) = dBRef_GK5_to_ETRS89_Geod_3D.Convert(gk5Rechts, gk5Hoch, h);
//            // GCG2016 geoid Heights
//            var geoid = Geoid.GetBKGBinaryGeoidHeights(etrs89Lat, etrs89Lon);
//            // ETRS89 ellipsoid heights (exact)
//            for (int i = 0; i < hEll.Length; i++)
//                hEll[i] = geoid[i] + h[i];
//            // DBREF ellipsoidal heights through ETRS89 to DBREF geodetic conversion and transformation 
//            var (_, _, dbRefHell) = eTRS89_Geod_3D_to_DBRef_Geod_3D.Convert(etrs89Lat, etrs89Lon, hEll);
//            // DBREF GK5 to EGBT22 (ETRS89) Local conversion and transformation
//            (etrs89Lat, etrs89Lon, _) = dBRef_GK5_to_ETRS89_Geod_3D.Convert(gk5Rechts, gk5Hoch, dbRefHell);

//            _logger?.LogInformation("Conversion from DBRef_GK5 to EGBT22_Local successful.");
//            return true;
//        }

//        public static bool EGBT22_Local_to_DBRef_GK5(double[] localRechts, double[] localHoch, double[] h, out double[] gk5Rechts, out double[] gk5Hoch)
//        {
//            if (localRechts.Length != localHoch.Length || localRechts.Length != h.Length)
//            {
//                _logger?.LogWarning("Input arrays have different lengths.");
//                gk5Rechts = Array.Empty<double>();
//                gk5Hoch = Array.Empty<double>();
//                return false;
//            }
//            // Local to Geodetic conversion
//            var (etrs89Lat, etrs89Lon) = eGBT22_Local_to_ETRS89_Geod_3D.Convert(localRechts, localHoch);
//            // GCG2016 geoid Heights
//            var geoid = Geoid.GetBKGBinaryGeoidHeights(etrs89Lat, etrs89Lon);
//            // ETRS89 ellipsoid heights
//            var hell = new double[geoid.Length];
//            for (int i = 0; i < hell.Length; i++)
//                hell[i] = geoid[i] + h[i];
//            // EGBT22(ETRS89) Local to DBREF GK5 conversion and transformation
//            (gk5Rechts, gk5Hoch, _) = eGBT22_Local_to_DBRef_GK5.Convert(localRechts, localHoch, hell);

//            _logger?.LogInformation("Conversion from EGBT22_Local to DBRef_GK5 successful.");
//            return true;
//        }

//        public static bool ETRS89_Geod_3D_to_DBRef_GK5(double[] etrs89Lat, double[] etrs89Lon, double[] hEll, out double[] gk5Rechts, out double[] gk5Hoch, out double[] h)
//        {
//            if (etrs89Lat.Length != etrs89Lon.Length || etrs89Lat.Length != hEll.Length)
//            {
//                _logger?.LogWarning("Input arrays have different lengths.");
//                gk5Rechts = Array.Empty<double>();
//                gk5Hoch = Array.Empty<double>();
//                h = Array.Empty<double>();
//                return false;
//            }
//            // GCG2016 geoid Heights
//            var geoid = Geoid.GetBKGBinaryGeoidHeights(etrs89Lat, etrs89Lon);
//            // ETRS89 ellipsoid heights
//            h = new double[geoid.Length];
//            for (int i = 0; i < h.Length; i++)
//                h[i] = hEll[i] - geoid[i];
//            // EGBT22(ETRS89) Local to DBREF GK5 conversion and transformation
//            (gk5Rechts, gk5Hoch, _) = eTRS89_Geod_3D_to_DBRef_GK5.Convert(etrs89Lat, etrs89Lon, hEll);

//            _logger?.LogInformation("Conversion from EGBT22_Local to DBRef_GK5 successful.");
//            return true;
//        }

//    }
//}
