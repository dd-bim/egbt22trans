using CommandLine;
using CommandLine.Text;

using egbt22lib;

using static egbt22lib.Convert;


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
            Console.WriteLine("Either set source and target system, or egbt22 value.");
            return;
        }
        if (opts.Source != 0 && opts.Target != 0 && opts.Source == opts.Target)
        {
            Console.WriteLine("Source and target system identical, no conversion.");
            return;
        }
        if (opts.Source != 0 && opts.Target != 0)
        {
            if (opts.Source is < 1 or > 5)
            {
                Console.WriteLine("The source system must have a value in the range 1 to 5.");
                return;
            }
            if (opts.Target is < 1 or > 5)
            {
                Console.WriteLine("The target system must have a value in the range 1 to 5.");
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

        if ((opts.Source == 1 && opts.Target == 2) || (opts.Source == 2 && opts.Target == 1))
        {
            var (xin, yin) = IO.ReadFile(opts.InputFile, opts.XAxis - 1, opts.YAxis - 1, opts.Delimiter[0], out string[][] coordinateLines);
            // Normal transformation/conversion
            if (!(opts.Source == 1
                ? EGBT22_Local_to_ETRS89_UTM33(xin, yin, out var xout, out var yout)
                : ETRS89_UTM33_to_EGBT22_Local(xin, yin, out xout, out yout)))
            {
                Console.WriteLine("Conversion failed.");
                return;
            }
            IO.WriteFile(opts.OutputFile, xout, yout, coordinateLines, opts.XAxis - 1, opts.YAxis - 1, opts.Precision, opts.Delimiter[0]);
        }
        else
        {
            var (xin, yin, zin) = IO.ReadFile(opts.InputFile, opts.XAxis - 1, opts.YAxis - 1, opts.ZAxis - 1, opts.Delimiter[0], out string[][] coordinateLines);

            var (xout, yout, zout) = opts.Egbt22 > 0
            ?   // EGBT22 transformation
                transformEGBT22(opts.Egbt22, xin, yin, zin)
            :   // Normal transformation/conversion
                convert(opts.Source, opts.Target, xin, yin, zin);

            IO.WriteFile(opts.OutputFile, xout, yout, zout, coordinateLines, opts.XAxis - 1, opts.YAxis - 1, opts.ZAxis - 1, opts.Precision, opts.Delimiter[0]);
        }
    }

    static (double[] x, double[] y, double[] z) transformEGBT22(int operation, double[] x, double[] y, double[] z)
    {
        // EGBT22
        double dx = -0.0028;
        double dy = -0.0023;
        double dz = 0.0029;
        switch (operation)
        {
            case 1:
            case 5:
                // ETRS89/DREF91 -> EGBT22
                break;
            case 2:
            case 6:
                // EGBT22 -> ETRS89/DREF91
                dx = -dx;
                dy = -dy;
                dz = -dz;
                break;
            case 3:
            case 7:
                // ETRS89-CZ -> EGBT22
                dx = -dx;
                dy = -dy;
                dz = -dz;
                break;
            case 4:
            case 8:
                // EGBT22 -> ETRS89-CZ
                break;
            default:
                throw new ArgumentException("Invalid operation");
        }
        double[] xout, yout, zout;
        if (operation > 4)
        {
            //EPSG:4937
            if (!ETRS89_Geod_3D_to_ETRS89_Geoc(x, y, z, out xout, out yout, out zout))
            {
                throw new ArgumentException("Conversion failed.");
            }
        }
        else
        {
            xout = new double[x.Length];
            yout = new double[x.Length];
            zout = new double[x.Length];
        }
        for (int i = 0; i < x.Length; i++)
        {
            xout[i] = x[i] + dx;
            yout[i] = y[i] + dy;
            xout[i] = z[i] + dz;
        }
        if (operation > 4)
        {
            //EPSG:4937
            if (!ETRS89_Geoc_to_ETRS89_Geod_3D(xout, yout, zout, out xout, out yout, out zout))
            {
                throw new ArgumentException("Conversion failed.");
            }
        }
        return (xout, yout, zout);
    }

    static (double[] x, double[] y, double[] z) convert(int source, int target, double[] x, double[] y, double[] z)
    {
        double[] xout, yout, zout;
        switch ((source, target))
        {
            case (1, 3):
                // ETRS89 EGBT_LDP Dresden-Prag -> DB_REF GK5
                if (!EGBT22_Local_to_DBRef_GK5(x, y, z, out xout, out yout))
                    throw new ArgumentException("Conversion failed.");
                zout = z;
                Console.WriteLine("Conversion ETRS89 EGBT_LDP Dresden-Prag -> DB_REF GK5");
                break;
            case (3, 1):
                // DB_REF GK5 -> ETRS89 EGBT_LDP Dresden-Prag
                if (!DBRef_GK5_to_EGBT22_Local(x, y, z, out xout, out yout))
                    throw new ArgumentException("Conversion failed.");
                zout = z;
                Console.WriteLine("Conversion DB_REF GK5 -> ETRS89 EGBT_LDP Dresden-Prag");
                break;
            case (1, 4):
                // ETRS89 EGBT_LDP Dresden-Prag -> ETRS89 Cartesian 3D Geocentric
                if (!EGBT22_Local_to_ETRS89_Geoc(x, y, z, out xout, out yout, out zout))
                    throw new ArgumentException("Conversion failed.");
                Console.WriteLine("Conversion ETRS89 EGBT_LDP Dresden-Prag -> ETRS89 Cartesian 3D Geocentric");
                break;
            case (4, 1):
                // ETRS89 Cartesian 3D Geocentric -> ETRS89 EGBT_LDP Dresden-Prag
                if (!ETRS89_Geoc_to_EGBT22_Local(x, y, z, out xout, out yout, out zout))
                    throw new ArgumentException("Conversion failed.");
                Console.WriteLine("Conversion ETRS89 Cartesian 3D Geocentric -> ETRS89 EGBT_LDP Dresden-Prag");
                break;
            case (1, 5):
                // ETRS89 EGBT_LDP Dresden-Prag -> ETRS89 Geodetic 3D
                if (!EGBT22_Local_to_ETRS89_Geod_3D(x, y, z, out xout, out yout, out zout))
                    throw new ArgumentException("Conversion failed.");
                Console.WriteLine("Conversion ETRS89 EGBT_LDP Dresden-Prag -> ETRS89 Geodetic 3D");
                break;
            case (5, 1):
                // ETRS89 Geodetic 3D -> ETRS89 EGBT_LDP Dresden-Prag
                if (!ETRS89_Geod_3D_to_EGBT22_Local(x, y, z, out xout, out yout, out zout))
                    throw new ArgumentException("Conversion failed.");
                Console.WriteLine("Conversion ETRS89 Geodetic 3D -> ETRS89 EGBT_LDP Dresden-Prag");
                break;
            case (2, 3):
                // ETRS89 UTM33 -> DB_REF GK5
                if (!ETRS89_UTM33_to_DBRef_GK5(x, y, z, out xout, out yout))
                    throw new ArgumentException("Conversion failed.");
                zout = z;
                Console.WriteLine("Conversion ETRS89 UTM33 -> DB_REF GK5");
                break;
            case (3, 2):
                // DB_REF GK5 -> ETRS89 UTM33
                if (!DBRef_GK5_to_ETRS89_UTM33(x, y, z, out xout, out yout))
                    throw new ArgumentException("Conversion failed.");
                zout = z;
                Console.WriteLine("Conversion DB_REF GK5 -> ETRS89 UTM33");
                break;
            case (2, 4):
                // ETRS89 UTM33 -> ETRS89 Cartesian 3D Geocentric
                if (!ETRS89_UTM33_to_ETRS89_Geoc(x, y, z, out xout, out yout, out zout))
                    throw new ArgumentException("Conversion failed.");
                Console.WriteLine("Conversion ETRS89 UTM33 -> ETRS89 Cartesian 3D Geocentric");
                break;
            case (4, 2):
                // ETRS89 Cartesian 3D Geocentric -> ETRS89 UTM33
                if (!ETRS89_Geoc_to_ETRS89_UTM33(x, y, z, out xout, out yout, out zout))
                    throw new ArgumentException("Conversion failed.");
                Console.WriteLine("Conversion ETRS89 Cartesian 3D Geocentric -> ETRS89 UTM33");
                break;
            case (2, 5):
                // ETRS89 UTM33 -> ETRS89 Geodetic 3D
                if (!ETRS89_UTM33_to_ETRS89_Geod_3D(x, y, z, out xout, out yout, out zout))
                    throw new ArgumentException("Conversion failed.");
                Console.WriteLine("Conversion ETRS89 UTM33 -> ETRS89 Geodetic 3D");
                break;
            case (5, 2):
                // ETRS89 Geodetic 3D -> ETRS89 UTM33
                if (!ETRS89_Geod_3D_to_ETRS89_UTM33(x, y, z, out xout, out yout, out zout))
                    throw new ArgumentException("Conversion failed.");
                Console.WriteLine("Conversion ETRS89 Geodetic 3D -> ETRS89 UTM33");
                break;
            case (3, 4):
                // DB_REF GK5 -> ETRS89 Cartesian 3D Geocentric
                if (!DBRef_GK5_to_ETRS89_Geoc(x, y, z, out xout, out yout, out zout))
                    throw new ArgumentException("Conversion failed.");
                Console.WriteLine("Conversion DB_REF GK5 -> ETRS89 Cartesian 3D Geocentric");
                break;
            case (4, 3):
                // ETRS89 Cartesian 3D Geocentric -> DB_REF GK5
                if (!ETRS89_Geod_3D_to_DBRef_GK5(x, y, z, out xout, out yout, out zout))
                    throw new ArgumentException("Conversion failed.");
                Console.WriteLine("Conversion ETRS89 Cartesian 3D Geocentric -> DB_REF GK5");
                break;
            case (3, 5):
                // DB_REF GK5 -> ETRS89 Geodetic 3D
                if (!DBRef_GK5_to_ETRS89_Geod_3D(x, y, z, out xout, out yout, out zout))
                    throw new ArgumentException("Conversion failed.");
                Console.WriteLine("Conversion DB_REF GK5 -> ETRS89 Geodetic 3D");
                break;
            case (5, 3):
                // ETRS89 Geodetic 3D -> DB_REF GK5
                if (!ETRS89_Geod_3D_to_DBRef_GK5(x, y, z, out xout, out yout, out zout))
                    throw new ArgumentException("Conversion failed.");
                Console.WriteLine("Conversion ETRS89 Geodetic 3D -> DB_REF GK5");
                break;
            case (4, 5):
                // ETRS89 Cartesian 3D Geocentric -> ETRS89 Geodetic 3D
                if (!ETRS89_Geoc_to_ETRS89_Geod_3D(x, y, z, out xout, out yout, out zout))
                    throw new ArgumentException("Conversion failed.");
                Console.WriteLine("Conversion ETRS89 Cartesian 3D Geocentric -> ETRS89 Geodetic 3D");
                break;
            case (5, 4):
                // ETRS89 Geodetic 3D -> ETRS89 Cartesian 3D Geocentric
                if (!ETRS89_Geod_3D_to_ETRS89_Geoc(x, y, z, out xout, out yout, out zout))
                    throw new ArgumentException("Conversion failed.");
                Console.WriteLine("Conversion ETRS89 Geodetic 3D -> ETRS89 Cartesian 3D Geocentric");
                break;
            default:
                throw new ArgumentException($"Invalid source {source} and target {target} combination.");

        }
        return (xout, yout, zout);
    }
}
