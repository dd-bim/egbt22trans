using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

using Microsoft.Extensions.Logging;

namespace egbt22lib
{
    /// <summary>
    /// Class for coordinate input/output
    /// </summary>
    public static class IO
    {
        private static ILogger? _logger;

        public static void InitializeLogger(ILogger logger) => _logger = logger;

        /// <summary>
        /// Read coordinates from a file
        /// </summary>
        /// <param name="file">Path to coordinate file</param>
        /// <param name="idIdx">0-based index of Identifier</param>
        /// <param name="xAxis">0-based index of X-axis</param>
        /// <param name="yAxis">0-based index of Y-axis</param>
        /// <param name="zAxis">0-based index of Z-axis</param>
        /// <param name="delimiter">Delimiter (column separator)</param>
        /// <returns>Imported coordinates</returns>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static (string[] id, double[] x, double[] y, double[] z) ReadFile(string file, int idIdx, int xAxis, int yAxis, int zAxis, char delimiter)
        {
            _logger?.LogDebug("Reading coordinate file: {file}", file);
            if (!File.Exists(file))
            {
                _logger?.LogError("File not found: {file}", file);
                throw new FileNotFoundException($"File {file} not found.");
            }
            if (idIdx == xAxis || xAxis == yAxis || xAxis == zAxis || yAxis == zAxis
                || idIdx < 0 || xAxis < 0 || yAxis < 0 || zAxis < 0)
            {
                _logger?.LogError("Invalid column indices: idIdx={idIdx}, xAxis={xAxis}, yAxis={yAxis}, zAxis={zAxis}", idIdx, xAxis, yAxis, zAxis);
                throw new ArgumentException("The indices of the columns must be different and greater as 0.");
            }
            var max = Math.Max(idIdx, Math.Max(xAxis, Math.Max(yAxis, zAxis)));

            var lines = File.ReadAllLines(file);
            var ids = new List<string>(lines.Length);
            var xs = new List<double>(lines.Length);
            var ys = new List<double>(lines.Length);
            var zs = new List<double>(lines.Length);

            foreach (var line in lines)
            {
                var parts = line.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length <= max
                    || !Double.TryParse(parts[xAxis], NumberStyles.Float, CultureInfo.InvariantCulture, out var x)
                    || !Double.TryParse(parts[yAxis], NumberStyles.Float, CultureInfo.InvariantCulture, out var y)
                    || !Double.TryParse(parts[zAxis], NumberStyles.Float, CultureInfo.InvariantCulture, out var z))
                {
                    _logger?.LogWarning("Skipping invalid line: {line}", line);
                    continue;
                }
                xs.Add(x);
                ys.Add(y);
                zs.Add(z);
                ids.Add(parts[idIdx]);
            }

            _logger?.LogDebug("Successfully read coordinates from file: {file}", file);
            return (ids.ToArray(), xs.ToArray(), ys.ToArray(), zs.ToArray());
        }


