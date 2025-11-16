# Complete Features Guide - Layout Designer

Umfassender Leitfaden zu allen Features des WPF Layout Designers.

## 📑 Inhaltsverzeichnis

1. [Element-Typen](#element-typen)
2. [Canvas-Editor](#canvas-editor)
3. [Ausrichtungs-Tools](#ausrichtungs-tools)
4. [Layer-Management](#layer-management)
5. [Undo/Redo-System](#undoredo-system)
6. [Grid & Snapping](#grid--snapping)
7. [Zoom & Navigation](#zoom--navigation)
8. [Speichern & Laden](#speichern--laden)
9. [Export-Funktionen](#export-funktionen)
10. [Tastenkombinationen](#tastenkombinationen)

---

## Element-Typen

### 📝 Text Element

**Verwendung**: Überschriften, Beschreibungen, Labels, Fließtext

**Eigenschaften**:
- **Text**: Beliebiger Text (mehrzeilig möglich)
- **Font-Familie**: Arial, Calibri, Times New Roman, Verdana, Segoe UI
- **Font-Größe**: Beliebige Punktgröße
- **Stil**: Bold, Italic, Underline (kombinierbar)
- **Farben**:
  - Vordergrundfarbe (Text)
  - Hintergrundfarbe (optional transparent)
- **Ausrichtung**:
  - Horizontal: Links, Mitte, Rechts
  - Vertikal: Oben, Mitte, Unten

**Best Practices**:
```
Überschriften: 48-96pt, Bold
Body-Text: 16-24pt, Normal
Labels: 12-16pt, Normal oder Bold
Fließtext: TextWrapping automatisch
```

**Anwendungsbeispiele**:
- Konferenzraum-Namen
- Informationstafeln
- Wegweiser
- Beschreibungen

---

### 🖼️ Image Element

**Verwendung**: Logos, Fotos, Icons, Illustrationen

**Eigenschaften**:
- **Image Path**: Dateipfad (absolut oder relativ)
- **Stretch Mode**:
  - `None`: Originalgröße
  - `Fill`: Gesamten Bereich füllen (kann verzerren)
  - `Uniform`: Seitenverhältnis beibehalten, in Bereich einpassen
  - `UniformToFill`: Seitenverhältnis beibehalten, Bereich füllen (kann abschneiden)
- **Maintain Aspect Ratio**: Checkbox für Seitenverhältnis

**Best Practices**:
```
Logos: Uniform, PNG mit Transparenz
Fotos: UniformToFill für Vollflächig
Icons: None oder Uniform
Hintergründe: Fill
```

**Unterstützte Formate**:
- PNG (empfohlen für Transparenz)
- JPG/JPEG
- BMP
- GIF

---

### 🔷 Shape Element

**Verwendung**: Rahmen, Hintergründe, Trennlinien, geometrische Designs

**Shape-Typen**:

#### Rectangle (Rechteck)
- Standardform für Boxen und Container
- Unterstützt Füllfarbe und Rahmen

#### Ellipse (Ellipse/Kreis)
- Für runde Elemente
- Kreis: Width = Height

#### Line (Linie)
- Trennlinien, Verbindungen
- Height = StrokeThickness für horizontale Linien

#### Rounded Rectangle (Abgerundetes Rechteck)
- Moderne, weiche Optik
- `CornerRadius` für Rundung (0 = eckig, höher = runder)

**Eigenschaften**:
- **Fill Color**: Füllfarbe (kann transparent sein: `#00FFFFFF`)
- **Stroke Color**: Rahmenfarbe
- **Stroke Thickness**: Rahmenstärke in Pixeln
- **Corner Radius**: Eckenradius (nur Rounded Rectangle)
- **Opacity**: Transparenz (0.0-1.0)

**Design-Tipps**:
```css
Hintergründe: Opacity 0.1-0.3 für Subtilität
Rahmen: 2-3px für normale Boxen, 1px für feine Linien
Farb-Schemas:
  - Primär: #FF0078D7 (Blau)
  - Erfolg: #FF27AE60 (Grün)
  - Warnung: #FFFFC107 (Gelb)
  - Fehler: #FFDC3545 (Rot)
```

---

### 📱 QR Code Element

**Verwendung**: Links, Kontaktdaten, WiFi-Credentials, Booking-Systeme

**Eigenschaften**:
- **Content**: Text/URL zum Kodieren
- **Foreground Color**: QR-Code-Farbe (Standard: Schwarz)
- **Background Color**: Hintergrund (Standard: Weiß)
- **Error Correction Level**: 0-3
  - 0 (Low): ~7% Fehlerkorrektur
  - 1 (Medium): ~15% Fehlerkorrektur
  - 2 (Quartile): ~25% Fehlerkorrektur
  - 3 (High): ~30% Fehlerkorrektur

**Content-Formate**:

```
URLs:
https://example.com
http://example.com/page

WiFi:
WIFI:T:WPA;S:NetworkName;P:Password;;

Email:
mailto:info@example.com

Telefon:
tel:+49123456789

SMS:
smsto:+49123456789:Hello

vCard:
BEGIN:VCARD
VERSION:3.0
FN:John Doe
TEL:+49123456789
EMAIL:john@example.com
END:VCARD
```

**Best Practices**:
```
Größe: Minimum 150x150 px für gute Lesbarkeit
Fehlerkorrektur: Level 2 (Quartile) für Drucke
Kontrast: Dunkler Code auf hellem Hintergrund
Testabstand: 2-3 Meter für große QR-Codes
```

---

### 🕐 Dynamic Field Element

**Verwendung**: Datum, Zeit, dynamische Informationen

**Field Types**:

#### DateTime (Datum und Zeit)
```
Format-Beispiele:
dd.MM.yyyy HH:mm:ss → 15.11.2025 14:30:45
dddd, dd. MMMM yyyy → Freitag, 15. November 2025
dd/MM/yyyy → 15/11/2025
HH:mm → 14:30
```

#### Date (nur Datum)
```
dd.MM.yyyy → 15.11.2025
yyyy-MM-dd → 2025-11-15
MMMM dd, yyyy → November 15, 2025
```

#### Time (nur Zeit)
```
HH:mm:ss → 14:30:45
HH:mm → 14:30
hh:mm tt → 02:30 PM
```

#### Custom (Benutzerdefiniert)
Eigene Format-Strings gemäß .NET DateTime-Format

**Format-Tokens**:
```
dd    - Tag (01-31)
MM    - Monat (01-12)
MMM   - Monat kurz (Jan, Feb, ...)
MMMM  - Monat lang (Januar, Februar, ...)
yyyy  - Jahr vierstellig
yy    - Jahr zweistellig
HH    - Stunde 24h (00-23)
hh    - Stunde 12h (01-12)
mm    - Minute (00-59)
ss    - Sekunde (00-59)
dddd  - Wochentag lang (Montag, ...)
ddd   - Wochentag kurz (Mo, ...)
tt    - AM/PM
```

**Properties**:
- Font-Familie, -Größe, -Stil (wie Text Element)
- Vorder- und Hintergrundfarbe

---

## Canvas-Editor

### Element-Manipulation

#### Hinzufügen
1. **Über Toolbox**: Button klicken → Element erscheint bei (50, 50)
2. Element wird automatisch ausgewählt
3. Properties im rechten Panel anpassen

#### Auswählen
- **Einzelauswahl**: Element anklicken
- **Mehrfachauswahl**: `Strg` + Klick
- **Alle auswählen**: `Strg + A`
- **Abwählen**: Klick auf leeren Canvas-Bereich

#### Verschieben
- **Drag & Drop**: Element ziehen
- **Präzise**: X/Y-Werte im Property Panel eingeben
- **Mit Snap**: Automatisches Einrasten am Grid (wenn aktiviert)
- **Mehrere**: Alle ausgewählten Elemente bewegen sich zusammen

#### Skalieren
- **Resize-Handles**: 8 Griffe (4 Ecken, 4 Kanten)
- **Proportional**: `Shift` halten beim Ziehen (zukünftig)
- **Präzise**: Width/Height-Werte direkt eingeben
- **Minimum**: 10x10 Pixel

#### Rotieren
- **Rotate-Handle**: Grüner Griff oben in der Mitte
- **Frei**: Horizontal ziehen für Rotation
- **Präzise**: Rotation-Wert (0-360°) eingeben
- **Snap**: 15°-Schritte (zukünftig)

#### Duplizieren
- **Strg + D**: Kopie erstellen mit +20px Offset
- **Copy/Paste**: Zukünftig
- **Mehrfach**: Alle ausgewählten Elemente werden dupliziert

#### Löschen
- **Entf-Taste**: Ausgewählte Elemente löschen
- **Button**: "Delete Selected" in Toolbox
- **Bestätigung**: Keine (nutzbar über Undo)

---

## Ausrichtungs-Tools

**Voraussetzung**: Mindestens 2 Elemente ausgewählt

### Horizontale Ausrichtung

#### Align Left
Alle Elemente linksbündig am linkesten Element ausrichten

#### Align Center
Alle Elemente zentriert zwischen links und rechts

#### Align Right
Alle Elemente rechtsbündig am rechtesten Element ausrichten

### Vertikale Ausrichtung

#### Align Top
Alle Elemente oben bündig am obersten Element ausrichten

#### Align Middle
Alle Elemente mittig zwischen oben und unten ausrichten

#### Align Bottom
Alle Elemente unten bündig am untersten Element ausrichten

### Verteilung

**Voraussetzung**: Mindestens 3 Elemente ausgewählt

#### Distribute Horizontally
Gleiche horizontale Abstände zwischen allen Elementen

#### Distribute Vertically
Gleiche vertikale Abstände zwischen allen Elementen

**Verwendung**:
```
1. Mehrere Elemente auswählen (Strg + Klick)
2. Arrange-Menü → Alignment-Option wählen
3. Elemente werden sofort ausgerichtet
```

**Anwendungsfälle**:
- Buttons in einer Reihe
- Cards gleichmäßig verteilen
- Icons ausrichten
- Text-Labels anordnen

---

## Layer-Management

### Z-Order (Ebenen-Reihenfolge)

Bestimmt, welche Elemente über anderen liegen.

#### Bring to Front
- **Funktion**: Element ganz nach vorne
- **Shortcut**: `Strg + ]`
- **Verwendung**: Wichtige Elemente sichtbar machen

#### Send to Back
- **Funktion**: Element ganz nach hinten
- **Shortcut**: `Strg + [`
- **Verwendung**: Hintergrund-Elemente

#### Bring Forward
- **Funktion**: Eine Ebene nach vorne
- **Shortcut**: `Strg + Shift + ]`
- **Verwendung**: Feinabstimmung

#### Send Backward
- **Funktion**: Eine Ebene nach hinten
- **Shortcut**: `Strg + Shift + [`
- **Verwendung**: Feinabstimmung

**Z-Index-Werte**:
- Werden automatisch verwaltet
- Höhere Werte = weiter vorne
- Kann auch manuell im Property Panel gesetzt werden

**Strategie**:
```
Ebene 0-10:    Hintergründe
Ebene 10-50:   Content-Elemente
Ebene 50-100:  Text & Icons
Ebene 100+:    Overlays & Highlights
```

---

## Undo/Redo-System

### Funktionalität
- **Undo**: `Strg + Z` - Letzte Aktion rückgängig machen
- **Redo**: `Strg + Y` - Rückgängig gemachte Aktion wiederherstellen
- **History-Tiefe**: Bis zu 100 Schritte
- **Älteste Aktionen**: Werden automatisch entfernt

### Was wird aufgezeichnet?
✅ Element hinzufügen
✅ Element löschen
✅ Element duplizieren
✅ Property-Änderungen (indirekt über Databinding)
✅ Layer-Änderungen
✅ Verschieben, Skalieren, Rotieren (indirekt)

### Best Practices
```
Häufig speichern (Strg + S)
Nach größeren Änderungen speichern
Undo als "Probieren"-Tool nutzen
Bei Experimenten: Undo ist sicher
```

---

## Grid & Snapping

### Grid-Anzeige

**Aktivierung**: `Ansicht` → `Show Grid`

**Eigenschaften**:
- **Grid Size**: Abstand zwischen Linien (Standard: 20px)
- **Farbe**: Hellgrau, halbtransparent
- **Anpassbar**: Grid Size im Property Panel ändern

**Verwendung**:
```
Kleine Layouts: 10px Grid
Mittel: 20px Grid (Standard)
Groß: 50px Grid
Sehr groß: 100px Grid
```

### Snap-to-Grid

**Aktivierung**: `Ansicht` → `Snap to Grid`

**Funktionsweise**:
- Elemente rasten automatisch am Grid ein
- Beim Verschieben
- Beim Erstellen
- Präzise Ausrichtung garantiert

**Vorteile**:
```
✅ Perfekte Ausrichtung
✅ Konsistente Abstände
✅ Professionelles Layout
✅ Schnelleres Arbeiten
```

**Tipp**: Grid ein-, Snap ausschalten für freies Positionieren mit visueller Hilfe

---

## Zoom & Navigation

### Zoom

**Vergrößern**:
- `Strg + Plus` oder `Strg + Mausrad aufwärts`
- Zoom-In-Button in Toolbar

**Verkleinern**:
- `Strg + Minus` oder `Strg + Mausrad abwärts`
- Zoom-Out-Button in Toolbar

**Reset**:
- `Strg + 0`
- Setzt Zoom auf 100% zurück
- Zentriert Ansicht

**Zoom-Range**: 10% - 1000%

**Aktuelle Zoom-Stufe**: Anzeige in Toolbar

### Pan (Verschieben)

**Scrollbars**: Horizontales und vertikales Scrollen

**Tipp für große Layouts**:
```
1. Herauszoomen für Übersicht
2. Hineinzoomen für Details
3. Pan mit Scrollbars
4. Zoom Reset für Neuorientierung
```

---

## Speichern & Laden

### Dateiformat: `.layout`

JSON-basiertes Format mit allen Layout-Informationen:
- Alle Elemente und ihre Properties
- Canvas-Einstellungen
- Grid-Konfiguration
- Metadaten (Datum, Version)

### Speichern

**Neu speichern**: `Strg + S` (beim ersten Mal → Save As)
**Speichern unter**: `Strg + Shift + S` oder `Datei` → `Save As`

**Speicherort**:
```
Empfohlen: Projekt-Ordner
Beispiel: C:\Projects\Layouts\MyLayout.layout
Team-Share: Netzwerk-Laufwerk
```

### Laden

**Öffnen**: `Strg + O` oder `Datei` → `Open`

**Recent Files**: Liste der letzten 10 Dateien (automatisch)

**Ung saved Changes**: Abfrage beim Öffnen neuer Datei

### Best Practices

```
Versionen: Layout_v1.layout, Layout_v2.layout
Descriptive Namen: MeetingRoom_A.layout
Backups: Regelmäßige Kopien
Cloud-Sync: OneDrive, Dropbox für Teamarbeit
```

---

## Export-Funktionen

### PNG Export

**Verwendung**: `Datei` → `Export` → `Export as PNG`

**Eigenschaften**:
- **Verlustfrei**: Keine Qualitätsverluste
- **Transparenz**: Möglich (abhängig von Canvas-Hintergrund)
- **DPI**: 96 DPI (Standard)
- **Größe**: Original Canvas-Größe

**Anwendungsfälle**:
```
✅ Web-Grafiken
✅ Präsentationen
✅ Social Media
✅ Dokumentation
✅ Wenn Transparenz benötigt wird
```

### JPG Export

**Verwendung**: `Datei` → `Export` → `Export as JPG`

**Eigenschaften**:
- **Qualität**: 95% (hohe Qualität)
- **Dateigröße**: Kleiner als PNG
- **Keine Transparenz**: Weißer Hintergrund statt transparent

**Anwendungsfälle**:
```
✅ Fotos/Bilder
✅ E-Mail-Anhänge
✅ Druck (geringe Auflösung)
✅ Schnellere Übertragung
```

### Export-Prozess

```
1. Layout fertigstellen
2. Datei → Export → Format wählen
3. Dateinamen und Speicherort wählen
4. Speichern
5. Erfolgsmeldung erscheint
```

**Tipps**:
```
Vor Export: Zoom auf 100% setzen
Alle Elemente sichtbar machen
Überprüfen: Vorschau vor Export
Benennung: Descriptive Namen verwenden
```

---

## Tastenkombinationen

### Datei-Operationen
```
Strg + N          Neues Layout
Strg + O          Layout öffnen
Strg + S          Speichern
Strg + Shift + S  Speichern unter
```

### Bearbeiten
```
Strg + Z          Undo (Rückgängig)
Strg + Y          Redo (Wiederherstellen)
Strg + D          Duplizieren
Strg + A          Alle auswählen
Entf              Löschen
```

### Ansicht
```
Strg + Plus       Zoom vergrößern
Strg + Minus      Zoom verkleinern
Strg + 0          Zoom zurücksetzen (100%)
```

### Layer (Ebenen)
```
Strg + ]          Bring to Front
Strg + [          Send to Back
Strg + Shift + ]  Bring Forward
Strg + Shift + [  Send Backward
```

### Sonstiges
```
Strg + Klick      Zur Auswahl hinzufügen
Esc               Auswahl aufheben (zukünftig)
```

---

## Workflow-Beispiele

### 1. Konferenzraum-Schild erstellen

```
1. Neu: Strg + N
2. Canvas-Größe: 1920 x 1080
3. Hintergrund: Shape (volle Größe, dunkle Farbe)
4. Titel: Text (groß, bold, zentriert)
5. Status: Shape (grün für verfügbar)
6. Uhrzeit: Dynamic Field
7. QR-Code: Für Buchungssystem
8. Speichern: Strg + S
9. Export: Als PNG für Display
```

### 2. Dashboard erstellen

```
1. Große Canvas: 3840 x 2160 (4K)
2. Grid aktivieren: 20px
3. Snap to Grid: Ein
4. Cards: Mehrere Shapes (abgerundet)
5. Content: Text-Elemente
6. Alignment: Alle Cards ausrichten
7. Dynamic Fields: Datum/Zeit
8. Export: Als PNG für Display
```

### 3. Poster gestalten

```
1. Canvas: A4-Format (2480 x 3508 @ 300 DPI)
2. Hintergrund: Image oder Shape
3. Überschrift: Großer Text, bold
4. Content: Mehrere Text-Bereiche
5. Visuals: Images und Shapes
6. QR-Code: Für mehr Info
7. Layer-Management: Wichtiges nach vorne
8. Export: Als PNG, dann in Drucksoftware
```

---

## Tipps & Tricks

### Design-Prinzipien

```
Kontrast: Dunkler Text auf hellem Hintergrund
Hierarchie: Größe für Wichtigkeit nutzen
Weißraum: Luft zwischen Elementen lassen
Konsistenz: Gleiche Farben und Fonts
Ausrichtung: Grid und Snap nutzen
```

### Performance

```
Große Bilder: Vorher komprimieren
Viele Elemente: Layer-Management nutzen
Komplexe Shapes: Zu Images kombinieren (extern)
Speichern: Regelmäßig, nicht nur am Ende
```

### Zusammenarbeit

```
Namenskonvention: Einheitliche Dateinamen
Version Control: Git für .layout-Dateien
Shared Folder: Team-Zugriff
Templates: Standard-Layouts als Basis
Dokumentation: README für Projekt
```

---

## Fehlerbehebung

### Element lässt sich nicht verschieben
→ Prüfen: IsLocked im Property Panel

### Grid nicht sichtbar
→ Ansicht → Show Grid aktivieren

### Export schlägt fehl
→ Alle Elemente im Canvas-Bereich?
→ Dateipfad gültig und beschreibbar?

### Undo funktioniert nicht mehr
→ Limit von 100 Aktionen erreicht
→ Lösung: Zwischenspeichern

### Performance-Probleme
→ Zu viele Elemente? (>100)
→ Große Bilder komprimieren
→ Zoom reduzieren

---

**Version**: 1.0.0
**Letzte Aktualisierung**: November 2025
**Weitere Hilfe**: README.md, QUICKSTART.md, ARCHITECTURE.md
