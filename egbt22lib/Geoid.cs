using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace egbt22lib
{
    public static class Geoid
    {
        public const string BinaryGeoidFile = "GCG2016v2023";

        private static readonly byte[]? _geoidData;
        private static readonly int _cols;
        private static readonly double _latMin, _latMax, _lonMin, _lonMax, _latWidth, _lonWidth;

        static Geoid()
        {
            try
            {
                _geoidData = File.ReadAllBytes(BinaryGeoidFile);
                using var reader = new BinaryReader(new MemoryStream(_geoidData));
                int latMinGrad = reader.ReadInt32();
                int latMinMin = reader.ReadInt32();
                int latMinSec = reader.ReadInt32();
                int latMaxGrad = reader.ReadInt32();
                int latMaxMin = reader.ReadInt32();
                int latMaxSec = reader.ReadInt32();
                int lonMinGrad = reader.ReadInt32();
                int lonMinMin = reader.ReadInt32();
                int lonMinSec = reader.ReadInt32();
                int lonMaxGrad = reader.ReadInt32();
                int lonMaxMin = reader.ReadInt32();
                int lonMaxSec = reader.ReadInt32();
                int latWidthGrad = reader.ReadInt32();
                int latWidthMin = reader.ReadInt32();
                int latWidthSec = reader.ReadInt32();
                int lonWidthGrad = reader.ReadInt32();
                int lonWidthMin = reader.ReadInt32();
                int lonWidthSec = reader.ReadInt32();

                _latMin = latMinGrad + (latMinMin / 60d) + (latMinSec / 3600000000d);
                _latMax = latMaxGrad + (latMaxMin / 60d) + (latMaxSec / 3600000000d);
                _lonMin = lonMinGrad + (lonMinMin / 60d) + (lonMinSec / 3600000000d);
                _lonMax = lonMaxGrad + (lonMaxMin / 60d) + (lonMaxSec / 3600000000d);
                _latWidth = latWidthGrad + (latWidthMin / 60d) + (latWidthSec / 3600000000d);
                _lonWidth = lonWidthGrad + (lonWidthMin / 60d) + (lonWidthSec / 3600000000d);
                //_rows = (int)Math.Round((_latMax - _latMin) / _latWidth) + 1;
                _cols = (int)Math.Round((_lonMax - _lonMin) / _lonWidth) + 1;
            }
            catch (Exception ex)
            {
                throw new Exception("Error loading BKG binary geoid file", ex);
            }
        }

        /// <summary>
        /// Reads the binary BKG geoid file and returns the geoid heights for the given ETRS89 coordinates
        /// </summary>
        /// <param name="etrs89Lat">Latitude in degrees</param>
        /// <param name="etrs89Lon">Longitude in degrees</param>
        /// <returns></returns>
        public static double[] GetBKGBinaryGeoidHeights(in double[] etrs89Lat, in double[] etrs89Lon)
        {
            try
            {
                if (etrs89Lat.Length != etrs89Lon.Length)
                {
                    throw new ArgumentException("Latitude and Longitude arrays must have the same length");
                }

                double[] elevations = new double[etrs89Lat.Length];
                double[] grid = new double[16];

                for (int i = 0; i < elevations.Length; i++)
                {
                    if (etrs89Lon[i] > _lonMax || etrs89Lon[i] < _lonMin
                        || etrs89Lat[i] > _latMax || etrs89Lat[i] < _latMin)
                    {
                        //_logger?.LogWarning("Coordinate out of bounds: lat={lat}, lon={lon}", etrs89Lat[i], etrs89Lon[i]);
                        elevations[i] = double.NaN;
                    }
                    else
                    {
                        double x = (etrs89Lon[i] - _lonMin) / _lonWidth;
                        double y = (_latMax - etrs89Lat[i]) / _latWidth;
                        int ix = (int)x;
                        int iy = (int)y;

                        int basePosition = (18 + ((iy - 1) * _cols) + (ix - 1)) << 2;
                        grid[0] = BitConverter.ToInt32(_geoidData, basePosition) / 10000.0;
                        grid[1] = BitConverter.ToInt32(_geoidData, basePosition + 4) / 10000.0;
                        grid[2] = BitConverter.ToInt32(_geoidData, basePosition + 8) / 10000.0;
                        grid[3] = BitConverter.ToInt32(_geoidData, basePosition + 12) / 10000.0;

                        basePosition = (18 + ((iy + 0) * _cols) + (ix - 1)) << 2;
                        grid[4] = BitConverter.ToInt32(_geoidData, basePosition) / 10000.0;
                        grid[5] = BitConverter.ToInt32(_geoidData, basePosition + 4) / 10000.0;
                        grid[6] = BitConverter.ToInt32(_geoidData, basePosition + 8) / 10000.0;
                        grid[7] = BitConverter.ToInt32(_geoidData, basePosition + 12) / 10000.0;

                        basePosition = (18 + ((iy + 1) * _cols) + (ix - 1)) << 2;
                        grid[8] = BitConverter.ToInt32(_geoidData, basePosition) / 10000.0;
                        grid[9] = BitConverter.ToInt32(_geoidData, basePosition + 4) / 10000.0;
                        grid[10] = BitConverter.ToInt32(_geoidData, basePosition + 8) / 10000.0;
                        grid[11] = BitConverter.ToInt32(_geoidData, basePosition + 12) / 10000.0;

                        basePosition = (18 + ((iy + 2) * _cols) + (ix - 1)) << 2;
                        grid[12] = BitConverter.ToInt32(_geoidData, basePosition) / 10000.0;
                        grid[13] = BitConverter.ToInt32(_geoidData, basePosition + 4) / 10000.0;
                        grid[14] = BitConverter.ToInt32(_geoidData, basePosition + 8) / 10000.0;
                        grid[15] = BitConverter.ToInt32(_geoidData, basePosition + 12) / 10000.0;

                        double fx = x - ix;
                        double fy = y - iy;
                        double value = BicubicInterpolate(grid, fx, fy);
                        elevations[i] = Math.Round(value, 4);
                    }
                }

                //_logger?.LogDebug("Successfully read geoid heights for {count} coordinates", elevations.Length);
                return elevations;
            }
            catch (Exception ex)
            {
                throw new Exception("Error reading BKG binary geoid file", ex);
            }
        }

        /// <summary>
        /// Reads the binary BKG geoid file and returns the geoid heights for the given ETRS89 coordinates
        /// </summary>
        /// <param name="etrs89geod">ETRS89 geodetic (Latitude in degrees, Longitude in degrees)</param>
        /// <returns></returns>
        public static double GetBKGBinaryGeoidHeight(in double lat, in double lon)
        {
            try
            {
                if (lon > _lonMax || lon < _lonMin
                    || lat > _latMax || lat < _latMin)
                {
                    //_logger?.LogWarning("Coordinate out of bounds: lat={lat}, lon={lon}", lat, lon);
                    return double.NaN;
                }
                else
                {
                    double x = (lon - _lonMin) / _lonWidth;
                    double y = (_latMax - lat) / _latWidth;
                    int ix = (int)x;
                    int iy = (int)y;

                    int basePosition = (18 + ((iy - 1) * _cols) + (ix - 1)) << 2;
                    double[] grid = new double[16];
                    grid[0] = BitConverter.ToInt32(_geoidData, basePosition) / 10000.0;
                    grid[1] = BitConverter.ToInt32(_geoidData, basePosition + 4) / 10000.0;
                    grid[2] = BitConverter.ToInt32(_geoidData, basePosition + 8) / 10000.0;
                    grid[3] = BitConverter.ToInt32(_geoidData, basePosition + 12) / 10000.0;

                    basePosition = (18 + ((iy + 0) * _cols) + (ix - 1)) << 2;
                    grid[4] = BitConverter.ToInt32(_geoidData, basePosition) / 10000.0;
                    grid[5] = BitConverter.ToInt32(_geoidData, basePosition + 4) / 10000.0;
                    grid[6] = BitConverter.ToInt32(_geoidData, basePosition + 8) / 10000.0;
                    grid[7] = BitConverter.ToInt32(_geoidData, basePosition + 12) / 10000.0;

                    basePosition = (18 + ((iy + 1) * _cols) + (ix - 1)) << 2;
                    grid[8] = BitConverter.ToInt32(_geoidData, basePosition) / 10000.0;
                    grid[9] = BitConverter.ToInt32(_geoidData, basePosition + 4) / 10000.0;
                    grid[10] = BitConverter.ToInt32(_geoidData, basePosition + 8) / 10000.0;
                    grid[11] = BitConverter.ToInt32(_geoidData, basePosition + 12) / 10000.0;

                    basePosition = (18 + ((iy + 2) * _cols) + (ix - 1)) << 2;
                    grid[12] = BitConverter.ToInt32(_geoidData, basePosition) / 10000.0;
                    grid[13] = BitConverter.ToInt32(_geoidData, basePosition + 4) / 10000.0;
                    grid[14] = BitConverter.ToInt32(_geoidData, basePosition + 8) / 10000.0;
                    grid[15] = BitConverter.ToInt32(_geoidData, basePosition + 12) / 10000.0;

                    double fx = x - ix;
                    double fy = y - iy;
                    double value = BicubicInterpolate(grid, fx, fy);
                    return Math.Round(value, 4);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error reading BKG binary geoid file", ex);
            }
        }

        public static double CubicInterpolate(double p0, double p1, double p2, double p3, double t) => 
            p1 + (0.5 * t * (p2 - p0 + (t * ((2.0 * p0) - (5.0 * p1) + (4.0 * p2) - p3 + (t * ((3.0 * (p1 - p2)) + p3 - p0))))));

        public static double BicubicInterpolate(double[] buffer, double fx, double fy)
        {
            double[] arr = new double[4];
            for (int i = 0; i < 4; i++)
            {
                arr[i] = CubicInterpolate(buffer[i], buffer[i + 4], buffer[i + 8], buffer[i + 12], fy);
            }

            return CubicInterpolate(arr[0], arr[1], arr[2], arr[3], fx);
        }
    }

}
