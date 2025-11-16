# Changelog

Alle wichtigen Änderungen an diesem Projekt werden in dieser Datei dokumentiert.

Das Format basiert auf [Keep a Changelog](https://keepachangelog.com/de/1.0.0/),
und dieses Projekt folgt [Semantic Versioning](https://semver.org/lang/de/).

## [1.0.0] - 2025-11-15

### Hinzugefügt

#### Kern-Features
- **MVVM-Architektur**: Vollständige Implementierung des Model-View-ViewModel-Patterns
- **Canvas-Editor**: Interaktiver Design-Canvas mit Drag & Drop
- **Element-Typen**:
  - TextElement: Formatierter Text mit Font-Optionen
  - ImageElement: Bilder mit verschiedenen Skalierungsmodi
  - ShapeElement: Rechteck, Ellipse, Linie, Abgerundete Rechtecke
  - QrCodeElement: QR-Code-Generierung
  - DynamicFieldElement: Datum/Zeit-Felder mit konfigurierbarem Format

#### Editor-Funktionen
- **Auswahl & Manipulation**:
  - Einzelne und mehrfache Elementauswahl
  - Drag & Drop zum Verschieben
  - Resize-Handles zum Skalieren
  - Rotate-Handle zum Drehen
  - SelectionAdorner mit 8 Resize-Punkten + Rotate-Handle
- **Grid-System**:
  - Sichtbares Raster (konfigurierbar)
  - Snap-to-Grid-Funktion
  - Anpassbare Rastergröße
- **Zoom & Pan**:
  - Mausrad-Zoom
  - Scroll-Panning
  - Zoom Reset (100%)
- **Undo/Redo**: Vollständiges Undo/Redo-System (bis zu 100 Schritte)
- **Layer-Management**:
  - Bring to Front / Send to Back
  - Bring Forward / Send Backward
  - Z-Index-Verwaltung

#### Daten & Speicherung
- **JSON-Serialisierung**: Layouts als .layout-Dateien speichern/laden
- **Polymorphe Serialisierung**: Custom JSON Converter für verschiedene Element-Typen
- **Recent Files**: Liste der zuletzt verwendeten Dateien
- **Export-Funktionen**: PNG und JPG Export (vorbereitet)

#### Services
- `ILayoutStorageService`: JSON-basierte Speicherung
- `IQrCodeService`: QR-Code-Generierung mit QRCoder-Library
- `IExportService`: Bild-Export-Funktionen
- `IAssetService`: Asset-Verwaltung
- `IUndoRedoService`: Undo/Redo-Stack-Verwaltung

#### Benutzeroberfläche
- **MainWindow**: Hauptfenster mit Menu, Toolbar, StatusBar
- **ToolboxPanel**: Element-Toolbox auf der linken Seite
- **PropertyPanel**: Dynamisches Eigenschaften-Panel auf der rechten Seite
- **DesignCanvas**: Haupt-Canvas mit Grid-Rendering

#### Tastenkombinationen
- `Strg+N`: Neues Layout
- `Strg+O`: Layout öffnen
- `Strg+S`: Speichern
- `Strg+Shift+S`: Speichern unter
- `Strg+Z`: Rückgängig
- `Strg+Y`: Wiederherstellen
- `Strg+D`: Duplizieren
- `Strg+A`: Alle auswählen
- `Entf`: Löschen
- `Strg +/-/0`: Zoom In/Out/Reset

#### Developer Features
- **Dependency Injection**: Einfaches Service-Container-System
- **Value Converters**: Umfangreiche XAML-Konverter
- **Helpers**: SnapHelper, GeometryHelper, FileDialogHelper
- **Custom Controls**: DesignCanvas, SelectionAdorner, GridLines

#### Dokumentation
- Umfassendes README.md mit:
  - Feature-Übersicht
  - Architektur-Beschreibung
  - Build-Anleitung
  - Verwendungshinweise
  - Erweiterungsmöglichkeiten
- .gitignore für .NET-Projekte
- Changelog (dieses Dokument)

### Technische Details
- **Framework**: .NET 8.0 / WPF
- **NuGet-Pakete**:
  - QRCoder v1.6.0
  - Microsoft.Xaml.Behaviors.Wpf v1.1.77
- **Architektur**: MVVM mit Services-Layer
- **Serialisierung**: System.Text.Json

### Bekannte Einschränkungen
- Export-Funktionen (PNG/JPG) sind vorbereitet, aber noch nicht vollständig implementiert
- Gruppierung von Elementen noch nicht implementiert
- Keine Auto-Alignment-Guides
- QR-Codes werden im Canvas noch nicht visuell gerendert (nur Platzhalter)

## [Unveröffentlicht]

### Geplant für v1.1.0
- [ ] Vollständige PNG/JPG-Export-Implementierung
- [ ] QR-Code-Rendering im Canvas
- [ ] Gruppierung von Elementen
- [ ] Auto-Alignment-Guides
- [ ] Color Picker Control
- [ ] Font Picker mit Preview
- [ ] Image Browser mit Asset Manager

### Geplant für v1.2.0
- [ ] Template-System
- [ ] Style-Library
- [ ] Multi-Page-Support
- [ ] Preview/Runtime-Mode
- [ ] Druckfunktion

### Geplant für v2.0.0
- [ ] Plugin-System (MEF)
- [ ] Custom Element Types
- [ ] Datenbindung
- [ ] Animation Designer
- [ ] Collaboration-Features

---

**Legende**:
- `Hinzugefügt`: Neue Features
- `Geändert`: Änderungen an bestehenden Features
- `Veraltet`: Bald zu entfernende Features
- `Entfernt`: Entfernte Features
- `Behoben`: Behobene Bugs
- `Sicherheit`: Sicherheits-Patches
