using egbt22lib.Conversions;

using System;
using System.Collections.Generic;
using System.Text;

namespace egbt22lib.Transformations
{
    public class Transformation
    {
        [Flags]
        public enum Kinds
        {
            None = 0,
            Translation = 1, 
            Rotation = 2, 
            Scale = 4   
        }

        public const double Sec2Rad = 4.8481368110953599358991410235795e-6;

        public readonly double tx, ty, tz, 
            rxx, rxy, rxz, 
            ryx, ryy, ryz, 
            rzx, rzy, rzz, 
            s;

        public readonly Kinds Kind;

        /// <summary>
        /// Constructs a transformation with translation, rotation, and scaling parameters.
        /// </summary>
        /// <param name="tx">Specifies the translation along the x-axis.</param>
        /// <param name="ty">Specifies the translation along the y-axis.</param>
        /// <param name="tz">Specifies the translation along the z-axis.</param>
        /// <param name="rotx_arcsec">Defines the rotation around the x-axis in arcseconds.</param>
        /// <param name="roty_arcsec">Defines the rotation around the y-axis in arcseconds.</param>
        /// <param name="rotz_arcsec">Defines the rotation around the z-axis in arcseconds.</param>
        /// <param name="scale_ppm">Indicates the scaling factor in parts per million.</param>
        public Transformation(double tx, double ty, double tz, double rotx_arcsec, double roty_arcsec, double rotz_arcsec, double scale_ppm)
        {
            this.tx = tx;
            this.ty = ty;
            this.tz = tz;

            double f = rotx_arcsec * Sec2Rad;
            double t = roty_arcsec * Sec2Rad;
            double p = rotz_arcsec * Sec2Rad;
            double cf = Math.Cos(f);
            double sf = Math.Sin(f);
            double ct = Math.Cos(t);
            double st = Math.Sin(t);
            double cp = Math.Cos(p);
            double sp = Math.Sin(p);

            (rxx, rxy, rxz, 
                ryx, ryy, ryz, 
                rzx, rzy, rzz) = (
                ct * cp, (cf * sp) + (sf * st * cp), (sf * sp) - (cf * st * cp),
               -ct * sp, (cf * cp) - (sf * st * sp), (sf * cp) + (cf * st * sp),
                     st,                   -sf * ct,                   cf * ct);

            s = 1.0 + (scale_ppm * 1.0e-6);

            Kind = Kinds.Translation | Kinds.Rotation | Kinds.Scale;
        }

        /// <summary>
        /// Initializes a transformation with specified translation values along the x, y, and z axes.
        /// </summary>
        /// <param name="tx">Specifies the translation distance along the x-axis.</param>
        /// <param name="ty">Specifies the translation distance along the y-axis.</param>
        /// <param name="tz">Specifies the translation distance along the z-axis.</param>
        public Transformation(double tx, double ty, double tz)
        {
            this.tx = tx;
            this.ty = ty;
            this.tz = tz;
            Kind = Kinds.Translation;
        }

        ///// <summary>
        ///// Initializes a transformation with a scaling factor based on parts per million.
        ///// </summary>
        ///// <param name="scale_ppm">The value represents a scaling factor that adjusts the transformation's magnitude.</param>
        //public Transformation(double scale_ppm)
        //{
        //    s = 1.0 + (scale_ppm * 1.0e-6);

        //    Kind = Kinds.Scale;
        //}

        ///// <summary>
        ///// Constructs a transformation matrix based on rotation angles and a scaling factor.
        ///// </summary>
        ///// <param name="rotx_arcsec">Specifies the rotation angle around the x-axis in arcseconds.</param>
        ///// <param name="roty_arcsec">Specifies the rotation angle around the y-axis in arcseconds.</param>
        ///// <param name="rotz_arcsec">Specifies the rotation angle around the z-axis in arcseconds.</param>
        ///// <param name="scale_ppm">Defines the scaling factor in parts per million.</param>
        //public Transformation(double rotx_arcsec, double roty_arcsec, double rotz_arcsec, double scale_ppm)
        //{
        //    double f = rotx_arcsec * Sec2Rad;
        //    double t = roty_arcsec * Sec2Rad;
        //    double p = rotz_arcsec * Sec2Rad;
        //    double cf = Math.Cos(f);
        //    double sf = Math.Sin(f);
        //    double ct = Math.Cos(t);
        //    double st = Math.Sin(t);
        //    double cp = Math.Cos(p);
        //    double sp = Math.Sin(p);

