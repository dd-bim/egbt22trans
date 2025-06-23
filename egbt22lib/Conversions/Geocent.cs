using GeographicLib;

using System;
using System.Collections.Generic;
using System.Text;

namespace egbt22lib.Conversions
{
    internal class Geocent
    {
        private readonly Geocentric _gc;

        public Geocent(double a, double f)
        {
            _gc = new Geocentric(a, f);
        }

        public Coordinate Forward(Coordinate llh) => new Coordinate(_gc.Forward(llh.X, llh.Y, llh.Z));
        public Coordinate Reverse(Coordinate xyz) => new Coordinate(_gc.Reverse(xyz.X, xyz.Y, xyz.Z));
    }
}
