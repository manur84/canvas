# Layout Designer - Architektur-Dokumentation

## Überblick

Layout Designer folgt einer klassischen **MVVM-Architektur** (Model-View-ViewModel) mit einer zusätzlichen **Services-Layer** für Geschäftslogik.

```
┌─────────────────────────────────────────────────────────┐
│                        Views                            │
│              (XAML + Code-Behind)                       │
└───────────────────┬─────────────────────────────────────┘
                    │ Data Binding
                    ▼
┌─────────────────────────────────────────────────────────┐
│                     ViewModels                          │
│        (Presentation Logic + Commands)                  │
└───────────┬─────────────────────────────┬───────────────┘
            │                             │
            │ Uses                        │ Updates
            ▼                             ▼
┌─────────────────────┐         ┌─────────────────────────┐
│      Services       │         │        Models           │
│  (Business Logic)   │         │    (Data Entities)      │
└─────────────────────┘         └─────────────────────────┘
```

## Layer-Beschreibung

### 1. Models (Daten-Layer)

**Verantwortlichkeit**: Datenstruktur und Domain-Logik

#### LayoutElementBase (Abstrakte Basisklasse)
Gemeinsame Eigenschaften für alle Layout-Elemente:
- Position (X, Y)
- Größe (Width, Height)
- Rotation, ZIndex, Opacity
- IsLocked, IsVisible
- INotifyPropertyChanged für UI-Updates

#### Konkrete Element-Typen
- `TextElement`: Text mit Font-Eigenschaften
- `ImageElement`: Bild mit Skalierungsmodus
- `ShapeElement`: Geometrische Formen
- `QrCodeElement`: QR-Code-Daten
- `DynamicFieldElement`: Dynamische Felder (Datum/Zeit)

#### LayoutDocument
Container für das gesamte Layout:
- ObservableCollection von Elementen
- Canvas-Eigenschaften (Größe, Hintergrund)
- Grid-Einstellungen
- Metadaten (Name, Datum, Version)

**Design-Entscheidungen**:
- Vererbung für gemeinsame Funktionalität
- INotifyPropertyChanged für Zwei-Wege-Databinding
- Immutable IDs für eindeutige Identifikation
- Clone()-Methode für Duplizierung

### 2. ViewModels (Präsentations-Layer)

**Verantwortlichkeit**: UI-Logik, Commands, State Management

#### ViewModelBase
Basis-Klasse mit:
- INotifyPropertyChanged-Implementierung
- SetProperty()-Helper für Property-Updates
- OnPropertyChanged()-Helper

#### RelayCommand & RelayCommand<T>
ICommand-Implementierung für:
- Action-Delegates
- CanExecute-Logik
- CommandManager-Integration

#### MainViewModel
Hauptfenster-Logik:
- File-Operationen (New, Open, Save, SaveAs, Exit)
- Undo/Redo-Commands
- Zoom-Commands
- Export-Commands
- Dirty-Tracking für ungespeicherte Änderungen
- Titel-Verwaltung

#### CanvasViewModel
Canvas-spezifische Logik:
- Element-Management (Add, Delete, Duplicate)
- Selection-Management
- Zoom & Pan-State
- Grid-Einstellungen
- Layer-Manipulation (Z-Order)
- Command-Factory für alle Canvas-Operationen

#### PropertyPanelViewModel
Property-Panel-Logik:
- Selected Element Tracking
- Multi-Selection Detection

**Design-Entscheidungen**:
- Separation of Concerns: Jedes ViewModel hat klare Verantwortlichkeit
- Commands für alle User-Aktionen (testbar, bindbar)
- ObservableCollections für automatische UI-Updates
- Keine direkte View-Referenz (lose Kopplung)

### 3. Views (Präsentations-Layer)

**Verantwortlichkeit**: UI-Darstellung

#### MainWindow
Hauptfenster-Layout:
- Menu (File, Edit, View, Arrange)
- Toolbar (Quick Actions)
- StatusBar (Statusinfo)
- 3-Spalten-Layout:
  - Links: ToolboxPanel
  - Mitte: Canvas mit ScrollViewer
  - Rechts: PropertyPanel

