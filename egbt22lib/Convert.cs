using System;
using System.Collections.Generic;
using static egbt22lib.Transformations.Defined;
using static egbt22lib.Conversions.Defined;
using static egbt22lib.Conversions.Common;
using egbt22lib.Conversions;


namespace egbt22lib
{
    public static class Convert
    {


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
            "ETRS89_DREF91_Geod", // String conversion seems not working correctly
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
            "ETRS89_DREF91_Geod", // String conversion seems not working correctly
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


        private static (double lat, double lon) DBRef_Geod_to_ETRS89_DREF91_Geod_DBRefZero(double lat, double lon)
        {
            double ellH = 0;
            var (x, y, z) = GC_Bessel.Forward(lat, lon, ellH);
            (x, y, z) = Trans_Datum_DBRef_to_ETRS89_DREF91.Forward(x, y, z);
            (lat, lon, _) = GC_GRS80.Reverse(x, y, z);
            return (lat, lon);
        }

        private static (double lat, double lon) DBRef_Geod_to_EGBT22_Geod_DBRefZero(double lat, double lon)
        {
            double ellH = 0;
            var (x, y, z) = GC_Bessel.Forward(lat, lon, ellH);
            (x, y, z) = Trans_Datum_DBRef_to_ETRS89_DREF91.Forward(x, y, z);
            (x, y, z) = Trans_Datum_ETRS89_DREF91_to_EGBT22.Forward(x, y, z);
            (lat, lon, _) = GC_GRS80.Reverse(x, y, z);
            return (lat, lon);
        }

        private static (double lat, double lon) ETRS89_DREF91_Geod_to_DBRef_Geod_DBRefZero(double lat, double lon, out double ellH)
        {
            ellH = 0;
            double dh = double.PositiveInfinity,
                dlat = double.NaN, dlon = double.NaN;
            while (Math.Abs(dh) > TOLM)
            {
                var (x, y, z) = GC_GRS80.Forward(lat, lon, ellH);
                (x, y, z) = Trans_Datum_ETRS89_DREF91_to_DBRef.Forward(x, y, z);
                (dlat, dlon, dh) = GC_Bessel.Reverse(x, y, z);
                ellH -= dh;
            }
            return (dlat, dlon);
        }

        private static (double lat, double lon) ETRS89_DREF91_Geod_to_DBRef_Geod_DBRefZero(double lat, double lon)
        {
            return ETRS89_DREF91_Geod_to_DBRef_Geod_DBRefZero(lat, lon, out _);
        }

        private static (double lat, double lon) EGBT22_Geod_to_DBRef_Geod_DBRefZero(double lat, double lon, out double ellH)
        {
            ellH = 0;
            double dh = double.PositiveInfinity,
                dlat = double.NaN, dlon = double.NaN;
            while (Math.Abs(dh) > TOLM)
            {
                var (x, y, z) = GC_GRS80.Forward(lat, lon, ellH);
                (x, y, z) = Trans_Datum_ETRS89_DREF91_to_EGBT22.Reverse(x, y, z);
                (x, y, z) = Trans_Datum_ETRS89_DREF91_to_DBRef.Forward(x, y, z);
                (dlat, dlon, dh) = GC_Bessel.Reverse(x, y, z);
                ellH -= dh;
            }
            return (dlat, dlon);
        }

        private static (double lat, double lon) EGBT22_Geod_to_DBRef_Geod_DBRefZero(double lat, double lon)
        {
            return EGBT22_Geod_to_DBRef_Geod_DBRefZero(lat, lon, out _);
        }

        private static (double lat, double lon) ETRS89_DREF91_Geod_to_EGBT22_Geod_Zero(double lat, double lon)
        {
            // no height scale change
            var (x, y, z) = GC_GRS80.Forward(lat, lon, 0);
            (x, y, z) = Trans_Datum_ETRS89_DREF91_to_EGBT22.Forward(x, y, z);
            (lat, lon, _) = GC_GRS80.Reverse(x, y, z);
            return (lat, lon);
        }

