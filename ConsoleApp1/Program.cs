using egbt22lib;

using static egbt22lib.Convert;

namespace ConsoleApp1;

internal class Program
{
    static void Main(string[] args)
    {

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

        _ = ETRS89_Geod_3D_to_DBRef_GK5(etrs89lat, etrs89lon, etrs89hell, out double[] gk5rechts1, out double[] gk5hoch1, out double[] h1);
        _ = ConvertProj.ETRS89_Geod_3D_to_DBRef_GK5(etrs89lat, etrs89lon, etrs89hell, out double[] gk5rechts2, out double[] gk5hoch2, out double[] h2);
        _ = DBRef_GK5_to_ETRS89_Geod_3D(gk5rechts1, gk5hoch1, h1, out double[] etrs89lat1, out double[] etrs89lon1, out double[] etrs89hell1);
        _ = ConvertProj.DBRef_GK5_to_ETRS89_Geod_3D(gk5rechts2, gk5hoch2, h2, out double[] etrs89lat2, out double[] etrs89lon2, out double[] etrs89hell2);


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
        double[] gk5rechtsSoll = [
        5418508.9562,  
        5415214.7868, 
        5419312.9614,  
        5414303.8553];
        double[] gk5hochSoll = [     
        5666725.8315,
        5669797.5590,
        5671336.6479,
        5666612.4742];

        for (int i = 0; i < etrs89lat.Length; i++)
        {
            Console.WriteLine($"dR1: {gk5rechts1[i]-gk5rechtsSoll[i],6:e1}");
            Console.WriteLine($"dH1: {gk5hoch1[i]-gk5hochSoll[i],6:e1}");
            Console.WriteLine($"dR2: {gk5rechts2[i]-gk5rechtsSoll[i],6:e1}");
            Console.WriteLine($"dH2: {gk5hoch2[i]-gk5hochSoll[i],6:e1}");
            Console.WriteLine($"dB1: {etrs89lat[i]-etrs89lat1[i],6:e1}");
            Console.WriteLine($"dL1: {etrs89lon[i]-etrs89lon1[i],6:e1}");
            Console.WriteLine($"dB2: {etrs89lat[i]-etrs89lat2[i],6:e1}");
            Console.WriteLine($"dL2: {etrs89lon[i]-etrs89lon2[i],6:e1}");
            Console.WriteLine();
        }
#endif

#if true
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
