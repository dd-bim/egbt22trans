using GeographicLib.Geocodes;

using System;
using System.Collections.Generic;
using System.Text;

namespace egbt22lib.Conversions
{
    internal static class Common
    {
        // Parameter from EPSG 7004 definition
        public const double Bessel_a = 6377397.155;
        public const double Bessel_f = 1d / 299.1528128;
        public const double Bessel_e2 = (2 * Bessel_f) - (Bessel_f * Bessel_f);
        public const double Bessel_b = Bessel_a * (1 - Bessel_f);

        // Parameter from EPSG 7019 definition
        public const double GRS80_a = 6378137.0;
        public const double GRS80_f = 1d / 298.257222101;
        public const double GRS80_e2 = (2 * GRS80_f) - (GRS80_f * GRS80_f);
        public const double GRS80_b = GRS80_a * (1 - GRS80_f);

        public static double GaussianRadius(in double a, in double f, in double phi)
        {
            //GaussianRadius
            double sinPhi = Math.Sin(phi * 180d / Math.PI);
            // simplified from: a*sqrt(1-e^2)/(1-e^2*sin^2 phi), where e^2= 2f - f^2
            return (a - (a * f)) / (((f - 2) * f * sinPhi * sinPhi) + 1);
        }

        public static double LocalScale(in double a, in double f, in double b0, in double h0, out double radius)
        {
            radius = GaussianRadius(in a, in f, in b0);
            return LocalScale(h0, radius);
        }

        public static double LocalScale(in double h0, in double radius) => 1 + (h0 / radius);

        public static double HeightOfScale(in double k0, in double radius) => (k0 - 1) * radius;

        public static double PointScaleAtHeight(in double k, in double radius, in double ellipsoidalHeight) => k * (radius / (radius + ellipsoidalHeight));

        public static double PointScaleAtHeight(in double k, in double phi, in double ellipsoidalHeight, in double a, in double f)
        {
            double radius = GaussianRadius(a, f, phi);
            return k * (radius / (radius + ellipsoidalHeight));
        }
    }
}
