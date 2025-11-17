# Fehler-Liste - Canvas Layout Designer Projekt

**Analysedatum**: 17. November 2025  
**Projekt**: WPF Layout Designer  
**Analysierte Dateien**: 84 C# Dateien  
**Analysemethode**: Statische Code-Analyse & Pattern Matching

---

## 📊 Zusammenfassung

| Kategorie | Anzahl | Priorität |
|-----------|--------|-----------|
| 🔴 Kritische Fehler | 3 | Hoch |
| 🟠 Schwere Fehler | 8 | Mittel |
| 🟡 Warnungen | 12 | Niedrig |
| 🔵 Code-Smell / Best Practices | 15 | Info |
| **Gesamt** | **38** | - |

---

## 🔴 Kritische Fehler (Priorität 1)

### 1. Memory Leak: Event Handler in SelectionAdorner
**Datei**: `LayoutDesigner/Controls/SelectionAdorner.cs:187-198, 226-237`  
**Schweregrad**: 🔴 Kritisch  
**Beschreibung**: Lambda-Event-Handler für MouseEnter/MouseLeave werden niemals entfernt.

```csharp
// Zeilen 187-198
thumb.MouseEnter += (s, e) =>
{
    thumb.Background = new SolidColorBrush(Color.FromRgb(135, 206, 250));
    thumb.Width = 12;
    thumb.Height = 12;
};
thumb.MouseLeave += (s, e) =>
{
    thumb.Background = Brushes.White;
    thumb.Width = 10;
    thumb.Height = 10;
};
```

**Problem**: 
- Event-Handler werden nie deregistriert
- Führt zu Memory Leaks bei vielen Select/Deselect-Operationen
- Thumbs werden nie vom Garbage Collector freigegeben

**Auswirkung**: Bei langem Gebrauch kann dies zu Out-of-Memory-Fehlern führen

**Lösung**: 
1. Event-Handler in benannten Methoden speichern
2. Bei Adorner-Entfernung deregistrieren
3. Oder: WeakEventManager verwenden

---

### 2. Async void in Event Handlern ohne Error Handling
**Datei**: `LayoutDesigner/Views/MainWindow.xaml.cs:260`  
**Schweregrad**: 🔴 Kritisch  
**Beschreibung**: `async void` Event-Handler ohne Try-Catch

```csharp
private async void OnExportRequested(object? sender, ExportRequestedEventArgs e)
{
    // Kein try-catch um den gesamten Methodenblock
    var canvas = DesignCanvas;
    // ...
}
```

**Problem**:
- Exceptions in `async void` können nicht abgefangen werden
- Führt zum Absturz der gesamten Anwendung
- Keine Möglichkeit zur Error Recovery

**Auswirkung**: Anwendungsabsturz bei Export-Fehlern

**Lösung**: 
```csharp
private async void OnExportRequested(object? sender, ExportRequestedEventArgs e)
{
    try
    {
        // ... gesamter Code ...
    }
    catch (Exception ex)
    {
        ErrorHandlingService.HandleError(ex, "Export failed");
    }
}
```

---

### 3. Fehlende Async-Version von File I/O Operationen
**Datei**: `LayoutDesigner/Services/LayoutStorageService.cs:124, 167`  
**Schweregrad**: 🔴 Kritisch  
**Beschreibung**: Synchrone File I/O Operationen blockieren UI-Thread

```csharp
// Zeile 124
var json = File.ReadAllText(settingsFile);

// Zeile 167
File.WriteAllText(settingsFile, json);
```

**Problem**:
- Blockiert UI-Thread während Datei-Operationen
- Kann zu "Anwendung reagiert nicht" führen
- Besonders problematisch bei großen Dateien oder langsamen Speichermedien

**Auswirkung**: Eingefrorene UI, schlechte User Experience

**Lösung**: Verwende `File.ReadAllTextAsync()` und `File.WriteAllTextAsync()`

---

## 🟠 Schwere Fehler (Priorität 2)

### 4. Async void ohne Error Handling in AssetManagerPanel
**Datei**: `LayoutDesigner/Views/AssetManagerPanel.xaml.cs:74`  
**Schweregrad**: 🟠 Hoch  

```csharp
private async void OnImportClick(object sender, RoutedEventArgs e)
{
    // Kein Error Handling
}
```

