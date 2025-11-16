# Integration Guide - Neue Features

Schritt-für-Schritt-Anleitung zur Integration aller neuen Features in die bestehende Anwendung.

## Übersicht

Folgende Features wurden implementiert und müssen integriert werden:

1. ✅ Performance-Optimierungen (bereits integriert)
2. ✅ Code-Quality-Verbesserungen (bereit zur Nutzung)
3. 🔧 Visual Features (benötigen Integration)
4. 🔧 Keyboard-Shortcuts (benötigen Integration)
5. 🔧 Grouping (benötigen Integration)
6. 🔧 Settings (benötigen Integration)

## 1. CanvasViewModel erweitern

### Schritt 1.1: Partial Method für Grouping hinzufügen

**Datei**: `LayoutDesigner/ViewModels/CanvasViewModel.cs`

**Am Anfang der Klasse** (nach den using-Statements):
```csharp
public partial class CanvasViewModel
{
    // Partial method declaration for grouping initialization
    partial void InitializeGroupingCommands();
```

**Im Constructor** (nach den bestehenden Commands):
```csharp
public CanvasViewModel()
{
    // ... existing code ...

    LockCommand = new RelayCommand(LockElements, () => SelectedElements.Count > 0);
    UnlockCommand = new RelayCommand(UnlockElements, () => SelectedElements.Count > 0);

    // Initialize grouping commands
    InitializeGroupingCommands();
}
```

**Result**: Grouping-Commands sind jetzt verfügbar (Ctrl+G, Ctrl+Shift+G)

---

## 2. MainWindow erweitern

### Schritt 2.1: Keyboard-Handler hinzufügen

**Datei**: `LayoutDesigner/Views/MainWindow.xaml`

**Im Window-Tag** hinzufügen:
```xaml
<Window x:Class="LayoutDesigner.Views.MainWindow"
        ...
        PreviewKeyDown="OnPreviewKeyDown">
```

**Datei**: `LayoutDesigner/Views/MainWindow.xaml.cs`

**Method hinzufügen**:
```csharp
private void OnPreviewKeyDown(object sender, KeyEventArgs e)
{
    if (DataContext is not MainViewModel viewModel)
        return;

    // Let CanvasViewModel handle the key
    if (viewModel.CanvasViewModel.HandleKeyDown(e.Key, Keyboard.Modifiers))
    {
        e.Handled = true;
    }
}
```

**Result**: Alle Keyboard-Shortcuts funktionieren jetzt (Arrow-Keys, Grouping, etc.)

---

## 3. DesignCanvas erweitern

### Schritt 3.1: Adorner-Layer Setup

**Datei**: `LayoutDesigner/Controls/DesignCanvas.cs`

**Felder hinzufügen**:
```csharp
private SnapLinesAdorner? _snapLinesAdorner;
private SelectionRectangleAdorner? _selectionAdorner;
private AdornerLayer? _adornerLayer;
```

**Im Constructor** (nach existing initialization):
```csharp
public DesignCanvas()
{
    // ... existing code ...

    MouseLeftButtonDown += OnMouseLeftButtonDown;
    MouseLeftButtonUp += OnMouseLeftButtonUp;
    MouseMove += OnMouseMove;

    // Initialize adorners when loaded
    Loaded += OnLoaded;
}

private void OnLoaded(object sender, RoutedEventArgs e)
{
    // Get or create adorner layer
    _adornerLayer = AdornerLayer.GetAdornerLayer(this);

    if (_adornerLayer != null)
    {
        // Create and add snap lines adorner
        _snapLinesAdorner = new SnapLinesAdorner(this);
        _adornerLayer.Add(_snapLinesAdorner);

        // Create and add selection rectangle adorner
        _selectionAdorner = new SelectionRectangleAdorner(this);
        _adornerLayer.Add(_selectionAdorner);
    }
}
```

### Schritt 3.2: Rectangle Selection Integration

**Im OnMouseLeftButtonDown**:
```csharp
private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
{
    var point = e.GetPosition(this);

    // Check if clicking on empty canvas (not on an element)
    if (e.Source == this)
    {
        // Start rectangle selection
        _selectionAdorner?.StartSelection(point);
        CaptureMouse();
        e.Handled = true;
        return;
    }

    // ... existing element drag code ...
}
```

**Im OnMouseMove**:
```csharp
private void OnMouseMove(object sender, MouseEventArgs e)
{
    var currentPoint = e.GetPosition(this);

    // Update selection rectangle if active
    if (_selectionAdorner?.IsSelecting == true)
    {
        _selectionAdorner.UpdateSelection(currentPoint);
        e.Handled = true;
        return;
    }

    // ... existing drag code ...
}
```

