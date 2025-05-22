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
    [Option('s', "source", Default = (int)0, HelpText = "Id of source system\n" +
        "1 EGBT22 EGBT_LDP Dresden-Prague\n" +
        "2 EGBT22 geographic 3D B/L\n" +
        "3 EGBT22 cartesian 3D geocentric\n" +
        "4 ETRS89/DREF91 UTM33\n" +
        "5 ETRS89/DREF91 geographic 3D B/L\n" +
        "6 ETRS89/DREF91 cartesian 3D geocentric\n" +
        "7 ETRS89/CZ geographic 3D B/L\n" +
        "8 ETRS89/CZ cartesian 3D geocentric\n" +
        "9 DB_REF GK5\n" +
        "10 DB_REF geographic 3D B/L\n" +
        "11 DB_REF cartesian 3D geocentric")]
    public int Source { get; set; } = 0;

    [Option('t', "target", Default = (int)0, HelpText = "Id of target system\n" +
        "1 EGBT22 EGBT_LDP Dresden-Prague\n" +
        "2 EGBT22 geographic 3D B/L\n" +
        "3 EGBT22 cartesian 3D geocentric\n" +
        "4 ETRS89/DREF91 UTM33\n" +
        "5 ETRS89/DREF91 geographic 3D B/L\n" +
        "6 ETRS89/DREF91 cartesian 3D geocentric\n" +
        "7 ETRS89/CZ geographic 3D B/L\n" +
        "8 ETRS89/CZ cartesian 3D geocentric\n" +
        "9 DB_REF GK5\n" +
        "10 DB_REF geographic 3D B/L\n" +
        "11 DB_REF cartesian 3D geocentric")]
    public int Target { get; set; } = 0;

    [Option('h', "height", Default = (int)0, HelpText = "Id of the height system\n" +
        "1 Normal heights (calculations based on the GCG2016 geoid)\n" +
        "2 Ellipsoidal heights (ETRS89 or DB_Ref)")]
    public int Height { get; set; } = 0;

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
        new("Normal case", new Options { Delimiter = ",", Source = 1, Target = 2, InputFile = "input.txt", OutputFile = "output.txt" })
    ];

}
