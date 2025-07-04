# egbt22trans

`egbt22trans.exe` ist ein Programm zur Umrechnung von Koordinaten zwischen verschiedenen Koordinatensystemen. Es sind Umrechnungen zwischen den nachfolgend definierten Koordinatensystemen möglich. Die umzurechnenden Koordinaten müssen in spaltenbasierten Textdateien vorliegen, als Dezimaltrennzeichen ist der Punkt zu verwenden. Alle Spalten bleiben wie eingelesen, nur die Koordinatenachsen x, y, z bzw. Breitengrad, Längengrad, Höhe oder Rechtswert, Hochwert, Höhe werden umgerechnet. Die Einheit der Koordinaten ist Meter und bei geodätischen Koordinaten Dezimalgrad.

Die Koordinaten werden geprüft ob sie innerhalb der für das System EGBT22 definierten Grenzen (siehe Tabelle) liegen. Sollte das nicht gegeben sein, wird für jede betroffene Koordinate eine Meldung auf der Konsole ausgegeben.

| ETRS89/EGBT22           | Minimum | Maximum |
|:------------------------|:--------|:--------|
| Breitengrad [°]         | 50,640  | 51,018  |
| Längengrad [°]          | 13,831  | 13,974  |
| Ellipsoidische Höhe [m] | 120     | 320     |


