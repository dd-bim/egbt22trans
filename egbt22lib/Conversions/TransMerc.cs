using GeographicLib.Projections;

using System;
using System.Collections.Generic;
using System.Text;

namespace egbt22lib.Conversions
{
    public class TransMerc
    {
        private readonly TransverseMercatorExact _tm;

        private readonly double _lon0, _fe, _fn;

        public TransMerc(double a, double f, double k0, double fe, double fn, double lon0, double? lat0 = null)
        {
            _tm = new TransverseMercatorExact(a, f, k0);
            _fe = fe;
            _fn = lat0.HasValue ? fn - _tm.Forward(0, lat0.Value, 0).y : fn;
            _lon0 = lon0;
        }
        public (double e, double n) Forward(double lat, double lon)
        {
            (double east, double north) = _tm.Forward(_lon0, lat, lon, out _, out _);
            return (east + _fe, north + _fn);
        }

        public (double lat, double lon) Reverse(double e, double n)
        {
            var (lat, lon) = _tm.Reverse(_lon0, e - _fe, n - _fn, out _, out _);
            return (lat, lon);
        }

        public (double e, double n, double dummy) Forward(double lat, double lon, double dummy)
        {
            (double east, double north) = _tm.Forward(_lon0, lat, lon, out _, out _);
            return (east + _fe, north + _fn, dummy);
        }

        public (double lat, double lon, double dummy) Reverse(double e, double n, double dummy)
        {
            var (lat, lon) = _tm.Reverse(_lon0, e - _fe, n - _fn, out _, out _);
            return (lat, lon, dummy);
        }

        public (double lat, double lon) Reverse(double e, double n, out double gamma, out double k) => _tm.Reverse(_lon0, e - _fe, n - _fn, out gamma, out k);

    }
}
