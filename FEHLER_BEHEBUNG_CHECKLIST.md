# Fehler-Behebung Checkliste (Error Fix Checklist)

Diese Checkliste hilft bei der systematischen Behebung der gefundenen Fehler.

---

## 🔴 KRITISCH - Sofort beheben (Critical - Fix Immediately)

### ✅ Phase 1A: Memory Leaks (2-3 Stunden)

- [ ] **SelectionAdorner.cs:187-198**
  - [ ] Refactor MouseEnter Handler für Resize-Thumbs
  - [ ] Refactor MouseLeave Handler für Resize-Thumbs
  - [ ] Add cleanup in Adorner removal

- [ ] **SelectionAdorner.cs:226-237**
  - [ ] Refactor MouseEnter Handler für Rotate-Thumb
  - [ ] Refactor MouseLeave Handler für Rotate-Thumb
  - [ ] Add cleanup in Adorner removal

**Lösung**:
```csharp
// Option 1: Benannte Handler-Methoden
private void CreateResizeHandle(...)
{
    thumb.MouseEnter += OnResizeHandleMouseEnter;
    thumb.MouseLeave += OnResizeHandleMouseLeave;
}

// In cleanup:
thumb.MouseEnter -= OnResizeHandleMouseEnter;
thumb.MouseLeave -= OnResizeHandleMouseLeave;
```

---

### ✅ Phase 1B: Async Void Error Handling (1-2 Stunden)

- [ ] **MainWindow.xaml.cs:260**
  - [ ] Add try-catch block to OnExportRequested
  - [ ] Add error handling service call
  - [ ] Test error scenarios

- [ ] **AssetManagerPanel.xaml.cs:74**
  - [ ] Add try-catch block to OnImportClick
  - [ ] Add error handling service call
  - [ ] Test error scenarios

**Lösung**:
```csharp
private async void OnExportRequested(object? sender, ExportRequestedEventArgs e)
{
    try
    {
        // ... existing code ...
    }
    catch (Exception ex)
    {
        var errorService = App.Services.Resolve<IErrorHandlingService>();
        errorService.HandleError(ex, "Export failed");
        e.Success = false;
    }
}
```

---

### ✅ Phase 1C: Blocking I/O Operations (2-3 Stunden)

- [ ] **LayoutStorageService.cs:124**
  - [ ] Replace `File.ReadAllText` with `File.ReadAllTextAsync`
  - [ ] Make method async if needed
  - [ ] Test functionality

- [ ] **LayoutStorageService.cs:167**
  - [ ] Replace `File.WriteAllText` with `File.WriteAllTextAsync`
  - [ ] Make method async if needed
  - [ ] Test functionality

- [ ] **TemplateService.cs:79**
  - [ ] Replace `File.WriteAllText` with `File.WriteAllTextAsync`
  - [ ] Make method async
  - [ ] Test functionality

- [ ] **TemplateService.cs:140**
  - [ ] Replace `File.ReadAllText` with `File.ReadAllTextAsync`
  - [ ] Update loop to use async/await
  - [ ] Test functionality

- [ ] **ApplicationSettings.cs:268**
  - [ ] Replace `File.WriteAllText` with `File.WriteAllTextAsync`
  - [ ] Make Save method async
  - [ ] Update all callers

- [ ] **ApplicationSettings.cs:286**
  - [ ] Replace `File.ReadAllText` with `File.ReadAllTextAsync`
  - [ ] Make Load method async
  - [ ] Update all callers

**Lösung**:
```csharp
// Before:
var json = File.ReadAllText(settingsFile);

// After:
var json = await File.ReadAllTextAsync(settingsFile);
```

---

## 🟠 HOCH - Diese Woche (High - This Week)

### ✅ Phase 2A: CancellationToken Support (3-4 Stunden)

- [ ] **IAssetService.cs & AssetService.cs**
  - [ ] Add CancellationToken parameter to ImportAssetAsync
  - [ ] Use token in async operations
  - [ ] Update callers

- [ ] **IExportService.cs & ExportService.cs**
  - [ ] Add CancellationToken to ExportToPngAsync
  - [ ] Add CancellationToken to ExportToJpgAsync
  - [ ] Use token in async operations
  - [ ] Update callers

