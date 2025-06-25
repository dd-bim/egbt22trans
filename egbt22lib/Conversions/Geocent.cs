using GeographicLib;

using System;
using System.Collections.Generic;
using System.Text;

namespace egbt22lib.Conversions
{
    public class Geocent
    {
        private readonly Geocentric _gc;

        public Geocent(double a, double f)
        {
            _gc = new Geocentric(a, f);
        }

        public (double x, double y, double z) Forward(double lat, double lon, double ellH) => _gc.Forward(lat, lon, ellH);
        public (double lat, double lon, double ellH) Reverse(double x, double y, double z) => _gc.Reverse(x, y, z);
    }
}
