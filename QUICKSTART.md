# Quick Start Guide - Layout Designer

Schnellstart-Anleitung für Layout Designer - von der Installation bis zum ersten Layout in 5 Minuten!

## 🚀 Installation (2 Minuten)

### Voraussetzungen prüfen
```bash
# .NET SDK prüfen
dotnet --version
# Sollte 8.0.x oder höher sein
```

Falls nicht installiert: [.NET 8.0 SDK herunterladen](https://dotnet.microsoft.com/download/dotnet/8.0)

### Projekt öffnen

**Option 1: Visual Studio 2022**
1. `LayoutDesigner.sln` doppelklicken
2. Warten bis NuGet-Pakete wiederhergestellt sind
3. `F5` drücken

**Option 2: Kommandozeile**
```bash
cd canvas
dotnet restore
dotnet run --project LayoutDesigner
```

**Option 3: JetBrains Rider**
1. "Open Solution" → `LayoutDesigner.sln`
2. Build → Run

## 📝 Erstes Layout erstellen (3 Minuten)

### Schritt 1: Beispiel-Layout öffnen
1. Anwendung starten
2. `Datei` → `Öffnen` (oder `Strg+O`)
3. Navigieren zu `Examples/WelcomeLayout.layout`
4. Öffnen

**Ergebnis**: Sie sehen ein vorgefertigtes Layout mit verschiedenen Element-Typen.

### Schritt 2: Elemente erkunden
- **Klicken** Sie auf verschiedene Elemente
- **Rechtes Panel** zeigt Eigenschaften des ausgewählten Elements
- **Ziehen** Sie Elemente, um sie zu verschieben
- **Griffe** an den Ecken zum Skalieren
- **Grüner Griff** oben zum Rotieren

### Schritt 3: Eigenes Element hinzufügen

#### Text hinzufügen
1. Linkes Panel: "➕ Add Text" klicken
2. Rechtes Panel: Text ändern zu "Mein erstes Element"
3. Font-Größe auf 24 setzen
4. Farbe ändern (z.B. `#FFFF0000` für Rot)

#### Form hinzufügen
1. Linkes Panel: "⬜ Rectangle" klicken
2. Rechtes Panel: Füllfarbe auf `#FF0078D7` setzen
3. Element verschieben und skalieren

### Schritt 4: Speichern
1. `Datei` → `Speichern unter` (oder `Strg+Shift+S`)
2. Namen eingeben: "MeinErstesLayout"
3. Speichern

## ⌨️ Wichtigste Tastenkombinationen

### Datei-Operationen
- `Strg+N` - Neues Layout
- `Strg+O` - Layout öffnen
- `Strg+S` - Speichern
- `Strg+Shift+S` - Speichern unter

### Bearbeiten
- `Strg+Z` - Rückgängig
- `Strg+Y` - Wiederherstellen
- `Strg+D` - Duplizieren
- `Strg+A` - Alle auswählen
- `Entf` - Löschen

### Ansicht
- `Strg +` - Zoom vergrößern
- `Strg -` - Zoom verkleinern
- `Strg 0` - Zoom zurücksetzen

### Mehrfachauswahl
- `Strg+Klick` - Element zur Auswahl hinzufügen
- `Strg+A` - Alle auswählen

## 🎨 Element-Typen Übersicht

### 📝 Text
```
Verwendung: Überschriften, Beschreibungen, Labels
Eigenschaften: Font, Größe, Farbe, Ausrichtung
Tipp: Nutzen Sie Bold für Überschriften
```

### 🖼️ Bild
```
Verwendung: Logos, Fotos, Icons
Eigenschaften: Pfad, Skalierungsmodus
Tipp: "Uniform" behält Seitenverhältnis bei
```

### 🔷 Formen
```
Verfügbar: Rechteck, Ellipse, Linie, Abgerundetes Rechteck
Verwendung: Rahmen, Trennlinien, Hintergründe
Tipp: Opacity auf 0.1-0.3 für subtile Hintergründe
```

### 📱 QR-Code
```
Verwendung: Links, Kontaktdaten
Eigenschaften: Inhalt, Farben, Fehlerkorrektur
Tipp: Höhere Fehlerkorrektur = besser lesbar bei Beschädigung
```

### 🕐 Dynamisches Feld
```
Verwendung: Datum/Zeit-Anzeigen
Format-Beispiele:
  - "dd.MM.yyyy" → 15.11.2025
  - "HH:mm:ss" → 14:30:45
  - "dddd, dd. MMMM yyyy" → Freitag, 15. November 2025
```

## 🎯 Tipps & Tricks

### Layout-Design
1. **Raster verwenden**: `Ansicht` → `Show Grid` aktivieren
2. **Snap-to-Grid**: Für präzise Ausrichtung aktivieren
3. **Ebenen nutzen**: Wichtige Elemente nach vorne (`Strg+Shift+F`)

### Arbeitsablauf
1. **Grobe Platzierung** zuerst, dann Feintuning
2. **Duplizieren** statt neu erstellen (`Strg+D`)
3. **Häufig speichern** (`Strg+S`)
4. **Undo nutzen** bei Fehlern (`Strg+Z`)

### Property Panel
- **Position**: X/Y-Koordinaten direkt eingeben
- **Größe**: Width/Height für exakte Maße
- **Rotation**: Slider oder direkte Eingabe
- **Opacity**: 0.0 = unsichtbar, 1.0 = komplett sichtbar

### Mehrfachbearbeitung
1. Mehrere Elemente auswählen (`Strg+Klick`)
2. "Multiple elements selected" erscheint im Property Panel
3. Alle verschieben sich gemeinsam
4. Layer-Befehle wirken auf alle

## 🔧 Häufige Aufgaben

### Layout für Präsentation erstellen
1. Canvas-Größe auf 1920x1080 setzen
2. Hintergrund-Rechteck: Vollflächig, Farbe wählen
3. Titel-Text: Groß (72pt), Fett, Mittig
4. Inhalt: Mehrere Boxen mit Text/Bildern
5. Export als PNG für Präsentation

### Raumbeschriftung erstellen
1. Canvas-Größe = Druckformat (z.B. A4: 2480x3508 Pixel @ 300 DPI)
2. Raum-Nummer: Großer Text, zentriert
3. QR-Code: Mit Raum-URL oder Info
4. Zusatzinfo: Kleinerer Text
5. Export als PNG/JPG zum Drucken

### Informationstafel gestalten
1. Grid aktivieren für Ausrichtung
2. Header-Bereich: Rechteck + Titel
3. Inhalts-Bereiche: Mehrere Boxen
4. Footer: Datum/Zeit als Dynamic Field
5. QR-Code: Für weitere Infos

## 📤 Export

### Als Bild exportieren
1. Layout fertigstellen
2. `Datei` → `Export` → `Export as PNG`
3. Dateinamen wählen
4. Speichern

**Unterstützte Formate**:
- PNG: Verlustfrei, transparenter Hintergrund möglich
- JPG: Kleiner, für Fotos geeignet

### Als Layout teilen
1. `Datei` → `Speichern unter`
2. `.layout`-Datei kann mit anderen geteilt werden
3. Empfänger öffnet mit Layout Designer

## ❓ Fehlerbehebung

### Element lässt sich nicht verschieben
- **Lösung**: Prüfen Sie "IsLocked" im Property Panel

### Raster wird nicht angezeigt
- **Lösung**: `Ansicht` → `Show Grid` aktivieren

### Undo funktioniert nicht
- **Lösung**: Undo-Stack hat max. 100 Schritte
- Ältere Aktionen werden automatisch entfernt

### Export-Funktion zeigt Meldung
- **Status**: Export ist vorbereitet, aber noch nicht vollständig implementiert
- **Workaround**: Screenshot verwenden (Windows+Shift+S)

### QR-Code wird nicht angezeigt
- **Status**: QR-Rendering im Canvas noch in Entwicklung
- Der QR-Code wird beim Export korrekt generiert

## 🎓 Nächste Schritte

### Anfänger
1. ✅ Beispiel-Layout öffnen und erkunden
2. ✅ Eigene Elemente hinzufügen
3. ✅ Speichern und wieder öffnen
4. → Verschiedene Element-Typen ausprobieren
5. → Erste eigene Designs erstellen

### Fortgeschritten
1. → Komplexe Layouts mit vielen Elementen
2. → Ebenen-Management nutzen
3. → Templates erstellen für Wiederverwendung
4. → Dynamische Felder für variable Inhalte

### Profi
1. → Eigene Element-Typen entwickeln (Code)
2. → Services erweitern
3. → Plugin-System nutzen (zukünftig)

## 📚 Weitere Ressourcen

- **README.md**: Vollständige Feature-Liste
- **ARCHITECTURE.md**: Technische Dokumentation
- **CHANGELOG.md**: Versionshistorie

## 💬 Hilfe & Support

- **GitHub Issues**: Bugs und Feature-Requests
- **Dokumentation**: Alle .md-Dateien im Projekt
- **Beispiele**: `Examples/`-Ordner

---

**Viel Erfolg mit Layout Designer!** 🎉

Haben Sie Ihr erstes Layout erstellt? Großartig! Teilen Sie es gerne und erkunden Sie weitere Features.
