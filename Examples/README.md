# Examples README

Dieser Ordner enthält vorgefertigte Beispiel-Layouts zum Erkunden und als Basis für eigene Projekte.

## 📁 Verfügbare Layouts

### 1. WelcomeLayout.layout
**Zweck**: Einführung in alle Features
**Größe**: 1920 × 1080 (Full HD)
**Schwierigkeit**: ⭐ Anfänger

**Enthält**:
- Alle 5 Element-Typen demonstriert
- Feature-Übersicht in Boxen
- Professionelles Header-Design
- QR-Code Beispiel
- Dynamisches Datum/Zeit-Feld
- Footer mit Branding

**Verwendung**: Perfekt zum Erkunden der Anwendung beim ersten Start

---

### 2. MeetingRoomSign.layout
**Zweck**: Konferenzraum-Beschilderung
**Größe**: 1920 × 1080 (Full HD)
**Schwierigkeit**: ⭐⭐ Mittel

**Enthält**:
- Raum-Name (CONFERENCE ROOM A)
- Status-Indikator (Verfügbar/Belegt)
- Live-Uhrzeit-Anzeige
- Raum-Informationen:
  - Kapazität: 12 Personen
  - Ausstattung: Video conference, Whiteboard, Projector
- QR-Code für Online-Buchung
- Professionelles Farbschema (Blau/Grün)

**Anwendungsfall**:
- Digital Signage für Büros
- Meeting-Raum-Displays
- Konferenz-Zentren
- Co-Working Spaces

**Anpassungen**:
```
Raum-Name ändern → Text "CONFERENCE ROOM A"
Status anpassen → Shape "Status Indicator" Farbe ändern
  - Grün (#27AE60) = Verfügbar
  - Rot (#E74C3C) = Belegt
  - Gelb (#F39C12) = Bald belegt
QR-Code → "Content" mit eigener Booking-URL
```

---

### 3. InformationDashboard.layout
**Zweck**: Corporate Information Display
**Größe**: 3840 × 2160 (Ultra HD 4K)
**Schwierigkeit**: ⭐⭐⭐ Fortgeschritten

**Enthält**:
- **News Card**: Firmen-Neuigkeiten (5 Einträge)
- **Events Card**: Bevorstehende Events mit Zeiten
- **Metrics Card**: KPIs und Kennzahlen
- **WiFi Info**: Netzwerk-Details mit QR-Code
- Dynamisches Datum/Zeit-Display
- Mehrfarbiges Card-System (Blau, Lila, Rot)

**Anwendungsfall**:
- Empfangsbereich
- Lobby-Displays
- Großbildschirme
- Information Kioske
- Corporate TV

**Anpassungen**:
```
News aktualisieren → Text "News Content"
Events ändern → Text "Events Content"
Metriken anpassen → Text "Metrics Content"
WiFi-Daten → Text "WiFi Info" und QR-Code "Content"
```

---

## 🚀 Verwendung

### Layout öffnen
```
1. Layout Designer starten
2. Datei → Öffnen (Strg+O)
3. Navigieren zu Examples/ Ordner
4. Gewünschtes .layout-File wählen
5. Öffnen
```

### Als Vorlage verwenden
```
1. Layout öffnen
2. Datei → Speichern unter (Strg+Shift+S)
3. Neuen Namen vergeben
4. Elemente nach Bedarf anpassen
5. Speichern
```

## 💡 Tipps

### Farben anpassen
Alle Farben als Hex-Werte im Property Panel:
```
Primär-Blau:   #FF0078D7
Erfolg-Grün:   #FF27AE60
Warnung-Gelb:  #FFF39C12
Fehler-Rot:    #FFE74C3C
Lila-Akzent:   #FF9B59B6
Dunkel-Grau:   #FF2C3E50
Hell-Grau:     #FFECF0F1
```

### QR-Code Formate

**URL**:
```
https://example.com
http://booking.company.com/room-a
```

**WiFi** (automatisches Verbinden):
```
WIFI:T:WPA;S:NetworkName;P:Password;;

Beispiel:
WIFI:T:WPA;S:Company_Guest;P:Welcome2024!;;
```

**E-Mail**:
```
mailto:info@example.com
```

**Telefon**:
```
tel:+49123456789
```

### Dynamische Felder

**Format-Beispiele**:
```
Datum & Zeit:   dd.MM.yyyy HH:mm:ss
Nur Datum:      dd.MM.yyyy
Nur Zeit:       HH:mm
Wochentag:      dddd, dd. MMMM yyyy
```

## 🎨 Design-Patterns

### Header-Pattern (WelcomeLayout)
```
1. Volle Breite Shape (Hintergrund)
2. Zentrierter Titel (groß, bold)
3. Untertitel (kleiner, italic)
```

### Card-Pattern (InformationDashboard)
```
1. Abgerundetes Rectangle (Container)
2. Farbiger Rahmen (3px)
3. Icon + Titel (oben)
4. Content (mehrzeilig)
5. Konsistente Abstände
```

### Status-Pattern (MeetingRoomSign)
```
1. Status-Box (abgerundet)
2. Große Status-Message
3. Symbol (✓, ✗, ⏰)
4. Passende Farbe
```

## 📏 Standard-Größen

**Display-Formate**:
```
Full HD:        1920 × 1080
4K Ultra HD:    3840 × 2160
WQHD:           2560 × 1440
Wide Screen:    2560 × 1080
```

**Druck-Formate** (300 DPI):
```
A4:             2480 × 3508
A3:             3508 × 4961
A2:             4961 × 7016
Letter:         2550 × 3300
```

## 🔄 Versionierung

Alle Beispiel-Layouts:
- **Version**: 1.0
- **Format-Version**: 1.0
- **Kompatibilität**: Layout Designer v1.0+

## 📝 Eigene Layouts teilen

Wenn Sie eigene Layouts erstellen und teilen möchten:

```
1. Layout finalisieren
2. Beschreibenden Namen wählen
3. Screenshot erstellen (Export → PNG)
4. README für Layout schreiben:
   - Zweck
   - Größe
   - Anwendungsfall
   - Anpassungs-Hinweise
5. In Examples/ speichern
```

## ❓ Fragen?

- **Dokumentation**: Siehe FEATURES.md für Details
- **Schnelleinstieg**: Siehe QUICKSTART.md
- **Probleme**: GitHub Issues

---

**Tipp**: Experimentieren Sie mit den Layouts! Alle Änderungen können mit Strg+Z rückgängig gemacht werden.
