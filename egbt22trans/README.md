# egbt22trans

`egbt22trans.exe` is a program for converting coordinates between different coordinate systems. Conversions are possible between the coordinate systems defined below. The coordinates to be converted must be provided in column-based text files, using a period as the decimal separator. All columns remain as read, only the coordinate axes x, y, z or latitude, longitude, height or easting, northing, height are converted. The unit of the coordinates is meters and decimal degrees for geodetic coordinates.

The coordinates are checked to see if they are within the limits defined for the EGBT22 system (see table). If this is not the case, a message is displayed on the console for each affected coordinate.

| ETRS89/EGBT22          | Minimum | Maximum |
|:-----------------------|:--------|:--------|
| Latitude [°]           | 50.640  | 51.018  |
| Longitude [°]          | 13.831  | 13.974  |
| Ellipsoidal height [m] | 120     | 320     |

The program is open source and can be found at [https://github.com/dd-bim/egbt22trans.git](https://github.com/dd-bim/egbt22trans.git).

## Supported Coordinate Systems (CRS)

|ID | Coordinate System (CRS)          | Coordinate Axes             |
|:--|:---------------------------------|:----------------------------|
|1. |**EGBT22 EGBT_LDP Dresden-Prague**| easting, northing, height   |
|2. |**EGBT22 geographic**             | latitude, longitude, height |
|3. |**EGBT22 geocentric**             | X, Y. Z                     |
|4. |**ETRS89/DREF91 UTM33**           | easting, northing, height   |
|5. |**ETRS89/DREF91 geographic**      | latitude, longitude, height |
|6. |**ETRS89/DREF91 geocentric**      | X, Y. Z                     |
|7. |**ETRS89/CZ geographic**          | latitude, longitude, height |
|8. |**ETRS89/CZ geocentric**          | X, Y. Z                     |
|9. |**DB_Ref GK5**                    | easting, northing, height   |
|10.|**DB_Ref geographic**             | latitude, longitude, height |
|11.|**DB_Ref geocentric**             | X, Y. Z                     |

## Supported Conversions / Transformations

|                                   |    | 1  | 2  | 3  | 4  | 5  | 6  | 7  | 8  | 9  | 10 | 11 |
| :---------------------------------|---:|:--:|:--:|:--:|:--:|:--:|:--:|:--:|:--:|:--:|:--:|:--:|
| **EGBT22 EGBT_LDP Dresden-Prague**| 1  |    | 2D | 3D | 3D | 3D | 3D | 3D | 3D | 3D | 3D | 3D |
| **EGBT22 geographic**             | 2  | 2D |    | 3D | 3D | 3D | 3D | 3D | 3D | 3D | 3D | 3D |
| **EGBT22 geocentric**             | 3  | 3D | 3D |    | 3D | 3D | 3D | 3D | 3D | 3D | 3D | 3D |
| **ETRS89/DREF91 UTM33**           | 4  | 3D | 3D | 3D |    | 2D | 3D |    |    | 3D | 3D | 3D |
| **ETRS89/DREF91 geographic**      | 5  | 3D | 3D | 3D | 2D |    | 3D |    |    | 3D | 3D | 3D |
| **ETRS89/DREF91 geocentric**      | 6  | 3D | 3D | 3D | 3D | 3D |    |    |    | 3D | 3D | 3D |
| **ETRS89/CZ geographic**          | 7  | 3D | 3D | 3D |    |    |    |    | 3D |    |    |    |
| **ETRS89/CZ geocentric**          | 8  | 3D | 3D | 3D |    |    |    | 3D |    |    |    |    |
| **DB_Ref GK5**                    | 9  | 3D | 3D | 3D | 3D | 3D | 3D |    |    |    | 2D | 3D |
| **DB_Ref geographic**             | 10 | 3D | 3D | 3D | 3D | 3D | 3D |    |    | 2D |    | 3D |
| **DB_Ref geocentric**             | 11 | 3D | 3D | 3D | 3D | 3D | 3D |    |    | 3D | 3D |    |

- **2D**: Simple conversion without heights.  
- **3D**: Conversion/transformation requires a normal height (DHHN2016, GCG2016) (Option `-h 1` is required), an ellipsoidal height (EGBT22, ETRS89 or DB_Ref) (Option `-h 2` is required) or geocentric coordinates with three axes.

## Supported Height Systems

1. **Normal heights** (internal calculations are based on the quasigeoid GCG2016)  
2. **Ellipsoidal heights** (ETRS89 or DB_Ref)  

Heights are used for the transformation from **ETRS89** to **DB_REF** and vice versa. Normal heights are internally transformed into ellipsoidal heights for this purpose. The **German Combined Quasigeoid (GCG2016)** is used for this ([BKG](https://www.bkg.bund.de/), [CC BY 4.0](https://creativecommons.org/licenses/by/4.0/)). For the **DB_Ref** datum, the ellipsoidal heights are calculated iteratively from the GCG2016 model. The ellipsoidal heights are converted to the respective target datum during datum transformations.

If the input heights are normal heights, the output will be unchanged (except for geocentric output coordinates).

## Transformations

The transformation into and out of the **EGBT22** datum is performed with the following transformation parameters (**ETRS89/DREF91**, **ETRS89/CZEPOS**).

|                    |         | EGBT22 |      |
|  Source system     | tx      | ty     | tz   |
|:-------------------|:--------|:-------|:-----|
|ETRS89/DREF91(R2016)| -0.0028 |-0.0023 |0.0029|
|ETRS89/CZ           | -0.0028 |-0.0023 |0.0029|

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

- Simple conversion from **ETRS89 EGBT_LDP Dresden-Prague** to **EGBT22 geographisch 3D B/L**:    
```
-s 1 -t 2
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
