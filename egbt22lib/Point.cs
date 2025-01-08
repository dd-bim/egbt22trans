using System;
using System.Collections.Generic;
using System.Text;

namespace egbt22lib
{
    public readonly struct Point
    {
        public double X { get; }

        public double Y { get; }

        public double Z { get; }

        public Point(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}
