using GeographicLib.Projections;

using System;
using System.Collections.Generic;
using System.Text;

namespace egbt22lib.Conversions
{
    internal class TransMerc
    {
        private readonly TransverseMercatorExact _tm;

        private readonly double _lon0, _fe, _fn, _a, _f;  

        public TransMerc(double a, double f, double k0, double fe, double fn, double lon0, double? lat0 = null)
        {
            _tm = new TransverseMercatorExact(a, f, k0);
            _fe = fe;
            _fn = lat0.HasValue ? fn - _tm.Forward(0, lat0.Value, 0).y : fn;
            _lon0 = lon0;
            _a = a;
            _f = f;
        }

        public TMCoordinate Forward(Coordinate llh)
        {
            (double east, double north) = _tm.Forward(_lon0, llh.X, llh.Y, out double gamma, out double k);
            double scale = Common.PointScaleAtHeight(k, llh.X, llh.Z, _a, _f);
            return new TMCoordinate(east + _fe, north + _fn, llh.Z, scale, gamma);
        }

        public TMCoordinate Reverse(Coordinate enh)
        {
            var (lat, lon) = _tm.Reverse(_lon0, enh.X - _fe, enh.Y - _fn, out double gamma, out double k);
            double scale = Common.PointScaleAtHeight(k, lat, enh.Z, _a, _f);
            return new TMCoordinate(lat, lon, enh.Z, scale, gamma);
        }

    }
}
