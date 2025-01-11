egbt22trans

Program for converting coordinates between different coordinate systems. 
The decimal separator is the point.
Only conversions between the coordinate systems defined below are possible. All columns are
as they were read in, only the coordinate axes x and y [or z] are converted.

The following coordinate systems (CRS) are supported:
1: ETRS89 EGBT_LDP Dresden-Prag (normal heights)
2: ETRS89 UTM33 (EPSG:25833) (normal heights)
3: DB_REF GK5 (EPSG:5685) (normal heights)
4: ETRS89 cartesian 3D geocentric (EPSG:4936)
5: ETRS89 geographic 3D B/L/ellipsoidal height (EPSG:4937)

Command line options:
- --help: Shows help

Coordinate conversion (normal case):
- -s, --source: Source system (by CRS-Index)
- -t, --target: Target system (by CRS-Index)

-s 1 -t 2 and -s 2 -t 1 Conversion from EGBT_LDP to UTM33 (or reverse), only 2D (heights not necessary)
-s 1 -t 3 and -s 3 -t 1 Conversion from EGBT_LDP to GK5 (or reverse), 3D with normal heights (no height change but necessary!)  
-s 1 -t 4 and -s 4 -t 1 Conversion from EGBT_LDP to ETRS89 geocentric (or reverse), 3D, EGBT_LDP with normal heights
-s 1 -t 5 and -s 5 -t 1 Conversion from EGBT_LDP to ETRS89 geographic (or reverse), 3D, EGBT_LDP with normal heights, ETRS89 geographic with ellipsoidal heights
-s 2 -t 3 and -s 3 -t 2 Conversion from UTM33 to GK5 (or reverse), 3D with normal heights (no height change but necessary!) 
-s 2 -t 4 and -s 4 -t 2 Conversion from UTM33 to ETRS89 geocentric (or reverse), 3D, UTM33 with normal heights
-s 2 -t 5 and -s 5 -t 2 Conversion from UTM33 to ETRS89 geographic (or reverse), 3D, UTM33 with normal heights, ETRS89 geographic with ellipsoidal heights
-s 3 -t 4 and -s 4 -t 3 Conversion from GK5 to ETRS89 geocentric (or reverse), 3D, GK5 with normal heights
-s 3 -t 5 and -s 5 -t 3 Conversion from GK5 to ETRS89 geographic (or reverse), 3D, GK5 with normal heights, ETRS89 geographic with ellipsoidal heights
-s 4 -t 5 and -s 5 -t 4 Conversion from ETRS89 geocentric to ETRS89 geographic (or reverse), 3D, ETRS89 geographic with ellipsoidal heights

For 3D calculations ist the Z-axis required (all except -s 1 -t 2 and -s 2 -t 1)!

Optional:
- -d, --delimiter: Delimiter of the coordinate columns (s=space, t=tabulator, ,=comma, ...), default: space
- -x, --xaxis: Column-index of the x-axis (1. defined coordinate axis), default: 2
- -y, --yaxis: Column-index of the y-axis (2. defined coordinate axis), default: 3
- -z, --zaxis: Column-index of the z-axis (cartesian 3D Z, ellipsoidal or normal height), default: 4 (ignored if not necessary)
- -p, --precision: Number of digits after the decimal point, default: 4
- -l, --latlon: Number of digits after the decimal point at latitude or longitude values in degrees, default: 10

EGBT22 datum transformation (special case):
Coordinates in systems 4 and 5 (ETRS89 Cartesian 3D geocentric / Geographic 3D ) can be transformed from or to the EGBT22 datum.
This is usually only necessary for precise GNSS observations.
Columns 2, 3 and 4 are used as standard for the x-, y- and z-axis eg. B-, L- and ell.h-axis.
The source and target system must be 4 or 9 and must not be specified with the -s and -t options. 

The -e option is available for this:
- -e, --egbt22: 
	1=Sapos to EGBT22  (EPSG:4936) (geocentric), 
	2=EGBT22 to Sapos  (EPSG:4936) (geocentric), 
	3=Czepos to EGBT22 (EPSG:4936) (geocentric), 
	4=EGBT22 to Czepos (EPSG:4936) (geocentric),
	5=Sapos to EGBT22  (EPSG:4937) (geographic), 
	6=EGBT22 to Sapos  (EPSG:4937) (geographic), 
	7=Czepos to EGBT22 (EPSG:4937) (geographic), 
	8=EGBT22 to Czepos (EPSG:4937) (geographic)

The input and output files are specified after the options, 
first the path to the input file then the path to the output file.

Examples:
egbt22trans.exe -s 1 -t 3 input.txt output.txt
egbt22trans.exe -e 1 input.txt output.txt

