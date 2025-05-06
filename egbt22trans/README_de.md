# egbt22trans

`egbt22trans.exe` ist ein Programm zur Umrechnung von Koordinaten zwischen verschiedenen Koordinatensystemen. Es sind Umrechnungen zwischen den nachfolgend definierten Koordinatensystemen möglich. Die umzurechnenden Koordinaten müssen in spaltenbasierten Textdateien vorliegen, als Dezimaltrennzeichen ist der Punkt zu verwenden. Alle Spalten bleiben wie eingelesen, nur die Koordinatenachsen `x` und `y` [oder `z`] werden umgerechnet.

Das Programm ist OpenSource und unter [https://github.com/dd-bim/egbt22trans.git](https://github.com/dd-bim/egbt22trans.git) zu finden.

## Unterstützte Koordinatensysteme (CRS)

1. **ETRS89 EGBT_LDP Dresden-Prag**  
2. **ETRS89 UTM33** (EPSG:25833)  
3. **ETRS89 geographisch 3D B/L** (EPSG:4258)  
4. **ETRS89 kartesisch 3D geozentrisch** (EPSG:4936)  
5. **DB_Ref GK5** (EPSG:5685)  
6. **DB_Ref geographisch 3D B/L** (EPSG:5681)  
7. **DB_Ref kartesisch 3D geozentrisch** (EPSG:4936)  

## Unterstützte Umrechnungen / Transformationen

|   | 1 | 2 | 3 | 4 | 5 | 6 | 7 |
|---|---|---|---|---|---|---|---|
| 1 |   | x | x | h | h | h | h |
| 2 | x |   | x | h | h | h | h |
| 3 | x | x |   | h | h | h | h |
| 4 | x | x | x |   | x | x | x |
| 5 | h | h | h | h |   | x | h |
| 6 | h | h | h | h | x |   | h |
| 7 | x | x | x | x | x | x |   |

- **x**: Einfache Umrechnung ohne Höhen möglich.  
- **h**: Umrechnung/Transformation benötigt eine ellipsoidische Höhe (ETRS89 oder DB_Ref) oder eine Normalhöhe (DHHN2016, GCG2016). (Option `-h 1` oder `-h 2` ist erforderlich)

## Unterstützte Höhensysteme

1. **Normalhöhen** (interne Berechnungen basieren auf dem Quasigeoid GCG2016)  
2. **Ellipsoidische Höhen** (ETRS89 oder DB_Ref)  

Die Höhen werden für die Transformation von **ETRS89** zu **DB_REF** und umgekehrt verwendet. Normalhöhen werden dafür intern in ellipsoidische Höhen transformiert. Dafür wird das **German Combined Quasigeoid (GCG2016)** verwendet ([BKG](https://www.bkg.bund.de/), [CC BY 4.0](https://creativecommons.org/licenses/by/4.0/)). Die ellipsoidischen Höhen werden bei Datumstransformationen in das jeweilige Zieldatum umgerechnet. Bei Transformationen von **DB_Ref** nach **ETRS89** mit Normalhöhen werden die ellipsoidischen Höhen für **DB_Ref** iterativ angenähert, da das GCG2016-Modell auf dem ETRS89-Datum basiert.

Die Transformation von **ETRS89** nach **DB_REF** und umgekehrt verwendet die offiziellen Transformationsparameter (unterschiedlich für beide Richtungen).  
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

- Einfache Umrechnung von **ETRS89 EGBT_LDP Dresden-Prag** nach **UTM33**:  
```
-s 1 -t 2
```
- Umrechnung und Transformation von **EGBT_LDP Dresden-Prag** nach **DB_REF GK5** mit Normalhöhen: 
```
-s 1 -t 5 -h 1
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
- `-l, --latlon`: Anzahl der Nachkommastellen bei Breitengrad- oder Längengrad in Grad  
  **Standard:** 10  

## Datumstransformation für EGBT22

Neben der normalen Umrechnung und Transformation ist auch eine spezielle Datumstransformation für das **EGBT22-Datum** möglich. Damit können Koordinaten in den Systemen 3 und 4 (Geographisch 3D mit ellipsoidischen Höhen / ETRS89 Kartesisch 3D geozentrisch) vom oder zum **EGBT22-Datum** transformiert werden. Dies ist normalerweise nur für präzise GNSS-Beobachtungen erforderlich.

Das Quell- und Zielsystem muss nicht mit den Optionen `-s` und `-t` angegeben werden. Für diese Transformation wird die Option `-e` bzw. `--egbt22` verwendet. Danach muss die gewünschte Transformation mit einer der folgenden Zahlen definiert werden:

1. **Sapos nach EGBT22** (geozentrisch, ID 4)  
2. **EGBT22 nach Sapos** (geozentrisch, ID 4)  
3. **Czepos nach EGBT22** (geozentrisch, ID 4)  
4. **EGBT22 nach Czepos** (geozentrisch, ID 4)  
5. **Sapos nach EGBT22** (geographisch, ID 3 mit ellipsoidischen Höhen)  
6. **EGBT22 nach Sapos** (geographisch, ID 3 mit ellipsoidischen Höhen)  
7. **Czepos nach EGBT22** (geographisch, ID 3 mit ellipsoidischen Höhen)  
8. **EGBT22 nach Czepos** (geographisch, ID 3 mit ellipsoidischen Höhen)  

Die Eingabe- und Ausgabedateien werden nach den Optionen angegeben, zuerst der Pfad zur Eingabedatei, dann der Pfad zur Ausgabedatei. Wenn die Ausgabedatei weggelassen wird, erfolgt die Ausgabe in der Konsole.

**Beispiele:**

- Transformation mit Normalhöhen und hoher Präzision:  
```
egbt22trans.exe -s 1 -t 3 -h 1 -l 12 input.txt output.txt
```
- Datumstransformation von Sapos nach EGBT22 mit geozentrischen Koordinaten: 
```
egbt22trans.exe -e 1 input.txt output.txt
```