**Im OnMouseLeftButtonUp**:
```csharp
private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
{
    // End rectangle selection if active
    if (_selectionAdorner?.IsSelecting == true)
    {
        var rect = _selectionAdorner.EndSelection();

        // Select all elements within rectangle
        if (DataContext is ICanvasViewModel viewModel)
        {
            SelectElementsInRectangle(rect, viewModel);
        }

        ReleaseMouseCapture();
        e.Handled = true;
        return;
    }

    // ... existing code ...
}

private void SelectElementsInRectangle(Rect rect, ICanvasViewModel viewModel)
{
    var selectedElements = new List<LayoutElementBase>();

    foreach (var element in viewModel.Elements)
    {
        var elementRect = new Rect(element.X, element.Y, element.Width, element.Height);

        // Check if element intersects with selection rectangle
        if (rect.IntersectsWith(elementRect))
        {
            selectedElements.Add(element);
        }
    }

    // Update selection
    viewModel.SelectedElements.Clear();
    foreach (var element in selectedElements)
    {
        viewModel.SelectedElements.Add(element);
    }
}
```

### Schritt 3.3: Snap Lines Integration

**Im OnMouseMove** (im drag-section):
```csharp
private void OnMouseMove(object sender, MouseEventArgs e)
{
    // ... selection rectangle code ...

    if (_isDragging && e.Source is FrameworkElement element &&
        element.DataContext is LayoutElementBase layoutElement)
    {
        var newX = layoutElement.X + delta.X;
        var newY = layoutElement.Y + delta.Y;

        // Clear previous snap lines
        _snapLinesAdorner?.Clear();

        // Try element-to-element snapping first
        if (DataContext is ICanvasViewModel viewModel)
        {
            var (snapX, snapY, snapped) = SnapHelper.SnapToElements(
                layoutElement,
                viewModel.Elements,
                ApplicationSettings.Instance.SnapDistance);

            if (snapped)
            {
                newX = snapX;
                newY = snapY;

                // Show snap lines
                ShowSnapLinesForElement(layoutElement, snapX, snapY);
            }
        }

        // Snap to grid if enabled
        if (SnapToGrid)
        {
            newX = SnapHelper.SnapToGrid(newX, GridSize);
            newY = SnapHelper.SnapToGrid(newY, GridSize);
        }

        layoutElement.X = Math.Max(0, newX);
        layoutElement.Y = Math.Max(0, newY);

        _dragStartPoint = currentPoint;
        e.Handled = true;
    }
}

private void ShowSnapLinesForElement(LayoutElementBase element, double snapX, double snapY)
{
    if (_snapLinesAdorner == null || !ApplicationSettings.Instance.ShowSnapLines)
        return;

    // Show vertical line at snap position
    if (Math.Abs(element.X - snapX) < 0.1)
    {
        _snapLinesAdorner.ShowVerticalLine(snapX, 0, ActualHeight);
    }

    // Show horizontal line at snap position
    if (Math.Abs(element.Y - snapY) < 0.1)
    {
        _snapLinesAdorner.ShowHorizontalLine(snapY, 0, ActualWidth);
    }
}
```

**Im OnMouseLeftButtonUp**:
```csharp
private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
{
    // ... existing code ...

    // Clear snap lines when drag ends
    _snapLinesAdorner?.Clear();

    // ... existing code ...
}
```

---

## 4. Settings Integration

### Schritt 4.1: Settings beim App-Start laden

**Datei**: `LayoutDesigner/App.xaml.cs`

**In OnStartup**:
```csharp
protected override void OnStartup(StartupEventArgs e)
{
    base.OnStartup(e);

    // Load settings
    var settings = ApplicationSettings.Instance;

    // Log settings location (optional)
    Debug.WriteLine($"Settings loaded from: {Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\LayoutDesigner\\settings.json");
}
```

### Schritt 4.2: Settings an CanvasViewModel binden

**Datei**: `LayoutDesigner/ViewModels/CanvasViewModel.cs`

**Constructor erweitern**:
```csharp
public CanvasViewModel()
{
    // ... existing code ...

    // Initialize from settings
    var settings = ApplicationSettings.Instance;
    ShowGrid = settings.ShowGrid;
    GridSize = settings.GridSize;
    SnapToGrid = settings.SnapToGrid;
    Zoom = settings.DefaultZoom;
}
```

**Properties erweitern** (optional - 2-way binding zu Settings):
```csharp
public bool ShowGrid
{
    get => _showGrid;
    set
    {
        if (SetProperty(ref _showGrid, value))
        {
            ApplicationSettings.Instance.ShowGrid = value;
        }
    }
}

public double GridSize
{
    get => _gridSize;
    set
    {
        if (SetProperty(ref _gridSize, value))
        {
            ApplicationSettings.Instance.GridSize = value;
        }
    }
}
```

### Schritt 4.3: Recent Files Integration

**Datei**: `LayoutDesigner/ViewModels/MainViewModel.cs`

