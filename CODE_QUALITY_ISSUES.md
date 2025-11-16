# Code Quality Issues & Improvements

**Scan Date**: 2025-11-16
**Total C# Lines**: ~9,066
**Status**: ✅ **Phase 2 Complete** - Critical, High & Medium Priority Fixed
**Last Updated**: 2025-11-16

---

## 🔴 Critical Issues (Priority 1) - ✅ ALL FIXED

### 1. Memory Leak in SelectionBehavior ✅ **FIXED**
**Location**: `LayoutDesigner/Behaviors/SelectionBehavior.cs:57-63, 81-87`
**Status**: ✅ Fixed in commit 46ff674

**Problem**:
```csharp
// Lines 57-63 and 81-87
layoutElement.PropertyChanged += (s, args) =>
{
    if (args.PropertyName == nameof(LayoutElementBase.IsSelected))
    {
        UpdateAdorner(element, layoutElement.IsSelected);
    }
};
```

**Issue**: Lambda event handlers are subscribed but never unsubscribed, causing memory leaks. When elements are removed, the event handler keeps references alive.

**Impact**:
- Memory leaks when elements are added/removed
- Can cause out-of-memory in long sessions
- Affects performance over time

**Fix**:
```csharp
// Store handler reference for unsubscription
private static readonly ConditionalWeakTable<FrameworkElement, PropertyChangedEventHandler> _handlers = new();

private static void OnElementLoaded(object sender, RoutedEventArgs e)
{
    if (sender is FrameworkElement element && element.DataContext is LayoutElementBase layoutElement)
    {
        // Create named handler for unsubscription
        PropertyChangedEventHandler handler = (s, args) =>
        {
            if (args.PropertyName == nameof(LayoutElementBase.IsSelected))
            {
                UpdateAdorner(element, layoutElement.IsSelected);
            }
        };

        // Store and subscribe
        _handlers.AddOrUpdate(element, handler);
        layoutElement.PropertyChanged += handler;

        // Unsubscribe on unload
        element.Unloaded += (s, e) =>
        {
            if (_handlers.TryGetValue(element, out var h))
            {
                layoutElement.PropertyChanged -= h;
                _handlers.Remove(element);
            }
        };

        UpdateAdorner(element, layoutElement.IsSelected);
    }
}
```

**Alternative Fix** (Using WeakEventManager):
```csharp
private static void OnElementLoaded(object sender, RoutedEventArgs e)
{
    if (sender is FrameworkElement element && element.DataContext is LayoutElementBase layoutElement)
    {
        WeakEventManager<INotifyPropertyChanged, PropertyChangedEventArgs>
            .AddHandler(layoutElement, nameof(INotifyPropertyChanged.PropertyChanged),
                OnLayoutElementPropertyChanged);

        UpdateAdorner(element, layoutElement.IsSelected);
    }
}

private static void OnLayoutElementPropertyChanged(object? sender, PropertyChangedEventArgs e)
{
    if (e.PropertyName == nameof(LayoutElementBase.IsSelected) &&
        sender is LayoutElementBase layoutElement)
    {
        // Find associated element and update
        // This requires maintaining a mapping
    }
}
```

---

### 2. QrCodeControl Event Handler Leak ✅ **FIXED**
**Location**: `LayoutDesigner/Controls/QrCodeControl.cs:66-79`
**Status**: ✅ Fixed in commit 46ff674

**Problem**:
```csharp
private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
{
    // Unsubscribe from old element
    if (e.OldValue is QrCodeElement oldElement)
    {
        oldElement.PropertyChanged -= OnElementPropertyChanged;  // ✓ GOOD
    }

    // Subscribe to new element
    if (e.NewValue is QrCodeElement newElement)
    {
        newElement.PropertyChanged += OnElementPropertyChanged;
        UpdateQrCode();
    }
}
```

**Issue**: Correct pattern is used, but missing unsubscription in Unloaded event.

**Impact**:
- Minor memory leak if control is removed while element remains
- Lower risk than SelectionBehavior