        private static (double lat, double lon) EGBT22_Geod_to_ETRS89_DREF91_Geod_Zero(double lat, double lon)
        {
            // No iteration needed, as the difference is near to epsilon, // no height scale change
            var (x, y, z) = GC_GRS80.Forward(lat, lon, 0);
            (x, y, z) = Trans_Datum_ETRS89_DREF91_to_EGBT22.Reverse(x, y, z);
            (lat, lon, _) = GC_GRS80.Reverse(x, y, z);
            return (lat, lon);
        }

        /// <summary>
        /// Attempts to retrieve a coordinate conversion function between the specified source and target coordinate
        /// reference systems (CRS).
        /// </summary>
        /// <remarks>This method validates the provided source and target CRS identifiers and attempts to
        /// construct a conversion function between them. If either CRS is invalid or the conversion cannot be
        /// determined, the method returns <see langword="false"/> and provides error details in the <paramref
        /// name="info"/> parameter.</remarks>
        /// <param name="source">The source CRS identifier as a string. Must be a valid CRS name.</param>
        /// <param name="target">The target CRS identifier as a string. Must be a valid CRS name.</param>
        /// <param name="conversion">When this method returns, contains a function that performs the coordinate conversion from the source CRS to
        /// the target CRS. The function takes two double parameters (representing coordinates) and returns a tuple of
        /// two doubles (the converted coordinates). If the method fails, this will be set to a function that returns
        /// <see cref="double.NaN"/> for both values.</param>
        /// <param name="info">When this method returns, contains additional information about the conversion process. If the method fails,
        /// this will contain an error message describing the reason for failure.</param>
        /// <param name="useZeroHeights">A boolean value indicating whether to use zero heights during the conversion and transformation process. If <see
        /// langword="true"/>, ellipsoidal heights will be treated as zero; otherwise, only conversions with the same datum are supported.</param>
        /// <returns><see langword="true"/> if the conversion function was successfully created; otherwise, <see
        /// langword="false"/>.</returns>
        public static bool GetConversion(string source, string target,
            out Func<double, double, (double x, double y)> conversion, out string info, bool useZeroHeights)
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
            if (getConversion(sourceCRS, targetCRS, ref steps, ref info, useZeroHeights))
            {
                conversion = calcSteps2D(steps);
                return true;
            }

            conversion = (x, y) => (double.NaN, double.NaN);
            return false;
        }

        private static bool getConversion(CRS source, CRS target,
            ref List<Func<double, double, (double x, double y)>> steps, ref string info, bool useZeroHeights)

