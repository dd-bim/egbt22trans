using System;
using System.Collections.Generic;
using System.Text;

namespace egbt22lib.Conversions
{
    internal class Coordinate
    {
        public double X { get; set; }

        public double Y { get; set; }

        public double Z { get; set; }

        public Coordinate(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Coordinate((double x, double y, double z) xyz) : this(xyz.x, xyz.y, xyz.z)
        {
        }

        public void Deconstruct(out double x, out double y, out double z)
        {
            x = X;
            y = Y;
            z = Z;
        }
    }

    internal sealed class TMCoordinate : Coordinate
    {
        public double Scale { get; }

        public double Gamma { get; }

        public TMCoordinate(double x, double y, double z, double scale, double gamma) : base(x, y, z)
        {
            Scale = scale;
            Gamma = gamma;
        }
    }



}
