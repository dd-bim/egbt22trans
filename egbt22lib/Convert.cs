using System;
using System.Collections.Generic;
using static egbt22lib.Transformations.Defined;
using static egbt22lib.Conversions.Defined;


namespace egbt22lib
{
    public static class Convert
    {
        [Flags]
        public enum CRS
        {
            Geod = 0,
            Geoc = 1,
            EGBT_LDP = 2,
            UTM33 = 4,
            GK5 = 8,
            Conversion = Geod | Geoc | EGBT_LDP | UTM33 | GK5,
            ETRS89 = 16,
            DB_Ref = 32,
            EGBT22 = 64,
            ETRS89_DREF91 = 128,
            ETRS89_CZ = 256,
            Datum = ETRS89 | DB_Ref | EGBT22 | ETRS89_DREF91 | ETRS89_CZ,

            EGBT22_EGBT_LDP = EGBT22 | EGBT_LDP,
            EGBT22_Geod = EGBT22 | Geod,
            EGBT22_Geoc = EGBT22 | Geoc,
            ETRS89_DREF91_UTM33 = ETRS89_DREF91 | UTM33,
            ETRS89_DREF91_Geod = ETRS89_DREF91 | Geod,
            ETRS89_DREF91_Geoc = ETRS89_DREF91 | Geoc,
            ETRS89_CZ_Geod = ETRS89_CZ | Geod,
            ETRS89_CZ_Geoc = ETRS89_CZ | Geoc,
            DB_Ref_GK5 = DB_Ref | GK5,
            DB_Ref_Geod = DB_Ref | Geod,
            DB_Ref_Geoc = DB_Ref | Geoc
        }

        public enum VRS
        {
            None,
            Normal,
            Ellipsoidal
        }

        /// <summary>
        /// Represents a collection of predefined Coordinate Reference System (CRS) identifiers as string values.
        /// </summary>
        /// <remarks>This static readonly array contains the string representations of various CRS
        /// identifiers. These identifiers are derived from the <see cref="CRS"/> enumeration by calling their <see
        /// cref="object.ToString"/> method. The collection includes commonly used CRS definitions such as EGBT22,
        /// ETRS89, and DB_Ref variants.</remarks>
        public static readonly string[] Defined_CRS =
        {
            CRS.EGBT22_EGBT_LDP.ToString(),
            CRS.EGBT22_Geod.ToString(),
            CRS.EGBT22_Geoc.ToString(),
            CRS.ETRS89_DREF91_UTM33.ToString(),
            "ETRS89_DREF91_Geod", // Conversion seems not working correctly
            CRS.ETRS89_DREF91_Geoc.ToString(),
            CRS.ETRS89_CZ_Geod.ToString(),
            CRS.ETRS89_CZ_Geoc.ToString(),
            CRS.DB_Ref_GK5.ToString(),
            CRS.DB_Ref_Geod.ToString(),
            CRS.DB_Ref_Geoc.ToString(),
        };

        /// <summary>
        /// Gets a predefined list of 2D Coordinate Reference System (CRS) identifiers as string representations.
        /// </summary>
        /// <remarks>This array contains the string representations of specific 2D CRS definitions,  which
        /// can be used for spatial reference purposes. The identifiers are derived  from the <see cref="CRS"/>
        /// enumeration.</remarks>
        public static readonly string[] Defined_CRS_2D =
        {
            CRS.EGBT22_EGBT_LDP.ToString(),
            CRS.EGBT22_Geod.ToString(),
            CRS.ETRS89_DREF91_UTM33.ToString(),
            "ETRS89_DREF91_Geod", // Conversion seems not working correctly
            CRS.DB_Ref_GK5.ToString(),
            CRS.DB_Ref_Geod.ToString(),
        };

        /// <summary>
        ///     Defined_VRS is a static readonly array of strings that represents the predefined supported
        ///     Virtual Reference System (VRS) types in the egbt22lib.Convert namespace.
        ///     It enumerates different VRS modes, such as None, Normal, and Ellipsoidal,
        ///     which are used to specify the type of vertical reference in coordinate transformations.
        /// </summary>
        public static readonly string[] Defined_VRS =
        {
            VRS.None.ToString(),
            VRS.Normal.ToString(),
            VRS.Ellipsoidal.ToString(),
        };

        private static (double lat, double lon, double ellH) ETRS89_Geod_Normal_to_Ellipsoidal(double lat, double lon,
            double h)
        {
            double ellH = h + Geoid.GetBKGBinaryGeoidHeight(lat, lon);
            return (lat, lon, ellH);
        }

        private static (double lat, double lon, double h) ETRS89_Geod_Ellipsoidal_to_Normal(double lat, double lon,
            double ellH)
        {
            double h = ellH - Geoid.GetBKGBinaryGeoidHeight(lat, lon);
            return (lat, lon, h);
        }

