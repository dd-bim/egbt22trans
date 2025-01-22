using egbt22lib;

namespace nbsOut;

internal class Program
{
    static void Main(string[] args)
    {
        string outputPath = @"D:\repos\nbs_dresden_prag\Daten\Lokal6";

        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }

        // Trassen einlesen
        (var idsL, var trasseLR, var trasseLH, var trasseLh) = IO.ReadFile(@"D:\repos\nbs_dresden_prag\Daten\Trassen\Koordinaten_L_Inkrement_und_Zwangspunkte.CSV", 0, 1, 2, 3, ';');
        (var idsR, var trasseRR, var trasseRH, var trasseRh) = IO.ReadFile(@"D:\repos\nbs_dresden_prag\Daten\Trassen\Koordinaten_R_Inkrement_und_Zwangspunkte.CSV", 0, 1, 2, 3, ';');

        // Trassen auf Länge bringen und vereinigen
        var indexL0 = Array.IndexOf(idsL, "1+990.00");
        var indexLN = Array.IndexOf(idsL, "32+230.00");
        var lenL = indexLN - indexL0 + 1;
        var indexR0 = Array.IndexOf(idsR, "1+990.00");
        var indexRN = Array.IndexOf(idsR, "32+240.00");
        var lenR = indexRN - indexR0 + 1;
        var trasseR = new double[lenL + lenR];
        var trasseH = new double[lenL + lenR];
        var trasseh = new double[lenL + lenR];
        Array.Copy(trasseLR, indexL0, trasseR, 0, lenL);
        Array.Copy(trasseLH, indexL0, trasseH, 0, lenL);
        Array.Copy(trasseLh, indexL0, trasseh, 0, lenL);
        Array.Copy(trasseRR, indexR0, trasseR, lenL, lenR);
        Array.Copy(trasseRH, indexR0, trasseH, lenL, lenR);
        Array.Copy(trasseRh, indexR0, trasseh, lenL, lenR);

        var ids = new string[lenL + lenR];
        Array.Copy(idsL.Select(s => $"L_{s}").ToArray(), indexL0, ids, 0, lenL);
        Array.Copy(idsR.Select(s => $"R_{s}").ToArray(), indexR0, ids, lenL, lenR);

        if (!egbt22lib.Convert.DBRef_GK5_Gamma_k(trasseR, trasseH, out var gamma, out var k))
        {
            Console.WriteLine("Fehler bei der Berechnung von Gamma und k.");
            return;
        }
        IO.WriteCSV(Path.Combine(outputPath, "alignment_dbref_gk5_dhhn2016.csv"), ["Id","E","N","Elevation DHHN2016","Scale","Gamma[°]"], ',', ids, trasseR, trasseH, trasseh, k, gamma);

        if (!egbt22lib.Convert.DBRef_GK5_Gamma_k(trasseR, trasseH, out gamma, out k))
        {
            Console.WriteLine("Fehler bei der Berechnung von Gamma und k.");
            return;
        }
        IO.WriteCSV(Path.Combine(outputPath, "alignment_dbref_gk5_dhhn2016.csv"), ["Id","E","N","Elevation DHHN2016","Scale","Gamma[°]"], ',', ids, trasseR, trasseH, trasseh, k, gamma);



        Console.WriteLine("Hello, World!");
    }
}