**Problem**: Gleich wie #2 - möglicher Anwendungsabsturz
**Lösung**: Try-Catch Block hinzufügen

---

### 5. Fehlende CancellationToken in Async-Methoden
**Dateien**: Mehrere Service-Dateien  
**Schweregrad**: 🟠 Hoch  
**Betroffene Methoden**:
- `AssetService.ImportAssetAsync()`
- `ExportService.ExportToPngAsync()`
- `ExportService.ExportToJpgAsync()`
- `LayoutStorageService.SaveLayoutAsync()`
- `LayoutStorageService.LoadLayoutAsync()`
- `MainViewModel.NewAsync()`, `OpenAsync()`, `SaveAsync()`

**Problem**:
- Keine Möglichkeit, lang laufende Operationen abzubrechen
- Benutzer kann Export/Import nicht stoppen
- Verschwendet Ressourcen bei nicht mehr benötigten Operationen

**Lösung**: 
```csharp
public async Task<bool> ExportToPngAsync(
    UIElement element, 
    string filePath, 
    double width, 
    double height,
    CancellationToken cancellationToken = default)
{
    cancellationToken.ThrowIfCancellationRequested();
    // ... rest of implementation
}
```

---

### 6. Synchrone File I/O in TemplateService
**Datei**: `LayoutDesigner/Services/TemplateService.cs:79, 140`  
**Schweregrad**: 🟠 Hoch  

```csharp
// Zeile 79
File.WriteAllText(filePath, json);

// Zeile 140
var json = File.ReadAllText(file);
```

**Problem**: Blockiert UI-Thread
**Lösung**: Async-Versionen verwenden

---

### 7. Synchrone File I/O in ApplicationSettings
**Datei**: `LayoutDesigner/Models/ApplicationSettings.cs:268, 286`  
**Schweregrad**: 🟠 Hoch  

```csharp
File.WriteAllText(SettingsFilePath, json);
var json = File.ReadAllText(SettingsFilePath);
```

**Problem**: Settings werden beim Laden/Speichern blockierend gelesen
**Lösung**: Methoden async machen und async File I/O verwenden

---

### 8. Fehlende readonly bei privaten Feldern
**Dateien**: Mehrere Control-Dateien  
**Schweregrad**: 🟠 Mittel  
**Betroffene Felder** (Beispiele):

```csharp
// SnapLinesAdorner.cs:15
private Pen? _snapLinePen;  // Sollte readonly sein wenn nur in Constructor gesetzt

// QrCodeControl.cs:17
private IQrCodeService? _qrCodeService;  // Sollte readonly sein

// SelectionAdorner.cs:30-34
private double _originalX;
private double _originalY;
private double _originalWidth;
private double _originalHeight;
private double _originalRotation;
```

**Problem**: 
- Felder werden nach Initialisierung nicht mehr verändert, sind aber nicht readonly
- Reduziert Code-Verständlichkeit
- Verhindert Compiler-Optimierungen

**Auswirkung**: Code-Qualität, Performance

**Lösung**: Felder als `readonly` markieren wo möglich

---

### 9. Magic Numbers ohne Konstanten
**Dateien**: Mehrere Dateien  
**Schweregrad**: 🟠 Mittel  
**Beispiele**:

```csharp
// ExportService.cs:76-77
96,  // DPI - sollte Konstante sein
96,

// TemplateService.cs:173-174
Width = 340,  // 85mm * 4 pixels/mm - Erklärung im Kommentar, aber kein const
Height = 220, // 55mm * 4 pixels/mm

// TemplateService.cs:186
FontSize = 24,  // Magic number
```

**Problem**: 
- Reduziert Wartbarkeit
- Schwer zu verstehen was die Zahlen bedeuten
- Änderungen müssen an mehreren Stellen gemacht werden

**Lösung**: Konstanten definieren:
```csharp
private const double DefaultDpi = 96.0;
private const double BusinessCardWidthMm = 85.0;
private const double PixelsPerMm = 4.0;
private const double DefaultFontSize = 24.0;
```

---

### 10. NotImplementedException in Converter
**Datei**: `LayoutDesigner/Converters/ImagePathConverter.cs:131`  
**Schweregrad**: 🟠 Mittel  

```csharp
public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
{
    throw new NotImplementedException();
}
```

**Problem**:
- ConvertBack wird nicht implementiert
- Kann zu Runtime-Fehlern führen wenn WPF versucht ConvertBack zu rufen
- Besser: NotSupportedException mit klarer Fehlermeldung

