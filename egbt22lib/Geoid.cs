//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Text;

//namespace egbt22lib
//{
//    public static class Geoid
//    {
//        /// <summary>
//        /// Reads the binary BKG geoid file and returns the geoid heights for the given ETRS89 coordinates
//        /// </summary>
//        /// <param name="binaryGeoidFile">Path to the unzipped binary BKG geoid file (Download at https://gdz.bkg.bund.de/index.php/default/digitale-geodaten/geodaetische-basisdaten/quasigeoid-der-bundesrepublik-deutschland-quasigeoid.html)</param>
//        /// <param name="etrs89Lat">Latitude in degrees</param>
//        /// <param name="etrs89Lon">Longitude in degrees</param>
//        /// <returns></returns>
//        public static double[] GetBKGBinaryGeoidHeights(string binaryGeoidFile, in double[] etrs89Lat, in double[] etrs89Lon)
//        {
//            try
//            {
//                if (etrs89Lat.Length != etrs89Lon.Length)
//                {
//                    throw new ArgumentException("Latitude and Longitude arrays must have the same length");
//                }
//                using var reader = new BinaryReader(File.OpenRead(binaryGeoidFile));
//                int latMinGrad = reader.ReadInt32();
//                int latMinMin = reader.ReadInt32();
//                int latMinSec = reader.ReadInt32();
//                int latMaxGrad = reader.ReadInt32();
//                int latMaxMin = reader.ReadInt32();
//                int latMaxSec = reader.ReadInt32();
//                int lonMinGrad = reader.ReadInt32();
//                int lonMinMin = reader.ReadInt32();
//                int lonMinSec = reader.ReadInt32();
//                int lonMaxGrad = reader.ReadInt32();
//                int lonMaxMin = reader.ReadInt32();
//                int lonMaxSec = reader.ReadInt32();
//                int latWidthGrad = reader.ReadInt32();
//                int latWidthMin = reader.ReadInt32();
//                int latWidthSec = reader.ReadInt32();
//                int lonWidthGrad = reader.ReadInt32();
//                int lonWidthMin = reader.ReadInt32();
//                int lonWidthSec = reader.ReadInt32();

//                double latMin = latMinGrad + latMinMin / 60d + latMinSec / 3600000000d;
//                double latMax = latMaxGrad + latMaxMin / 60d + latMaxSec / 3600000000d;
//                double lonMin = lonMinGrad + lonMinMin / 60d + lonMinSec / 3600000000d;
//                double lonMax = lonMaxGrad + lonMaxMin / 60d + lonMaxSec / 3600000000d;
//                double latWidth = latWidthGrad + latWidthMin / 60d + latWidthSec / 3600000000d;
//                double lonWidth = lonWidthGrad + lonWidthMin / 60d + lonWidthSec / 3600000000d;
//                int rows = (int)Math.Round((latMax - latMin) / latWidth) + 1;
//                int cols = (int)Math.Round((lonMax - lonMin) / lonWidth) + 1;

//                double[] elevations = new double[etrs89Lat.Length];
//                double[] grid = new double[16];
//                byte[] buffer = new byte[16];

//                for (int i = 0; i < elevations.Length; i++)
//                {
//                    if (etrs89Lon[i] > lonMax || etrs89Lon[i] < lonMin
//                        || etrs89Lat[i] > latMax || etrs89Lat[i] < latMin)
//                    {
//                        Console.WriteLine("Coordinate out of bounds: lat={0}, lon={1}", etrs89Lat[i], etrs89Lon[i]);
//                        elevations[i] = Double.NaN;
//                    }
//                    else
//                    {
//                        double x = (etrs89Lon[i] - lonMin) / lonWidth;
//                        double y = (latMax - etrs89Lat[i]) / latWidth;
//                        int ix = (int)x;
//                        int iy = (int)y;

//                        reader.BaseStream.Position = (18 + ((iy - 1) * cols) + (ix - 1)) << 2;
//                        reader.Read(buffer, 0, 16);
//                        grid[0] = BitConverter.ToInt32(buffer, 0) / 10000.0;
//                        grid[1] = BitConverter.ToInt32(buffer, 4) / 10000.0;
//                        grid[2] = BitConverter.ToInt32(buffer, 8) / 10000.0;
//                        grid[3] = BitConverter.ToInt32(buffer, 12) / 10000.0;

//                        reader.BaseStream.Position = (18 + ((iy + 0) * cols) + (ix - 1)) << 2;
//                        reader.Read(buffer, 0, 16);
//                        grid[4] = BitConverter.ToInt32(buffer, 0) / 10000.0;
//                        grid[5] = BitConverter.ToInt32(buffer, 4) / 10000.0;
//                        grid[6] = BitConverter.ToInt32(buffer, 8) / 10000.0;
//                        grid[7] = BitConverter.ToInt32(buffer, 12) / 10000.0;

//                        reader.BaseStream.Position = (18 + ((iy + 1) * cols) + (ix - 1)) << 2;
//                        reader.Read(buffer, 0, 16);
//                        grid[8] = BitConverter.ToInt32(buffer, 0) / 10000.0;
//                        grid[9] = BitConverter.ToInt32(buffer, 4) / 10000.0;
//                        grid[10] = BitConverter.ToInt32(buffer, 8) / 10000.0;
//                        grid[11] = BitConverter.ToInt32(buffer, 12) / 10000.0;

//                        reader.BaseStream.Position = (18 + ((iy + 2) * cols) + (ix - 1)) << 2;
//                        reader.Read(buffer, 0, 16);
//                        grid[12] = BitConverter.ToInt32(buffer, 0) / 10000.0;
//                        grid[13] = BitConverter.ToInt32(buffer, 4) / 10000.0;
//                        grid[14] = BitConverter.ToInt32(buffer, 8) / 10000.0;
//                        grid[15] = BitConverter.ToInt32(buffer, 12) / 10000.0;

//                        double fx = x - ix;
//                        double fy = y - iy;
//                        double value = BicubicInterpolate(grid, fx, fy);
//                        elevations[i] = Math.Round(value, 3);
//                    }
//                }
//                return elevations;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("Error reading BKG binary geoid file: {0}", ex.Message);
//                throw;
//            }
//        }


//        public static double CubicInterpolate(double p0, double p1, double p2, double p3, double t)
//        {
//            return p1 + 0.5 * t*(p2 - p0 + t*(2.0*p0 - 5.0*p1 + 4.0*p2 - p3 + t*(3.0*(p1 - p2) + p3 - p0)));
//        }

//        public static double BicubicInterpolate(double[] buffer, double fx, double fy)
//        {
//            double[] arr = new double[4];
//            for (int i = 0; i < 4; i++)
//            {
//                arr[i] = CubicInterpolate(buffer[i], buffer[i + 4], buffer[i + 8], buffer[i + 12], fy);
//            }

//            return CubicInterpolate(arr[0], arr[1], arr[2], arr[3], fx);
//        }
//    }

//}
