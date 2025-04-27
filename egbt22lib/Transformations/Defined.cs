using System;
using System.Collections.Generic;
using System.Text;

using static egbt22lib.Conversions.Defined;

namespace egbt22lib.Transformations
{
    public static class Defined
    {
        public const double TOLRAD = 1e-12;
        public const double TOLM = 1e-6;

        public static readonly Transformation Trans_Datum_ETRS89_to_DBRef = new Transformation(
                -584.9567,
                -107.7277,
                -413.8036,
                1.1155257601,
                0.2824170155,
                -3.1384505907,
                -7.992171
        );

        public static readonly Transformation Trans_Datum_DBRef_to_ETRS89 = new Transformation(
                584.9636,
                107.7175,
                413.8067,
                -1.1155214628,
                -0.2824339890,
                3.1384490633,
                7.992235
        );

        public static readonly Transformation Trans_Datum_ETRS89_DREF91_to_EGBT22 = new Transformation(-0.0028, -0.0023, 0.0029);

        public static readonly Transformation Trans_Datum_ETRS89_CZ_to_EGBT22 = new Transformation(0.0028, 0.0023, -0.0029);

    }
}
