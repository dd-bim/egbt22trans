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
            EGBT22_LDP = 2,
            UTM33 = 4,
            GK5 = 8,
            Conversion = Geod | Geoc | EGBT22_LDP | UTM33 | GK5,
            ETRS89 = 16,
            DB_Ref = 32,
            Datum = ETRS89 | DB_Ref,

            ETRS89_EGBT22_LDP = ETRS89 | EGBT22_LDP,
            ETRS89_UTM33 = ETRS89 | UTM33,
            ETRS89_Geod = ETRS89 | Geod,
            ETRS89_Geoc = ETRS89 | Geoc,
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
        ///     Defined_CRS is a static readonly array of strings that represents the predefined supported
        ///     Coordinate Reference Systems (CRS) in the egbt22lib.Convert namespace.
        ///     It includes various combinations of CRS types, such as ETRS89 and DB_Ref,
        ///     mapped to specific projections or transformations like EGBT22_LDP or UTM33.
        /// </summary>
        public static readonly string[] Defined_CRS =
        {
            "ETRS89_EGBT22_LDP",
            "ETRS89_UTM33",
            "ETRS89_Geod",
            "ETRS89_Geoc",
            "DB_Ref_GK5",
            "DB_Ref_Geod",
            "DB_Ref_Geoc"
        };

        /// <summary>
        ///     Defined_VRS is a static readonly array of strings that represents the predefined supported
        ///     Virtual Reference System (VRS) types in the egbt22lib.Convert namespace.
        ///     It enumerates different VRS modes, such as None, Normal, and Ellipsoidal,
        ///     which are used to specify the type of vertical reference in coordinate transformations.
        /// </summary>
        public static readonly string[] Defined_VRS =
        {
            "None",
            "Normal",
            "Ellipsoidal"
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
                (x, y, z) = Trans_Datum_DBRef_to_ETRS89.Forward(x, y, z);
                double elat;
                double elon;
                (elat, elon, _) = GC_GRS80.Reverse(x, y, z);
                double eh = h + Geoid.GetBKGBinaryGeoidHeight(elat, elon);
                (x, y, z) = GC_GRS80.Forward(elat, elon, eh);
                (x, y, z) = Trans_Datum_DBRef_to_ETRS89
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
            (x, y, z) = Trans_Datum_DBRef_to_ETRS89.Forward(x, y, z);
            (double elat, double elon, double eh) = GC_GRS80.Reverse(x, y, z);
            double h = eh - Geoid.GetBKGBinaryGeoidHeight(elat, elon);
            return (lat, lon, h);
        }

        private static (double lat, double lon) DBRef_Geod_to_ETRS89_Geod_DBRefZero(double lat, double lon)
        {
            (double x, double y, double z) = GC_Bessel.Forward(lat, lon, 0);
            (x, y, z) = Trans_Datum_DBRef_to_ETRS89.Forward(x, y, z);
            (double elat, double elon, _) = GC_GRS80.Reverse(x, y, z);
            return (elat, elon);
        }

        private static (double lat, double lon) ETRS89_Geod_to_DBRef_Geod_DBRefZero(double lat, double lon)
        {
            double dlat = double.NaN, dlon = double.NaN, dh = double.MaxValue, th = 0;
            while (Math.Abs(dh) > TOLM)
            {
                double x;
                double y;
                double z;
                (x, y, z) = GC_GRS80.Forward(lat, lon, th);
                (x, y, z) = Trans_Datum_ETRS89_to_DBRef.Forward(x, y, z);
                (dlat, dlon, dh) = GC_Bessel.Reverse(x, y, z);
                th -= dh;
            }

            return (dlat, dlon);
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
            out Func<double, double, (double x, double y)> conversion, out string info, bool isDBREFZero = false)
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
            if (getConversion(sourceCRS, targetCRS, ref steps, ref info, isDBREFZero))
            {
                conversion = calcSteps(steps);
                return true;
            }

            conversion = (x, y) => (double.NaN, double.NaN);
            return false;
        }

        private static bool getConversion(CRS source, CRS target,
            ref List<Func<double, double, (double x, double y)>> steps, ref string info, bool isDBREFZero = false)
        {
            if (source == target)
            {
                info += $"Conversion from {source} to {target} is not needed.\n";
                return true;
            }

            switch (source)
            {
                case CRS.ETRS89_EGBT22_LDP:
                    info += $"Conversion from {source} to {CRS.ETRS89_Geod}.\n";
                    steps.Add(TM_GRS80_EGBT22.Reverse);
                    return getConversion(CRS.ETRS89_Geod, target, ref steps, ref info, isDBREFZero);
                case CRS.ETRS89_UTM33:
                    info += $"Conversion from {source} to {CRS.ETRS89_Geod}.\n";
                    steps.Add(TM_GRS80_UTM33.Reverse);
                    return getConversion(CRS.ETRS89_Geod, target, ref steps, ref info, isDBREFZero);
                case CRS.ETRS89_Geod:
                    switch (target)
                    {
                        case CRS.ETRS89_EGBT22_LDP:
                            info += $"Conversion from {source} to {CRS.ETRS89_EGBT22_LDP}:\n";
                            steps.Add(TM_GRS80_EGBT22.Forward);
                            return true;
                        case CRS.ETRS89_UTM33:
                            info += $"Conversion from {source} to {CRS.ETRS89_UTM33}:\n";
                            steps.Add(TM_GRS80_UTM33.Forward);
                            return true;
                    }

                    if (isDBREFZero)
                    {
                        info +=
                            $"Transformation from {source} to {CRS.DB_Ref_Geod} with DB_Ref ellipsoidal height 0.\n";
                        steps.Add(ETRS89_Geod_to_DBRef_Geod_DBRefZero);
                        switch (target)
                        {
                            case CRS.DB_Ref_Geod:
                                return true;
                            case CRS.DB_Ref_GK5:
                                return getConversion(CRS.DB_Ref_Geod, target, ref steps, ref info);
                        }
                    }

                    break;
                case CRS.DB_Ref_GK5:
                    info += $"Conversion from {source} to {CRS.DB_Ref_Geod}.\n";
                    steps.Add(TM_Bessel_GK5.Reverse);
                    return getConversion(CRS.DB_Ref_Geod, target, ref steps, ref info, isDBREFZero);
                case CRS.DB_Ref_Geod:
                    if (target == CRS.DB_Ref_GK5)
                    {
                        info += $"Conversion from {source} to {CRS.DB_Ref_GK5}.\n";
                        steps.Add(TM_Bessel_GK5.Forward);
                        return true;
                    }

                    if (isDBREFZero)
                    {
                        info +=
                            $"Transformation from {source} to {CRS.ETRS89_Geod} with DB_Ref ellipsoidal height 0.\n";
                        steps.Add(DBRef_Geod_to_ETRS89_Geod_DBRefZero);
                        switch (target)
                        {
                            case CRS.ETRS89_EGBT22_LDP:
                            case CRS.ETRS89_UTM33:
                                return getConversion(CRS.ETRS89_Geod, target, ref steps, ref info);
                            case CRS.ETRS89_Geod: return true;
                        }
                    }

                    break;
            }

            info += $"Conversion from {source} to {target} is not supported.\n";
            steps.Clear();
            return false;
        }

        /// <summary>
        ///     Attempts to retrieve a coordinate conversion function from a source CRS and VRS
        ///     to a target CRS. Provides information on the conversion process.
        /// </summary>
        /// <param name="source">The source coordinate reference system (CRS) as a string.</param>
        /// <param name="sourceVRS">The source vertical reference system (VRS) as a string.</param>
        /// <param name="target">The target coordinate reference system (CRS) as a string.</param>
        /// <param name="conversion">
        ///     An output parameter that will be assigned a function. This function takes x, y, and z
        ///     coordinates as inputs and returns the transformed x, y, and z coordinates for the specified conversion.
        /// </param>
        /// <param name="info">
        ///     An output parameter that will be assigned a string describing the conversion process or any issues encountered.
        /// </param>
        /// <returns>
        ///     A boolean value indicating whether the conversion function was successfully retrieved.
        ///     Returns true if the conversion was successful; otherwise, false.
        /// </returns>
        public static bool GetConversion(string source, string sourceVRS, string target,
            out Func<double, double, double, (double x, double y, double z)> conversion, out string info)
        {
            if (!Enum.TryParse(source, out CRS sourceCRS))
            {
                info = $"Source CRS {source} is not supported.";
                conversion = (x, y, z) => (double.NaN, double.NaN, double.NaN);
                return false;
            }

            if (!Enum.TryParse(sourceVRS, out VRS sourceVRS_))
            {
                info = $"Source VRS {sourceVRS} is not supported.";
                conversion = (x, y, z) => (double.NaN, double.NaN, double.NaN);
                return false;
            }

            if (!Enum.TryParse(target, out CRS targetCRS))
            {
                info = $"Target CRS {target} is not supported.";
                conversion = (x, y, z) => (double.NaN, double.NaN, double.NaN);
                return false;
            }


            var steps = new List<Func<double, double, double, (double x, double y, double z)>>();
            info = "";
            if (getConversion(sourceCRS, sourceVRS_, targetCRS, ref steps, ref info))
            {
                conversion = calcSteps(steps);
                return true;
            }

            conversion = (x, y, z) => (double.NaN, double.NaN, double.NaN);
            return false;
        }

        public static bool getConversion(CRS source, VRS sourceVRS, CRS target,
            ref List<Func<double, double, double, (double x, double y, double z)>> steps, ref string info)
        {
            if (source == target) return true;
            switch (source)
            {
                case CRS.ETRS89_EGBT22_LDP:
                    info += $"Conversion from {source} to {CRS.ETRS89_Geod}.\n";
                    steps.Add(TM_GRS80_EGBT22.Reverse);
                    return getConversion(CRS.ETRS89_Geod, sourceVRS, target, ref steps, ref info);
                case CRS.ETRS89_UTM33:
                    info += $"Conversion from {source} to {CRS.ETRS89_Geod}.\n";
                    steps.Add(TM_GRS80_UTM33.Reverse);
                    return getConversion(CRS.ETRS89_Geod, sourceVRS, target, ref steps, ref info);
                case CRS.ETRS89_Geod:
                    switch (target)
                    {
                        case CRS.ETRS89_EGBT22_LDP:
                            info += $"Conversion from {source} to {target}.\n";
                            steps.Add(TM_GRS80_EGBT22.Forward);
                            return true;
                        case CRS.ETRS89_UTM33:
                            info += $"Conversion from {source} to {target}.\n";
                            steps.Add(TM_GRS80_UTM33.Forward);
                            return true;
                        case CRS.ETRS89_Geoc:
                        case CRS.DB_Ref_GK5:
                        case CRS.DB_Ref_Geod:
                        case CRS.DB_Ref_Geoc:
                            if (sourceVRS == VRS.Normal)
                            {
                                info += $"Transformation from {sourceVRS} heights to {VRS.Ellipsoidal} heights.\n";
                                steps.Add(ETRS89_Geod_Normal_to_Ellipsoidal);
                            }

                            info += $"Conversion from {source} to {CRS.ETRS89_Geoc}.\n";
                            steps.Add(GC_GRS80.Forward);
                            return getConversion(CRS.ETRS89_Geoc, VRS.None, target, ref steps, ref info);
                    }

                    break;
                case CRS.ETRS89_Geoc:
                    switch (target)
                    {
                        case CRS.ETRS89_EGBT22_LDP:
                        case CRS.ETRS89_UTM33:
                        case CRS.ETRS89_Geod:
                            info += $"Conversion from {source} to {CRS.ETRS89_Geod}.\n";
                            steps.Add(GC_GRS80.Reverse);
                            return getConversion(CRS.ETRS89_Geod, VRS.Ellipsoidal, target, ref steps, ref info);
                        case CRS.DB_Ref_GK5:
                        case CRS.DB_Ref_Geod:
                        case CRS.DB_Ref_Geoc:
                            info += $"Transformation from {source} to {CRS.DB_Ref_Geoc}.\n";
                            steps.Add(Trans_Datum_ETRS89_to_DBRef.Forward);
                            return getConversion(CRS.DB_Ref_Geoc, VRS.None, target, ref steps, ref info);
                    }

                    break;
                case CRS.DB_Ref_GK5:
                    info += $"Conversion from {source} to {CRS.DB_Ref_Geod}.\n";
                    steps.Add(TM_Bessel_GK5.Reverse);
                    return getConversion(CRS.DB_Ref_Geod, sourceVRS, target, ref steps, ref info);
                case CRS.DB_Ref_Geod:
                    switch (target)
                    {
                        case CRS.ETRS89_EGBT22_LDP:
                        case CRS.ETRS89_UTM33:
                        case CRS.ETRS89_Geod:
                        case CRS.ETRS89_Geoc:
                        case CRS.DB_Ref_Geoc:
                            if (sourceVRS == VRS.Normal)
                            {
                                info +=
                                    $"Iterative transformation from {sourceVRS} heights to {VRS.Ellipsoidal} heights.\n";
                                steps.Add(DBRef_Geod_Normal_to_Ellipsoidal);
                            }

                            info += $"Conversion from {source} to {CRS.DB_Ref_Geoc}.\n";
                            steps.Add(GC_Bessel.Forward);
                            return getConversion(CRS.DB_Ref_Geoc, VRS.None, target, ref steps, ref info);
                        case CRS.DB_Ref_GK5:
                            info += $"Conversion from {source} to {CRS.DB_Ref_GK5}.\n";
                            steps.Add(TM_Bessel_GK5.Forward);
                            return true;
                    }

                    break;
                case CRS.DB_Ref_Geoc:
                    switch (target)
                    {
                        case CRS.ETRS89_EGBT22_LDP:
                        case CRS.ETRS89_UTM33:
                        case CRS.ETRS89_Geod:
                        case CRS.ETRS89_Geoc:
                            info += $"Transformation from {source} to {CRS.ETRS89_Geoc}.\n";
                            steps.Add(Trans_Datum_DBRef_to_ETRS89.Forward);
                            return getConversion(CRS.ETRS89_Geoc, VRS.None, target, ref steps, ref info);
                        case CRS.DB_Ref_GK5:
                        case CRS.DB_Ref_Geod:
                            info += $"Conversion from {source} to {CRS.DB_Ref_Geod}.\n";
                            steps.Add(GC_Bessel.Reverse);
                            return getConversion(CRS.DB_Ref_Geod, sourceVRS, target, ref steps, ref info);
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
                switch (crs_ & CRS.Conversion)
                {
                    case CRS.EGBT22_LDP:
                        calculation = (e, n) =>
                        {
                            _ = TM_GRS80_EGBT22.Reverse(e, n, out double gamma, out double k);
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