# egbt22trans

`egbt22trans.exe` ist ein Programm zur Umrechnung von Koordinaten zwischen verschiedenen Koordinatensystemen. Es sind Umrechnungen zwischen den nachfolgend definierten Koordinatensystemen möglich. Die umzurechnenden Koordinaten müssen in spaltenbasierten Textdateien vorliegen, als Dezimaltrennzeichen ist der Punkt zu verwenden. Alle Spalten bleiben wie eingelesen, nur die Koordinatenachsen x und y [oder z] (bzw. Breitengrad,Längengrad,[Höhe] oder Rechtswert, Hochwert,[Höhe]) werden umgerechnet. Die Einheit der Koordinaten ist Meter und bei geodätischen Koordinaten Dezimalgrad.

Das Programm ist OpenSource und unter [https://github.com/dd-bim/egbt22trans.git](https://github.com/dd-bim/egbt22trans.git) zu finden.

## Unterstützte Koordinatensysteme (CRS)

1. **EGBT22 EGBT_LDP Dresden-Prag**  
2. **EGBT22 geographisch 3D B/L** 
3. **EGBT22 kartesisch 3D geozentrisch** 
4. **ETRS89/DREF91 UTM33**
5. **ETRS89/DREF91 geographisch 3D B/L**
6. **ETRS89/DREF91 kartesisch 3D geozentrisch**
7. **ETRS89/CZ geographisch 3D B/L**
8. **ETRS89/CZ kartesisch 3D geozentrisch**
9. **DB_Ref GK5**
10. **DB_Ref geographisch 3D B/L**
11. **DB_Ref kartesisch 3D geozentrisch** 

## Unterstützte Umrechnungen / Transformationen

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

- **x**: Einfache Umrechnung ohne Höhen möglich bzw. geozentrische Koordinaten mit drei Achsen.  
- **h**: Umrechnung/Transformation benötigt eine ellipsoidische Höhe (ETRS89 oder DB_Ref) oder eine Normalhöhe (DHHN2016, GCG2016). (Option `-h 1` oder `-h 2` ist erforderlich)

## Unterstützte Höhensysteme

1. **Normalhöhen** (interne Berechnungen basieren auf dem Quasigeoid GCG2016)  
2. **Ellipsoidische Höhen** (ETRS89 oder DB_Ref)  

Die Höhen werden für die Transformation von **ETRS89** zu **DB_REF** und umgekehrt verwendet. Normalhöhen werden dafür intern in ellipsoidische Höhen transformiert. Dafür wird das **German Combined Quasigeoid (GCG2016)** verwendet ([BKG](https://www.bkg.bund.de/), [CC BY 4.0](https://creativecommons.org/licenses/by/4.0/)). Die ellipsoidischen Höhen werden bei Datumstransformationen in das jeweilige Zieldatum umgerechnet. Bei Transformationen von **DB_Ref** nach **ETRS89** mit Normalhöhen werden die ellipsoidischen Höhen für **DB_Ref** iterativ angenähert, da das GCG2016-Modell auf dem ETRS89-Datum basiert.

## Transformationen

Die Transformation in das EGBT22 Datum und zurück erfolgt mit den folgenden Transformationsparametern (ETRS89/DREF91, ETRS89/CZEPOS).
```
ETRS89/DREF91(R2016) -> EGBT22: tx=-0.0028 ty=-0.0023 tz=0.0029
ETRS89/CZ            -> EGBT22: tx=-0.0028 ty=-0.0023 tz=0.0029
```
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

- Einfache Umrechnung von **ETRS89 EGBT_LDP Dresden-Prag** nach **UTM33**:  
```
-s 1 -t 4
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
- `-l, --latlon`: Anzahl der Nachkommastellen bei Breitengrad- oder Längengrad in Grad  
  **Standard:** 10  

Die Eingabe- und Ausgabedateien werden nach den Optionen angegeben, zuerst der Pfad zur Eingabedatei, dann der Pfad zur Ausgabedatei. Wenn die Ausgabedatei weggelassen wird, erfolgt die Ausgabe in der Konsole.

**Beispiel:**

Transformation mit Normalhöhen und hoher Präzision:  
```
egbt22trans.exe -s 1 -t 5 -h 1 -l 12 input.txt output.txt
```