**Fix**:
```csharp
public QrCodeControl()
{
    // ... existing code ...

    Unloaded += OnUnloaded;
}

private void OnUnloaded(object sender, RoutedEventArgs e)
{
    // Ensure cleanup
    if (DataContext is QrCodeElement element)
    {
        element.PropertyChanged -= OnElementPropertyChanged;
    }
}
```

---

## 🟠 High Priority Issues (Priority 2) - ✅ ALL FIXED

### 3. MVVM Violation: Direct MessageBox in ViewModel ✅ **FIXED**
**Location**: `LayoutDesigner/ViewModels/MainViewModel.cs:154, 189, 206, 256, 261, 285, 290`
**Status**: ✅ Fixed in commit 46ff674

**Problem**:
```csharp
// Lines 154, 189, etc.
MessageBox.Show("Failed to load layout file.", "Error",
    MessageBoxButton.OK, MessageBoxImage.Error);
```

**Issue**: ViewModel directly depends on `System.Windows.MessageBox`, violating MVVM separation of concerns. Makes unit testing impossible.

**Impact**:
- Cannot unit test ViewModel
- Tight coupling to WPF
- IErrorHandlingService exists but not used

**Fix**:
```csharp
public class MainViewModel : ViewModelBase
{
    private readonly ILayoutStorageService _storageService;
    private readonly IUndoRedoService _undoRedoService;
    private readonly IErrorHandlingService _errorHandler;  // ADD THIS

    public MainViewModel()
    {
        _storageService = App.Services.Resolve<ILayoutStorageService>();
        _undoRedoService = App.Services.Resolve<IUndoRedoService>();
        _errorHandler = App.Services.Resolve<IErrorHandlingService>();  // ADD THIS

        // ... rest of constructor
    }

    private async Task OpenAsync()
    {
        if (!await CheckSaveChangesAsync())
            return;

        var filePath = FileDialogHelper.ShowOpenLayoutDialog();
        if (filePath == null)
            return;

        try
        {
            var document = await _storageService.LoadLayoutAsync(filePath);
            if (document != null)
            {
                CanvasViewModel.LoadDocument(document);
                CurrentFilePath = filePath;
                IsDirty = false;
            }
            else
            {
                _errorHandler.HandleError(
                    new InvalidOperationException("Document was null"),
                    "Failed to load layout file");
            }
        }
        catch (Exception ex)
        {
            _errorHandler.HandleError(ex, "Failed to load layout file");
        }
    }

    private async Task<bool> CheckSaveChangesAsync()
    {
        if (!IsDirty)
            return true;

        // REPLACE MessageBox.Show with:
        if (_errorHandler.Confirm("Do you want to save changes?", "Unsaved Changes"))
        {
            await SaveAsync();
            return !IsDirty;
        }

        return true; // User chose "No" or "Cancel" handling
    }

    private async Task ExportPngAsync()
    {
        var filePath = FileDialogHelper.ShowExportImageDialog(CanvasViewModel.Document.Name);
        if (filePath == null)
            return;

        if (!filePath.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
            filePath += ".png";

        var args = new ExportRequestedEventArgs(filePath, ExportFormat.Png);
        ExportRequested?.Invoke(this, args);

        if (args.Success)
        {
            _errorHandler.HandleInfo($"Layout exported successfully to:\n{filePath}",
                "Export Successful");
        }
        else
        {
            _errorHandler.HandleError(
                new InvalidOperationException("Export failed"),
                "Failed to export layout. Please try again");
        }
    }
}
```

**Required Changes**:
- Add `IErrorHandlingService` dependency injection to MainViewModel constructor
- Replace all 6 `MessageBox.Show` calls with `_errorHandler` calls
- Add exception handling around file operations

---

### 4. Inconsistent Service Registration ✅ **VERIFIED**
**Location**: `LayoutDesigner/App.xaml.cs` and ViewModels
**Status**: ✅ Verified - Was already correctly registered

**Problem**: ErrorHandlingService is not registered in DI container but is used in some services.