        private static (double lat, double lon, double ellH) EGBT22_Geod_Normal_to_Ellipsoidal(double lat, double lon,
            double h)
        {
            double th = h;
            double diff = double.MaxValue;
            while (diff > TOLRAD)
            {
                double x;
                double y;
                double z;
                (x, y, z) = GC_GRS80.Forward(lat, lon, th);
                (x, y, z) = Trans_Datum_ETRS89_DREF91_to_EGBT22.Reverse(x, y, z);
                double elat;
                double elon;
                (elat, elon, _) = GC_GRS80.Reverse(x, y, z);
                double eh = h + Geoid.GetBKGBinaryGeoidHeight(elat, elon);
                (x, y, z) = GC_GRS80.Forward(elat, elon, eh);
                (x, y, z) = Trans_Datum_ETRS89_DREF91_to_EGBT22.Forward(x, y, z);
                double tlat;
                double tlon;
                (tlat, tlon, th) = GC_Bessel.Reverse(x, y, z);
                diff = Math.Max(Math.Abs(tlat - lat), Math.Abs(tlon - lon));
            }
#if DEBUG
            // Comparison of result to calculation without transformation
            double hcheck = h + Geoid.GetBKGBinaryGeoidHeight(lat, lon);
            Console.WriteLine($"EGBT22_Geod_Normal_to_Ellipsoidal Difference h: {th - hcheck} full:{th} short:{hcheck}");
#endif

            return (lat, lon, th);
        }

        private static (double lat, double lon, double h) EGBT22_Geod_Ellipsoidal_to_Normal(double lat, double lon,
            double ellH)
        {
            (double x, double y, double z) = GC_GRS80.Forward(lat, lon, ellH);
            (x, y, z) = Trans_Datum_ETRS89_DREF91_to_EGBT22.Reverse(x, y, z);
            (double elat, double elon, double eh) = GC_GRS80.Reverse(x, y, z);
            double h = eh - Geoid.GetBKGBinaryGeoidHeight(elat, elon);
#if DEBUG
            // Comparison of result to calculation without transformation
            double hcheck = ellH - Geoid.GetBKGBinaryGeoidHeight(lat, lon);
            Console.WriteLine($"EGBT22_Geod_Ellipsoidal_to_Normal Difference h: {h - hcheck} full:{h} short:{hcheck}");
#endif
            return (lat, lon, h);
        }

        private static (double lat, double lon, double ellH) DBRef_Geod_Normal_to_Ellipsoidal(double lat, double lon,
            double h)
        {
            double th = h;
            double diff = double.MaxValue;
            while (diff > TOLRAD)
            {
                double x;
                double y;
                double z;
                (x, y, z) = GC_Bessel.Forward(lat, lon, th);
                (x, y, z) = Trans_Datum_DBRef_to_ETRS89_DREF91.Forward(x, y, z);
                double elat;
                double elon;
                (elat, elon, _) = GC_GRS80.Reverse(x, y, z);
                double eh = h + Geoid.GetBKGBinaryGeoidHeight(elat, elon);
                (x, y, z) = GC_GRS80.Forward(elat, elon, eh);
                (x, y, z) = Trans_Datum_DBRef_to_ETRS89_DREF91
                    .Reverse(x, y, z); // same transformation reverse to avoid differences through different parameters
                double tlat;
                double tlon;
                (tlat, tlon, th) = GC_Bessel.Reverse(x, y, z);
                diff = Math.Max(Math.Abs(tlat - lat), Math.Abs(tlon - lon));
            }

            return (lat, lon, th);
        }

        private static (double lat, double lon, double h) DBRef_Geod_Ellipsoidal_to_Normal(double lat, double lon,
            double ellH)
        {
            (double x, double y, double z) = GC_Bessel.Forward(lat, lon, ellH);
            (x, y, z) = Trans_Datum_DBRef_to_ETRS89_DREF91.Forward(x, y, z);
            (double elat, double elon, double eh) = GC_GRS80.Reverse(x, y, z);
            double h = eh - Geoid.GetBKGBinaryGeoidHeight(elat, elon);
            return (lat, lon, h);
        }

        private static (double lat, double lon) DBRef_Geod_to_ETRS89_DREF91_Geod_DBRefZero(double lat, double lon)
        {
            (double x, double y, double z) = GC_Bessel.Forward(lat, lon, 0);
            (x, y, z) = Trans_Datum_DBRef_to_ETRS89_DREF91.Forward(x, y, z);
            (double elat, double elon, _) = GC_GRS80.Reverse(x, y, z);
            return (elat, elon);
        }

        private static (double lat, double lon) ETRS89_DREF91_Geod_to_DBRef_Geod_DBRefZero(double lat, double lon)
        {
            double dlat = double.NaN, dlon = double.NaN, dh = double.MaxValue, th = 0;
            while (Math.Abs(dh) > TOLM)
            {
                double x;
                double y;
                double z;
                (x, y, z) = GC_GRS80.Forward(lat, lon, th);
                (x, y, z) = Trans_Datum_ETRS89_DREF91_to_DBRef.Forward(x, y, z);
                (dlat, dlon, dh) = GC_Bessel.Reverse(x, y, z);
                th -= dh;
            }

            return (dlat, dlon);
        }

