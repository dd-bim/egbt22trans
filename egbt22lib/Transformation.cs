using System;
using System.Collections.Generic;
using System.Text;

namespace egbt22lib
{
    public static class Transformation
    {
        public const double Sec2Rad = 4.8481368110953599358991410235795e-6;

        public static readonly double[] Param_ETRS892DBRef =
        { -584.9567,
           - 107.7277,
           - 413.8036,
           1.1155257601,
           0.2824170155,
           - 3.1384505907,
           - 7.992171 };

        public static readonly double[] Param_DBRef2ETRS89 =
        { 584.9636,
           107.7175,
           413.8067,
           - 1.1155214628,
           - 0.2824339890,
           3.1384490633,
           7.992235 };

        private static readonly double[,] e2d, d2e;

        static Transformation()
        {
            e2d = toRotationMatrix(Param_ETRS892DBRef);
            d2e = toRotationMatrix(Param_DBRef2ETRS89);
        }

        private static double[,] toRotationMatrix(in double[] m)
        {
            double rX = m[3] * Sec2Rad;
            double rY = m[4] * Sec2Rad;
            double rZ = m[5] * Sec2Rad;
            double cx = Math.Cos(rX);
            double sx = Math.Sin(rX);
            double cy = Math.Cos(rY);
            double sy = Math.Sin(rY);
            double cz = Math.Cos(rZ);
            double sz = Math.Sin(rZ);

            return new double[,]
            {
                {  cy * cz, (cx * sz) + (sx * sy * cz), (sx * sz) - (cx * sy * cz), m[0] },
                { -cy * sz, (cx * cz) - (sx * sy * sz), (sx * cz) + (cx * sy * sz), m[1] },
                {       sy,                   -sx * cy,                     cx* cy, m[2] },
                {      0.0,                        0.0,                        0.0, 1.0 + (m[6] * 1.0e-6) }
            };
        }

        private static Point rotateByMatrix(in double[,] m, in Point p)
        {
            return new Point(
                (m[3, 3] * ((m[0, 0] * p.X) + (m[0, 1] * p.Y) + (m[0, 2] * p.Z))) + m[0, 3],
                (m[3, 3] * ((m[1, 0] * p.X) + (m[1, 1] * p.Y) + (m[1, 2] * p.Z))) + m[1, 3],
                (m[3, 3] * ((m[2, 0] * p.X) + (m[2, 1] * p.Y) + (m[2, 2] * p.Z))) + m[2, 3]);
        }

        public static Point DbrefToEtrs89(in Point p)
        {
            return rotateByMatrix(d2e, p);
        }

        public static Point Etrs89ToDbref(in Point p)
        {
            return rotateByMatrix(e2d, p);
        }



    }
}