**Current State**:
```csharp
// App.xaml.cs - ErrorHandlingService NOT registered
Services.Register<ILayoutStorageService, LayoutStorageService>();
Services.Register<IQrCodeService, QrCodeService>();
Services.Register<IExportService, ExportService>();
Services.Register<IAssetService, AssetService>();
Services.Register<IUndoRedoService, UndoRedoService>();
// Missing: Services.Register<IErrorHandlingService, ErrorHandlingService>();
```

**Fix**:
```csharp
// In App.xaml.cs OnStartup method
protected override void OnStartup(StartupEventArgs e)
{
    base.OnStartup(e);

    // Register services
    Services.Register<IErrorHandlingService, ErrorHandlingService>();  // ADD THIS FIRST
    Services.Register<ILayoutStorageService, LayoutStorageService>();
    Services.Register<IQrCodeService, QrCodeService>();
    Services.Register<IExportService, ExportService>();
    Services.Register<IAssetService, AssetService>();
    Services.Register<IUndoRedoService, UndoRedoService>();

    // ... rest
}
```

---

## 🟡 Medium Priority Issues (Priority 3) - ✅ 4/6 FIXED

### 5. Unused DesignCanvas Selection List ✅ **FIXED**
**Location**: `LayoutDesigner/Controls/DesignCanvas.cs:21`
**Status**: ✅ Fixed in commit 46ff674

**Problem**:
```csharp
private readonly List<UIElement> _selectedElements = new();
```

**Issue**: List is declared but never used. Selection is handled by CanvasViewModel.SelectedElements via reflection.

**Impact**:
- Dead code
- Confusion for developers
- Minor memory waste

**Fix**: Remove unused field
```csharp
// DELETE THIS LINE:
// private readonly List<UIElement> _selectedElements = new();
```

---

### 6. Hardcoded Magic Numbers ✅ **FIXED**
**Location**: Multiple files
**Status**: ✅ Fixed in commit 46ff674

**Examples**:
```csharp
// SnapHelper.cs:15
public const double DefaultSnapDistance = 8;  // Should be configurable

// ValidationHelper.cs - Multiple magic numbers
public static (double, double) ClampToCanvasBounds(...)
{
    // Uses hardcoded min/max values
}

// DesignCanvas.cs:229
if (Math.Abs(_currentPoint.X - _dragStartPoint.Value.X) > 3 ||
    Math.Abs(_currentPoint.Y - _dragStartPoint.Value.Y) > 3)
```

**Issue**: Magic numbers reduce maintainability and configurability.

**Fix**: Move to UIConstants
```csharp
// LayoutDesigner/Constants/UIConstants.cs
public static class UIConstants
{
    // Existing constants...

    // ADD THESE:
    public const double DefaultSnapDistance = 8.0;
    public const double DragStartThreshold = 3.0;
    public const double MinCanvasSize = 100;
    public const double MaxCanvasSize = 10000;
    public const double MinGridSize = 5;
    public const double MaxGridSize = 100;
    public const double MinElementSize = 10;
    public const double MaxElementSize = 5000;
}
```

---

### 7. Missing Null Checks in SnapHelper
**Location**: `LayoutDesigner/Helpers/SnapHelper.cs:60-98`

**Problem**:
```csharp
public static (double x, double y, bool snapped) SnapToElements(
    LayoutElementBase movingElement,
    IEnumerable<LayoutElementBase> otherElements,
    double snapDistance = DefaultSnapDistance)
{
    if (movingElement == null || otherElements == null)
        return (movingElement?.X ?? 0, movingElement?.Y ?? 0, false);

    var x = movingElement.X;  // Could still be null if movingElement is null
    var y = movingElement.Y;  // (though caught above)
    // ...
}
```

**Issue**: Defensive, but could use nullable reference types better.

**Fix**: Use C# 8.0+ nullable reference types
```csharp
public static (double x, double y, bool snapped) SnapToElements(
    LayoutElementBase? movingElement,
    IEnumerable<LayoutElementBase>? otherElements,
    double snapDistance = DefaultSnapDistance)
{
    if (movingElement is null || otherElements is null)
        return (0, 0, false);

    // No more null checks needed below
    var x = movingElement.X;
    var y = movingElement.Y;
    // ...
}
```