**In OpenCommand**:
```csharp
private void Open()
{
    var dialog = new OpenFileDialog
    {
        Filter = "Layout Files (*.json)|*.json|All Files (*.*)|*.*",
        InitialDirectory = ApplicationSettings.Instance.LastExportPath
    };

    if (dialog.ShowDialog() == true)
    {
        // Load file...

        // Add to recent files
        ApplicationSettings.Instance.AddRecentFile(dialog.FileName);
    }
}
```

**In SaveCommand**:
```csharp
private void Save()
{
    if (string.IsNullOrEmpty(_currentFilePath))
    {
        SaveAs();
        return;
    }

    // Save file...

    // Add to recent files
    ApplicationSettings.Instance.AddRecentFile(_currentFilePath);
}
```

---

## 5. Menu Integration

### Schritt 5.1: Group/Ungroup Menu Items

**Datei**: `LayoutDesigner/Views/MainWindow.xaml`

**Im Edit-Menu** (nach den bestehenden Items):
```xaml
<MenuItem Header="_Edit">
    <!-- ... existing items ... -->
    <Separator/>
    <MenuItem Header="_Group"
              Command="{Binding CanvasViewModel.GroupCommand}"
              InputGestureText="Ctrl+G"/>
    <MenuItem Header="_Ungroup"
              Command="{Binding CanvasViewModel.UngroupCommand}"
              InputGestureText="Ctrl+Shift+G"/>
</MenuItem>
```

### Schritt 5.2: Recent Files Menu (Optional)

**Im File-Menu**:
```xaml
<MenuItem Header="_File">
    <MenuItem Header="_New" Command="{Binding NewCommand}" InputGestureText="Ctrl+N"/>
    <MenuItem Header="_Open..." Command="{Binding OpenCommand}" InputGestureText="Ctrl+O"/>
    <Separator/>
    <MenuItem Header="Recent Files" ItemsSource="{Binding Source={x:Static models:ApplicationSettings.Instance}, Path=RecentFiles}">
        <MenuItem.ItemContainerStyle>
            <Style TargetType="MenuItem">
                <Setter Property="Header" Value="{Binding}"/>
                <Setter Property="Command" Value="{Binding DataContext.OpenRecentCommand, RelativeSource={RelativeSource AncestorType=Window}}"/>
                <Setter Property="CommandParameter" Value="{Binding}"/>
            </Style>
        </MenuItem.ItemContainerStyle>
    </MenuItem>
    <!-- ... existing items ... -->
</MenuItem>
```

---

## 6. Async Commands Integration (Optional aber empfohlen)

### Schritt 6.1: MainViewModel auf Async umstellen

**Datei**: `LayoutDesigner/ViewModels/MainViewModel.cs`

**Commands ändern**:
```csharp
public ICommand SaveCommand { get; }
public ICommand OpenCommand { get; }
public ICommand ExportPngCommand { get; }
public ICommand ExportJpgCommand { get; }

public MainViewModel()
{
    // Use async commands for file operations
    SaveCommand = new AsyncRelayCommand(SaveAsync, CanSave);
    OpenCommand = new AsyncRelayCommand(OpenAsync);
    ExportPngCommand = new AsyncRelayCommand(ExportPngAsync);
    ExportJpgCommand = new AsyncRelayCommand(ExportJpgAsync);
}

private async Task SaveAsync()
{
    try
    {
        await Task.Run(() =>
        {
            // Save operation
            _storageService.Save(CanvasViewModel.Document, _currentFilePath);
        });

        ApplicationSettings.Instance.AddRecentFile(_currentFilePath);
    }
    catch (Exception ex)
    {
        _errorHandler.HandleError(ex, "Failed to save file");
    }
}
```

---

## 7. Error Handling Integration

### Schritt 7.1: Service registrieren

**Datei**: `LayoutDesigner/App.xaml.cs` (oder DI-Container):

```csharp
// In service registration
Services.Register<IErrorHandlingService, ErrorHandlingService>();
```

### Schritt 7.2: In ViewModels verwenden

**Beispiel in MainViewModel**:
```csharp
private readonly IErrorHandlingService _errorHandler;

public MainViewModel()
{
    _errorHandler = App.Services.Resolve<IErrorHandlingService>();
}

private async Task ExportPngAsync()
{
    try
    {
        // Export logic...
    }
    catch (Exception ex)
    {
        _errorHandler.HandleError(ex, "Failed to export PNG");
    }
}
```

---

## 8. Validation Integration

### Schritt 8.1: In Property-Settern