#### ToolboxPanel
Element-Toolbox:
- Buttons für Element-Typen
- Gruppiert nach Kategorien
- Commands gebunden an CanvasViewModel

#### PropertyPanel
Dynamisches Eigenschaften-Panel:
- GroupBoxes für verschiedene Eigenschaften
- Type-basierte Visibility (DataTriggers)
- Zwei-Wege-Databinding zu SelectedElement

**Design-Entscheidungen**:
- XAML für deklarative UI
- DataTemplates für Element-Rendering
- Styles für konsistentes Design
- Converters für komplexe Bindings

### 4. Services (Geschäftslogik-Layer)

**Verantwortlichkeit**: Wiederverwendbare Geschäftslogik

#### Interface-First Design
Alle Services haben Interfaces für:
- Dependency Injection
- Testbarkeit
- Austauschbarkeit

#### ILayoutStorageService
JSON-Serialisierung:
- Polymorphe Serialisierung (LayoutElementConverter)
- Recent Files Management
- AppData-Storage

#### IQrCodeService
QR-Code-Generierung:
- QRCoder-Library-Wrapper
- Color-Konfiguration
- Error Correction Level

#### IExportService
Bild-Export:
- RenderTargetBitmap für PNG/JPG
- UIElement → Bitmap Conversion

#### IAssetService
Asset-Verwaltung:
- Zentrale Asset-Ordner-Verwaltung
- Import-Funktionen
- Asset-Auflistung

#### IUndoRedoService
Undo/Redo-System:
- Stack-basierter Verlauf
- Action-based (Command Pattern)
- State Change Events

**Design-Entscheidungen**:
- Singleton-Services (DI-Container)
- Interface-Segregation
- Async/Await für I/O-Operationen
- Event-basierte Benachrichtigungen

### 5. Controls (Custom UI-Controls)

**Verantwortlichkeit**: Wiederverwendbare UI-Komponenten

#### DesignCanvas
Canvas mit Editor-Features:
- Grid-Rendering (OnRender)
- Mouse-Event-Handling (Drag & Drop)
- Snap-to-Grid-Logik
- Element-Selection

#### SelectionAdorner
Adorner für ausgewählte Elemente:
- 8 Resize-Handles (Corner + Edge)
- 1 Rotate-Handle
- Visual Feedback (gestrichelte Linie)
- DragDelta-Events für Manipulation

#### GridLines
Grid-Rendering-Control:
- Konfigurierbare Grid-Größe
- Grid-Farbe
- On/Off-Toggle

**Design-Entscheidungen**:
- Control-Ableitung für eigene Rendering-Logik
- Adorner-Pattern für überlagerte UI-Elemente
- Dependency Properties für XAML-Bindung

### 6. Helpers (Utility-Funktionen)

**Verantwortlichkeit**: Wiederverwendbare Hilfsfunktionen

#### SnapHelper
Grid-Snapping:
- SnapToGrid(value, gridSize)
- Point/Rect-Snapping

#### GeometryHelper
Geometrische Berechnungen:
- Intersection-Tests
- Distance-Berechnungen
- Aspect-Ratio-Berechnungen

#### FileDialogHelper
Dialog-Wrapper:
- OpenLayoutDialog()
- SaveLayoutDialog()
- OpenImageDialog()
- ExportImageDialog()

**Design-Entscheidungen**:
- Static-Klassen für pure Functions
- Keine Abhängigkeiten zu anderen Layern
- Testbar durch pure Funktionen

### 7. Converters (XAML Value Converters)

**Verantwortlichkeit**: Wert-Konvertierung für Databinding

- `BoolToVisibilityConverter`: bool → Visibility
- `ColorToBrushConverter`: Hex-String → SolidColorBrush
- `ImageStretchConverter`: ImageStretchMode → Stretch
- `BoolToFontWeightConverter`: bool → FontWeight
- `BoolToFontStyleConverter`: bool → FontStyle
- `TypeToVisibilityConverter`: Type → Visibility
- `ShapeTypeToVisibilityConverter`: ShapeType → Visibility