- [ ] **ILayoutStorageService.cs & LayoutStorageService.cs**
  - [ ] Add CancellationToken to SaveLayoutAsync
  - [ ] Add CancellationToken to LoadLayoutAsync
  - [ ] Add CancellationToken to EmbedImagesAsync
  - [ ] Use token in async operations
  - [ ] Update callers

- [ ] **MainViewModel.cs**
  - [ ] Add CancellationToken to NewAsync, OpenAsync, SaveAsync
  - [ ] Create CancellationTokenSource in commands
  - [ ] Allow cancellation from UI

**Lösung**:
```csharp
public async Task<string?> ImportAssetAsync(
    string sourceFilePath, 
    CancellationToken cancellationToken = default)
{
    cancellationToken.ThrowIfCancellationRequested();
    // ... rest of code
    await File.CopyAsync(..., cancellationToken);
}
```

---

### ✅ Phase 2B: Readonly Felder (1 Stunde)

- [ ] **SelectionAdorner.cs**
  - [ ] Review all private fields
  - [ ] Mark readonly where appropriate

- [ ] **SnapLinesAdorner.cs**
  - [ ] Make `_snapLinePen` readonly if possible

- [ ] **QrCodeControl.cs**
  - [ ] Make `_qrCodeService` readonly

- [ ] **SelectionRectangleAdorner.cs**
  - [ ] Review and mark readonly where appropriate

- [ ] **DesignCanvas.cs**
  - [ ] Review and mark readonly where appropriate

**Prüfung**: Feld ist readonly wenn:
- Nur im Konstruktor zugewiesen
- Nie neu zugewiesen nach Konstruktor

---

### ✅ Phase 2C: Magic Numbers (2-3 Stunden)

- [ ] **Create UIConstants.cs file** if not exists
  - [ ] Add DPI constant (96.0)
  - [ ] Add default pixels per mm (4.0)
  - [ ] Add default font sizes
  - [ ] Add template dimensions

- [ ] **ExportService.cs**
  - [ ] Replace hardcoded 96 with UIConstants.DefaultDpi

- [ ] **TemplateService.cs**
  - [ ] Replace 340, 220 with calculated constants
  - [ ] Replace 24 with UIConstants.DefaultFontSize
  - [ ] Replace other magic numbers

**Lösung**:
```csharp
public static class UIConstants
{
    public const double DefaultDpi = 96.0;
    public const double PixelsPerMm = 4.0;
    public const double DefaultFontSize = 24.0;
    
    // Business card dimensions
    public const double BusinessCardWidthMm = 85.0;
    public const double BusinessCardHeightMm = 55.0;
    public const double BusinessCardWidth = BusinessCardWidthMm * PixelsPerMm;
    public const double BusinessCardHeight = BusinessCardHeightMm * PixelsPerMm;
}
```

---

### ✅ Phase 2D: Exception Improvements (0.5 Stunden)

- [ ] **ImagePathConverter.cs:131**
  - [ ] Change NotImplementedException to NotSupportedException
  - [ ] Add clear error message

**Lösung**:
```csharp
public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
{
    throw new NotSupportedException(
        "ConvertBack is not supported for ImagePathConverter. " +
        "This converter is for one-way binding only.");
}
```

---

### ✅ Phase 2E: AsyncRelayCommand Error Handling (1 Stunde)

- [ ] **AsyncRelayCommand.cs:51**
  - [ ] Add try-catch around ExecuteAsync call
  - [ ] Add error event or callback

- [ ] **AsyncRelayCommand.cs:131**
  - [ ] Add try-catch around ExecuteAsync call
  - [ ] Add error event or callback

**Lösung**:
```csharp
public async void Execute(object? parameter)
{
    try
    {
        await ExecuteAsync(parameter);
    }
    catch (Exception ex)
    {
        // Option 1: Event
        UnhandledException?.Invoke(this, ex);
        
        // Option 2: Service
        var errorService = App.Services.Resolve<IErrorHandlingService>();
        errorService?.HandleError(ex, "Command execution failed");
    }
}
```

---

## 🟡 MITTEL - Nächster Sprint (Medium - Next Sprint)

### ✅ Phase 3A: XML Dokumentation (4-6 Stunden)