        private static (double lat, double lon) ETRS89_DREF91_Geod_to_EGBT22_Geod_ETRS89Zero(double lat, double lon)
        {
            (double x, double y, double z) = GC_GRS80.Forward(lat, lon, 0);
            (x, y, z) = Trans_Datum_ETRS89_DREF91_to_EGBT22.Forward(x, y, z);
            (double elat, double elon, _) = GC_GRS80.Reverse(x, y, z);
            return (elat, elon);
        }

        private static (double lat, double lon) EGBT22_Geod_to_ETRS89_DREF91_Geod_ETRS89Zero(double lat, double lon)
        {
            // No iteration needed, as the difference is near to epsilon
            (double tx, double ty, double tz) = GC_GRS80.Forward(lat, lon, 0);
            (tx, ty, tz) = Trans_Datum_ETRS89_DREF91_to_EGBT22.Reverse(tx, ty, tz);
            (double elat, double elon, _) = GC_GRS80.Reverse(tx, ty, tz);
            return (elat, elon);
        }



        /// <summary>
        ///     Attempts to retrieve a coordinate conversion function between two specified coordinate reference systems (CRS)
        ///     and provides additional information about the conversion process.
        /// </summary>
        /// <param name="source">The identifier of the source coordinate reference system.</param>
        /// <param name="target">The identifier of the target coordinate reference system.</param>
        /// <param name="conversion">
        ///     An output parameter that, if the method succeeds, contains the function converting coordinates
        ///     from the source CRS to the target CRS. The function takes x and y coordinates as inputs and
        ///     returns a tuple with the transformed x and y coordinates.
        /// </param>
        /// <param name="info">An output parameter containing additional information about the conversion process or errors.</param>
        /// <param name="isDBREFZero">
        ///     Specifies whether the conversion should consider the DBREF ellipsoidal height as zero.
        ///     Defaults to false if not provided.
        /// </param>
        /// <returns>
        ///     True if the conversion function was successfully retrieved; otherwise, false.
        ///     If false, the `conversion` function will return NaN for all inputs, and `info` will contain an error message.
        /// </returns>
        public static bool GetConversion(string source, string target,
            out Func<double, double, (double x, double y)> conversion, out string info)
        {
            if (!Enum.TryParse(source, out CRS sourceCRS))
            {
                info = $"Source CRS {source} is not supported.";
                conversion = (x, y) => (double.NaN, double.NaN);
                return false;
            }

            if (!Enum.TryParse(target, out CRS targetCRS))
            {
                info = $"Target CRS {target} is not supported.";
                conversion = (x, y) => (double.NaN, double.NaN);
                return false;
            }

            var steps = new List<Func<double, double, (double x, double y)>>();
            info = "";
            if (getConversion(sourceCRS, targetCRS, ref steps, ref info))
            {
                conversion = calcSteps(steps);
                return true;
            }

            conversion = (x, y) => (double.NaN, double.NaN);
            return false;
        }