        //    (rxx, rxy, rxz,
        //        ryx, ryy, ryz,
        //        rzx, rzy, rzz) = (
        //        ct * cp, (cf * sp) + (sf * st * cp), (sf * sp) - (cf * st * cp),
        //       -ct * sp, (cf * cp) - (sf * st * sp), (sf * cp) + (cf * st * sp),
        //             st, -sf * ct, cf * ct);

        //    s = 1.0 + (scale_ppm * 1.0e-6);

        //    Kind = Kinds.Rotation | Kinds.Scale;
        //}

        ///// <summary>
        ///// Constructs a transformation matrix based on rotation angles in arcseconds and a boolean indicating rotation
        ///// type.
        ///// </summary>
        ///// <param name="rotx_arcsec">Specifies the rotation angle around the x-axis in arcseconds.</param>
        ///// <param name="roty_arcsec">Specifies the rotation angle around the y-axis in arcseconds.</param>
        ///// <param name="rotz_arcsec">Specifies the rotation angle around the z-axis in arcseconds.</param>
        ///// <param name="rotation">Indicates whether the transformation is a rotation. (the value is irrelevant, just to differentiate the constructor)</param>
        //public Transformation(double rotx_arcsec, double roty_arcsec, double rotz_arcsec, bool rotation)
        //{
        //    double f = rotx_arcsec * Sec2Rad;
        //    double t = roty_arcsec * Sec2Rad;
        //    double p = rotz_arcsec * Sec2Rad;
        //    double cf = Math.Cos(f);
        //    double sf = Math.Sin(f);
        //    double ct = Math.Cos(t);
        //    double st = Math.Sin(t);
        //    double cp = Math.Cos(p);
        //    double sp = Math.Sin(p);

        //    (rxx, rxy, rxz,
        //        ryx, ryy, ryz,
        //        rzx, rzy, rzz) = (
        //        ct * cp, (cf * sp) + (sf * st * cp), (sf * sp) - (cf * st * cp),
        //       -ct * sp, (cf * cp) - (sf * st * sp), (sf * cp) + (cf * st * sp),
        //             st, -sf * ct, cf * ct);

        //    Kind = Kinds.Rotation;
        //}


        #region private methods
        private (double x, double y, double z) rotateForward(double x, double y, double z)
        {
            return (
                (rxx * x) + (rxy * y) + (rxz * z),
                (ryx * x) + (ryy * y) + (ryz * z),
                (rzx * x) + (rzy * y) + (rzz * z));
        }

        private (double x, double y, double z) rotateReverse(double x, double y, double z)
        {
            return (
                (rxx * x) + (ryx * y) + (rzx * z),
                (rxy * x) + (ryy * y) + (rzy * z),
                (rxz * x) + (ryz * y) + (rzz * z));
        }

        private (double x, double y, double z) translateForward(double x, double y, double z) => (x + tx, y + ty, z + tz);

        private (double x, double y, double z) translateReverse(double x, double y, double z) => (x - tx, y - ty, z - tz);

        private (double x, double y, double z) scaleForward(double x, double y, double z) => (x * s, y * s, z * s);

        private (double x, double y, double z) scaleReverse(double x, double y, double z) => (x / s, y / s, z / s);

        #endregion

        public (double x, double y, double z) Forward(double x, double y, double z)
        {
            if (Kind.HasFlag(Kinds.Rotation))
            {
                (x, y, z) = rotateForward(x, y, z);
            }
            if (Kind.HasFlag(Kinds.Scale))
            {
                (x, y, z) = scaleForward(x, y, z);
            }
            if (Kind.HasFlag(Kinds.Translation))
            {
                (x, y, z) = translateForward(x, y, z);
            }
            return (x, y, z);
        }

        public (double x, double y, double z) Reverse(double x, double y, double z)
        {
            if (Kind.HasFlag(Kinds.Translation))
            {
                (x, y, z) = translateReverse(x, y, z);
            }
            if (Kind.HasFlag(Kinds.Scale))
            {
                (x, y, z) = scaleReverse(x, y, z);
            }
            if (Kind.HasFlag(Kinds.Rotation))
            {
                (x, y, z) = rotateReverse(x, y, z);
            }
            return (x, y, z);
        }


    }
}
