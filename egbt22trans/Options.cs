using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandLine;
using CommandLine.Text;

namespace egbt22trans;
internal class Options
{
    [Option('s', "source", Default = (int)0, HelpText = "Number of source system\n" +
        "1 ETRS89 EGBT_LDP Dresden-Prag\n" +
        "2 ETRS89 UTM33 (EPSG:25833)\n" +
        "3 DB_REF GK5 (EPSG:5685)\n" +
        "4 ETRS89 Kartesisch 3D Geozentrisch (EPSG:4936)\n" +
        "5 ETRS89 Geographisch 3D B/L/ellip. h (EPSG:4937)" )]
    public int Source { get; set; } = 0;

    [Option('t', "target", Default = (int)0, HelpText = "Number of target system\n" +
        "1 ETRS89 EGBT_LDP Dresden-Prag\n" +
        "2 ETRS89 UTM33 (EPSG:25833)\n" +
        "3 DB_REF GK5 (EPSG:5685)\n" +
        "4 ETRS89 Kartesisch 3D Geozentrisch (EPSG:4936)\n" +
        "5 ETRS89 Geographisch 3D B/L/ellip. h (EPSG:4937)" )]
    public int Target { get; set; } = 0;

    [Option('e', "egbt22", Default = (int)0, 
        HelpText = "Special, Transformation from/to EGBT22\n" +
        "1=Sapos(ETRS89/DREF91(R2016)) to EGBT22 (EPSG:4936), \n" +
        "2=EGBT22 to Sapos (EPSG:4936), \n" +
        "3=Czepos(ETRS89-CZ) to EGBT22 (EPSG:4936), \n" +
        "4=EGBT22 to Czepos (EPSG:4936), \n" +
        "5=Sapos(ETRS89/DREF91(R2016)) to EGBT22 (EPSG:4937), \n" +
        "6=EGBT22 to Sapos (EPSG:4937), \n" +
        "7=Czepos(ETRS89-CZ) to EGBT22 (EPSG:4937), \n" +
        "8=EGBT22 to Czepos (EPSG:4937)"
        )]
    public int Egbt22 { get; set; } = 0;

    [Option('p', "precision", Default = (int)4, HelpText = "Precision (decimal places) of coordinates in target file")]
    public int Precision { get; set; } = 4;

    [Option('l', "latlon", Default = (int)10, HelpText = "Precision (decimal places) of latitude and longitude values in target file")]
    public int LatLon { get; set; } = 10;

    [Option('x', "xaxis", Default = (int)2, HelpText = "Column-index of x-axis")]
    public int XAxis { get; set; } = 2;

    [Option('y', "yaxis", Default = (int)3, HelpText = "Column-index of y-axis")]
    public int YAxis { get; set; } = 3;

    [Option('z', "zaxis", Default = (int)4, HelpText = "Column-index of z-axis")]
    public int ZAxis { get; set; } = 4;

    [Option('d', "delimiter", Default = (string)" ", HelpText = "Delimiter of the columns in the coordinate file. (default: space")]
    public string Delimiter { get; set; } = " ";

    [Value(0, Required = true)]
    public string InputFile { get; set; } = null!;

    [Value(1, Required = true)]
    public string OutputFile { get; set; } = null!;

    [Usage(ApplicationAlias = "egbt22trans.exe")]
    public static IEnumerable<Example> Examples => [
        new("Normal case", new Options { Delimiter = ",", Source = 1, Target = 2, InputFile = "input.txt", OutputFile = "output.txt" }),
        new("Special case", new Options { Delimiter = ",", Egbt22 = 1, ZAxis = 4, InputFile = "input.txt", OutputFile = "output.txt" })
    ];

}
