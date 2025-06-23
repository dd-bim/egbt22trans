using System;
using System.Collections.Generic;
using System.Text;

using static egbt22lib.Conversions.Common;

namespace egbt22lib.Conversions
{
    internal static class Defined
    {
        public static readonly Geocent GC_GRS80 = new Geocent(GRS80_a, GRS80_f);

        public static readonly Geocent GC_Bessel = new Geocent(Bessel_a, Bessel_f);

        public static readonly TransMerc TM_Bessel_GK5 = new TransMerc(Bessel_a, Bessel_f, 1.0, 5500000.0, 0.0, 15.0);

        public static readonly TransMerc TM_GRS80_UTM33 = new TransMerc(GRS80_a, GRS80_f, 0.9996, 500000.0, 0.0, 15.0);

        public static readonly TransMerc TM_GRS80_EGBT_LDP = new TransMerc(GRS80_a, GRS80_f, 1.0000346, 10000.0, 50000.0, 13.9027, 50.8247);
    }
}