Das Programm ist OpenSource und unter [https://github.com/dd-bim/egbt22trans.git](https://github.com/dd-bim/egbt22trans.git) zu finden.

## Unterstützte Koordinatensysteme (CRS)

|ID | Koordinatensystem (CRS)        | Koordinatenachsen             |
|:--|:-------------------------------|:------------------------------|
|1. |**EGBT22 EGBT_LDP Dresden-Prag**| Rechstwert, Hochwert, Höhe    |
|2. |**EGBT22 geographisch**         | Breitengrad, Längengrad, Höhe |
|3. |**EGBT22 geozentrisch**         | X, Y. Z                       |
|4. |**ETRS89/DREF91 UTM33**         | Easting, Northing, Höhe       |
|5. |**ETRS89/DREF91 geographisch**  | Breitengrad, Längengrad, Höhe |
|6. |**ETRS89/DREF91 geozentrisch**  | X, Y. Z                       |
|7. |**ETRS89/CZ geographisch**      | Breitengrad, Längengrad, Höhe |
|8. |**ETRS89/CZ geozentrisch**      | X, Y. Z                       |
|9. |**DB_Ref GK5**                  | Rechstwert, Hochwert, Höhe    |
|10.|**DB_Ref geographisch**         | Breitengrad, Längengrad, Höhe |
|11.|**DB_Ref geozentrisch**         | X, Y. Z                       | 

## Unterstützte Umrechnungen / Transformationen

|                                  |    | 1  | 2  | 3  | 4  | 5  | 6  | 7  | 8  | 9  | 10 | 11 |
| :--------------------------------|---:|:--:|:--:|:--:|:--:|:--:|:--:|:--:|:--:|:--:|:--:|:--:|
| **EGBT22 EGBT_LDP Dresden-Prag** | 1  |    | 2D | 3D | 3D | 3D | 3D | 3D | 3D | 3D | 3D | 3D |
| **EGBT22 geographisch**          | 2  | 2D |    | 3D | 3D | 3D | 3D | 3D | 3D | 3D | 3D | 3D |
| **EGBT22 geozentrisch**          | 3  | 3D | 3D |    | 3D | 3D | 3D | 3D | 3D | 3D | 3D | 3D |
| **ETRS89/DREF91 UTM33**          | 4  | 3D | 3D | 3D |    | 2D | 3D |    |    | 3D | 3D | 3D |
| **ETRS89/DREF91 geographisch**   | 5  | 3D | 3D | 3D | 2D |    | 3D |    |    | 3D | 3D | 3D |
| **ETRS89/DREF91 geozentrisch**   | 6  | 3D | 3D | 3D | 3D | 3D |    |    |    | 3D | 3D | 3D |
| **ETRS89/CZ geographisch**       | 7  | 3D | 3D | 3D |    |    |    |    | 3D |    |    |    |
| **ETRS89/CZ geozentrisch**       | 8  | 3D | 3D | 3D |    |    |    | 3D |    |    |    |    |
| **DB_Ref GK5**                   | 9  | 3D | 3D | 3D | 3D | 3D | 3D |    |    |    | 2D | 3D |
| **DB_Ref geographisch**          | 10 | 3D | 3D | 3D | 3D | 3D | 3D |    |    | 2D |    | 3D |
| **DB_Ref geozentrisch**          | 11 | 3D | 3D | 3D | 3D | 3D | 3D |    |    | 3D | 3D |    |

- **2D**: Einfache Umrechnung ohne Höhen möglich.  
- **3D**: Umrechnung/Transformation benötigt eine Normalhöhe (DHHN2016, GCG2016) (Option `-h 1` ist erforderlich),  eine ellipsoidische Höhe (EGBT22, ETRS89 oder DB_Ref) (Option `-h 2` ist erforderlich) oder geozentrische Koordinaten mit drei Achsen.

## Unterstützte Höhensysteme

1. **Normalhöhen** (interne Berechnungen basieren auf dem Quasigeoid GCG2016)  
2. **Ellipsoidische Höhen** (ETRS89 oder DB_Ref)  

Die Höhen werden für die Transformation zwischen den Systemen **EGBT22**, **ETRS89** und **DB_REF** verwendet. Normalhöhen werden dafür intern in ellipsoidische Höhen transformiert. Dafür wird das **German Combined Quasigeoid (GCG2016)** verwendet ([BKG](https://www.bkg.bund.de/), [CC BY 4.0](https://creativecommons.org/licenses/by/4.0/)). Für das **DB_Ref** Datum werden die  ellipsoidischen Höhen iterativ aus dem GCG2016-Modell berechnet. Die ellipsoidischen Höhen werden bei Datumstransformationen in das jeweilige Zieldatum umgerechnet.

Sind die Eingabehöhen Normalhöhen, dann werden diese bei der Ausgabe unverändert ausgegeben (außer bei geozentrischen Ausgabekoordinaten).

## Transformationen

Die Transformation in das **EGBT22** Datum und zurück erfolgt mit den folgenden Transformationsparametern (**ETRS89/DREF91**, **ETRS89/CZEPOS**).

|                    |         | EGBT22 |      |
|  Quellsystem       | tx      | ty     | tz   |
|:-------------------|:--------|:-------|:-----|
|ETRS89/DREF91(R2016)| -0.0028 |-0.0023 |0.0029|
|ETRS89/CZ           | -0.0028 |-0.0023 |0.0029|

Die Transformation von **ETRS89** nach **DB_REF** und umgekehrt verwendet die offiziellen Transformationsparameter aus der DB-Richtlinie 883.9010 (unterschiedlich für beide Richtungen).  

## Umrechnungen
Die Umrechnungsberechnungen (geodätisch, geozentrisch, Transversal-Mercator) werden mit der Bibliothek [GeographicLib.Net](https://github.com/noelex/GeographicLib.NET) durchgeführt.

## Kommandozeilenoptionen

Das Konsolenprogramm `egbt22trans.exe` kann mit den nachfolgenden Optionen konfiguriert werden:

### Allgemeine Optionen

- `--help`: Zeigt die Hilfe an.

### Optionen für Koordinatenumrechnung und Transformation

- `-s, --source`: Quellsystem (CRS-Index)  
- `-t, --target`: Zielsystem (CRS-Index)  
- `-h, --height`: Höhensystem (1: Normalhöhen, 2: ellipsoidische Höhen)  

**Beispiele:**

- Einfache Umrechnung von **EGBT22 EGBT_LDP Dresden-Prag** nach **EGBT22 geographisch 3D B/L**:  
```
-s 1 -t 2
```
- Umrechnung und Transformation von **EGBT_LDP Dresden-Prag** nach **DB_REF GK5** mit Normalhöhen: 
```
-s 1 -t 9 -h 1
```

### Zusätzliche Optionen

- `-d, --delimiter`: Trennzeichen der Koordinatenspalten (`s`=Leerzeichen, `t`=Tabulator, `,`=Komma, ...)  
  **Standard:** Leerzeichen  
- `-x, --xaxis`: Spaltenindex der x-Achse (1. definierte Koordinatenachse)  
  **Standard:** 2  
- `-y, --yaxis`: Spaltenindex der y-Achse (2. definierte Koordinatenachse)  
  **Standard:** 3  
- `-z, --zaxis`: Spaltenindex der z-Achse (Z-Achse, Normalhöhe oder ellipsoidische Höhe)  
  **Standard:** 4 (wird ignoriert, falls nicht erforderlich)  
- `-p, --precision`: Anzahl der Nachkommastellen in der Ausgabe  
  **Standard:** 4  
- `-l, --latlon`: Anzahl der Nachkommastellen in der Ausgabe bei Breitengrad- oder Längengrad in Grad  
  **Standard:** 10  

Die Eingabe- und Ausgabedateien werden nach den Optionen angegeben, zuerst der Pfad zur Eingabedatei, dann der Pfad zur Ausgabedatei. Wenn die Ausgabedatei weggelassen wird, erfolgt die Ausgabe in der Konsole.

**Beispiel:**

Transformation mit Normalhöhen und hoher Präzision:  
```
egbt22trans.exe -s 1 -t 5 -h 1 -l 12 input.txt output.txt
```