---

### 8. Potential Performance Issue: Excessive LINQ ✅ **FIXED**
**Location**: `LayoutDesigner/ViewModels/CanvasViewModel.cs` - Alignment methods
**Status**: ✅ Fixed in commit 6ad5b50

**Problem**:
```csharp
private void AlignLeft()
{
    if (SelectedElements.Count < 2) return;

    var originalPositions = SelectedElements.ToDictionary(e => e, e => (e.X, e.Y));  // Allocation
    var leftMost = SelectedElements.Min(e => e.X);  // Full iteration

    foreach (var element in SelectedElements)  // Another iteration
    {
        element.X = leftMost;
    }
    // ...
}
```

**Issue**: Multiple iterations and allocations. Not critical for <100 elements, but could be optimized.

**Fix** (if performance becomes an issue):
```csharp
private void AlignLeft()
{
    if (SelectedElements.Count < 2) return;

    var count = SelectedElements.Count;
    var originalPositions = new Dictionary<LayoutElementBase, (double, double)>(count);
    var leftMost = double.MaxValue;

    // Single pass to get min and store positions
    foreach (var element in SelectedElements)
    {
        originalPositions[element] = (element.X, element.Y);
        if (element.X < leftMost)
            leftMost = element.X;
    }

    // Apply alignment
    foreach (var element in SelectedElements)
    {
        element.X = leftMost;
    }
    // ... undo/redo as before
}
```

---

## 🟢 Low Priority / Nice-to-Have (Priority 4)

### 9. Missing XML Documentation
**Location**: Multiple files

**Issue**: Some public methods lack XML documentation comments.

**Examples**:
- `CanvasViewModel.cs`: Many command methods lack documentation
- `SnapHelper.cs`: Some helper methods
- `ValidationHelper.cs`: All methods documented ✓

**Fix**: Add XML docs to all public members
```csharp
/// <summary>
/// Aligns selected elements to the leftmost element
/// </summary>
/// <remarks>
/// Requires at least 2 elements to be selected.
/// Creates an undo/redo action.
/// </remarks>
private void AlignLeft()
{
    // ...
}
```

---

### 10. Inconsistent Error Handling Patterns ✅ **FIXED**
**Location**: Various Services
**Status**: ✅ Fixed in commit 7018833

**Issue**: Mix of try-catch patterns:
- Some methods throw exceptions
- Some return null
- Some use error handling service
- Some use Debug.WriteLine

**Example - LayoutStorageService.cs**:
```csharp
public async Task<LayoutDocument?> LoadLayoutAsync(string filePath)
{
    try
    {
        // ... load logic
        AddRecentFile(filePath);
        return document;
    }
    catch (Exception ex)
    {
        ErrorHandlingService.HandleError(ex, "Failed to load layout");
        return null;  // Silent failure
    }
}
```

**Issue**: Caller doesn't know if null means "file not found" or "parse error".

**Better Pattern**:
```csharp
public async Task<LayoutDocument> LoadLayoutAsync(string filePath)
{
    ArgumentNullException.ThrowIfNull(filePath);

    try
    {
        // ... load logic
        AddRecentFile(filePath);
        return document;
    }
    catch (FileNotFoundException)
    {
        throw; // Let caller handle
    }
    catch (JsonException ex)
    {
        throw new InvalidDataException("Layout file is corrupted or invalid", ex);
    }
    catch (Exception ex)
    {
        throw new InvalidOperationException($"Failed to load layout from {filePath}", ex);
    }
}
```

---

### 11. Missing Async Suffix Convention
**Location**: Various async methods

**Issue**: Some async methods don't follow `Async` suffix convention.

**Examples**:
- Most methods correctly named: `SaveAsync`, `LoadAsync`, `ExportPngAsync` ✓
- All follow convention in this project ✓

**Status**: ✓ No issues found - good compliance