        /// <summary>
        /// Read coordinates from a file
        /// </summary>
        /// <param name="file">Path to coordinate file</param>
        /// <param name="xAxis">0-based index of X-axis</param>
        /// <param name="yAxis">0-based index of Y-axis</param>
        /// <param name="zAxis">0-based index of Z-axis</param>
        /// <param name="delimiter">Delimiter (column separator)</param>
        /// <param name="coordinateLines">Imported lines for later usage at export</param>
        /// <returns>Imported coordinates</returns>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static (double[] x, double[] y, double[] z) ReadFile(string file, int xAxis, int yAxis, int zAxis, char delimiter, out string[][] coordinateLines)
        {
            _logger?.LogDebug("Reading coordinate file: {file}", file);
            if (!File.Exists(file))
            {
                _logger?.LogError("File not found: {file}", file);
                throw new FileNotFoundException($"File {file} not found.");
            }
            if (xAxis == yAxis || xAxis == zAxis || yAxis == zAxis
                || xAxis < 0 || yAxis < 0 || zAxis < 0)
            {
                _logger?.LogError("Invalid column indices: xAxis={xAxis}, yAxis={yAxis}, zAxis={zAxis}", xAxis, yAxis, zAxis);
                throw new ArgumentException("The indices of the columns must be different and greater as 0.");
            }
            var max = Math.Max(xAxis, Math.Max(yAxis, zAxis));

            var lines = File.ReadAllLines(file);
            var xs = new List<double>(lines.Length);
            var ys = new List<double>(lines.Length);
            var zs = new List<double>(lines.Length);
            var clines = new List<string[]>(lines.Length);

            foreach (var line in lines)
            {
                var parts = line.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length <= max
                    || !Double.TryParse(parts[xAxis], NumberStyles.Float, CultureInfo.InvariantCulture, out var x)
                    || !Double.TryParse(parts[yAxis], NumberStyles.Float, CultureInfo.InvariantCulture, out var y)
                    || !Double.TryParse(parts[zAxis], NumberStyles.Float, CultureInfo.InvariantCulture, out var z))
                {
                    _logger?.LogWarning("Skipping invalid line: {line}", line);
                    continue;
                }
                xs.Add(x);
                ys.Add(y);
                zs.Add(z);
                clines.Add(parts);
            }

