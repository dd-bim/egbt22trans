using System;

namespace egbt22lib
{
    [Flags]
    public enum CRS
    {
        Geod = 0,
        Geoc = 1,
        EGBT_LDP = 2,
        UTM33 = 4,
        GK5 = 8,
        Conversion = Geod | Geoc | EGBT_LDP | UTM33 | GK5,
        ETRS89 = 16,
        DB_Ref = 32,
        EGBT22 = 64,
        ETRS89_DREF91 = 128,
        ETRS89_CZ = 256,
        Datum = ETRS89 | DB_Ref | EGBT22 | ETRS89_DREF91 | ETRS89_CZ,

        EGBT22_EGBT_LDP = EGBT22 | EGBT_LDP,
        EGBT22_Geod = EGBT22 | Geod,
        EGBT22_Geoc = EGBT22 | Geoc,
        ETRS89_DREF91_UTM33 = ETRS89_DREF91 | UTM33,
        ETRS89_DREF91_Geod = ETRS89_DREF91 | Geod,
        ETRS89_DREF91_Geoc = ETRS89_DREF91 | Geoc,
        ETRS89_CZ_Geod = ETRS89_CZ | Geod,
        ETRS89_CZ_Geoc = ETRS89_CZ | Geoc,
        DB_Ref_GK5 = DB_Ref | GK5,
        DB_Ref_Geod = DB_Ref | Geod,
        DB_Ref_Geoc = DB_Ref | Geoc
    }

}
