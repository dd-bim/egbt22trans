egbt22trans

Program for converting coordinates between different coordinate systems. 
The decimal separator is the point.
Only conversions between the coordinate systems defined below are possible. All columns are
as they were read in, only the coordinate axes x and y [or z] are converted.

The following coordinate systems (CRS) are supported:
1 : ETRS89 EGBT_LDP Dresden-Prag   
2 : ETRS89 UTM33                   (EPSG:25833)
3 : ETRS89 geographic 3D B/L       (EPSG:4258)
4 : ETRS89 cartesian 3D geocentric (EPSG:4936)
5 : DB_Ref GK5                     (EPSG:5685)
6 : DB_Ref geographic 3D B/L       (EPSG:5681)
7 : DB_Ref cartesian 3D geocentric (EPSG:4936)


The following conversions/transformations are supported:
  | 1 | 2 | 3 | 4 | 5 | 6 | 7
 1|   | x | x | h | h | h | h 
 2| x |   | x | h | h | h | h
 3| x | x |   | h | h | h | h
 4| x | x | x |   | x | x | x
 5| h | h | h | h |   | x | h
 6| h | h | h | h | x |   | h
 7| x | x | x | x | x | x |  
 
An 'x' means that a simple conversion without heights is possible.
An 'h' means that the conversion/transformation needs an ellipsoidal (ETRS89 or DB_Ref) or normal (DHHN2016, GCG2016) height. (option '-h 1' or '-h 2' is mandatory)

The supported height systems are:
1 : Normal heights (calculations based on the GCG2016 geoid)
2 : Ellipsoidal heights (ETRS89 or DB_Ref)

The heights are used for the transformation from ETRS89 to DB_REF and vice versa. 
Normal heights remain unchanged, except the target coordinates geocentric (the internal transformation uses ellipsoidal heights).
For the transformation from ETRS89 to DB_REF and vice versa, the differences between DHHN2016 and GCG2016 could be ignored.
The transformation from ellipsoidal to normal height and vice versa uses the GCG2016 Geoid. 
At transformations from DB_Ref to ETRS89 with normal heights the ellipsoidal heights for DB_REf are iterative approximated.
The transformation from ETRS89 to DB_REF and vice versa uses the official transformation parameters (different for both directions).
At the datum transformation ellipsoidal heights are transformed to the other datum.
The conversion calculations (geodetic, geocentric, transverse mercator) are done with the library GeographicLib.Net (https://github.com/noelex/GeographicLib.NET).
 
Command line options:
	- --help: Shows help

Coordinate conversion (normal case):
	- -s, --source: Source system (by CRS-Index)
	- -t, --target: Target system (by CRS-Index)
	- -h, --height: Height system (1: normal heights, 2: ETRS89 ellipsoidal heights)

Exampels:
	Simple conversion from ETRS89 EGBT_LDP Dresden-Prag to UTM33:
	-s 1 -t 2 
	Conversion and transformation from EGBT_LDP Dresden-Prag to DB_REF GK5:
	-s 1 -t 5 -h 2

Optional:
	- -d, --delimiter: Delimiter of the coordinate columns (s=space, t=tabulator, ,=comma, ...), default: space
	- -x, --xaxis: Column-index of the x-axis (1. defined coordinate axis), default: 2
	- -y, --yaxis: Column-index of the y-axis (2. defined coordinate axis), default: 3
	- -z, --zaxis: Column-index of the z-axis (cartesian 3D Z axis, normal height or ellipsoidal height), default: 4 (ignored if not necessary)
	- -p, --precision: Number of digits after the decimal point in the output, default: 4
	- -l, --latlon: Number of digits after the decimal point at latitude or longitude values in degrees, default: 10

EGBT22 datum transformation (special case):
Coordinates in systems 3 and 4 (Geographic 3D with ellipsoidal heights / ETRS89 Cartesian 3D geocentric ) can be transformed from or to the EGBT22 datum.
This is usually only necessary for precise GNSS observations.
Columns 2, 3 and 4 are used as standard for the x-, y- and z-axis eg. B-, L- and ell.h-axis.
The source and target system must not be specified with the -s and -t options. 

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
If the output file ommited, the output is written to the console.

Examples:
egbt22trans.exe -s 1 -t 3 input.txt output.txt
egbt22trans.exe -e 1 input.txt output.txt

