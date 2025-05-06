# egbt22trans

`egbt22trans.exe` is a program for converting coordinates between different coordinate systems. Conversions are possible between the coordinate systems defined below. The coordinates to be converted must be provided in column-based text files, using a period as the decimal separator. All columns remain as read, only the coordinate axes `x` and `y` [or `z`] are converted.

The program is open source and can be found at [https://github.com/dd-bim/egbt22trans.git](https://github.com/dd-bim/egbt22trans.git).

## Supported Coordinate Systems (CRS)

1. **ETRS89 EGBT_LDP Dresden-Prag**  
2. **ETRS89 UTM33** (EPSG:25833)  
3. **ETRS89 geographic 3D B/L** (EPSG:4258)  
4. **ETRS89 cartesian 3D geocentric** (EPSG:4936)  
5. **DB_Ref GK5** (EPSG:5685)  
6. **DB_Ref geographic 3D B/L** (EPSG:5681)  
7. **DB_Ref cartesian 3D geocentric** (EPSG:4936)  

## Supported Conversions / Transformations

|   | 1 | 2 | 3 | 4 | 5 | 6 | 7 |
|---|---|---|---|---|---|---|---|
| 1 |   | x | x | h | h | h | h |
| 2 | x |   | x | h | h | h | h |
| 3 | x | x |   | h | h | h | h |
| 4 | x | x | x |   | x | x | x |
| 5 | h | h | h | h |   | x | h |
| 6 | h | h | h | h | x |   | h |
| 7 | x | x | x | x | x | x |   |

- **x**: Simple conversion without heights is possible.  
- **h**: Conversion/transformation requires an ellipsoidal height (ETRS89 or DB_Ref) or a normal height (DHHN2016, GCG2016). (Option `-h 1` or `-h 2` is required)

## Supported Height Systems

1. **Normal heights** (internal calculations are based on the quasigeoid GCG2016)  
2. **Ellipsoidal heights** (ETRS89 or DB_Ref)  

Heights are used for the transformation from **ETRS89** to **DB_REF** and vice versa. Normal heights are internally transformed into ellipsoidal heights for this purpose. The **German Combined Quasigeoid (GCG2016)** is used for this ([BKG](https://www.bkg.bund.de/), [CC BY 4.0](https://creativecommons.org/licenses/by/4.0/)). Ellipsoidal heights are converted to the respective target datum during datum transformations. For transformations from **DB_Ref** to **ETRS89** with normal heights, ellipsoidal heights for **DB_Ref** are iteratively approximated, as the GCG2016 model is based on the ETRS89 datum.

The transformation from **ETRS89** to **DB_REF** and vice versa uses the official transformation parameters (different for both directions).  
The conversion calculations (geodetic, geocentric, transverse Mercator) are performed using the library [GeographicLib.Net](https://github.com/noelex/GeographicLib.NET).

## Command Line Options

The console program `egbt22trans.exe` can be configured with the following options:

### General Options

- `--help`: Displays the help.

### Options for Coordinate Conversion and Transformation

- `-s, --source`: Source system (CRS index)  
- `-t, --target`: Target system (CRS index)  
- `-h, --height`: Height system (1: normal heights, 2: ellipsoidal heights)  

**Examples:**

- Simple conversion from **ETRS89 EGBT_LDP Dresden-Prag** to **UTM33**:  
```
-s 1 -t 2
```
- Conversion and transformation from **EGBT_LDP Dresden-Prag** to **DB_REF GK5** with normal heights: 
```
-s 1 -t 5 -h 1
```

### Additional Options

- `-d, --delimiter`: Delimiter of the coordinate columns (`s`=space, `t`=tab, `,`=comma, ...)  
  **Default:** space  
- `-x, --xaxis`: Column index of the x-axis (1st defined coordinate axis)  
  **Default:** 2  
- `-y, --yaxis`: Column index of the y-axis (2nd defined coordinate axis)  
  **Default:** 3  
- `-z, --zaxis`: Column index of the z-axis (z-axis, normal height, or ellipsoidal height)  
  **Default:** 4 (ignored if not required)  
- `-p, --precision`: Number of decimal places in the output  
  **Default:** 4  
- `-l, --latlon`: Number of decimal places for latitude or longitude in degrees  
  **Default:** 10  

## Datum Transformation for EGBT22

In addition to normal conversion and transformation, a special datum transformation for the **EGBT22 datum** is also possible. This allows coordinates in systems 3 and 4 (geographic 3D with ellipsoidal heights / ETRS89 cartesian 3D geocentric) to be transformed to or from the **EGBT22 datum**. This is usually only required for precise GNSS observations.

The source and target system do not need to be specified with the options `-s` and `-t`. For this transformation, the option `-e` or `--egbt22` is used. The desired transformation must then be defined with one of the following numbers:

1. **Sapos to EGBT22** (geocentric, ID 4)  
2. **EGBT22 to Sapos** (geocentric, ID 4)  
3. **Czepos to EGBT22** (geocentric, ID 4)  
4. **EGBT22 to Czepos** (geocentric, ID 4)  
5. **Sapos to EGBT22** (geographic, ID 3 with ellipsoidal heights)  
6. **EGBT22 to Sapos** (geographic, ID 3 with ellipsoidal heights)  
7. **Czepos to EGBT22** (geographic, ID 3 with ellipsoidal heights)  
8. **EGBT22 to Czepos** (geographic, ID 3 with ellipsoidal heights)  

The input and output files are specified after the options, first the path to the input file, then the path to the output file. If the output file is omitted, the output is written to the console.

**Examples:**

- Transformation with normal heights and high precision:  
```
egbt22trans.exe -s 1 -t 3 -h 1 -l 12 input.txt output.txt
```
- Datum transformation from Sapos to EGBT22 with geocentric coordinates: 
```
egbt22trans.exe -e 1 input.txt output.txt
```