# Layout Designer - Build & Deployment Guide

Anleitung zum Kompilieren, Testen und Bereitstellen des Layout Designers.

## 🛠️ Build-Voraussetzungen

### Software
- **Windows 10/11** (64-bit empfohlen)
- **.NET 8.0 SDK** oder höher
- **Visual Studio 2022** (empfohlen) oder **JetBrains Rider**
- **Git** (für Repository-Verwaltung)

### Optionale Tools
- **Visual Studio Code** (für Markdown-Bearbeitung)
- **Git GUI** Client (z.B. GitKraken, SourceTree)

## 📥 Projekt klonen

```bash
# HTTPS
git clone https://github.com/your-username/canvas.git

# SSH
git clone git@github.com:your-username/canvas.git

# Repository wechseln
cd canvas
```

## 🔨 Build-Prozess

### Kommandozeile

**Einfacher Build**:
```bash
# NuGet-Pakete wiederherstellen
dotnet restore

# Debug-Build
dotnet build

# Release-Build
dotnet build -c Release
```

**Mit spezifischen Optionen**:
```bash
# Build mit detaillierter Ausgabe
dotnet build -v detailed

# Build ohne NuGet-Restore
dotnet build --no-restore

# Clean vor Build
dotnet clean
dotnet build
```

### Visual Studio 2022

1. **Solution öffnen**: `LayoutDesigner.sln`
2. **Build-Konfiguration** wählen:
   - `Debug` für Entwicklung
   - `Release` für Distribution
3. **Build ausführen**: `Build` → `Build Solution` (Strg+Shift+B)

### JetBrains Rider

1. **Solution öffnen**: `LayoutDesigner.sln`
2. **Build-Konfiguration** wählen (oben)
3. **Build**: `Build` → `Build Solution` (Strg+F9)

## ▶️ Ausführen

### Kommandozeile

```bash
# Debug-Version
dotnet run --project LayoutDesigner

# Release-Version
dotnet run --project LayoutDesigner -c Release

# Mit Argumenten
dotnet run --project LayoutDesigner -- --open "Examples/WelcomeLayout.layout"
```

### Visual Studio / Rider

1. **Startprojekt** setzen: `LayoutDesigner`
2. **Start**: `F5` (mit Debugger) oder `Strg+F5` (ohne Debugger)

## 🧪 Tests

Aktuell sind keine Unit-Tests im Projekt, aber die Architektur ist testbar:

```bash
# Zukünftig (wenn Tests hinzugefügt):
dotnet test
```

### Manuelle Tests

**Testplan**:
```
1. Anwendung starten
2. Beispiel-Layout öffnen (WelcomeLayout.layout)
3. Element hinzufügen (Text, Shape, etc.)
4. Element bearbeiten (Eigenschaften ändern)
5. Element verschieben/skalieren/rotieren
6. Undo/Redo testen (Strg+Z/Y)
7. Speichern (Strg+S)
8. Export PNG/JPG testen
9. Neues Layout erstellen
10. Alle Element-Typen testen
```

## 📦 Deployment

### Self-Contained Deployment

**Windows x64**:
```bash
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true
```

**Output**: `LayoutDesigner\bin\Release\net8.0-windows\win-x64\publish\`

### Framework-Dependent Deployment

```bash
dotnet publish -c Release
```

Erfordert .NET 8.0 Runtime auf Ziel-System.

### Installer erstellen

**Mit WiX Toolset** (optional):

1. WiX Toolset installieren
2. WiX-Projekt zur Solution hinzufügen
3. Installer konfigurieren
4. Build

**Alternative**: Inno Setup oder NSIS

## 🗂️ Output-Struktur

Nach Build/Publish:

```
publish/
├── LayoutDesigner.exe          # Haupt-Executable
├── LayoutDesigner.dll          # Haupt-Assembly
├── QRCoder.dll                 # Dependency
├── Microsoft.Xaml.Behaviors.Wpf.dll
├── LayoutDesigner.deps.json    # Dependency-Manifest
├── LayoutDesigner.runtimeconfig.json
└── Examples/                   # Beispiel-Layouts (optional kopieren)
    ├── WelcomeLayout.layout
    ├── MeetingRoomSign.layout
    └── InformationDashboard.layout
```

## 📋 Build-Konfigurationen

### Debug
- Optimierungen: Aus
- Debugger-Informationen: Voll
- Warnings as Errors: Nein
- Verwendung: Entwicklung

### Release
- Optimierungen: An
- Debugger-Informationen: Minimal
- Warnings as Errors: Ja (empfohlen)
- Verwendung: Production

## 🔍 Troubleshooting

### NuGet-Pakete fehlen
```bash
dotnet restore --force
```

### Build-Fehler nach Update
```bash
dotnet clean
dotnet restore
dotnet build
```

### "Target framework not found"
**.NET 8.0 SDK** installieren:
```bash
dotnet --version  # Version prüfen
```

### XAML-Fehler
1. Solution schließen
2. `bin/` und `obj/` Ordner löschen
3. Solution neu öffnen
4. Rebuild

## 📊 Performance-Optimierung

### Release Build optimieren

**Projektdatei erweitern** (`LayoutDesigner.csproj`):
```xml
<PropertyGroup Condition="'$(Configuration)' == 'Release'">
  <DebugType>none</DebugType>
  <DebugSymbols>false</DebugSymbols>
  <Optimize>true</Optimize>
  <PublishTrimmed>true</PublishTrimmed>
  <PublishSingleFile>true</PublishSingleFile>
</PropertyGroup>
```

### Startup-Performance

**Code-Optimierungen**:
- Lazy-Loading von Services
- Asynchrone Initialisierung
- Minimale Startup-UI

## 🌍 Multi-Language Build

Für internationale Versionen:

```bash
# Mit Resource-Dateien
dotnet build /p:UICulture=de-DE
dotnet build /p:UICulture=en-US
```

## 📱 Platform-spezifische Builds

**Nur Windows**:
```bash
dotnet publish -r win-x64
dotnet publish -r win-x86
dotnet publish -r win-arm64
```

## 🔐 Code Signing (Optional)

Für Production-Deployments:

```bash
signtool sign /f certificate.pfx /p password /t http://timestamp.digicert.com LayoutDesigner.exe
```

## 📖 Weitere Informationen

- **Architektur**: Siehe `ARCHITECTURE.md`
- **Features**: Siehe `FEATURES.md`
- **Changelog**: Siehe `CHANGELOG.md`

---

**Build-System**: MSBuild via .NET CLI
**Target Framework**: net8.0-windows
**Language Version**: C# 12.0
