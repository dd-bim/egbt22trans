using CommandLine;
using CommandLine.Text;

using egbt22lib;
using egbt22lib.Transformations;
using static egbt22lib.Convert;
using Convert = egbt22lib.Convert;


namespace egbt22trans;

internal class Program
{
    static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args)
           .WithParsed(Run);
    }

    private static void Run(Options opts)
    {
        if (!File.Exists(opts.InputFile))
        {
            Console.WriteLine($"Input file: {opts.InputFile} does not exist.");
            return;
        }
        if (File.Exists(opts.OutputFile))
        {
            Console.WriteLine($"Warning, the output file: {opts.OutputFile} will be overwritten.");
        }
        if ((opts.Source == 0 && opts.Target == 0 && opts.Egbt22 == 0)
            || ((opts.Source != 0 || opts.Target != 0) && opts.Egbt22 != 0))
        {
            Console.WriteLine("Either set source '-s' and target '-t' system , or egbt22 '-e' value.");
            return;
        }
        if (opts.Source != 0 && opts.Target != 0 && opts.Source == opts.Target)
        {
            Console.WriteLine("Source '-s' and target '-t' system identical, no conversion.");
            return;
        }
        if (opts.Source != 0 && opts.Target != 0)
        {
            if (opts.Source is < 1 or > 7)
            {
                Console.WriteLine("The source system must have a value in the range 1 to 7.");
                return;
            }
            if (opts.Target is < 1 or > 7)
            {
                Console.WriteLine("The target system must have a value in the range 1 to 7.");
                return;
            }
        }
        else if (opts.Egbt22 is < 1 or > 8)
        {
            Console.WriteLine("The EGBT22 index must have a value in the range 1 to 8.");
            return;
        }
        if (opts.Precision < 0)
        {
            Console.WriteLine("The precision must have a value greater or equal as 0.");
            return;
        }
        if (opts.LatLon < 0)
        {
            Console.WriteLine("The latlon precision must have a value greater or equal as 0.");
            return;
        }
        if (opts.Delimiter.Length != 1)
        {
            Console.WriteLine("The delimiter must have exactly one character.");
            return;
        }
        if (opts.Delimiter == "s")
        {
            opts.Delimiter = " ";
        }
        else if (opts.Delimiter == "t")
        {
            opts.Delimiter = "\t";
        }

        try
        {
            if (opts.Egbt22 < 1)
            {
                string source = Defined_CRS[opts.Source - 1], 
                    target = Defined_CRS[opts.Target - 1];
                if (opts.ZAxis < 1)
                {
                    // Determine conversion function
                    if (!GetConversion(source, target, out var conversion, out string info))
                    {
                        Console.WriteLine($"Conversion from source {source} (ID {opts.Source}) to target {target} (ID {opts.Target}) is not supported.");
                        return;
                    }
                    Console.WriteLine($"Conversion info: \n{info}");
                    
                    // Read input file
                    (double[] xin, double[] yin) = IO.ReadFile(opts.InputFile, opts.XAxis - 1, opts.YAxis - 1, opts.Delimiter[0], out string[][] coordinateLines);

                    // Apply conversion
                    (double[] xout, double[] yout) = CalcArrays2(xin, yin, conversion);

                    // Write output file
                    IO.WriteFile(opts.OutputFile, xout, yout, coordinateLines, opts.XAxis - 1, opts.YAxis - 1, opts.Precision, opts.Delimiter[0]);
                }
                else
                {
                    if (opts.Height < 0 || opts.Height > 3)
                    {
                        Console.WriteLine("The index for the height system must have a value in the range 0 to 3.");
                        return;
                    }
                    string sourceVRS = Defined_VRS[opts.Height];
                    // Determine conversion function
                    if (!GetConversion(source, sourceVRS, target, out var conversion, out string info))
                    {
                        Console.WriteLine($"Conversion from source {source} (ID {opts.Source}) to target {target} (ID {opts.Target}) with {sourceVRS} heights (ID {opts.Height}) is not supported.");
                        return;
                    }

                    Console.WriteLine($"Conversion info: \n{info}");
                   
                    // Read input file
                    var (xin, yin, zin) = IO.ReadFile(opts.InputFile, opts.XAxis - 1, opts.YAxis - 1, opts.ZAxis - 1,
                        opts.Delimiter[0], out var coordinateLines);

                    // Apply conversion
                    (double[] xout, double[] yout, double[] zout) = CalcArrays3(xin, yin, zin, conversion);

                    // Write output file
                    if (opts.Height == 1 && !target.EndsWith("Geoc"))
                    {
                        // Normal heights keep unchanged
                        IO.WriteFile(opts.OutputFile, xout, yout, coordinateLines, opts.XAxis - 1, opts.YAxis - 1, opts.Precision, opts.Delimiter[0]);
                    }
                    else
                    {
                        IO.WriteFile(opts.OutputFile, xout, yout, zout, coordinateLines, opts.XAxis - 1, opts.YAxis - 1, opts.ZAxis - 1, opts.Precision, opts.Delimiter[0]);
                    }
                }

                Console.WriteLine("Conversion completed successfully.");
            }
            else
            {
                (double[] xin, double[] yin, double[] zin) = IO.ReadFile(opts.InputFile, opts.XAxis - 1, opts.YAxis - 1, opts.ZAxis - 1, opts.Delimiter[0], out string[][] coordinateLines);
                
                bool isGeod = opts.Egbt22 > 4;
                if (isGeod)
                {
                    (xin, yin, zin) = CalcArrays3(xin, yin, zin, egbt22lib.Conversions.Defined.GC_GRS80.Forward);
                    opts.Egbt22 -= 4;
                }

                (double[] xout, double[] yout, double[] zout) = opts.Egbt22 switch
                {
                    1 => CalcArrays3(xin, yin, zin, Defined.Trans_Datum_ETRS89_DREF91_to_EGBT22.Forward),
                    2 => CalcArrays3(xin, yin, zin, Defined.Trans_Datum_ETRS89_DREF91_to_EGBT22.Reverse),
                    3 => CalcArrays3(xin, yin, zin, Defined.Trans_Datum_ETRS89_CZ_to_EGBT22.Forward),
                    4 => CalcArrays3(xin, yin, zin, Defined.Trans_Datum_ETRS89_CZ_to_EGBT22.Reverse),
                    _ => throw new ArgumentException("Invalid operation")
                };

                if (isGeod)
                {
                    (xout, yout, zout) = CalcArrays3(xout, yout, zout, egbt22lib.Conversions.Defined.GC_GRS80.Reverse);
                }
                
                IO.WriteFile(opts.OutputFile, xout, yout, zout, coordinateLines, opts.XAxis - 1, opts.YAxis - 1, opts.ZAxis - 1, opts.Precision, opts.Delimiter[0]);
            }


        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred during the conversion: {ex.Message}");
        }
        
    }

}