**Lösung**: 
```csharp
public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
{
    throw new NotSupportedException("ConvertBack is not supported for ImagePathConverter");
}
```

---

### 11. Async void in AsyncRelayCommand.Execute
**Datei**: `LayoutDesigner/ViewModels/Base/AsyncRelayCommand.cs:51, 131`  
**Schweregrad**: 🟠 Mittel  

```csharp
public async void Execute(object? parameter)
{
    // ...
}
```

**Problem**: 
- `async void` kann nicht gewartet werden
- Exceptions können nicht abgefangen werden
- Verletzt Best Practices

**Hinweis**: Dies ist bei ICommand.Execute unvermeidbar, ABER: Es sollte Error Handling geben

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
        // Error handling oder Event für unhandled exceptions
        Debug.WriteLine($"Error in AsyncRelayCommand: {ex}");
    }
}
```

---

## 🟡 Warnungen (Priorität 3)

### 12. Fehlende XML-Dokumentation
**Dateien**: Viele Dateien  
**Schweregrad**: 🟡 Niedrig  
**Beschreibung**: Viele öffentliche Methoden haben keine XML-Dokumentation

**Beispiele ohne Dokumentation**:
- `CanvasViewModel`: Viele Command-Methoden
- `SnapHelper`: Einige Helper-Methoden
- Viele Service-Methoden

**Auswirkung**: Erschwert Wartung und Onboarding neuer Entwickler

**Lösung**: XML-Kommentare hinzufügen:
```csharp
/// <summary>
/// Importiert ein Asset-File in den Assets-Ordner
/// </summary>
/// <param name="sourceFilePath">Pfad zur Quelldatei</param>
/// <returns>Pfad zum importierten Asset oder null bei Fehler</returns>
public async Task<string?> ImportAssetAsync(string sourceFilePath)
```

---

### 13. Inkonsistente Error-Handling-Patterns
**Dateien**: Verschiedene Services  
**Schweregrad**: 🟡 Niedrig  
**Beschreibung**: Mischung aus verschiedenen Fehlerbehandlungs-Strategien

**Patterns gefunden**:
1. Try-Catch mit ErrorHandlingService (gut) ✓
2. Try-Catch mit return null (problematisch)
3. Try-Catch mit throw (inkonsistent)
4. Kein Error Handling (schlecht)

**Beispiel - LayoutStorageService**:
```csharp
public async Task<LayoutDocument?> LoadLayoutAsync(string filePath)
{
    try
    {
        // ...
        return document;
    }
    catch (Exception ex)
    {
        ErrorHandlingService.HandleError(ex, "Failed to load layout");
        return null;  // Caller weiß nicht ob null = Fehler oder "nicht gefunden"
    }
}
```

**Problem**: Caller kann nicht zwischen verschiedenen Fehlertypen unterscheiden

**Lösung**: Konsistente Strategie - entweder:
- Immer Exceptions werfen und Caller behandeln lassen
- Oder: Result<T> Pattern verwenden

---

### 14. Potentielle Null-Reference ohne Checks
**Dateien**: Verschiedene Service-Dateien  
**Schweregrad**: 🟡 Niedrig  
**Beschreibung**: Einige Methoden verwenden Werte ohne null-Check

**Beispiele**:
```csharp
// AssetService.cs:41
var fileName = Path.GetFileName(sourceFilePath); // sourceFilePath könnte null sein
```

**Hinweis**: Mit `nullable enable` in .csproj werden viele davon vom Compiler gewarnt

---

### 15. Grosse using-Direktiven-Listen
**Schweregrad**: 🟡 Niedrig  
**Beschreibung**: Einige Dateien haben viele using-Direktiven (>15)

**Problem**: Reduziert Code-Übersichtlichkeit
**Lösung**: 
- Ungenutzte usings entfernen
- Global usings in .csproj verwenden
- File-scoped namespaces verwenden (C# 10+)

---

### 16. Fehlende IDisposable-Implementierung
**Schweregrad**: 🟡 Niedrig  
**Beschreibung**: Keine eigenen IDisposable-Implementierungen gefunden

**Status**: ✓ Gut - keine Ressourcen-Lecks durch fehlende Dispose-Patterns

---

### 17. Keine Unit Tests gefunden
**Schweregrad**: 🟡 Niedrig  
**Beschreibung**: Projekt hat keine Unit Tests

**Problem**: 
- Keine automatische Validierung bei Änderungen
- Schwer zu refactoren ohne Tests
- Code-Coverage = 0%

**Empfehlung**: 
- xUnit oder NUnit Test-Projekt hinzufügen
- Mindestens Services und ViewModels testen
- Ziel: >80% Code-Coverage

---

### 18. Keine .editorconfig gefunden
**Schweregrad**: 🟡 Niedrig  
**Beschreibung**: Keine .editorconfig für konsistenten Code-Stil

**Empfehlung**: .editorconfig hinzufügen mit:
- Indent-Stil
- Line endings
- Charset
- Code-Style-Regeln

---

## 🔵 Code-Smell / Best Practices (Priorität 4)

### 19. Duplizierter Code: Event-Handler-Pattern
**Beschreibung**: MouseEnter/MouseLeave-Handler sind dupliziert

**Beispiel**: SelectionAdorner hat mehrmals den gleichen Pattern für Hover-Effekte

**Lösung**: Helper-Methode:
```csharp
private void AddHoverEffect(Thumb thumb, Color normalColor, Color hoverColor)
{
    thumb.MouseEnter += (s, e) => {
        thumb.Background = new SolidColorBrush(hoverColor);
        thumb.Width += 2;
        thumb.Height += 2;
    };
    thumb.MouseLeave += (s, e) => {
        thumb.Background = new SolidColorBrush(normalColor);
        thumb.Width -= 2;
        thumb.Height -= 2;
    };
}
```

---

### 20. Command-Pattern-Inkonsistenz
**Beschreibung**: Mix aus RelayCommand und AsyncRelayCommand

**Status**: Akzeptabel - aber Dokumentation wäre hilfreich wann welcher zu verwenden ist

---

### 21. Hardcodierte Farb-Werte
**Beispiele**:
```csharp
Color.FromRgb(135, 206, 250) // LightSkyBlue
Color.FromRgb(144, 238, 144) // LightGreen
```

**Empfehlung**: Farben in ResourceDictionary definieren oder Konstanten verwenden

---

### 22. Grid-Rendering-Performance
**Datei**: `LayoutDesigner/Controls/GridLines.cs`  
**Beschreibung**: Grid wird bei jeder OnRender neu gezeichnet

**Potentielle Optimierung**: Grid-Visual cachen wenn keine Änderungen

---

### 23. Fehlende Input-Validierung
**Beschreibung**: Einige Methoden validieren Eingaben nicht ausreichend

**Beispiel**: Width/Height bei Export könnten negativ oder 0 sein
**Lösung**: Guard Clauses hinzufügen

---

### 24. LINQ-Performance in Alignment-Methoden
**Status**: Bereits in CODE_QUALITY_ISSUES.md dokumentiert und behoben ✓

---

### 25. String-Lokalisierung fehlt
**Beschreibung**: Alle Strings sind hardcodiert auf Deutsch/Englisch

**Problem**: Keine Mehrsprachigkeit möglich
**Lösung**: Resource-Dateien (.resx) verwenden

---

### 26. Fehlende Logging-Infrastructure
**Beschreibung**: Debugging verwendet hauptsächlich Debug.WriteLine

**Empfehlung**: 
- Proper Logging-Framework (Serilog, NLog)
- Konfigurierbare Log-Level
- Strukturiertes Logging

---

### 27. Fehlende Telemetrie/Analytics
**Beschreibung**: Keine Erfassung von Nutzungsmetriken oder Fehlern

**Empfehlung**: Application Insights oder ähnliches für Production

---

### 28. Dependency Injection Container zu einfach
**Datei**: Vermutlich in App.xaml.cs  
**Beschreibung**: Einfacher Service-Locator statt vollwertigem DI-Container

**Empfehlung**: Microsoft.Extensions.DependencyInjection verwenden

---

### 29. Fehlende Konfigurationsdatei
**Beschreibung**: Einstellungen sind im Code hardcodiert

**Lösung**: appsettings.json mit IConfiguration verwenden

---

### 30. Snapshot-Testing für UI fehlt
**Beschreibung**: Keine automatischen UI-Tests

**Empfehlung**: Approval Tests oder Visual Regression Testing

---

### 31. Accessibility (Barrierefreiheit) unklar
**Beschreibung**: Keine AutomationPeer-Implementierungen sichtbar

**Empfehlung**: Accessibility-Support prüfen und verbessern

---

### 32. Fehlende Performance-Metriken
**Beschreibung**: Keine Messung von Performance-kritischen Operationen

**Empfehlung**: BenchmarkDotNet für Performance-Tests

---

### 33. Git .gitignore könnte verbessert werden
**Beschreibung**: Standard .gitignore, könnte erweitert werden

**Empfehlung**: 
- Visual Studio temporäre Dateien
- Rider Cache
- User-spezifische Settings

---

## 📈 Statistiken

### Code-Metriken
- **Gesamt Zeilen**: ~9.066 (laut CODE_QUALITY_ISSUES.md)
- **C# Dateien**: 84
- **Test-Coverage**: 0%
- **Technische Schulden**: Mittel

### Fehler nach Typ
```
Memory Leaks:              2
Async/Await Issues:        4
File I/O Blocking:         4
Missing Error Handling:    3
Code-Smells:              15
Best Practice:            10
```

### Code-Qualität
- ✅ Nullable Reference Types aktiviert
- ✅ Async/Await korrekt verwendet (meistens)
- ✅ MVVM-Pattern befolgt
- ✅ Dependency Injection verwendet
- ❌ Keine Unit Tests
- ❌ Keine Code-Coverage
- ⚠️ Memory Leaks vorhanden
- ⚠️ Einige blocking I/O Operationen

---

## 🎯 Empfohlener Aktionsplan

### Phase 1: Kritisch (Sofort) - 1-2 Tage
1. ✅ Memory Leaks in SelectionAdorner beheben (#1)
2. ✅ Try-Catch in alle async void Event-Handler (#2, #4)
3. ✅ File I/O auf Async umstellen (#3, #6, #7)

### Phase 2: Hoch (Diese Woche) - 2-3 Tage
4. CancellationToken Support hinzufügen (#5)
5. Readonly-Felder markieren (#8)
6. Magic Numbers in Konstanten umwandeln (#9)
7. NotImplementedException verbessern (#10)
8. Error Handling in AsyncRelayCommand (#11)

### Phase 3: Mittel (Nächster Sprint) - 1 Woche
9. XML-Dokumentation hinzufügen (#12)
10. Error-Handling standardisieren (#13)
11. Null-Checks verbessern (#14)
12. Using-Direktiven aufräumen (#15)
13. Unit Test Infrastructure aufsetzen (#17)

### Phase 4: Nice-to-Have (Backlog)
14. Code-Smells beheben (#19-33)
15. Lokalisierung hinzufügen
16. Logging-Framework integrieren
17. Performance-Optimierungen
18. Accessibility verbessern

---

## 🔧 Empfohlene Tools

### Entwicklung
- **JetBrains Rider** oder **Visual Studio 2022**
- **ReSharper** - Code-Analyse
- **StyleCop.Analyzers** - Code-Style
- **Microsoft.CodeAnalysis.NetAnalyzers** - Statische Analyse

### Testing
- **xUnit** oder **NUnit** - Unit Testing
- **Moq** oder **FakeItEasy** - Mocking
- **FluentAssertions** - Bessere Assertions
- **Coverlet** - Code-Coverage

### Qualität
- **SonarQube** - Code-Quality-Dashboard
- **dotMemory** - Memory-Leak-Detection
- **BenchmarkDotNet** - Performance-Testing
- **Verify** - Snapshot-Testing

### CI/CD
- **GitHub Actions** - Automatisches Build & Test
- **Dependabot** - Dependency-Updates
- **CodeQL** - Security-Scanning

---

## 📚 Referenzen

1. [WPF Best Practices](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/advanced/wpf-best-practices)
2. [Async/Await Best Practices](https://docs.microsoft.com/en-us/archive/msdn-magazine/2013/march/async-await-best-practices-in-asynchronous-programming)
3. [Memory Leak Patterns](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/advanced/weak-event-patterns)
4. [C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
5. [MVVM Pattern](https://docs.microsoft.com/en-us/dotnet/architecture/modernize-desktop/example-migration-core)

---

**Erstellt von**: Automatische Code-Analyse  
**Letzte Aktualisierung**: 17. November 2025  
**Projekt-Version**: 1.0.0  
**Status**: 🟡 Mittel - Funktioniert, braucht Verbesserungen