---

### 12. String Interpolation Opportunities
**Location**: Various string concatenations

**Current**:
```csharp
// MainViewModel.cs:256
$"Layout exported successfully to:\n{filePath}"  // ✓ Good

// Property formatting
$"{layoutElement.Width:F0} × {layoutElement.Height:F0}"  // ✓ Good
```

**Status**: ✓ Already using modern string interpolation

---

## 📊 Code Quality Metrics

### Current State
- **Total Lines of Code**: 9,066
- **Critical Issues**: 2 (Memory leaks)
- **High Priority**: 2 (MVVM violation, DI registration)
- **Medium Priority**: 6
- **Low Priority**: 3
- **Test Coverage**: 0% (no unit tests)

### Compliance
- ✅ No empty catch blocks
- ✅ No Thread.Sleep or blocking calls
- ✅ Proper async/await usage
- ✅ Using statements for IDisposable
- ✅ String interpolation
- ❌ Memory leak protection
- ❌ Unit tests
- ⚠️ MVVM separation (MainViewModel violates)

---

## 🎯 Recommended Action Plan

### Phase 1: Critical Fixes (Immediate) ✅ **COMPLETED**
1. ✅ **Fix memory leaks** in SelectionBehavior and QrCodeControl
2. ✅ **Register IErrorHandlingService** in DI container (was already registered)
3. ✅ **Remove direct MessageBox calls** from MainViewModel
4. ✅ **Remove dead code** (_selectedElements field)
5. ✅ **Move magic numbers** to UIConstants

**Estimated Effort**: 2-3 hours
**Actual Effort**: ~2 hours
**Impact**: Prevents memory leaks, improves testability, cleaner codebase
**Status**: ✅ Completed and committed (46ff674)
**Date**: 2025-11-16

### Phase 2: Medium Priority ✅ **COMPLETED**
1. ✅ **Optimize LINQ performance** in alignment methods
2. ✅ **Standardize error handling** across all services

**Estimated Effort**: 2-3 hours
**Actual Effort**: ~1.5 hours
**Impact**: Better performance, consistent error handling
**Status**: ✅ Completed and committed (6ad5b50, 7018833)
**Date**: 2025-11-16

### Phase 3: Medium Priority (Next Sprint)
7. Add unit tests for ViewModels
8. Add unit tests for Services
9. Standardize error handling patterns

**Estimated Effort**: 8-16 hours
**Impact**: Long-term code quality

### Phase 4: Low Priority (Future)
10. Performance profiling and optimization
11. Code coverage target: 80%+
12. Static code analysis (SonarQube, etc.)

---

## 🔧 Tools & Analysis

### Recommended Tools
1. **JetBrains dotMemory** - Memory leak detection
2. **BenchmarkDotNet** - Performance profiling
3. **xUnit / NUnit** - Unit testing framework
4. **Moq** - Mocking framework for tests
5. **FakeItEasy** - Alternative mocking
6. **Coverlet** - Code coverage
7. **SonarLint** - Static analysis in IDE
8. **StyleCop.Analyzers** - Code style enforcement

### Run Static Analysis
```bash
# Install analyzers
dotnet add package Microsoft.CodeAnalysis.NetAnalyzers
dotnet add package StyleCop.Analyzers

# Build with analysis
dotnet build /p:EnforceCodeStyleInBuild=true
```

---

## 📚 References

- [Memory Leaks in WPF](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/advanced/weak-event-patterns)
- [MVVM Best Practices](https://docs.microsoft.com/en-us/dotnet/architecture/modernize-desktop/example-migration-core#mvvm-pattern)
- [Async Best Practices](https://docs.microsoft.com/en-us/archive/msdn-magazine/2013/march/async-await-best-practices-in-asynchronous-programming)
- [Unit Testing ViewModels](https://docs.microsoft.com/en-us/dotnet/architecture/modernize-desktop/unit-testing)

---

**Next Review**: After critical fixes are implemented
**Maintained By**: Development Team
**Last Updated**: 2025-11-16