        {
            if (source == target)
                return true;
            switch (source)
            {
                case CRS.EGBT22_EGBT_LDP:
                    info += $"Conversion from {source} to {CRS.EGBT22_Geod}.\n";
                    steps.Add(TM_GRS80_EGBT_LDP.Reverse);
                    return getConversion(CRS.EGBT22_Geod, target, ref steps, ref info, useZeroHeights);
                case CRS.EGBT22_Geod:
                    switch (target)
                    {
                        case CRS.EGBT22_EGBT_LDP:
                            info += $"Conversion from {source} to {CRS.EGBT22_EGBT_LDP}.\n";
                            steps.Add(TM_GRS80_EGBT_LDP.Forward);
                            return true;
                        case CRS.ETRS89_DREF91_UTM33:
                        case CRS.ETRS89_DREF91_Geod:
                            if (useZeroHeights)
                            {
                                info += $"Transformation from {source} to {CRS.ETRS89_DREF91_Geod} with ellipsoidal height 0.\n";
                                steps.Add(EGBT22_Geod_to_ETRS89_DREF91_Geod_Zero);
                                return getConversion(CRS.ETRS89_DREF91_Geod, target, ref steps, ref info, useZeroHeights);
                            }
                            break;
                        case CRS.DB_Ref_GK5:
                        case CRS.DB_Ref_Geod:
                            if (useZeroHeights)
                            {
                                info += $"Iterative Transformation from {source} to {CRS.DB_Ref_Geod} with DB_Ref ellipsoidal height 0.\n";
                                steps.Add(EGBT22_Geod_to_DBRef_Geod_DBRefZero);
                                return getConversion(CRS.DB_Ref_Geod, target, ref steps, ref info, useZeroHeights);
                            }
                            break;
                    }
                    break;
                case CRS.ETRS89_DREF91_UTM33:
                    info += $"Conversion from {source} to {CRS.ETRS89_DREF91_Geod}.\n";
                    steps.Add(TM_GRS80_UTM33.Reverse);
                    return getConversion(CRS.ETRS89_DREF91_Geod, target, ref steps, ref info, useZeroHeights);
                case CRS.ETRS89_DREF91_Geod:
                    switch (target)
                    {
                        case CRS.EGBT22_EGBT_LDP:
                        case CRS.EGBT22_Geod:
                            if (useZeroHeights)
                            {
                                info += $"Transformation from {source} to {CRS.EGBT22_Geod} with ellipsoidal height 0.\n";
                                steps.Add(ETRS89_DREF91_Geod_to_EGBT22_Geod_Zero);
                                return getConversion(CRS.EGBT22_Geod, target, ref steps, ref info, useZeroHeights);
                            }
                            break;
                        case CRS.ETRS89_DREF91_UTM33:
                            info += $"Conversion from {source} to {CRS.ETRS89_DREF91_UTM33}:\n";
                            steps.Add(TM_GRS80_UTM33.Forward);
                            return true;
                        case CRS.DB_Ref_Geod:
                        case CRS.DB_Ref_GK5:
                            if (useZeroHeights)
                            {
                                info += $"Iterative Transformation from {source} to {CRS.DB_Ref_Geod} with DB_Ref ellipsoidal height 0.\n";
                                steps.Add(ETRS89_DREF91_Geod_to_DBRef_Geod_DBRefZero);
                                return getConversion(CRS.DB_Ref_Geod, target, ref steps, ref info, useZeroHeights);
                            }
                            break;
                    }
                    break;
                case CRS.DB_Ref_GK5:
                    info += $"Conversion from {source} to {CRS.DB_Ref_Geod}.\n";
                    steps.Add(TM_Bessel_GK5.Reverse);
                    return getConversion(CRS.DB_Ref_Geod, target, ref steps, ref info, useZeroHeights);
                case CRS.DB_Ref_Geod:
                    switch (target)
                    {
                        case CRS.EGBT22_EGBT_LDP:
                        case CRS.EGBT22_Geod:
                            if (useZeroHeights)
                            {
                                info += $"Transformation from {source} to {CRS.EGBT22_Geod} with DB_Ref ellipsoidal height 0.\n";
                                steps.Add(DBRef_Geod_to_EGBT22_Geod_DBRefZero);
                                return getConversion(CRS.EGBT22_Geod, target, ref steps, ref info, useZeroHeights);
                            }
                            break;
                        case CRS.ETRS89_DREF91_UTM33:
                        case CRS.ETRS89_DREF91_Geod:
                            if (useZeroHeights)
                            {
                                info += $"Transformation from {source} to {CRS.ETRS89_DREF91_Geod} with DB_Ref ellipsoidal height 0.\n";
                                steps.Add(DBRef_Geod_to_ETRS89_DREF91_Geod_DBRefZero);
                                return getConversion(CRS.ETRS89_DREF91_Geod, target, ref steps, ref info, useZeroHeights);
                            }
                            break;
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

        private static Func<double, double, (double x, double y)> calcSteps2D(
            List<Func<double, double, (double x, double y)>> steps)
        {
            return (x, y) =>
            {
                foreach (var step in steps)
                {
                    (x, y) = step(x, y);
                }
                return (x, y);
            };
        }

        /// <summary>
        ///     Obtains the calculation for gamma (convergence of meridians in degrees), k (scale at point) and check if point inside bounding box based on the provided
        ///     coordinate reference system (CRS).
        /// </summary>
        /// <param name="crs">The coordinate reference system (CRS) used to determine the calculation method as a string..</param>
        /// <param name="calculation">
        ///     An output delegate that takes coordinates (easting and northing) as inputs
        ///     and returns a tuple containing the gamma (convergence of meridians), k (scale at point) and check if point inside bounding box as results.
        /// </param>
        /// <param name="useDBRefZero">
        ///     Use DB_Ref ellipsoidal height 0 for calculations. If true, the method will use DB_Ref ellipsoidal height 0 for calculations,
        ///     otherwise it will use the default ellipsoidal height 0 for the specified CRS.
        /// </param>
        /// <returns>
        ///     A boolean value indicating whether a valid calculation method was determined for the specified CRS.
        ///     Returns true if a valid calculation was found, otherwise false.
        /// </returns>
        public static bool GetGammaKInsideCalculation(string crs,
            out Func<double, double, (double gamma, double k, bool isInsideBBox)> calculation, bool useDBRefZero)
        {
            if (Enum.TryParse(crs, out CRS crs_))
            {
                switch (crs_)
                {
                    case CRS.EGBT22_EGBT_LDP:
                        calculation = (e, n) =>
                        {
                            var (lat, lon) = TM_GRS80_EGBT_LDP.Reverse(e, n, out double gamma, out double k);
                            bool isInside = IsInsideBBox_ETRS89(lat, lon);
                            if (useDBRefZero)
                            {
                                _ = EGBT22_Geod_to_DBRef_Geod_DBRefZero(lat, lon, out double ellH);
                                k = PointScaleAtHeight(k, lat, ellH, GRS80_a, GRS80_f);
                            }
                            return (gamma, k, isInside);
                        };
                        return true;
                    case CRS.ETRS89_DREF91_UTM33:
                        calculation = (e, n) =>
                        {
                            var (lat, lon) = TM_GRS80_UTM33.Reverse(e, n, out double gamma, out double k);
                            bool isInside = IsInsideBBox_ETRS89(lat, lon);
                            if (useDBRefZero)
                            {
                                _ = ETRS89_DREF91_Geod_to_DBRef_Geod_DBRefZero(lat, lon, out double ellH);
                                k = PointScaleAtHeight(k, lat, ellH, GRS80_a, GRS80_f);
                            }
                            return (gamma, k, isInside);
                        };
                        return true;
                    case CRS.DB_Ref_GK5:
                        calculation = (e, n) =>
                        {
                            var (lat, lon) = TM_Bessel_GK5.Reverse(e, n, out double gamma, out double k);
                            bool isInside = IsInsideBBox_DB_Ref(lat, lon);
                            return (gamma, k, isInside);
                        };
                        return true;
                }
            }

            calculation = (e, n) => (double.NaN, double.NaN, false);
            return false;
        }
        public static bool GetInsideCalculation(string crs,
            out Func<double, double, bool> calculation)
        {
            if (Enum.TryParse(crs, out CRS crs_))
            {
                switch (crs_)
                {
                    case CRS.EGBT22_EGBT_LDP:
                        calculation = (e, n) =>
                        {
                            var (lat, lon) = TM_GRS80_EGBT_LDP.Reverse(e, n, out _, out _);
                            return IsInsideBBox_ETRS89(lat, lon);
                        };
                        return true;
                    case CRS.EGBT22_Geod:
                    case CRS.ETRS89_DREF91_Geod:
                    case CRS.ETRS89_CZ_Geod:
                        calculation = IsInsideBBox_ETRS89;
                        return true;
                    case CRS.ETRS89_DREF91_UTM33:
                        calculation = (e, n) =>
                        {
                            var (lat, lon) = TM_GRS80_UTM33.Reverse(e, n, out _, out _);
                            return IsInsideBBox_ETRS89(lat, lon);
                        };
                        return true;
                    case CRS.DB_Ref_GK5:
                        calculation = (e, n) =>
                        {
                            var (lat, lon) = TM_Bessel_GK5.Reverse(e, n, out _, out _);
                            return IsInsideBBox_DB_Ref(lat, lon);
                        };
                        return true;
                    case CRS.DB_Ref_Geod:
                        calculation = IsInsideBBox_DB_Ref;
                        return true;
                }
            }

            calculation = (e, n) => false;
            return false;
        }


        /// <summary>
        /// Attempts to retrieve a coordinate conversion function between the specified source and target coordinate
        /// reference systems (CRS).
        /// </summary>
        /// <remarks>This method validates the provided source and target CRS and VRS identifiers. If any
        /// of the identifiers are invalid,  the method will return <see langword="false"/> and provide an appropriate
        /// error message in the <paramref name="info"/> parameter.</remarks>
        /// <param name="source">The source coordinate reference system (CRS) as a string. Must be a valid CRS identifier.</param>
        /// <param name="sourceVRS">The vertical reference system (VRS) associated with the source CRS. Must be a valid VRS identifier.</param>
        /// <param name="target">The target coordinate reference system (CRS) as a string. Must be a valid CRS identifier.</param>
        /// <param name="conversion">When this method returns, contains a function that performs the coordinate conversion.  The function takes
        /// three input parameters (x, y, z) and returns a tuple (x, y, z) representing the converted coordinates. If
        /// the method fails, this will be set to a function that returns NaN for all outputs.</param>
        /// <param name="info">When this method returns, contains additional information about the conversion process.  If the method
        /// fails, this will contain an error message describing the issue.</param>
        /// <param name="targetVRS">When this method returns, contains the vertical reference system (VRS) associated with the target CRS.  If
        /// the method fails, this will be an empty string.</param>
        /// <returns><see langword="true"/> if the conversion function was successfully retrieved; otherwise, <see
        /// langword="false"/>.</returns>
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
        private static bool getConversion(CRS source, VRS sourceVRS, CRS target,
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
                            if (sourceVRS == VRS.Normal)
                            {
                                info += $"Iterative transformation from {sourceVRS} heights to {VRS.Ellipsoidal} heights.\n";
                                steps.Add(DB_Ref_Geod_Normal_to_Ellipsoidal);
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
        /// Determines whether a calculation function for gamma, k, and bounding box checks can be provided  based on
        /// the specified coordinate reference system (CRS) and vertical reference system (VRS).
        /// </summary>
        /// <remarks>The returned calculation function performs coordinate transformations and checks
        /// based on the specified CRS and VRS. If the CRS or VRS values are invalid, the method returns <see
        /// langword="false"/> and the <paramref name="calculation"/>  parameter is set to a default function that
        /// returns NaN values and <see langword="false"/> for all checks.</remarks>
        /// <param name="crs">The coordinate reference system identifier as a string. Must match a valid <see cref="CRS"/> enumeration
        /// value.</param>
        /// <param name="vrs">The vertical reference system identifier as a string. Must match a valid <see cref="VRS"/> enumeration
        /// value.</param>
        /// <param name="calculation">When this method returns <see langword="true"/>, contains a function that computes gamma, k,  bounding box
        /// inclusion, and height range inclusion based on the provided coordinates. The function signature is
        /// <c>Func&lt;double, double, double, (double gamma, double k, bool isInsideBBox, bool
        /// isInHeightRange)&gt;</c>.</param>
        /// <returns><see langword="true"/> if a calculation function is successfully provided for the specified CRS and VRS; 
        /// otherwise, <see langword="false"/>.</returns>
        public static bool GetGammaKInsideCalculation(string crs, string vrs,
            out Func<double, double, double, (double gamma, double k, bool isInsideBBox, bool isInHeightRange)> calculation)
        {
            if (Enum.TryParse(crs, out CRS crs_) && Enum.TryParse(vrs, out VRS vrs_) && vrs_ != VRS.None)
            {
                switch (crs_)
                {
                    case CRS.EGBT22_EGBT_LDP:
                        calculation = (e, n, h) =>
                        {
                            var (lat, lon) = TM_GRS80_EGBT_LDP.Reverse(e, n, out double gamma, out double k);
                            bool isInside = IsInsideBBox_ETRS89(lat, lon);
                            double ellH = vrs_ == VRS.Normal ? EGBT22_Geod_Normal_to_Ellipsoidal(lat, lon, h).ellH : h;
                            k = PointScaleAtHeight(k, lat, ellH, GRS80_a, GRS80_f);
                            return (gamma, k, isInside, IsInsideHeightRange_ETRS89(ellH));
                        };
                        return true;
                    case CRS.ETRS89_DREF91_UTM33:
                        calculation = (e, n, h) =>
                        {
                            var (lat, lon) = TM_GRS80_UTM33.Reverse(e, n, out double gamma, out double k);
                            bool isInside = IsInsideBBox_ETRS89(lat, lon);
                            double ellH = vrs_ == VRS.Normal ? ETRS89_Geod_Normal_to_Ellipsoidal(lat, lon, h).ellH : h;
                            k = PointScaleAtHeight(k, lat, ellH, GRS80_a, GRS80_f);
                            return (gamma, k, isInside, IsInsideHeightRange_ETRS89(ellH));
                        };
                        return true;
                    case CRS.DB_Ref_GK5:
                        calculation = (e, n, h) =>
                        {
                            var (lat, lon) = TM_Bessel_GK5.Reverse(e, n, out double gamma, out double k);
                            bool isInside = IsInsideBBox_DB_Ref(lat, lon);
                            double ellH = vrs_ == VRS.Normal ? DB_Ref_Geod_Normal_to_Ellipsoidal(lat, lon, h).ellH : h;
                            k = PointScaleAtHeight(k, lat, ellH, Bessel_a, Bessel_f);
                            return (gamma, k, isInside, IsInsideHeightRange_DB_Ref(ellH));
                        };
                        return true;
                }
            }
            calculation = (e, n, h) => (double.NaN, double.NaN, false, false);
            return false;
        }

        /// <summary>
        /// Determines whether a given coordinate reference system (CRS) and vertical reference system (VRS) can be used
        /// to calculate whether a point is inside a bounding box and within a height range.
        /// </summary>
        /// <remarks>This method supports multiple CRS and VRS combinations. If the CRS or VRS is invalid,
        /// the method returns <see langword="false"/> and the <paramref name="calculation"/> output parameter is set to
        /// a default function that always returns <see langword="false"/> for both values in the tuple.</remarks>
        /// <param name="crs">The coordinate reference system identifier as a string. Must correspond to a valid CRS enumeration value.</param>
        /// <param name="vrs">The vertical reference system identifier as a string. Must correspond to a valid VRS enumeration value.</param>
        /// <param name="calculation">An output parameter that, if the method returns <see langword="true"/>, contains a function to perform the
        /// calculation. The function takes three double parameters representing coordinates or height values and
        /// returns a tuple indicating: <list type="bullet"> <item><description><see langword="true"/> if the point is
        /// inside the bounding box; otherwise, <see langword="false"/>.</description></item> <item><description><see
        /// langword="true"/> if the point is within the height range; otherwise, <see
        /// langword="false"/>.</description></item> </list></param>
        /// <returns><see langword="true"/> if the CRS and VRS are valid and a calculation function is successfully created;
        /// otherwise, <see langword="false"/>.</returns>
        public static bool GetInsideCalculation(string crs, string vrs,
            out Func<double, double, double, (bool isInsideBBox, bool isInHeightRange)> calculation)
        {
            if (Enum.TryParse(crs, out CRS crs_))
            {
                switch (crs_)
                {
                    // EGBT22
                    case CRS.EGBT22_EGBT_LDP:
                        if (Enum.TryParse(vrs, out VRS vrs_))
                        {
                            if(vrs_ == VRS.Normal)
                            {
                                calculation = (e, n, h) =>
                                {
                                    var (lat, lon) = TM_GRS80_EGBT_LDP.Reverse(e, n, out _, out _);
                                    double ellH = EGBT22_Geod_Normal_to_Ellipsoidal(lat, lon, h).ellH;
                                    return (IsInsideBBox_ETRS89(lat, lon), IsInsideHeightRange_ETRS89(ellH));
                                };
                                return true;
                            }
                            else if(vrs_ == VRS.Ellipsoidal)
                            {
                                calculation = (e, n, h) =>
                                {
                                    var (lat, lon) = TM_GRS80_EGBT_LDP.Reverse(e, n, out _, out _);
                                    return (IsInsideBBox_ETRS89(lat, lon), IsInsideHeightRange_ETRS89(h));
                                };
                                return true;
                            }
                        }
                        break;
                    case CRS.EGBT22_Geod:
                        if (Enum.TryParse(vrs, out vrs_) && vrs_ != VRS.None)
                        {
                            calculation = (lat, lon, h) =>
                            {
                                bool isInside = IsInsideBBox_ETRS89(lat, lon);
                                double ellH = vrs_ == VRS.Normal ? EGBT22_Geod_Normal_to_Ellipsoidal(lat, lon, h).ellH : h;
                                return (isInside, IsInsideHeightRange_ETRS89(ellH));
                            };
                            return true;
                        }
                        break;
                    case CRS.EGBT22_Geoc:
                        calculation = (x, y, z) =>
                        {
                            var (lat, lon, ellH) = GC_GRS80.Reverse(x, y, z);
                            bool isInside = IsInsideBBox_ETRS89(lat, lon);
                            return (isInside, IsInsideHeightRange_ETRS89(ellH));
                        };
                        return true;
                    // ETRS89_DREF91
                    case CRS.ETRS89_DREF91_UTM33:
                        if (Enum.TryParse(vrs, out vrs_) && vrs_ != VRS.None)
                        {
                            calculation = (e, n, h) =>
                            {
                                var (lat, lon) = TM_GRS80_UTM33.Reverse(e, n, out _, out _);
                                bool isInside = IsInsideBBox_ETRS89(lat, lon);
                                double ellH = vrs_ == VRS.Normal ? EGBT22_Geod_Normal_to_Ellipsoidal(lat, lon, h).ellH : h;
                                return (isInside, IsInsideHeightRange_ETRS89(ellH));
                            };
                            return true;
                        }
                        break;
                    case CRS.ETRS89_DREF91_Geod:
                        if (Enum.TryParse(vrs, out vrs_) && vrs_ != VRS.None)
                        {
                            calculation = (lat, lon, h) =>
                            {
                                bool isInside = IsInsideBBox_ETRS89(lat, lon);
                                double ellH = vrs_ == VRS.Normal ? ETRS89_Geod_Normal_to_Ellipsoidal(lat, lon, h).ellH : h;
                                return (isInside, IsInsideHeightRange_ETRS89(ellH));
                            };
                            return true;
                        }
                        break;
                    case CRS.ETRS89_DREF91_Geoc:
                        calculation = (x, y, z) =>
                        {
                            var (lat, lon, ellH) = GC_GRS80.Reverse(x, y, z);
                            bool isInside = IsInsideBBox_ETRS89(lat, lon);
                            return (isInside, IsInsideHeightRange_ETRS89(ellH));
                        };
                        return true;
                    // DB_Ref
                    case CRS.DB_Ref_GK5:
                        if (Enum.TryParse(vrs, out vrs_) && vrs_ != VRS.None)
                        {
                            calculation = (e, n, h) =>
                            {
                                var (lat, lon) = TM_Bessel_GK5.Reverse(e, n, out _, out _);
                                bool isInside = IsInsideBBox_DB_Ref(lat, lon);
                                double ellH = vrs_ == VRS.Normal ? DB_Ref_Geod_Normal_to_Ellipsoidal(lat, lon, h).ellH : h;
                                return (isInside, IsInsideHeightRange_DB_Ref(ellH));
                            };
                            return true;
                        }
                        break;
                    case CRS.DB_Ref_Geod:
                        if (Enum.TryParse(vrs, out vrs_) && vrs_ != VRS.None)
                        {
                            calculation = (lat, lon, h) =>
                            {
                                bool isInside = IsInsideBBox_DB_Ref(lat, lon);
                                double ellH = vrs_ == VRS.Normal ? DB_Ref_Geod_Normal_to_Ellipsoidal(lat, lon, h).ellH : h;
                                return (isInside, IsInsideHeightRange_DB_Ref(ellH));
                            };
                            return true;
                        }
                        break;
                    case CRS.DB_Ref_Geoc:
                        calculation = (x, y, z) =>
                        {
                            var (lat, lon, ellH) = GC_Bessel.Reverse(x, y, z);
                            bool isInside = IsInsideBBox_DB_Ref(lat, lon);
                            return (isInside, IsInsideHeightRange_DB_Ref(ellH));
                        };
                        return true;
                    // ETRS89_CZ
                    case CRS.ETRS89_CZ_Geod:
                        if (Enum.TryParse(vrs, out vrs_) && vrs_ == VRS.Ellipsoidal)
                        {
                            calculation = (lat, lon, h) =>
                            {
                                bool isInside = IsInsideBBox_ETRS89(lat, lon);
                                return (isInside, IsInsideHeightRange_ETRS89(h));
                            };
                            return true;
                        }
                        break;
                    case CRS.ETRS89_CZ_Geoc:
                        calculation = (x, y, z) =>
                        {
                            var (lat, lon, ellH) = GC_GRS80.Reverse(x, y, z);
                            bool isInside = IsInsideBBox_ETRS89(lat, lon);
                            return (isInside, IsInsideHeightRange_ETRS89(ellH));
                        };
                        return true;
                }
            }
            calculation = (e, n, h) => (false, false);
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