            coordinateLines = clines.ToArray();
            _logger?.LogDebug("Successfully read coordinates from file: {file}", file);
            return (xs.ToArray(), ys.ToArray(), zs.ToArray());
        }

        /// <summary>
        /// Read coordinates from a file
        /// </summary>
        /// <param name="file">Path to coordinate file</param>
        /// <param name="xAxis">0-based index of X-axis</param>
        /// <param name="yAxis">0-based index of Y-axis</param>
        /// <param name="delimiter">Delimiter (column separator)</param>
        /// <param name="coordinateLines">Imported lines for later usage at export</param>
        /// <returns>Imported coordinates</returns>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static (double[] x, double[] y) ReadFile(string file, int xAxis, int yAxis, char delimiter, out string[][] coordinateLines)
        {
            _logger?.LogDebug("Reading coordinate file: {file}", file);
            if (!File.Exists(file))
            {
                _logger?.LogError("File not found: {file}", file);
                throw new FileNotFoundException($"File {file} not found.");
            }
            if (xAxis == yAxis || xAxis < 0 || yAxis < 0)
            {
                _logger?.LogError("Invalid column indices: xAxis={xAxis}, yAxis={yAxis}", xAxis, yAxis);
                throw new ArgumentException("The indices of the columns must be different and greater as 0.");
            }
            var max = Math.Max(xAxis, yAxis);

            var lines = File.ReadAllLines(file);
            var xs = new List<double>(lines.Length);
            var ys = new List<double>(lines.Length);
            var clines = new List<string[]>(lines.Length);

            foreach (var line in lines)
            {
                var parts = line.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length <= max
                    || !Double.TryParse(parts[xAxis], NumberStyles.Float, CultureInfo.InvariantCulture, out var x)
                    || !Double.TryParse(parts[yAxis], NumberStyles.Float, CultureInfo.InvariantCulture, out var y))
                {
                    _logger?.LogWarning("Skipping invalid line: {line}", line);
                    continue;
                }
                xs.Add(x);
                ys.Add(y);
                clines.Add(parts);
            }

            coordinateLines = clines.ToArray();
            _logger?.LogDebug("Successfully read coordinates from file: {file}", file);
            return (xs.ToArray(), ys.ToArray());
        }

        /// <summary>
        /// Write coordinates to a file
        /// </summary>
        /// <param name="file">File path</param>
        /// <param name="x">X-coordinates</param>
        /// <param name="y">Y-coordinates</param>
        /// <param name="z">Z-coordinates</param>
        /// <param name="coordinateLines">Original lines</param>
        /// <param name="xAxis">0-based index of X-axis</param>
        /// <param name="yAxis">0-based index of Y-axis</param>
        /// <param name="zAxis">0-based index of Z-axis</param>
        /// <param name="precision">Output precision</param>
        /// <param name="delimiter">Delimiter (column separator)</param>
        public static void WriteFile(string file, double[] x, double[] y, double[] z, string[][] coordinateLines, int xAxis, int yAxis, int zAxis, int precision, char delimiter)
        {
            _logger?.LogDebug("Writing coordinates to file: {file}", file);
            var format = $"F{precision}";
            try
            {
                using var writer = new StreamWriter(file);
                for (var i = 0; i < x.Length; i++)
                {
                    var line = coordinateLines[i];
                    for (var j = 0; j < line.Length; j++)
                    {
                        if (j > 0)
                        {
                            writer.Write(delimiter);
                        }
                        if (j == xAxis)
                        {
                            writer.Write(x[i].ToString(format, CultureInfo.InvariantCulture));
                        }
                        else if (j == yAxis)
                        {
                            writer.Write(y[i].ToString(format, CultureInfo.InvariantCulture));
                        }
                        else if (j == zAxis)
                        {
                            writer.Write(z[i].ToString(format, CultureInfo.InvariantCulture));
                        }
                        else
                        {
                            writer.Write(line[j]);
                        }
                    }
                    writer.WriteLine();
                }
                _logger?.LogDebug("Successfully wrote coordinates to file: {file}", file);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error writing coordinates to file: {file}", file);
                throw;
            }
        }

        /// <summary>
        /// Write coordinates to a file
        /// </summary>
        /// <param name="file">File path</param>
        /// <param name="x">X-coordinates</param>
        /// <param name="y">Y-coordinates</param>
        /// <param name="coordinateLines">Original lines</param>
        /// <param name="xAxis">0-based index of X-axis</param>
        /// <param name="yAxis">0-based index of Y-axis</param>
        /// <param name="precision">Output precision</param>
        /// <param name="delimiter">Delimiter (column separator)</param>
        public static void WriteFile(string file, double[] x, double[] y, string[][] coordinateLines, int xAxis, int yAxis, int precision, char delimiter)
        {
            _logger?.LogDebug("Writing coordinates to file: {file}", file);
            var format = $"F{precision}";
            try
            {
                using var writer = new StreamWriter(file);
                for (var i = 0; i < x.Length; i++)
                {
                    var line = coordinateLines[i];
                    for (var j = 0; j < line.Length; j++)
                    {
                        if (j > 0)
                        {
                            writer.Write(delimiter);
                        }
                        if (j == xAxis)
                        {
                            writer.Write(x[i].ToString(format, CultureInfo.InvariantCulture));
                        }
                        else if (j == yAxis)
                        {
                            writer.Write(y[i].ToString(format, CultureInfo.InvariantCulture));
                        }
                        else
                        {
                            writer.Write(line[j]);
                        }
                    }
                    writer.WriteLine();
                }
                _logger?.LogDebug("Successfully wrote coordinates to file: {file}", file);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error writing coordinates to file: {file}", file);
                throw;
            }
        }

        public static void WriteCSV(in string fileName, in string[] header, in char separator, in string[] ids, params double[][] data)
        {
            var lines = new string[ids.Length + 1];
            var lineLength = data.Length + 1;
            lines[0] = String.Join(separator, header);
            for (var i = 0; i < ids.Length; i++)
            {
                var line = new string[lineLength];
                line[0] = ids[i];
                for (var j = 0; j < data.Length; j++)
                {
                    var valStr = FormattableString.Invariant($"{data[j][i]:G}");
                    line[j + 1] = valStr;
                }
                lines[1 + i] = string.Join(separator, line);
            }
            File.WriteAllLines(fileName, lines);
        }

    }


}

