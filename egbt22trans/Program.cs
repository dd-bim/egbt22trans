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
        if (opts.Source < Options.MinSystemId || opts.Source > Options.MaxSystemId)
        {
            Console.WriteLine($"The source system must have a value in the range {Options.MinSystemId} to {Options.MaxSystemId}.");
            return;
        }
        if (opts.Target < Options.MinSystemId || opts.Target > Options.MaxSystemId)
        {
            Console.WriteLine($"The target system must have a value in the range {Options.MinSystemId} to {Options.MaxSystemId}.");
            return;
        }
        if (opts.Source == opts.Target)
        {
            Console.WriteLine("Source '-s' and target '-t' system identical, no conversion.");
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
            string source = Defined_CRS[opts.Source - 1],
                target = Defined_CRS[opts.Target - 1];
            if (opts.ZAxis < 1)
            {
                // Determine conversion function
                if (!GetConversion(source, target, out var conversion, out string info, false))
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
                IO.WriteFile(opts.OutputFile, xout, yout, coordinateLines, opts.XAxis - 1, opts.YAxis - 1, target.EndsWith("Geod") ? opts.LatLon : opts.Precision, opts.Delimiter[0]);
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
                if (!GetConversion(source, sourceVRS, target, out var conversion, out string info, out _))
                {
                    Console.WriteLine($"Conversion from source {source} (ID {opts.Source}) to target {target} (ID {opts.Target}) with {sourceVRS} heights (ID {opts.Height}) is not supported.");
                    return;
                }

                Console.WriteLine($"Conversion info: \n{info}");

                // Read input file
                var (xin, yin, zin) = IO.ReadFile(opts.InputFile, opts.XAxis - 1, opts.YAxis - 1, opts.ZAxis - 1,
                    opts.Delimiter[0], out string[][]? coordinateLines);

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
                    IO.WriteFile(opts.OutputFile, xout, yout,
                        zout, coordinateLines, opts.XAxis - 1, opts.YAxis - 1, opts.ZAxis - 1,
                        target.EndsWith("Geod") ? opts.LatLon : opts.Precision, opts.Precision, opts.Delimiter[0]);
                }
            }

            Console.WriteLine("Conversion completed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred during the conversion: {ex.Message}");
        }

    }

}