        private static bool getConversion(CRS source, CRS target,
            ref List<Func<double, double, (double x, double y)>> steps, ref string info)
        {
            if (source == target)
                return true;
            switch (source)
            {
                case CRS.EGBT22_EGBT_LDP:
                    info += $"Conversion from {source} to {CRS.EGBT22_Geod}.\n";
                    steps.Add(TM_GRS80_EGBT_LDP.Reverse);
                    return getConversion(CRS.EGBT22_Geod, target, ref steps, ref info);
                case CRS.EGBT22_Geod:
                    switch (target)
                    {
                        case CRS.EGBT22_EGBT_LDP:
                            info += $"Conversion from {source} to {CRS.EGBT22_EGBT_LDP}.\n";
                            steps.Add(TM_GRS80_EGBT_LDP.Forward);
                            return true;
                        case CRS.ETRS89_DREF91_UTM33:
                        case CRS.ETRS89_DREF91_Geod:
                        case CRS.DB_Ref_GK5:
                        case CRS.DB_Ref_Geod:
                            info += $"Transformation from {source} to {CRS.ETRS89_DREF91_Geod} with ETRS89 ellipsoidal height 0.\n";
                            steps.Add(EGBT22_Geod_to_ETRS89_DREF91_Geod_ETRS89Zero);
                            return getConversion(CRS.ETRS89_DREF91_Geod, target, ref steps, ref info);
                    }
                    break;
                case CRS.ETRS89_DREF91_UTM33:
                    info += $"Conversion from {source} to {CRS.ETRS89_DREF91_Geod}.\n";
                    steps.Add(TM_GRS80_UTM33.Reverse);
                    return getConversion(CRS.ETRS89_DREF91_Geod, target, ref steps, ref info);
                case CRS.ETRS89_DREF91_Geod:
                    switch (target)
                    {
                        case CRS.EGBT22_EGBT_LDP:
                        case CRS.EGBT22_Geod:
                            info += $"Transformation from {source} to {CRS.EGBT22_Geod} with ETRS89 ellipsoidal height 0.\n";
                            steps.Add(ETRS89_DREF91_Geod_to_EGBT22_Geod_ETRS89Zero);
                            return getConversion(CRS.EGBT22_Geod, target, ref steps, ref info);
                        case CRS.ETRS89_DREF91_UTM33:
                            info += $"Conversion from {source} to {CRS.ETRS89_DREF91_UTM33}:\n";
                            steps.Add(TM_GRS80_UTM33.Forward);
                            return true;
                        case CRS.DB_Ref_Geod:
                        case CRS.DB_Ref_GK5:
                            info += $"Transformation from {source} to {CRS.DB_Ref_Geod} with DB_Ref ellipsoidal height 0.\n";
                            steps.Add(ETRS89_DREF91_Geod_to_DBRef_Geod_DBRefZero);
                            return getConversion(CRS.DB_Ref_Geod, target, ref steps, ref info);
                    }
                    break;
                case CRS.DB_Ref_GK5:
                    info += $"Conversion from {source} to {CRS.DB_Ref_Geod}.\n";
                    steps.Add(TM_Bessel_GK5.Reverse);
                    return getConversion(CRS.DB_Ref_Geod, target, ref steps, ref info);
                case CRS.DB_Ref_Geod:
                    switch (target)
                    {
                        case CRS.EGBT22_EGBT_LDP:
                        case CRS.EGBT22_Geod:
                        case CRS.ETRS89_DREF91_UTM33:
                        case CRS.ETRS89_DREF91_Geod:
                            info += $"Transformation from {source} to {CRS.ETRS89_DREF91_Geod} with DB_Ref ellipsoidal height 0.\n";
                            steps.Add(DBRef_Geod_to_ETRS89_DREF91_Geod_DBRefZero);
                            return getConversion(CRS.ETRS89_DREF91_Geod, target, ref steps, ref info);
                        case CRS.DB_Ref_GK5:
                            info += $"Conversion from {source} to {CRS.DB_Ref_GK5}.\n";
                            steps.Add(TM_Bessel_GK5.Forward);
                            return true;
                    }
                    break;
            }

            info += $"Conversion from {source} to {target} is not supported.\n";
            steps.Clear();
            return false;
        }


        public static bool GetConversion(string source, string sourceVRS, string target,
            out Func<double, double, double, (double x, double y, double z)> conversion, out string info, out string targetVRS)
        {
            if (!Enum.TryParse(source, out CRS sourceCRS))
            {
                info = $"Source CRS {source} is not supported.";
                conversion = (x, y, z) => (double.NaN, double.NaN, double.NaN);
                targetVRS = string.Empty;
                return false;
            }

            if (!Enum.TryParse(sourceVRS, out VRS sourceVRS_))
            {
                info = $"Source VRS {sourceVRS} is not supported.";
                conversion = (x, y, z) => (double.NaN, double.NaN, double.NaN);
                targetVRS = string.Empty;
                return false;
            }

            if (!Enum.TryParse(target, out CRS targetCRS))
            {
                info = $"Target CRS {target} is not supported.";
                conversion = (x, y, z) => (double.NaN, double.NaN, double.NaN);
                targetVRS = string.Empty;
                return false;
            }


            var steps = new List<Func<double, double, double, (double x, double y, double z)>>();
            info = "";
            var targetVRS_ = sourceVRS_;
            if (getConversion(sourceCRS, sourceVRS_, targetCRS, ref steps, ref info, ref targetVRS_))
            {
                conversion = calcSteps(steps);
                targetVRS = targetVRS_.ToString();
                return true;
            }

            conversion = (x, y, z) => (double.NaN, double.NaN, double.NaN);
            targetVRS = string.Empty;
            return false;
        }

