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

        public (double east, double north) Forward(double lat, double lon)
        {
            (double east, double north) = _tm.Forward(_lon0, lat, lon);
            return (east + _fe, north + _fn);
        }
        public (double east, double north, double bypass) Forward(double lat, double lon, double bypass)
        {
            (double east, double north) = _tm.Forward(_lon0, lat, lon);
            return (east + _fe, north + _fn, bypass);
        }
        public (double east, double north) Forward(double lat, double lon, out double gamma, out double k)
        {
            (double east, double north) = _tm.Forward(_lon0, lat, lon, out gamma, out k);
            return (east + _fe, north + _fn);
        }

        public (double lat, double lon) Reverse(double east, double north)
        {
            var (lat, lon) = _tm.Reverse(_lon0, east - _fe, north - _fn);
            return (lat, lon);
        }
        public (double lat, double lon, double bypass) Reverse(double east, double north, double bypass)
        {
            var (lat, lon) = _tm.Reverse(_lon0, east - _fe, north - _fn);
            return (lat, lon, bypass);
        }
        public (double lat, double lon) Reverse(double east, double north, out double gamma, out double k)
        {
            var (lat, lon) = _tm.Reverse(_lon0, east - _fe, north - _fn, out gamma, out k);
            return (lat, lon);
        }

    }
}
