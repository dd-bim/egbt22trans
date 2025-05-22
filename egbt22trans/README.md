# egbt22trans

`egbt22trans.exe` is a program for converting coordinates between different coordinate systems. Conversions are possible between the coordinate systems defined below. The coordinates to be converted must be provided in column-based text files, using a period as the decimal separator. All columns remain as read; only the coordinate axes x and y [or z] (i.e., latitude, longitude, [height] or easting, northing, [height]) are converted. The unit of the coordinates is meters, and for geodetic coordinates, decimal degrees.

The program is open source and can be found at [https://github.com/dd-bim/egbt22trans.git](https://github.com/dd-bim/egbt22trans.git).

## Supported Coordinate Systems (CRS)

1. **EGBT22 EGBT_LDP Dresden-Prague**  
2. **EGBT22 geographic 3D lat/lon**  
3. **EGBT22 cartesian 3D geocentric**  
4. **ETRS89/DREF91 UTM33**  
5. **ETRS89/DREF91 geographic 3D lat/lon**  
6. **ETRS89/DREF91 cartesian 3D geocentric**  
7. **ETRS89/CZ geographic 3D lat/lon**  
8. **ETRS89/CZ cartesian 3D geocentric**  
9. **DB_Ref GK5**  
10. **DB_Ref geographic 3D lat/lon**  
11. **DB_Ref cartesian 3D geocentric**  

## Supported Conversions / Transformations

|   | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9 | 10 | 11 |
|---|---|---|---|---|---|---|---|---|---|---|---|
| **1** |   | x | h | h | h | h | h | h | h | h | h |
| **2** | x |   | h | h | h | h | h | h | h | h | h |
| **3** | x | x |   | x | x | x | x | x | x | x | x |
| **4** | h | h | h |   | x | h |   |   | h | h | h |
| **5** | h | h | h | x |   | h |   |   | x | x | x |
| **6** | x | x | x | x | x |   |   |   | x | x | x |
| **7** | h | h | h |   |   |   |   | h |   |   |   |
| **8** | x | x | x |   |   |   | x |   |   |   |   |
| **9** | h | h | h | h | h | h |   |   |   | x | h |
| **10** | h | h | h | h | h | h |   |   | x |   | h |
| **11** | x | x | x | x | x | x |   |   | x | x |   |

- **x**: Simple conversion without heights is possible or geocentric coordinates with three axes.  
- **h**: Conversion/transformation requires an ellipsoidal height (ETRS89 or DB_Ref) or a normal height (DHHN2016, GCG2016). (Option `-h 1` or `-h 2` is required)

## Supported Height Systems

1. **Normal heights** (internal calculations are based on the quasigeoid GCG2016)  
2. **Ellipsoidal heights** (ETRS89 or DB_Ref)  

Heights are used for the transformation from **ETRS89** to **DB_REF** and vice versa. Normal heights are internally transformed into ellipsoidal heights for this purpose. The **German Combined Quasigeoid (GCG2016)** is used for this ([BKG](https://www.bkg.bund.de/), [CC BY 4.0](https://creativecommons.org/licenses/by/4.0/)). Ellipsoidal heights are converted to the respective target datum during datum transformations. For transformations from **DB_Ref** to **ETRS89** with normal heights, ellipsoidal heights for **DB_Ref** are iteratively approximated, as the GCG2016 model is based on the ETRS89 datum.

## Transformations

The transformation into and out of the EGBT22 datum is performed with the following transformation parameters (ETRS89/DREF91, ETRS89/CZEPOS).
```
ETRS89/DREF91(R2016) -> EGBT22: tx=-0.0028 ty=-0.0023 tz=0.0029
ETRS89/CZ            -> EGBT22: tx=-0.0028 ty=-0.0023 tz=0.0029
```
The transformation from **ETRS89** to **DB_REF** and vice versa uses the official transformation parameters from DB Guideline 883.9010 (different for both directions).  

## Conversions

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

- Simple conversion from **ETRS89 EGBT_LDP Dresden-Prague** to **UTM33**:    
```
-s 1 -t 4
```
- Conversion and transformation from **EGBT_LDP Dresden-Prague** to **DB_REF GK5** with normal heights: 
```
-s 1 -t 9 -h 1
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

The input and output files are specified after the options, first the path to the input file, then the path to the output file. If the output file is omitted, the output is written to the console.

**Examples:**

Transformation with normal heights and high precision:  
```
egbt22trans.exe -s 1 -t 5 -h 1 -l 12 input.txt output.txt
```