        /// <summary>
        /// Determines the sequence of transformation steps required to convert coordinates  from a source coordinate
        /// reference system (CRS) to a target CRS.
        /// </summary>
        /// <remarks>This method recursively determines the necessary transformation steps by analyzing
        /// the relationship  between the source and target CRSs. If the source and target CRSs are the same, the method
        /// returns  immediately with <see langword="true"/>. Otherwise, it identifies the appropriate intermediate
        /// steps  and transformations required to complete the conversion.  The method also handles specific cases
        /// where vertical reference systems (VRS) need to be transformed,  such as converting between normal and
        /// ellipsoidal heights. If the conversion is not supported, the  <paramref name="steps"/> list will be cleared,
        /// and the <paramref name="info"/> string will indicate  that the conversion is not possible.</remarks>
        /// <param name="source">The source coordinate reference system (CRS) from which the conversion starts.</param>
        /// <param name="sourceVRS">The vertical reference system (VRS) associated with the source CRS.  This may influence the transformation
        /// steps, particularly for height conversions.</param>
        /// <param name="target">The target coordinate reference system (CRS) to which the conversion is performed.</param>
        /// <param name="steps">A reference to a list of transformation functions. Each function represents a step in the conversion
        /// process. The list will be populated with the required steps to perform the conversion.</param>
        /// <param name="info">A reference to a string that will be appended with detailed information about the conversion process, 
        /// including intermediate steps and transformations.</param>
        /// <param name="targetVRS">A reference to the vertical reference system (VRS) associated with the target CRS.  This will be updated to
        /// reflect the VRS of the target CRS after the conversion.</param>
        /// <returns><see langword="true"/> if the conversion is supported and the required steps are successfully determined; 
        /// otherwise, <see langword="false"/> if the conversion is not supported.</returns>
        public static bool getConversion(CRS source, VRS sourceVRS, CRS target,
            ref List<Func<double, double, double, (double x, double y, double z)>> steps, ref string info, ref VRS targetVRS)
        {
            if (source == target)
                return true;
            switch (source)
            {
                // EGBT22
                case CRS.EGBT22_EGBT_LDP:
                    info += $"Conversion from {source} to {CRS.EGBT22_Geod}.\n";
                    steps.Add(TM_GRS80_EGBT_LDP.Reverse);
                    return getConversion(CRS.EGBT22_Geod, sourceVRS, target, ref steps, ref info, ref targetVRS);
                case CRS.EGBT22_Geod:
                    switch (target)
                    {
                        case CRS.EGBT22_EGBT_LDP:
                            info += $"Conversion from {source} to {target}.\n";
                            steps.Add(TM_GRS80_EGBT_LDP.Forward);
                            return true;
                        case CRS.EGBT22_Geoc:
                        case CRS.ETRS89_DREF91_UTM33:
                        case CRS.ETRS89_DREF91_Geod:
                        case CRS.ETRS89_DREF91_Geoc:
                        case CRS.ETRS89_CZ_Geod:
                        case CRS.ETRS89_CZ_Geoc:
                        case CRS.DB_Ref_GK5:
                        case CRS.DB_Ref_Geod:
                        case CRS.DB_Ref_Geoc:
                            if (sourceVRS == VRS.Normal)
                            {
                                info += $"Iterative Transformation from {sourceVRS} heights to {VRS.Ellipsoidal} heights.\n";
                                steps.Add(EGBT22_Geod_Normal_to_Ellipsoidal);
                            }

                            info += $"Conversion from {source} to {CRS.EGBT22_Geoc}.\n";
                            steps.Add(GC_GRS80.Forward);
                            targetVRS = VRS.None;
                            return getConversion(CRS.EGBT22_Geoc, VRS.None, target, ref steps, ref info, ref targetVRS);
                    }
                    break;
                case CRS.EGBT22_Geoc:
                    switch (target)
                    {
                        case CRS.EGBT22_EGBT_LDP:
                        case CRS.EGBT22_Geod:
                            info += $"Conversion from {source} to {CRS.EGBT22_Geod}.\n";
                            steps.Add(GC_GRS80.Reverse);
                            targetVRS = VRS.Ellipsoidal;
                            return getConversion(CRS.EGBT22_Geod, VRS.Ellipsoidal, target, ref steps, ref info, ref targetVRS);
                        case CRS.ETRS89_DREF91_UTM33:
                        case CRS.ETRS89_DREF91_Geod:
                        case CRS.ETRS89_DREF91_Geoc:
                        case CRS.DB_Ref_GK5:
                        case CRS.DB_Ref_Geod:
                        case CRS.DB_Ref_Geoc:
                            info += $"Transformation from {source} to {CRS.ETRS89_DREF91_Geoc}.\n";
                            steps.Add(Trans_Datum_ETRS89_DREF91_to_EGBT22.Reverse);
                            return getConversion(CRS.ETRS89_DREF91_Geoc, VRS.None, target, ref steps, ref info, ref targetVRS);
                        case CRS.ETRS89_CZ_Geod:
                        case CRS.ETRS89_CZ_Geoc:
                            info += $"Transformation from {source} to {CRS.ETRS89_CZ_Geoc}.\n";
                            steps.Add(Trans_Datum_ETRS89_CZ_to_EGBT22.Reverse);
                            return getConversion(CRS.ETRS89_CZ_Geoc, VRS.None, target, ref steps, ref info, ref targetVRS);
                    }
                    break;
                // ETRS89_DREF91
                case CRS.ETRS89_DREF91_UTM33:
                    info += $"Conversion from {source} to {CRS.ETRS89_DREF91_Geod}.\n";
                    steps.Add(TM_GRS80_UTM33.Reverse);
                    return getConversion(CRS.ETRS89_DREF91_Geod, sourceVRS, target, ref steps, ref info, ref targetVRS);
                case CRS.ETRS89_DREF91_Geod:
                    switch (target)
                    {
                        case CRS.ETRS89_DREF91_UTM33:
                            info += $"Conversion from {source} to {target}.\n";
                            steps.Add(TM_GRS80_UTM33.Forward);
                            return true;
                        case CRS.EGBT22_EGBT_LDP:
                        case CRS.EGBT22_Geod:
                        case CRS.EGBT22_Geoc:
                        case CRS.ETRS89_DREF91_Geoc:
                        case CRS.DB_Ref_GK5:
                        case CRS.DB_Ref_Geod:
                        case CRS.DB_Ref_Geoc:
                        //case CRS.ETRS89_CZ_Geod:
                        //case CRS.ETRS89_CZ_Geoc:
                            if (sourceVRS == VRS.Normal)
                            {
                                info += $"Transformation from {sourceVRS} heights to {VRS.Ellipsoidal} heights.\n";
                                steps.Add(ETRS89_Geod_Normal_to_Ellipsoidal);
                            }

                            info += $"Conversion from {source} to {CRS.ETRS89_DREF91_Geoc}.\n";
                            steps.Add(GC_GRS80.Forward);
                            targetVRS = VRS.None;
                            return getConversion(CRS.ETRS89_DREF91_Geoc, VRS.None, target, ref steps, ref info, ref targetVRS);
                    }
                    break;
                case CRS.ETRS89_DREF91_Geoc:
                    switch (target)
                    {
                        case CRS.EGBT22_EGBT_LDP:
                        case CRS.EGBT22_Geod:
                        case CRS.EGBT22_Geoc:
                        //case CRS.ETRS89_CZ_Geod:
                        //case CRS.ETRS89_CZ_Geoc:
                            info += $"Transformation from {source} to {CRS.EGBT22_Geoc}.\n";
                            steps.Add(Trans_Datum_ETRS89_DREF91_to_EGBT22.Forward);
                            return getConversion(CRS.EGBT22_Geoc, VRS.None, target, ref steps, ref info, ref targetVRS);
                        case CRS.ETRS89_DREF91_UTM33:
                        case CRS.ETRS89_DREF91_Geod:
                            info += $"Conversion from {source} to {CRS.ETRS89_DREF91_Geod}.\n";
                            steps.Add(GC_GRS80.Reverse);
                            targetVRS = VRS.Ellipsoidal;
                            return getConversion(CRS.ETRS89_DREF91_Geod, VRS.Ellipsoidal, target, ref steps, ref info, ref targetVRS);
                        case CRS.DB_Ref_GK5:
                        case CRS.DB_Ref_Geod:
                        case CRS.DB_Ref_Geoc:
                            info += $"Transformation from {source} to {CRS.DB_Ref_Geoc}.\n";
                            steps.Add(Trans_Datum_ETRS89_DREF91_to_DBRef.Forward);
                            return getConversion(CRS.DB_Ref_Geoc, VRS.None, target, ref steps, ref info, ref targetVRS);
                    }
                    break;
                // DB_Ref
                case CRS.DB_Ref_GK5:
                    info += $"Conversion from {source} to {CRS.DB_Ref_Geod}.\n";
                    steps.Add(TM_Bessel_GK5.Reverse);
                    return getConversion(CRS.DB_Ref_Geod, sourceVRS, target, ref steps, ref info, ref targetVRS);
                case CRS.DB_Ref_Geod:
                    switch (target)
                    {
                        case CRS.EGBT22_EGBT_LDP:
                        case CRS.EGBT22_Geod:
                        case CRS.EGBT22_Geoc:
                        case CRS.ETRS89_DREF91_UTM33:
                        case CRS.ETRS89_DREF91_Geod:
                        case CRS.ETRS89_DREF91_Geoc:
                        case CRS.DB_Ref_Geoc:
                        //case CRS.ETRS89_CZ_Geod:
                        //case CRS.ETRS89_CZ_Geoc:
                            if (sourceVRS == VRS.Normal)
                            {
                                info +=
                                    $"Iterative transformation from {sourceVRS} heights to {VRS.Ellipsoidal} heights.\n";
                                steps.Add(DBRef_Geod_Normal_to_Ellipsoidal);
                            }
                            info += $"Conversion from {source} to {CRS.DB_Ref_Geoc}.\n";
                            steps.Add(GC_Bessel.Forward);
                            targetVRS = VRS.None;
                            return getConversion(CRS.DB_Ref_Geoc, VRS.None, target, ref steps, ref info, ref targetVRS);
                        case CRS.DB_Ref_GK5:
                            info += $"Conversion from {source} to {CRS.DB_Ref_GK5}.\n";
                            steps.Add(TM_Bessel_GK5.Forward);
                            return true;
                    }
                    break;
                case CRS.DB_Ref_Geoc:
                    switch (target)
                    {
                        case CRS.EGBT22_EGBT_LDP:
                        case CRS.EGBT22_Geod:
                        case CRS.EGBT22_Geoc:
                        case CRS.ETRS89_DREF91_UTM33:
                        case CRS.ETRS89_DREF91_Geod:
                        case CRS.ETRS89_DREF91_Geoc:
                        //case CRS.ETRS89_CZ_Geod:
                        //case CRS.ETRS89_CZ_Geoc:
                            info += $"Transformation from {source} to {CRS.ETRS89_DREF91_Geoc}.\n";
                            steps.Add(Trans_Datum_DBRef_to_ETRS89_DREF91.Forward);
                            return getConversion(CRS.ETRS89_DREF91_Geoc, VRS.None, target, ref steps, ref info, ref targetVRS);
                        case CRS.DB_Ref_GK5:
                        case CRS.DB_Ref_Geod:
                            info += $"Conversion from {source} to {CRS.DB_Ref_Geod}.\n";
                            targetVRS = VRS.Ellipsoidal;
                            steps.Add(GC_Bessel.Reverse);
                            return getConversion(CRS.DB_Ref_Geod, sourceVRS, target, ref steps, ref info, ref targetVRS);
                    }
                    break;
                // ETRS89_CZ
                case CRS.ETRS89_CZ_Geod:
                    switch (target)
                    {
                        case CRS.EGBT22_EGBT_LDP:
                        case CRS.EGBT22_Geod:
                        case CRS.EGBT22_Geoc:
                        //case CRS.ETRS89_DREF91_UTM33:
                        //case CRS.ETRS89_DREF91_Geod:
                        //case CRS.ETRS89_DREF91_Geoc:
                        //case CRS.DB_Ref_GK5:
                        //case CRS.DB_Ref_Geod:
                        //case CRS.DB_Ref_Geoc:
                        case CRS.ETRS89_CZ_Geoc:
                            if (sourceVRS == VRS.Ellipsoidal)
                            {
                                info += $"Conversion from {source} to {CRS.ETRS89_CZ_Geoc}.\n";
                                targetVRS = VRS.None;
                                steps.Add(GC_GRS80.Forward);
                                return getConversion(CRS.ETRS89_CZ_Geoc, VRS.None, target, ref steps, ref info, ref targetVRS);
                            }
                            break;
                    }
                    break;
                case CRS.ETRS89_CZ_Geoc:
                    switch (target)
                    {
                        case CRS.EGBT22_EGBT_LDP:
                        case CRS.EGBT22_Geod:
                        case CRS.EGBT22_Geoc:
                        //case CRS.ETRS89_DREF91_UTM33:
                        //case CRS.ETRS89_DREF91_Geod:
                        //case CRS.ETRS89_DREF91_Geoc:
                        //case CRS.DB_Ref_GK5:
                        //case CRS.DB_Ref_Geod:
                        //case CRS.DB_Ref_Geoc:
                            info += $"Transformation from {source} to {CRS.EGBT22_Geoc}.\n";
                            steps.Add(Trans_Datum_ETRS89_CZ_to_EGBT22.Forward);
                            return getConversion(CRS.EGBT22_Geoc, VRS.None, target, ref steps, ref info, ref targetVRS);
                        case CRS.ETRS89_CZ_Geod:
                            info += $"Conversion from {source} to {CRS.ETRS89_CZ_Geod}.\n";
                            targetVRS = VRS.Ellipsoidal;
                            steps.Add(GC_GRS80.Reverse);
                            return getConversion(CRS.ETRS89_CZ_Geod, VRS.Ellipsoidal, target, ref steps, ref info, ref targetVRS);
                    }
                    break;

            }

            string sourceVRSStr = sourceVRS == VRS.None ? "" : $" with {sourceVRS} heights";
            info += $"Conversion from {source}{sourceVRSStr} to {target} is not supported.";
            steps.Clear();
            return false;
        }