**Design-Entscheidungen**:
- IValueConverter-Interface
- Einzel-Verantwortlichkeit pro Converter
- Bidirektionale Konvertierung wo sinnvoll

## Dependency Injection

### ServiceContainer
Einfaches DI-Container-System:
```csharp
// Registration (App.xaml.cs)
Services.Register<ILayoutStorageService, LayoutStorageService>();

// Resolution
var service = App.Services.Resolve<ILayoutStorageService>();
```

**Features**:
- Singleton-Lifetime
- Lazy-Initialization
- Type-Safe Resolution

**Limitierungen**:
- Nur Singleton (keine Scoped/Transient)
- Keine Constructor-Injection
- Manuelle Registration

## Data Flow

### Element Manipulation
```
User Action (View)
    ↓
Command (ViewModel)
    ↓
Update Model (LayoutElementBase)
    ↓
INotifyPropertyChanged
    ↓
UI Update (View)
```

### Undo/Redo
```
User Action
    ↓
Create UndoRedoAction
    ↓
Add to UndoRedoService
    ↓
Execute Undo/Redo
    ↓
Restore Model State
    ↓
UI Update
```

### Serialization
```
LayoutDocument
    ↓
JSON (with LayoutElementConverter)
    ↓
Polymorphic Deserialization
    ↓
LayoutDocument
```

## Threading Model

- **UI Thread**: Alle UI-Updates, Commands
- **Background Threads**: File I/O (async/await)
- **No explicit threading**: WPF Dispatcher automatisch

## Error Handling

- **Try/Catch**: In Services für I/O-Operationen
- **Null-Checks**: Überall wo nötig
- **Validation**: Im ViewModel (CanExecute)
- **User Feedback**: MessageBox für Fehler

## Performance-Überlegungen

### Aktuelle Optimierungen
- ObservableCollection für minimale Updates
- Lazy-Initialization von Services
- RenderTransform statt Layout-Updates

### Zukünftige Optimierungen
- Virtualisierung für viele Elemente
- Dirty Tracking für Serialisierung
- Render Caching
- Background Rendering

## Testbarkeit

### Was ist testbar?
- ✅ ViewModels (Commands, Properties)
- ✅ Services (Business Logic)
- ✅ Helpers (Pure Functions)
- ✅ Models (Clone, Validation)

### Was ist schwer testbar?
- ❌ Views (XAML)
- ❌ Custom Controls (Rendering)
- ❌ Adorners (Visual Layer)

## Erweiterbarkeit

### Neue Element-Typen hinzufügen
1. Klasse von `LayoutElementBase` ableiten
2. Properties hinzufügen
3. `Clone()` implementieren
4. `ElementType` setzen
5. DataTemplate in XAML erstellen
6. LayoutElementConverter erweitern
7. Toolbox-Button hinzufügen

### Neue Services hinzufügen
1. Interface definieren
2. Implementierung erstellen
3. In ServiceContainer registrieren
4. In ViewModels verwenden

## Best Practices

1. **ViewModels**:
   - Keine View-Referenzen
   - Commands für alle Actions
   - SetProperty für Updates

2. **Models**:
   - INotifyPropertyChanged implementieren
   - Immutable IDs
   - Clone-Methode

3. **Services**:
   - Interface-First
   - Async für I/O
   - Event für State Changes

4. **Views**:
   - XAML-first
   - Minimal Code-Behind
   - DataTemplates für Polymorphie

## Bekannte Einschränkungen

1. **Einfaches DI**: Kein vollwertiger Container
2. **Keine Plugin-Architektur**: Noch fest verdrahtet
3. **Synchrone Rendering**: Keine Background-Threads
4. **Globaler State**: Services sind Singletons

## Zukünftige Architektur-Verbesserungen

1. **MEF für Plugins**: Modulare Erweiterungen
2. **CQRS für Undo/Redo**: Separate Command/Query-Models
3. **Repository-Pattern**: Abstraction für Storage
4. **Unit of Work**: Transaktionale Änderungen
5. **Event Aggregator**: Loose-Coupled Events
