# Layout Designer

Ein professionelles WPF/.NET Layout-Design-Tool zum Erstellen schöner Raum- und Übersichtsdisplays.

![Version](https://img.shields.io/badge/version-1.0.0-blue)
![.NET](https://img.shields.io/badge/.NET-8.0-purple)
![WPF](https://img.shields.io/badge/WPF-Windows-green)

## 📋 Überblick

Layout Designer ist ein eigenständiges Windows-Anwendungstool, das eine intuitive Benutzeroberfläche zum Erstellen, Bearbeiten und Exportieren von visuellen Layouts bietet. Perfekt für:
- Raum- und Gebäudepläne
- Übersichtsdisplays
- Beschilderungen
- Informationstafeln
- Dashboard-Layouts

## ✨ Hauptfunktionen

### Canvas-Editor
- **Elemente hinzufügen**: Text, Bilder, Formen, QR-Codes, Dynamische Felder
- **Auswählen & Manipulieren**: Klicken zum Auswählen, Ziehen zum Verschieben
- **Resize & Rotate**: Intuitive Handles zum Ändern der Größe und Rotation
- **Mehrfachauswahl**: Strg+Klick für mehrere Elemente
- **Snap-to-Grid**: Automatisches Einrasten am Raster
- **Zoom & Pan**: Mausrad zum Zoomen, Scrollen zum Verschieben

### Element-Typen

#### 📝 TextElement
- Benutzerdefinierter Text
- Font-Familie, Größe, Stil (Bold, Italic, Underline)
- Vorder- und Hintergrundfarbe
- Textausrichtung (Links, Mitte, Rechts)

#### 🖼️ ImageElement
- Bildpfad (lokal oder URL)
- Verschiedene Skalierungsmodi (None, Fill, Uniform, UniformToFill)
- Seitenverhältnis beibehalten

#### 🔷 ShapeElement
- Formen: Rechteck, Ellipse, Linie, Abgerundetes Rechteck
- Füllfarbe und Rahmenfarbe
- Rahmenstärke
- Eckenradius (für abgerundete Rechtecke)

#### 📱 QrCodeElement
- QR-Code-Generierung aus Text/URLs
- Anpassbare Farben
- Fehlerkorrektur-Level (0-3)

#### 🕐 DynamicFieldElement
- Datum/Zeit-Anzeige mit konfigurierbarem Format
- Unterstützte Typen: DateTime, Date, Time, Custom
- Format-String: z.B. "dd.MM.yyyy HH:mm:ss"

### Layout-Verwaltung

- **Speichern/Laden**: Layouts als JSON-Dateien (.layout)
- **Export**: PNG und JPG Export für fertige Layouts
- **Recent Files**: Liste der zuletzt verwendeten Dateien

### Editor-Features

- **Undo/Redo**: Unbegrenzte Rückgängig/Wiederherstellen-Funktionen (bis zu 100 Schritte)
- **Ebenen-Steuerung**:
  - Bring to Front / Send to Back
  - Bring Forward / Send Backward
- **Grid-System**:
  - Sichtbares Raster (ein-/ausschaltbar)
  - Snap-to-Grid (ein-/ausschaltbar)
  - Konfigurierbare Rastergröße
- **Ausrichtungs-Tools** ✨ NEU:
  - Align Left/Center/Right (horizontal)
  - Align Top/Middle/Bottom (vertikal)
  - Distribute Horizontally/Vertically
  - Automatische Ausrichtung mehrerer Elemente
- **Tastenkombinationen**:
  - `Strg+N`: Neues Layout
  - `Strg+O`: Layout öffnen
  - `Strg+S`: Speichern
  - `Strg+Z`: Rückgängig
  - `Strg+Y`: Wiederherstellen
  - `Strg+D`: Duplizieren
  - `Strg+A`: Alle auswählen
  - `Entf`: Löschen
  - `Strg +/-/0`: Zoom In/Out/Reset
  - `Strg+]`: Bring to Front
  - `Strg+[`: Send to Back

## 🏗️ Architektur

### MVVM Pattern
Das Projekt folgt dem Model-View-ViewModel-Pattern für saubere Trennung von Zuständigkeiten:

```
Models/           - Datenmodelle (TextElement, ImageElement, etc.)
ViewModels/       - Business-Logik (MainViewModel, CanvasViewModel)
Views/            - UI-Definitionen (XAML)
Services/         - Geschäftsdienste (Storage, QR-Code, Export)
Controls/         - Benutzerdefinierte Controls (DesignCanvas, SelectionAdorner)
Helpers/          - Hilfsfunktionen (SnapHelper, GeometryHelper)
Converters/       - XAML Value Converters
```

### Dependency Injection
Einfaches Service-Container-System für:
- `ILayoutStorageService` - JSON-Serialisierung
- `IQrCodeService` - QR-Code-Generierung
- `IExportService` - Bild-Export
- `IAssetService` - Asset-Verwaltung
- `IUndoRedoService` - Undo/Redo-Verwaltung

## 📚 Beispiel-Layouts

Das Projekt enthält fertige Beispiel-Layouts zum Erkunden und Lernen:

### `Examples/WelcomeLayout.layout`
- Professionelles Welcome-Layout
- Zeigt alle 5 Element-Typen
- Feature-Boxen mit Beschreibungen
- QR-Code und Datum/Zeit-Anzeige
- **Perfekt zum Einstieg!**

### `Examples/MeetingRoomSign.layout` ✨ NEU
- Konferenzraum-Beschilderung
- Status-Anzeige (Verfügbar/Belegt)
- Aktuelle Uhrzeit
- Raum-Informationen
- QR-Code für Raumbuchung
- **4K-optimiert (1920x1080)**

### `Examples/InformationDashboard.layout` ✨ NEU
- Großes Informations-Dashboard
- Multiple Content-Cards
- Firmen-News, Events, Metriken
- WiFi-Informationen mit QR-Code
- Dynamische Datum/Zeit-Anzeige
- **Ultra HD (3840x2160)**

**Verwendung**:
1. Anwendung starten
2. `Datei` → `Öffnen` (Strg+O)
3. Navigieren zu `Examples/` Ordner
4. Layout auswählen und öffnen
5. Erkunden, bearbeiten, als Basis verwenden!

## 🚀 Installation & Build

### Voraussetzungen
- Windows 10/11
- .NET 8.0 SDK oder höher
- Visual Studio 2022 (empfohlen) oder Rider

### Build-Anleitung

```bash
# Repository klonen
git clone <repository-url>
cd canvas

# NuGet-Pakete wiederherstellen
dotnet restore

# Projekt kompilieren
dotnet build

# Anwendung starten
dotnet run --project LayoutDesigner
```

### In Visual Studio
1. `LayoutDesigner.sln` öffnen
2. `F5` drücken zum Kompilieren und Starten

## 📦 NuGet-Abhängigkeiten

- **QRCoder** (v1.6.0) - QR-Code-Generierung
- **Microsoft.Xaml.Behaviors.Wpf** (v1.1.77) - MVVM Behaviors

## 🎨 Verwendung

### Layout erstellen
1. Starten Sie die Anwendung
2. Wählen Sie Elemente aus der Toolbox auf der linken Seite
3. Klicken Sie auf "Add Text", "Add Image", etc.
4. Passen Sie Eigenschaften im Property Panel rechts an
5. Verschieben, skalieren und rotieren Sie Elemente auf dem Canvas

### Element bearbeiten
1. Klicken Sie auf ein Element, um es auszuwählen
2. Bearbeiten Sie Eigenschaften im rechten Panel
3. Ziehen Sie Handles zum Skalieren
4. Ziehen Sie den grünen Handle zum Rotieren

### Speichern & Exportieren
- **Speichern**: Datei → Speichern (Strg+S) - speichert als .layout JSON-Datei
- **Export PNG**: Datei → Export → Export as PNG
- **Export JPG**: Datei → Export → Export as JPG

## 🔧 Erweiterungsmöglichkeiten

Das Design ermöglicht einfache Erweiterungen:

### Neue Element-Typen hinzufügen
1. Erstellen Sie eine neue Klasse, die von `LayoutElementBase` erbt
2. Fügen Sie spezifische Eigenschaften hinzu
3. Implementieren Sie `Clone()` und `ElementType`
4. Erstellen Sie ein DataTemplate in XAML
5. Fügen Sie einen Button in der Toolbox hinzu

### Verbesserungsideen (bereits implementiert)

✅ **Bereits vorhanden**:
- MVVM-Architektur
- Undo/Redo-System
- Snap-to-Grid
- Multiple Element-Typen
- JSON-Serialisierung
- QR-Code-Generator
- Export-Funktionen
- Tastenkombinationen

🎯 **Zukünftige Verbesserungen**:

#### Kurz- bis mittelfristig
- **Alignment Guides**: PowerPoint-ähnliche automatische Ausrichtungslinien
- **Smart Guides**: Automatisches Ausrichten an anderen Objekten
- **Gruppierung**: Mehrere Elemente zu Gruppen zusammenfassen
- **Template Library**: Vorgefertigte Layout-Vorlagen
- **Asset Manager**: Zentrale Bilderverwaltung mit Thumbnails
- **Color Picker**: Visueller Farbwähler statt Hex-Eingabe
- **Recent Files Menu**: MRU-Liste im Dateimenü

#### Langfristig
- **Plugin-System**: MEF-basierte Erweiterungen
- **Custom Element Types**: Barcodes, Charts, Tabellen via Plugins
- **Animation Designer**: Animationen für Runtime-Modus
- **Multi-Page Support**: Mehrere Canvases in einem Dokument
- **Datenbindung**: Live-Datenverbindungen
- **Collaboration**: Multi-User-Editing
- **Lokalisierung**: Mehrsprachige Oberfläche
- **Dark/Light Theme**: Theme-Switching
- **Cloud Storage**: OneDrive/Dropbox-Integration

#### Performance-Optimierungen
- **Virtualisierung**: Für viele Elemente
- **Dirty Tracking**: Nur geänderte Elemente speichern
- **Lazy Loading**: Bilder erst bei Bedarf laden
- **Render Caching**: Cache für komplexe Elemente

## 📁 Projektstruktur

```
LayoutDesigner/
├── LayoutDesigner.sln              # Visual Studio Solution
├── README.md                        # Dieses Dokument
├── .gitignore                       # Git Ignore-Datei
└── LayoutDesigner/
    ├── LayoutDesigner.csproj        # Projektdatei
    ├── App.xaml                     # Application Definition
    ├── App.xaml.cs                  # Application Code-Behind mit DI
    │
    ├── Models/                      # Datenmodelle
    │   ├── Base/
    │   │   └── LayoutElementBase.cs # Basis für alle Elemente
    │   ├── TextElement.cs
    │   ├── ImageElement.cs
    │   ├── ShapeElement.cs
    │   ├── QrCodeElement.cs
    │   ├── DynamicFieldElement.cs
    │   ├── LayoutDocument.cs        # Container für Layout
    │   └── UndoRedoAction.cs        # Undo/Redo-Aktion
    │
    ├── ViewModels/                  # ViewModels
    │   ├── Base/
    │   │   ├── ViewModelBase.cs     # Basis-ViewModel
    │   │   └── RelayCommand.cs      # Command-Implementierung
    │   ├── MainViewModel.cs         # Haupt-ViewModel
    │   ├── CanvasViewModel.cs       # Canvas-Logik
    │   └── PropertyPanelViewModel.cs# Properties-Panel
    │
    ├── Views/                       # XAML-Views
    │   ├── MainWindow.xaml          # Hauptfenster
    │   ├── PropertyPanel.xaml       # Eigenschafts-Panel
    │   └── ToolboxPanel.xaml        # Werkzeug-Panel
    │
    ├── Controls/                    # Custom Controls
    │   ├── DesignCanvas.cs          # Haupt-Canvas
    │   ├── SelectionAdorner.cs      # Resize/Rotate Handles
    │   └── GridLines.cs             # Grid-Rendering
    │
    ├── Services/                    # Services
    │   ├── Interfaces/
    │   │   ├── ILayoutStorageService.cs
    │   │   ├── IQrCodeService.cs
    │   │   ├── IExportService.cs
    │   │   ├── IAssetService.cs
    │   │   └── IUndoRedoService.cs
    │   ├── LayoutStorageService.cs  # JSON Storage
    │   ├── QrCodeService.cs         # QR-Code Generator
    │   ├── ExportService.cs         # PNG/JPG Export
    │   ├── AssetService.cs          # Asset-Verwaltung
    │   └── UndoRedoService.cs       # Undo/Redo
    │
    ├── Converters/                  # XAML Converters
    │   ├── BoolToVisibilityConverter.cs
    │   ├── ColorToBrushConverter.cs
    │   ├── ImageStretchConverter.cs
    │   ├── BoolToFontWeightConverter.cs
    │   ├── BoolToFontStyleConverter.cs
    │   ├── TypeToVisibilityConverter.cs
    │   └── ShapeTypeToVisibilityConverter.cs
    │
    ├── Helpers/                     # Helper-Klassen
    │   ├── SnapHelper.cs            # Snap-to-Grid
    │   ├── GeometryHelper.cs        # Geometrie-Berechnungen
    │   └── FileDialogHelper.cs      # Dateidialoge
    │
    └── Resources/                   # Ressourcen
        ├── Styles.xaml              # Globale Styles
        └── Icons/                   # Icons & Bilder
```

## 🤝 Mitwirken

Beiträge sind willkommen! Bitte:
1. Forken Sie das Repository
2. Erstellen Sie einen Feature-Branch
3. Committen Sie Ihre Änderungen
4. Pushen Sie zum Branch
5. Erstellen Sie einen Pull Request

## 📄 Lizenz

Dieses Projekt ist lizenziert unter der MIT-Lizenz.

## 👨‍💻 Autor

Layout Designer Team

## 📖 Weitere Dokumentation

- **[QUICKSTART.md](QUICKSTART.md)** - 5-Minuten-Schnelleinstieg für Anfänger
- **[FEATURES.md](FEATURES.md)** ✨ NEU - Vollständige Feature-Dokumentation mit allen Details
- **[ARCHITECTURE.md](ARCHITECTURE.md)** - Technische Architektur-Dokumentation für Entwickler
- **[CHANGELOG.md](CHANGELOG.md)** - Versionshistorie und Änderungen
- **[CODE_IMPROVEMENTS.md](CODE_IMPROVEMENTS.md)** - Code-Qualität und Verbesserungen
- **[CODE_QUALITY_ISSUES.md](CODE_QUALITY_ISSUES.md)** ⚠️ NEU - Gefundene Probleme und Fixes

## 📞 Support

Bei Fragen oder Problemen:
- Erstellen Sie ein Issue auf GitHub
- Konsultieren Sie die [FEATURES.md](FEATURES.md) für detaillierte Anleitungen
- Kontaktieren Sie den Entwickler

---

**Version**: 1.0.0
**Letzte Aktualisierung**: November 2025
**Platform**: Windows (.NET 8.0 / WPF)