        private static Func<double, double, (double x, double y)> calcSteps(
            List<Func<double, double, (double x, double y)>> steps)
        {
            return (x, y) =>
            {
                foreach (var step in steps) (x, y) = step(x, y);
                return (x, y);
            };
        }

        private static Func<double, double, double, (double x, double y, double z)> calcSteps(
            List<Func<double, double, double, (double x, double y, double z)>> steps)
        {
            return (x, y, z) =>
            {
                foreach (var step in steps) (x, y, z) = step(x, y, z);
                return (x, y, z);
            };
        }

        /// <summary>
        ///     Obtains the calculation for gamma (convergence of meridians) and k (scale at point) values based on the provided
        ///     coordinate reference system (CRS).
        /// </summary>
        /// <param name="crs">The coordinate reference system (CRS) used to determine the calculation method as a string..</param>
        /// <param name="calculation">
        ///     An output delegate that takes coordinates (easting and northing) as inputs
        ///     and returns a tuple containing the gamma (convergence of meridians) and k (scale at point) values as results.
        /// </param>
        /// <returns>
        ///     A boolean value indicating whether a valid calculation method was determined for the specified CRS.
        ///     Returns true if a valid calculation was found, otherwise false.
        /// </returns>
        public static bool GetGammaKCalculation(string crs,
            out Func<double, double, (double gamma, double k)> calculation)
        {
            if (Enum.TryParse(crs, out CRS crs_))
            {
                switch (crs_ & CRS.Conversion)
                {
                    case CRS.EGBT_LDP:
                        calculation = (e, n) =>
                        {
                            _ = TM_GRS80_EGBT_LDP.Reverse(e, n, out double gamma, out double k);
                            return (gamma, k);
                        };
                        return true;
                    case CRS.UTM33:
                        calculation = (e, n) =>
                        {
                            _ = TM_GRS80_UTM33.Reverse(e, n, out double gamma, out double k);
                            return (gamma, k);
                        };
                        return true;
                    case CRS.GK5:
                        calculation = (e, n) =>
                        {
                            _ = TM_Bessel_GK5.Reverse(e, n, out double gamma, out double k);
                            return (gamma, k);
                        };
                        return true;
                }
            }

            calculation = (e, n) => (double.NaN, double.NaN);
            return false;
        }