**Beispiel in LayoutElementBase**:
```csharp
public double Width
{
    get => _width;
    set
    {
        // Validate and clamp
        var validated = ValidationHelper.ClampValue(value, 1, 5000);
        SetProperty(ref _width, validated);
    }
}

public string BackgroundColor
{
    get => _backgroundColor;
    set
    {
        // Normalize color
        var normalized = ValidationHelper.NormalizeHexColor(value);
        SetProperty(ref _backgroundColor, normalized);
    }
}
```

---

## 9. Testing der Integration

### Checklist:

- [ ] **Keyboard Shortcuts**:
  - [ ] Arrow Keys bewegen Elemente
  - [ ] Shift + Arrow Keys bewegen 10px
  - [ ] Ctrl + G gruppiert Elemente
  - [ ] Ctrl + Shift + G löst Gruppe auf
  - [ ] Delete löscht Elemente

- [ ] **Rectangle Selection**:
  - [ ] Click & Drag auf Canvas erstellt Selection-Box
  - [ ] Alle Elemente im Rechteck werden selektiert
  - [ ] Visuelle Feedback mit blauem Rechteck

- [ ] **Snap Lines**:
  - [ ] Magenta Linien erscheinen beim Dragging
  - [ ] Elemente snappen zu anderen Elementen
  - [ ] Linien verschwinden nach Drag

- [ ] **Grouping**:
  - [ ] Gruppieren erstellt ElementGroup
  - [ ] Gruppe kann bewegt werden
  - [ ] Ungroup stellt Elemente wieder her
  - [ ] Undo/Redo funktioniert

- [ ] **Settings**:
  - [ ] Settings werden geladen beim Start
  - [ ] Änderungen werden automatisch gespeichert
  - [ ] Recent Files werden aktualisiert
  - [ ] Settings persistieren zwischen Sessions

- [ ] **Performance**:
  - [ ] Dragging ist flüssig
  - [ ] Keine Lags beim Snapping
  - [ ] UI bleibt responsive

---

## 10. Troubleshooting

### Problem: Keyboard Shortcuts funktionieren nicht
**Lösung**:
1. Prüfen ob `PreviewKeyDown` event verdrahtet ist
2. Prüfen ob Focus auf Window/Canvas ist
3. Prüfen ob Elemente gesperrt sind

### Problem: Snap Lines erscheinen nicht
**Lösung**:
1. Prüfen ob `ApplicationSettings.Instance.ShowSnapLines == true`
2. Prüfen ob Adorner-Layer korrekt initialisiert
3. Prüfen ob `SnapDistance` nicht zu klein

### Problem: Rectangle Selection funktioniert nicht
**Lösung**:
1. Prüfen ob Click auf Canvas (nicht auf Element)
2. Prüfen ob Adorner hinzugefügt wurde
3. Event-Handling in richtige Reihenfolge

### Problem: Grouping-Commands nicht verfügbar
**Lösung**:
1. Prüfen ob `InitializeGroupingCommands()` aufgerufen wird
2. Prüfen ob partial method deklariert ist
3. Prüfen ob mindestens 2 Elemente selektiert

### Problem: Settings werden nicht gespeichert
**Lösung**:
1. Prüfen Schreibrechte in `%AppData%`
2. Prüfen ob Auto-Save in SetProperty funktioniert
3. JSON-Datei manuell prüfen

---

## 11. Performance-Tipps

1. **Snap Lines**: Bei vielen Elementen (>100) kann Snapping langsam werden
   - Lösung: Snap Distance reduzieren oder deaktivieren

2. **Rectangle Selection**: Bei großen Canvas-Bereichen
   - Lösung: Virtualisierung (zukünftige Optimierung)

3. **Settings Auto-Save**: Jede Änderung speichert
   - Ist OK weil schnell, aber bei Bedarf Debouncing einbauen

---

## 12. Nächste Schritte (Optional)

Nach erfolgreicher Integration:

1. **Preferences Dialog** erstellen
   - Grid-Einstellungen
   - Snap-Einstellungen
   - Keyboard-Shortcuts anpassen

2. **Help Dialog** mit Shortcuts-Liste
   - Nutze `KeyboardHandler.GetAllShortcuts()`

3. **Visual Resize Handles**
   - Adorner mit 8 Handles um Element

4. **Visual Rotation Handle**
   - Adorner mit Rotation-Kreis

5. **Layers Panel**
   - TreeView mit Element-Hierarchie
   - Drag & Drop für Z-Index

---

## Zusammenfassung

Nach Abschluss dieser Integration hast du:

✅ Vollständige Keyboard-Steuerung
✅ Professionelle Rectangle-Selection
✅ Visuelle Snap-Lines
✅ Element-Gruppierung
✅ Persistente Settings
✅ Recent Files
✅ Async File-Operations
✅ Robustes Error-Handling
✅ Input-Validation

**Geschätzte Integrationszeit**: 2-3 Stunden

**Schwierigkeitsgrad**: Mittel

**Breaking Changes**: Keine (alle Features sind additiv)
