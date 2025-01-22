using System;
using System.Collections.Generic;
using System.Text;

namespace egbt22lib
{
    public static class Transformation
    {
        public const double Sec2Rad = 4.8481368110953599358991410235795e-6;

        //public static readonly double[] Param_ETRS892DBRef =
        //{ -584.9567,
        //   - 107.7277,
        //   - 413.8036,
        //   1.1155257601,
        //   0.2824170155,
        //   - 3.1384505907,
        //   - 7.992171 };

        public static readonly double[] Param_DBRef2ETRS89 =
        { 584.9636,
           107.7175,
           413.8067,
           - 1.1155214628,
           - 0.2824339890,
           3.1384490633,
           7.992235 };

        private static readonly double[] //RotationMatrix_ETRS892DBRef, 
            RotationMatrix_DBRef2ETRS89;
        private static readonly double //Scale_ETRS892DBRef,
                                       Scale_DBRef2ETRS89;

        static Transformation()
        {
            //Scale_ETRS892DBRef = 1.0 + (Param_ETRS892DBRef[6] * 1.0e-6);
            Scale_DBRef2ETRS89 = 1.0 + (Param_DBRef2ETRS89[6] * 1.0e-6);
            //RotationMatrix_ETRS892DBRef = toRotationMatrix(Param_ETRS892DBRef);
            RotationMatrix_DBRef2ETRS89 = toRotationMatrix(Param_DBRef2ETRS89);
        }

        private static double[] toRotationMatrix(in double[] m)
        {
            var f = m[3] * Sec2Rad;
            var t = m[4] * Sec2Rad;
            var p = m[5] * Sec2Rad;
            var cf = Math.Cos(f);
            var sf = Math.Sin(f);
            var ct = Math.Cos(t);
            var st = Math.Sin(t);
            var cp = Math.Cos(p);
            var sp = Math.Sin(p);

            return new double[]
            {
                 ct * cp, (cf * sp) + (sf * st * cp), (sf * sp) - (cf * st * cp),
                -ct * sp, (cf * cp) - (sf * st * sp), (sf * cp) + (cf * st * sp),
                      st,                  -sf * ct ,                   cf * ct
            };
        }

        private static (double x, double y, double z) rotateForward(double[] m, (double x, double y, double z) p)
        {
            var (x, y, z) = p;
            return (
                (m[0] * x) + (m[1] * y) + (m[2] * z),
                (m[3] * x) + (m[4] * y) + (m[5] * z),
                (m[6] * x) + (m[7] * y) + (m[8] * z));
        }

        private static (double x, double y, double z) rotateReverse(double[] m, (double x, double y, double z) p)
        {
            var (x, y, z) = p;
            return (
                (m[0] * x) + (m[3] * y) + (m[6] * z),
                (m[1] * x) + (m[4] * y) + (m[7] * z),
                (m[2] * x) + (m[5] * y) + (m[8] * z));
        }

        private static (double x, double y, double z) translateForward(double[] t, (double x, double y, double z) p)
        {
            var (x, y, z) = p;
            return (x + t[0], y + t[1], z + t[2]);
        }

        private static (double x, double y, double z) translateReverse(double[] t, (double x, double y, double z) p)
        {
            var (x, y, z) = p;
            return (x - t[0], y - t[1], z - t[2]);
        }
        private static (double x, double y, double z) scaleForward(double s, (double x, double y, double z) p)
        {
            var (x, y, z) = p;
            return (x * s, y * s, z * s);
        }

        private static (double x, double y, double z) scaleReverse(double s, (double x, double y, double z) p)
        {
            var (x, y, z) = p;
            return (x / s, y / s, z / s);
        }

        public static (double x, double y, double z) DbrefToEtrs89(double x, double y, double z)
        {
            return translateForward(Param_DBRef2ETRS89,
                scaleForward(Scale_DBRef2ETRS89,
                rotateForward(RotationMatrix_DBRef2ETRS89, (x, y, z))));
        }

        //public static (double x, double y, double z) Etrs89ToDbref(double x, double y, double z)
        //{
        //    return translateForward(Param_ETRS892DBRef,
        //        scaleForward(Scale_ETRS892DBRef,
        //        rotateForward(RotationMatrix_ETRS892DBRef, (x, y, z))));
        //}

        public static (double x, double y, double z) DbrefToEtrs89Inv(double x, double y, double z)
        {
            return rotateReverse(RotationMatrix_DBRef2ETRS89,
                scaleReverse(Scale_DBRef2ETRS89,
                translateReverse(Param_DBRef2ETRS89, (x, y, z))));
        }

    }
}