        #region arrays

        /// <summary>
        ///     Processes arrays of coordinates (x, y, z) using a provided calculation function
        ///     and returns the resulting arrays.
        /// </summary>
        /// <param name="xin">Input array of x-coordinates.</param>
        /// <param name="yin">Input array of y-coordinates.</param>
        /// <param name="zin">Input array of z-coordinates.</param>
        /// <param name="calc">
        ///     A function that takes x, y, and z coordinates as inputs and returns a tuple
        ///     containing the transformed x, y, and z coordinates.
        /// </param>
        /// <returns>
        ///     A tuple containing three arrays: the resulting x-coordinates, y-coordinates, and z-coordinates
        ///     after applying the provided calculation function.
        /// </returns>
        public static (double[] x, double[] y, double[] z) CalcArrays3(double[] xin, double[] yin, double[] zin,
            Func<double, double, double, (double x, double y, double z)> calc)
        {
            int n = xin.Length;
            double[] x = new double[n];
            double[] y = new double[n];
            double[] z = new double[n];
            for (int i = 0; i < n; i++) (x[i], y[i], z[i]) = calc(xin[i], yin[i], zin[i]);
            return (x, y, z);
        }

        /// <summary>
        ///     Processes an array of 3D points using a provided calculation function
        ///     and returns the resulting array of transformed points.
        /// </summary>
        /// <param name="points">A jagged array where each inner array represents a 3D point with x, y, and z coordinates.</param>
        /// <param name="calc">
        ///     A function that takes x, y, and z coordinates as input and returns a tuple
        ///     containing the transformed x, y, and z coordinates.
        /// </param>
        /// <returns>
        ///     A jagged array of transformed 3D points, where each inner array contains the x, y, and z coordinates
        ///     resulting from applying the provided calculation function.
        /// </returns>
        public static double[][] CalcArray3(double[][] points,
            Func<double, double, double, (double x, double y, double z)> calc)
        {
            int n = points.Length;
            double[][] xyz = new double[n][];
            for (int i = 0; i < n; i++)
            {
                (double x, double y, double z) = calc(points[i][0], points[i][1], points[i][2]);
                xyz[i] = new[] { x, y, z };
            }

            return xyz;
        }

