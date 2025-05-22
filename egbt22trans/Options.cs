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
    // Value range constants
    public const int MinSystemId = 1;
    public const int MaxSystemId = 11;
    public const int MinHeightId = 0;
    public const int MaxHeightId = 2;

    [Option('s', "source", Default = 0, HelpText = "ID of the source system (1-11). See documentation for details.")]
    public int Source { get; set; }

    [Option('t', "target", Default = 0, HelpText = "ID of the target system (1-11). See documentation for details.")]
    public int Target { get; set; }

    [Option('h', "height", Default = 0, HelpText = "ID of the height system (0: none, 1: normal heights, 2: ellipsoidal heights)")]
    public int Height { get; set; }

    [Option('p', "precision", Default = 4, HelpText = "Decimal places for coordinates in the output file (>=0).")]
    public int Precision { get; set; }

    [Option('l', "latlon", Default = 10, HelpText = "Decimal places for latitude/longitude in the output file (>=0).")]
    public int LatLon { get; set; }

    [Option('x', "xaxis", Default = 2, HelpText = "Column index of the x-axis (1-based).")]
    public int XAxis { get; set; }

    [Option('y', "yaxis", Default = 3, HelpText = "Column index of the y-axis (1-based).")]
    public int YAxis { get; set; }

    [Option('z', "zaxis", Default = 4, HelpText = "Column index of the z-axis (1-based, 0 = no z-coordinate).")]
    public int ZAxis { get; set; }

    [Option('d', "delimiter", Default = " ", HelpText = "Delimiter for columns: space (' '), tab ('\\t'), comma (','), etc.")]
    public string Delimiter { get; set; } = " ";

    [Value(0, Required = true, HelpText = "Input file path.")]
    public string InputFile { get; set; } = string.Empty;

    [Value(1, Required = true, HelpText = "Output file path.")]
    public string OutputFile { get; set; } = string.Empty;

    [Usage(ApplicationAlias = "egbt22trans.exe")]
    public static IEnumerable<Example> Examples => [
        new("Normal case", new Options { Delimiter = ",", Source = 1, Target = 2, InputFile = "input.txt", OutputFile = "output.txt" })
    ];
}