- [ ] **Services/**
  - [ ] Document all public methods in AssetService
  - [ ] Document all public methods in ExportService
  - [ ] Document all public methods in LayoutStorageService
  - [ ] Document all public methods in QrCodeService
  - [ ] Document all public methods in TemplateService
  - [ ] Document all public methods in UndoRedoService

- [ ] **ViewModels/**
  - [ ] Document all public methods in MainViewModel
  - [ ] Document all public methods in CanvasViewModel
  - [ ] Document all command properties

- [ ] **Helpers/**
  - [ ] Document SnapHelper methods
  - [ ] Document GeometryHelper methods

**Template**:
```csharp
/// <summary>
/// [Was macht die Methode]
/// </summary>
/// <param name="[name]">[Beschreibung des Parameters]</param>
/// <returns>[Was wird zurückgegeben]</returns>
/// <exception cref="[Type]">[Wann wird Exception geworfen]</exception>
public async Task<string?> ImportAssetAsync(string sourceFilePath)
```

---

### ✅ Phase 3B: Error Handling Standardisierung (3-4 Stunden)

- [ ] **Define standard error handling strategy**
  - [ ] Document when to throw vs return null
  - [ ] Document exception types to use
  - [ ] Create error handling guidelines

- [ ] **Update Services**
  - [ ] Review LayoutStorageService error handling
  - [ ] Review ExportService error handling
  - [ ] Review AssetService error handling
  - [ ] Make consistent across all services

---

### ✅ Phase 3C: Unit Test Infrastructure (6-8 Stunden)

- [ ] **Create Test Project**
  - [ ] Add xUnit test project
  - [ ] Add Moq for mocking
  - [ ] Add FluentAssertions
  - [ ] Configure test runner

- [ ] **First Tests**
  - [ ] Write tests for LayoutStorageService
  - [ ] Write tests for QrCodeService
  - [ ] Write tests for MainViewModel
  - [ ] Aim for >50% coverage initially

- [ ] **CI/CD Integration**
  - [ ] Add test step to GitHub Actions
  - [ ] Add coverage reporting

---

### ✅ Phase 3D: Code Style (2-3 Stunden)

- [ ] **Create .editorconfig**
  - [ ] Set indent style (spaces/tabs)
  - [ ] Set line endings
  - [ ] Set charset
  - [ ] Add C# code style rules

- [ ] **Remove unused usings**
  - [ ] Run "Remove Unused Usings" in all files
  - [ ] Consider file-scoped namespaces (C# 10+)

---

## 🔵 NIEDRIG - Backlog (Low - Backlog)

### Code Quality Improvements

- [ ] Refactor duplicate code patterns
- [ ] Extract hardcoded colors to resources
- [ ] Add input validation to all public methods
- [ ] Review LINQ performance
- [ ] Add logging framework
- [ ] Add telemetry/analytics
- [ ] Improve accessibility
- [ ] Add localization support

---

## 📊 Progress Tracking

### Critical (Week 1)
```
[░░░░░░░░░░] 0% - Not Started
[████░░░░░░] 40% - In Progress  
[██████████] 100% - Complete
```

### High (Week 2)
```
[░░░░░░░░░░] 0% - Not Started
```

### Medium (Sprint 1)
```
[░░░░░░░░░░] 0% - Not Started
```

---

## 🧪 Testing Checklist

Nach jeder Phase:

- [ ] Code kompiliert ohne Errors
- [ ] Code kompiliert ohne Warnings
- [ ] Alle bestehenden Features funktionieren
- [ ] Neue Tests (wenn vorhanden) sind grün
- [ ] Code Review durchgeführt
- [ ] Dokumentation aktualisiert
- [ ] Git commit mit aussagekräftiger Message

---

## 📝 Notizen

### Phase 1 Notes
```
Start: [Datum]
Ende:  [Datum]
Probleme:
- 
Gelöst:
- 
```

### Phase 2 Notes
```
Start: [Datum]
Ende:  [Datum]
Probleme:
- 
Gelöst:
- 
```

---

**Letzte Aktualisierung**: 17. November 2025  
**Erstellt für**: Canvas Layout Designer v1.0.0  
**Maintainer**: Development Team