        /// <summary>
        ///     Processes arrays of 2D coordinates (x, y) using a provided calculation function
        ///     and returns the resulting arrays.
        /// </summary>
        /// <param name="xin">Input array of x-coordinates.</param>
        /// <param name="yin">Input array of y-coordinates.</param>
        /// <param name="calc">
        ///     A function that takes x and y coordinates as inputs and returns a tuple
        ///     containing the transformed x and y coordinates.
        /// </param>
        /// <returns>
        ///     A tuple containing two arrays: the resulting x-coordinates and y-coordinates
        ///     after applying the provided calculation function.
        /// </returns>
        public static (double[] x, double[] y) CalcArrays2(double[] xin, double[] yin,
            Func<double, double, (double x, double y)> calc)
        {
            int n = xin.Length;
            double[] x = new double[n];
            double[] y = new double[n];
            for (int i = 0; i < n; i++) (x[i], y[i]) = calc(xin[i], yin[i]);
            return (x, y);
        }

        /// <summary>
        ///     Processes an array of coordinate pairs using a provided calculation function
        ///     and returns the resulting array of transformed coordinate pairs.
        /// </summary>
        /// <param name="points">Input array of points, where each point is an array containing two coordinates (x, y).</param>
        /// <param name="calc">
        ///     A function that takes two inputs, x and y coordinates, and returns a tuple
        ///     with transformed x and y coordinates.
        /// </param>
        /// <returns>
        ///     A jagged array where each inner array contains the transformed x and y coordinates
        ///     for the corresponding input point.
        /// </returns>
        public static double[][] CalcArray2(double[][] points, Func<double, double, (double x, double y)> calc)
        {
            int n = points.Length;
            double[][] xy = new double[n][];
            for (int i = 0; i < n; i++)
            {
                (double x, double y) = calc(points[i][0], points[i][1]);
                xy[i] = new[] { x, y };
            }

            return xy;
        }

        #endregion
    }
}