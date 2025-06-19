using egbt22lib;
using egbt22lib.Conversions;

using System.Collections.Generic;
using System.Globalization;
using System.Text;

using static egbt22lib.Conversions.Defined;
using static egbt22lib.Convert;
using static egbt22lib.IO;
using static egbt22lib.Transformations.Defined;
using static System.Collections.Specialized.BitVector32;

namespace ConsoleApp1;

internal class Program
{
    static void Main(string[] args)
    {

#if false
        double[][] dbrefgk5normal =
        [
            [5421897.2654 , 5644771.2149 , 203.3812],
            [5416390.6455 , 5633711.0274 , 507.7331],
            [5415656.3876 , 5632486.3572 , 565.4736],
            [5439228.99090, 5643205.09820, 135.2334],
            [5420933.62460, 5649263.87400, 120.4556],
            [5426249.0224 , 5649795.6315 , 163.7305],
            [5424092.13100, 5647393.08580, 128.0842],
            [5431140.4471 , 5645156.4992 , 250.8269],
            [5443362.91830, 5640942.57420, 278.3743],
            [5444757.69170, 5638504.90390, 290.9577],
            [5416508.0905 , 5628483.2167 , 473.2350],
            [5415796.4543 , 5625442.0687 , 694.5982],
            [5421413.86900, 5650328.63870, 114.4409],
            [5422566.26850, 5647830.44290, 119.0223],
            [5445383.98330, 5636295.54910, 130.7665] ,
        ];

        double[][] etrs89utm33normal = [
            [421792.3661, 5642958.9791, 203.385],
            [416287.8657, 5631903.2542, 507.727],
            [415553.8868, 5630679.0821, 565.464],
            [439117.1712, 5641393.3449, 135.239],
            [420829.1498, 5647449.8491, 120.452],
            [426142.4342, 5647981.3544, 163.736],
            [423986.3778, 5645579.7869, 128.091],
            [431031.8688, 5643344.0359, 250.831],
            [443249.4294, 5639131.6914, 278.386],
            [444643.6305, 5636694.9815, 290.957],
            [416405.2187, 5626677.5322, 473.258],
            [415693.8473, 5623637.6053, 694.620],
            [421309.2120, 5648514.1839, 114.438],
            [422461.1306, 5646016.9766, 119.024],
            [445269.6560, 5634486.5035, 130.772],
            ];

        double[][] etrs89utm33ellip = [
            [421792.3661, 5642958.9791, 246.6889],
            [416287.8657, 5631903.2542, 551.3906],
            [415553.8868, 5630679.0821, 609.1752],
            [439117.1712, 5641393.3449, 178.4506],
            [420829.1498, 5647449.8491, 163.6957],
            [426142.4342, 5647981.3544, 206.9308],
            [423986.3778, 5645579.7869, 171.3165],
            [431031.8688, 5643344.0359, 294.0435],
            [443249.4294, 5639131.6914, 321.6158],
            [444643.6305, 5636694.9815, 334.2047],
            [416405.2187, 5626677.5322, 517.0616],
            [415693.8473, 5623637.6053, 738.5180],
            [421309.2120, 5648514.1839, 157.6664],
            [422461.1306, 5646016.9766, 162.2630],
            [445269.6560, 5634486.5035, 174.0392]
        ];

        double[][] etrs89geoc = [
            [3910245.2605,  966749.0848, 4929038.8646],
            [3920103.7151,  963686.2550, 4922244.1661],
            [3921240.4392,  963229.9399, 4921507.9977],
            [3907007.5453,  983823.9027, 4928145.2490],
            [3907066.2933,  964900.8670, 4931794.9268],
            [3905356.8389,  969944.5708, 4932212.7157],
            [3907680.6004,  968334.8878, 4930652.6051],
            [3907658.0119,  975623.6656, 4929401.0284],
            [3907744.7147,  988299.6065, 4926859.4030],
            [3909226.0239,  990140.3313, 4925341.0235],
            [3923967.2717,  964843.4880, 4918915.7108],
            [3926557.4720,  964797.8119, 4917157.0016],
            [3906142.8821,  965150.6492, 4932465.1331],
            [3907730.6533,  966768.9154, 4930906.7756],
            [3910624.7346,  991164.8896, 4923826.7347]
            ];

        double[][] egbt22 = new double[dbrefgk5normal.Length][];

        Console.WriteLine("DBRef_GK5 0 -> ETRS89_EGBT22_LDP");

        if (GetConversion(CRS.DB_Ref_GK5, CRS.ETRS89_EGBT22_LDP, out var conversion, out var info, true))
        {
            Console.WriteLine(info);
            Console.WriteLine();
            for (int i = 0; i < dbrefgk5normal.Length; i++)
            {
                var (e, n) = conversion(dbrefgk5normal[i][0], dbrefgk5normal[i][1]);
                egbt22[i] = new double[] { e, n, dbrefgk5normal[i][2] };
                Console.WriteLine(FormattableString.Invariant($"{i,2} {e,12:F4} {n,12:F4}"));
            }
        }
        else
        {
            Console.WriteLine(info);
        }
        Console.WriteLine();

        Console.WriteLine("ETRS89_EGBT22_LDP -> DBRef_GK5 0");
        if (GetConversion(CRS.ETRS89_EGBT22_LDP, CRS.DB_Ref_GK5, out conversion, out info, true))
        {
            Console.WriteLine(info);
            Console.WriteLine();
            for (int i = 0; i < egbt22.Length; i++)
            {
                var (e, n) = conversion(egbt22[i][0], egbt22[i][1]);
                Console.WriteLine(FormattableString.Invariant($"{i,2} {(dbrefgk5normal[i][0] - e) * 1000,8:F2} {(dbrefgk5normal[i][1] - n) * 1000,8:F2}"));
            }

        }
        else
        {
            Console.WriteLine(info);
        }
        Console.WriteLine();

        Console.WriteLine("ETRS89_EGBT22_LDP gamma k");
        if (GetGammaKCalculation(CRS.ETRS89_EGBT22_LDP, out var gammak))
        {
            for (int i = 0; i < egbt22.Length; i++)
            {
                var (gamma, k) = gammak(egbt22[i][0], egbt22[i][1]);
                Console.WriteLine(FormattableString.Invariant($"{i,2} {gamma,12:F6} {k,12:F9}"));
            }
        }
        else
        {
            Console.WriteLine("GammaK failed");
        }
        Console.WriteLine();











#endif

#if true
        static (double x, double y, double z)[] readCoordinates(string file)
        {
            string[] lines = File.ReadAllLines(file, Encoding.UTF8);
            var data = new (double x, double y, double z)[lines.Length];
            for (int i = 0; i < lines.Length; i++)
            {
                string[] parts = lines[i].Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                data[i] = (
                    double.Parse(parts[0], CultureInfo.InvariantCulture),
                    double.Parse(parts[1], CultureInfo.InvariantCulture),
                    double.Parse(parts[2], CultureInfo.InvariantCulture));
            }
            return data;
        }

        static string formatInfo(string info)
        {
            string[] parts = info.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
            return string.Join("|>", parts);
        }

        string dataDirectory = @"E:\source\egbt22trans\TestData";

        //string extension = "wei";
        string extension = "txt";

        string[] dataFiles = [.. Directory.GetFiles(dataDirectory, $"*.{extension}", SearchOption.TopDirectoryOnly)];

        var sources = new List<(string crs, string vrs, string origin, (double x, double y, double z)[] coordinates)>();
        var crsMap = new Dictionary<string, int>();
        foreach (string file in dataFiles)
        {
            string crs = Defined_CRS.FirstOrDefault(file.Contains, "");
            crsMap.Add(crs, sources.Count);
            string vrs = Defined_VRS.FirstOrDefault(file.Contains, "None");
            int idx = file.LastIndexOf("_", StringComparison.OrdinalIgnoreCase);
            string origin = file.Substring(idx + 1, file.Length - idx - 5);
            var coordinates = readCoordinates(file);
            sources.Add((crs, vrs, origin, coordinates));
        }

        // Test 2D
        string header = "sourceCRS,targetCRS,sourceOrigin,dxRoundTrip,dyRoundTrip,dxOriginal,dyOriginal,dxProj,dyProj,stepsForward,stepsReverse";

        var csv = new StringBuilder(header);
        csv.AppendLine();
        for (int i = 0; i < sources.Count; i++)
        {
            (string source, string vrs, string origin, (double x, double y, double z)[] coordinates) = sources[i];
            if (!Defined_CRS_2D.Contains(source))
                continue;
            for (int j = 0; j < Defined_CRS_2D.Length; j++)
            {
                string target = Defined_CRS_2D[j];
                if (source == target)
                    continue;
                if (GetConversion(source, target, out var conversion, out string forwardInfo, true))
                {
                    var result = new (double x, double y)[coordinates.Length];
                    for (int k = 0; k < coordinates.Length; k++)
                    {
                        var (sx, sy, _) = coordinates[k];
                        result[k] = conversion(sx, sy);
                    }
                    int targetIdx = crsMap.TryGetValue(target, out int idx) ? idx : -1;
                    bool targetProj = false;
                    (double dx, double dy)[]? origDiff = null;
                    if (targetIdx >= 0)
                    {
                        var targetOrig = sources[targetIdx].coordinates;
                        targetProj = sources[targetIdx].origin == "proj";
                        origDiff = new (double dx, double dy)[targetOrig.Length];
                        for (int k = 0; k < targetOrig.Length; k++)
                        {
                            var (tx, ty, _) = targetOrig[k];
                            var (x, y) = result[k];
                            origDiff[k] = (tx - x, ty - y);
                        }
                    }
                    if (GetConversion(target, source, out conversion, out string reverseInfo, true))
                    {
                        for (int k = 0; k < result.Length; k++)
                        {
                            var (ox, oy, _) = coordinates[k];
                            var (x, y) = result[k];
                            (x, y) = conversion(x, y);
                            if (origDiff != null)
                            {
                                if (targetProj)
                                {
                                    csv.AppendLine(FormattableString.Invariant
                                        ($"\"{source}\",\"{target}\",\"{origin}\",{ox - x:E3},{oy - y:E3},,,{origDiff[k].dx:E3},{origDiff[k].dy:E3},\"{formatInfo(forwardInfo)}\",\"{formatInfo(reverseInfo)}\""));
                                }
                                else
                                {
                                    csv.AppendLine(FormattableString.Invariant
                                        ($"\"{source}\",\"{target}\",\"{origin}\",{ox - x:E3},{oy - y:E3},{origDiff[k].dx:E3},{origDiff[k].dy:E3},,,\"{formatInfo(forwardInfo)}\",\"{formatInfo(reverseInfo)}\""));
                                }
                            }
                            else
                            {
                                csv.AppendLine(FormattableString.Invariant
                                ($"\"{source}\",\"{target}\",\"{origin}\",{ox - x:E3},{oy - y:E3},,,,,\"{formatInfo(forwardInfo)}\",\"{formatInfo(reverseInfo)}\""));
                            }
                        }
                    }
                    else
                    {
                        csv.AppendLine(FormattableString.Invariant($"\"{source}\",\"{target}\",\"\",,,,,,,\"{formatInfo(forwardInfo)}\",\"{formatInfo(reverseInfo)}\""));
                    }
                }
                else
                {
                    _ = GetConversion(source, target, out _, out string reverseInfo, true);
                    csv.AppendLine(FormattableString.Invariant($"\"{source}\",\"{target}\",\"\",,,,,,,\"{formatInfo(forwardInfo)}\",\"{formatInfo(reverseInfo)}\""));
                }
            }
        }
        File.WriteAllText(Path.Combine(dataDirectory, $"results2D_{extension}.csv"), csv.ToString(), Encoding.UTF8);

        // Test 3D

        header = "sourceCRS,sourceVRS,targetCRS,sourceOrigin,dxRoundTrip,dyRoundTrip,dzRoundTrip,dxOriginal,dyOriginal,dzOriginal,dxProj,dyProj,dzProj,stepsForward,stepsReverse";
        csv = new StringBuilder(header);
        csv.AppendLine();
        for (int i = 0; i < sources.Count; i++)
        {
            (string source, string vrs, string origin, (double x, double y, double z)[] coordinates) = sources[i];
            if (!Defined_CRS.Contains(source))
                continue;
            if(!Defined_VRS.Contains(vrs))
                continue;
            for (int j = 0; j < Defined_CRS.Length; j++)
            {
                string target = Defined_CRS[j];
                if (source == target)
                    continue;
                if (GetConversion(source, vrs, target, out var conversion, out string forwardInfo, out string targetVRS))
                {
                    var result = new (double x, double y, double z)[coordinates.Length];
                    for (int k = 0; k < coordinates.Length; k++)
                    {
                        var (sx, sy, sz) = coordinates[k];
                        result[k] = conversion(sx, sy, sz);
                    }
                    int targetIdx = crsMap.TryGetValue(target, out int idx) ? idx : -1;
                    bool targetProj = false;
                    (double dx, double dy, double dz)[]? origDiff = null;
                    if (targetIdx >= 0)
                    {
                        var targetOrig = sources[targetIdx].coordinates;
                        targetProj = sources[targetIdx].origin == "proj";
                        origDiff = new (double dx, double dy, double dz)[targetOrig.Length];
                        for (int k = 0; k < targetOrig.Length; k++)
                        {
                            var (tx, ty, tz) = targetOrig[k];
                            var (x, y, z) = result[k];
                            origDiff[k] = (tx - x, ty - y, tz - z);
                        }
                    }
                    if (GetConversion(target, targetVRS, source, out conversion, out string reverseInfo, out _))
                    {
                        for (int k = 0; k < result.Length; k++)
                        {
                            var (ox, oy, oz) = coordinates[k];
                            var (x, y, z) = result[k];
                            (x, y, z) = conversion(x, y, z);
                            if (origDiff != null)
                            {
                                if (targetProj)
                                {
                                    csv.AppendLine(FormattableString.Invariant
                                        ($"\"{source}\",\"{vrs}\",\"{target}\",\"{origin}\",{ox - x:E3},{oy - y:E3},{oz - z:E3},,,,{origDiff[k].dx:E3},{origDiff[k].dy:E3},{origDiff[k].dz:E3},\"{formatInfo(forwardInfo)}\",\"{formatInfo(reverseInfo)}\""));
                                }
                                else
                                {
                                    csv.AppendLine(FormattableString.Invariant
                                        ($"\"{source}\",\"{vrs}\",\"{target}\",\"{origin}\",{ox - x:E3},{oy - y:E3},{oz - z:E3},{origDiff[k].dx:E3},{origDiff[k].dy:E3},{origDiff[k].dz:E3},,,,\"{formatInfo(forwardInfo)}\",\"{formatInfo(reverseInfo)}\""));
                                }
                            }
                            else
                            {
                                csv.AppendLine(FormattableString.Invariant
                                ($"\"{source}\",\"{vrs}\",\"{target}\",\"{origin}\",{ox - x:E3},{oy - y:E3},{oz - z:E3},,,,,,,\"{formatInfo(forwardInfo)}\",\"{formatInfo(reverseInfo)}\""));
                            }
                        }
                    }
                    else
                    {
                        csv.AppendLine(FormattableString.Invariant($"\"{source}\",\"{vrs}\",\"{target}\",\"\",,,,,,,,,,\"{formatInfo(forwardInfo)}\",\"{formatInfo(reverseInfo)}\""));
                    }
                }
                else
                {
                    _ = GetConversion(source, target, out _, out string reverseInfo, true);
                    csv.AppendLine(FormattableString.Invariant($"\"{source}\",\"{vrs}\",\"{target}\",\"\",,,,,,,,,,\"{formatInfo(forwardInfo)}\",\"{formatInfo(reverseInfo)}\""));
                }
            }
        }
        File.WriteAllText(Path.Combine(dataDirectory, $"results3D_{extension}.csv"), csv.ToString(), Encoding.UTF8);

#endif



#if false
        var (Station, Rechtswert, Hochwert, Hoehe) = ReadFile(@"E:\source\nbs_dresden_prag\Daten\Trassen\Koordinaten_L_Inkrement_und_Zwangspunkte.CSV", 0, 1, 2, 3, ';');
        double[] zeroEllH = new double[Station.Length];
        bool ok = DBRef_GK5_to_EGBT22_Local_Ell(Rechtswert, Hochwert, zeroEllH, out double[] localR, out double[] localH, out double[] localEllH);

        if (!ok)
        {
            Console.WriteLine("DBRef_GK5_to_EGBT22_Local_Ell failed");
            return;
        }
        //// Schreibe neue Koordinaten in Konsole
        //Console.WriteLine("Lokal");
        //for (int i = 0; i < localR.Length; i++)
        //{
        //    Console.WriteLine(FormattableString.Invariant($"{Station[i],12} {localR[i],12:f4} {localH[i],12:f4} {localEllH[i],12:f4}"));
        //}

        ok = DBRef_GK5_to_ETRS89_UTM33_Ell(Rechtswert, Hochwert, zeroEllH, out double[] easting, out double[] northing, out double[] etrs89EllH);

        if (!ok)
        {
            Console.WriteLine("DBRef_GK5_to_ETRS89_UTM33_Ell failed");
            return;
        }
        //// Schreibe neue Koordinaten in Konsole
        //Console.WriteLine("Diff EllH Local-ETRS89 (soll 0)");
        //for (int i = 0; i < localR.Length; i++)
        //{
        //    Console.WriteLine(FormattableString.Invariant($"{Station[i],12} {localEllH[i] - etrs89EllH[i],12:f4}"));
        //}

        ok = EGBT22_Local_to_DBRef_GK5_Ell(localR, localH, localEllH, out double[] gk5R, out double[] gk5H, out double[] gk5EllH);

        if (!ok)
        {
            Console.WriteLine("EGBT22_Local_to_DBRef_GK5_Ell failed");
            return;
        }
        // Schreibe neue Koordinaten in Konsole
        Console.WriteLine("Lokal -> DB_Ref");
        for (int i = 0; i < localR.Length; i++)
        {
            Console.WriteLine(FormattableString.Invariant($"{Station[i],12} {Rechtswert[i] - gk5R[i],12:f4} {Hochwert[i] - gk5H[i],12:f4} {gk5EllH[i],12:f4}"));
        }


#endif


#if false
        double[] etrs89lat = [
            51.12979158,
            51.15691781,
            51.17134218,
            51.12815886];
        double[] etrs89lon = [
            13.83390966,
            13.78613802,
            13.84435879,
            13.77387860];
        double[] etrs89hell = [
            261.7528,
            250.4893,
            233.3473,
            267.3302 ];
        double[] gk5rechts = [
            5418508.9562,  
            5415214.7868, 
            5419312.9614,  
            5414303.8553];
        double[] gk5hoch = [     
            5666725.8315,
            5669797.5590,
            5671336.6479,
            5666612.4742];

        _ =  ETRS89_Geod_3D_to_DBRef_GK5(etrs89lat, etrs89lon, etrs89hell, out var gk5rechts1, out var gk5hoch1, out var h1);
        //_ = ETRS89_Geod_3D_to_DBRef_GK52(etrs89lat, etrs89lon, etrs89hell, out var gk5rechts2, out var gk5hoch2, out var h2);
 
        //_ = ConvertProj.ETRS89_Geod_3D_to_DBRef_GK5(etrs89lat, etrs89lon, etrs89hell, out double[] gk5rechts2, out double[] gk5hoch2, out double[] h2);
 
        _ = DBRef_GK5_to_ETRS89_Geod_3D(gk5rechts1, gk5hoch1, h1, out var etrs89lat1, out var etrs89lon1, out var etrs89hell1);
        //_ = DBRef_GK5_to_ETRS89_Geod_3D(gk5rechts2, gk5hoch2, h2, out var etrs89lat2, out var etrs89lon2, out var etrs89hell2);

        //_ = ConvertProj.DBRef_GK5_to_ETRS89_Geod_3D(gk5rechts2, gk5hoch2, h2, out double[] etrs89lat2, out double[] etrs89lon2, out double[] etrs89hell2);


        // double[] etrs89xSoll = [
        // 3894616.9775,
        // 3893125.9700,
        // 3890929.1249,
        // 3895760.4351];
        // double[] etrs89ySoll = [
        // 959054.0689,	
        // 955244.7966,	
        // 958898.5898,	
        // 955007.5036];
        // double[] etrs89zSoll = [
        // 4942822.6100,
        // 4944707.2172,
        // 4945700.2168,
        // 4942712.9557];
        //double[] nSoll = [
        // 43.1887,
        // 43.144,
        // 43.1593,
        // 43.162,];

        for (var i = 0; i < etrs89lat.Length; i++)
        {
            Console.WriteLine($"dR1: {gk5rechts1[i]-gk5rechts[i],6:e1}");
            Console.WriteLine($"dH1: {gk5hoch1[i]-gk5hoch[i],6:e1}");
            //Console.WriteLine($"dR2: {gk5rechts2[i] - gk5rechts[i],6:e1}");
            //Console.WriteLine($"dH2: {gk5hoch2[i] - gk5hoch[i],6:e1}");
            Console.WriteLine($"dB1: {etrs89lat[i]-etrs89lat1[i],6:e1}");
            Console.WriteLine($"dL1: {etrs89lon[i]-etrs89lon1[i],6:e1}");
            //Console.WriteLine($"dB2: {etrs89lat[i] - etrs89lat2[i],6:e1}");
            //Console.WriteLine($"dL2: {etrs89lon[i] - etrs89lon2[i],6:e1}");
            Console.WriteLine();
        }
#endif

#if false
        var dbrefR = new double[]
        {
            5422510.205,
            5422513.769,
            5422515.795,
            5422521.357,
            5422526.888,
            5422526.976,
            5422532.390,
            5422537.862
        };
        var dbrefH = new double[]
        {
            5647607.034,
            5647601.755,
            5647598.743,
            5647590.432,
            5647582.101,
            5647581.968,
            5647573.751,
            5647565.381
        };
        var dbrefh = new double[]
        {
            133.474,
            133.510,
            133.530,
            133.579,
            133.623,
            133.623,
            133.663,
            133.703
        };

        if (!DBRef_GK5_to_EGBT22_Local(dbrefR, dbrefH, dbrefh, out double[] localR1, out double[] localH1))
        {
            Console.WriteLine("DBRef_GK5_to_EGBT22_Local failed");
            return;
        }
        Console.WriteLine("Lokal erste Berechnung");
        for (int i = 0; i < localR1.Length; i++)
        {
            Console.WriteLine($"{localR1[i],12:f4} {localH1[i],12:f4}");
        }

        if (!ConvertProj.DBRef_GK5_to_EGBT22_Local(dbrefR, dbrefH, dbrefh, out double[] localR2, out double[] localH2))
        {
            Console.WriteLine("DBRef_GK5_to_EGBT22_Local2 failed");
            return;
        }
        Console.WriteLine("Lokal zweite Berechnung");
        for (int i = 0; i < localR1.Length; i++)
        {
            Console.WriteLine($"{localR2[i],12:f4} {localH2[i],12:f4}");
        }
        Console.WriteLine("Lokal Differenz");
        for (int i = 0; i < localR1.Length; i++)
        {
            Console.WriteLine($"{localR1[i] - localR2[i],6:e1} {localH1[i] - localH2[i],6:e1}");
        }

        if (!EGBT22_Local_to_DBRef_GK5(localR1, localH1, dbrefh, out double[] dbrefR1, out double[] dbrefH1))
        {
            Console.WriteLine("EGBT22_Local_to_DBRef_GK5 failed");
            return;
        }
        Console.WriteLine("erste Berechnung Differenz");
        for (int i = 0; i < dbrefR1.Length; i++)
        {
            Console.WriteLine($"{dbrefR1[i] - dbrefR[i],6:e1} {dbrefH1[i] - dbrefH[i],6:e1}");
        }
        if (!ConvertProj.EGBT22_Local_to_DBRef_GK5(localR2, localH2, dbrefh, out double[] dbrefR2, out double[] dbrefH2))
        {
            Console.WriteLine("EGBT22_Local_to_DBRef_GK52 failed");
            return;
        }
        Console.WriteLine("zweite Berechnung Differenz");
        for (int i = 0; i < dbrefR1.Length; i++)
        {
            Console.WriteLine($"{dbrefR2[i] - dbrefR[i],6:e1} {dbrefH2[i] - dbrefH[i],6:e1}");
        }


#endif
        Console.WriteLine("Hello, World!");
    }
}
