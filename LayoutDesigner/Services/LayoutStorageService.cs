using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LayoutDesigner.Models;
using LayoutDesigner.Models.Base;
using LayoutDesigner.Services.Interfaces;
using LayoutDesigner.Constants;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LayoutDesigner.Services
{
    /// <summary>
    /// Service for saving and loading layouts using JSON
    /// </summary>
    public class LayoutStorageService : ILayoutStorageService
    {
        private readonly string _appDataPath;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly IErrorHandlingService _errorHandlingService;
        private readonly IAppLogger? _logger;

        public LayoutStorageService(IErrorHandlingService errorHandlingService, IAppLogger logger)
        {
            ArgumentNullException.ThrowIfNull(errorHandlingService);
            ArgumentNullException.ThrowIfNull(logger);

            _errorHandlingService = errorHandlingService;
            _logger = logger;
            _appDataPath = ConfigurationDefaults.AppDataPath;

            Directory.CreateDirectory(_appDataPath);

            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true,
                Converters = { new LayoutElementConverter() }
            };
        }

        public async Task<bool> SaveLayoutAsync(LayoutDocument document, string filePath, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(document);
            ArgumentNullException.ThrowIfNull(filePath);

            try
            {
                document.ModifiedDate = DateTime.Now;

                // Embed images as Base64 before saving
                await EmbedImagesAsync(document, cancellationToken);

                var json = JsonSerializer.Serialize(document, _jsonOptions);
                await File.WriteAllTextAsync(filePath, json, cancellationToken);

                await AddRecentFileAsync(filePath, cancellationToken);
                _logger?.LogInfo($"Layout saved successfully: {filePath}");
                return true;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Error saving layout to {filePath}");
                _errorHandlingService.HandleError(ex, "Error saving layout", showDialog: false);
                return false;
            }
        }

        /// <summary>
        /// Embeds all images from ImageElements as Base64 data
        /// </summary>
        private async Task EmbedImagesAsync(LayoutDocument document, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(document);

            foreach (var element in document.Elements.OfType<ImageElement>())
            {
                // Only embed if we have a valid ImagePath and no ImageData yet
                if (!string.IsNullOrWhiteSpace(element.ImagePath) &&
                    File.Exists(element.ImagePath))
                {
                    try
                    {
                        // Read image file and convert to Base64
                        var imageBytes = await File.ReadAllBytesAsync(element.ImagePath, cancellationToken);
                        element.ImageData = Convert.ToBase64String(imageBytes);
                    }
                    catch (Exception ex)
                    {
                        // If embedding fails, just skip this image (it will use the path instead)
                        _logger?.LogWarning($"Could not embed image: {element.ImagePath}");
                        _errorHandlingService.HandleError(ex,
                            $"Could not embed image: {element.ImagePath}",
                            showDialog: false);
                    }
                }
            }
        }

        public async Task<LayoutDocument?> LoadLayoutAsync(string filePath, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(filePath);

            try
            {
                if (!File.Exists(filePath))
                {
                    _logger?.LogWarning($"Layout file not found: {filePath}");
                    return null;
                }

                var json = await File.ReadAllTextAsync(filePath, cancellationToken);
                var document = JsonSerializer.Deserialize<LayoutDocument>(json, _jsonOptions);

                if (document != null)
                {
                    await AddRecentFileAsync(filePath, cancellationToken);
                    _logger?.LogInfo($"Layout loaded successfully: {filePath}");
                }

                return document;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Error loading layout from {filePath}");
                _errorHandlingService.HandleError(ex, "Error loading layout", showDialog: false);
                return null;
            }
        }

        public async Task<List<string>> GetRecentFilesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var settingsFile = ConfigurationDefaults.SettingsFilePath;
                if (!File.Exists(settingsFile))
                    return new List<string>();

                var json = await File.ReadAllTextAsync(settingsFile, cancellationToken);
                var settings = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

                if (settings != null && settings.ContainsKey(ConfigurationDefaults.RecentFilesKey))
                {
                    var recentFiles = settings[ConfigurationDefaults.RecentFilesKey].Deserialize<List<string>>();
                    return recentFiles?.Where(File.Exists).ToList() ?? new List<string>();
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error loading recent files");
                _errorHandlingService.HandleError(ex, "Error loading recent files", showDialog: false);
            }

            return new List<string>();
        }

        public async Task AddRecentFileAsync(string filePath, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(filePath);

            try
            {
                var recentFiles = await GetRecentFilesAsync(cancellationToken);

                // Remove if already exists
                recentFiles.Remove(filePath);

                // Add to beginning
                recentFiles.Insert(0, filePath);

                // Keep only max recent files
                if (recentFiles.Count > ConfigurationDefaults.MaxRecentFiles)
                {
                    recentFiles = recentFiles.Take(ConfigurationDefaults.MaxRecentFiles).ToList();
                }

                // Save
                var settingsFile = ConfigurationDefaults.SettingsFilePath;
                var settings = new Dictionary<string, object>
                {
                    { ConfigurationDefaults.RecentFilesKey, recentFiles }
                };

                var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(settingsFile, json, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Error adding recent file: {filePath}");
                _errorHandlingService.HandleError(ex, "Error adding recent file", showDialog: false);
            }
        }
    }

    /// <summary>
    /// Custom JSON converter for polymorphic LayoutElementBase types
    /// </summary>
    public class LayoutElementConverter : JsonConverter<LayoutElementBase>
    {
        public override LayoutElementBase? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;

            if (!root.TryGetProperty("ElementType", out var elementTypeProperty))
            {
                throw new JsonException("Missing ElementType property");
            }

            var elementType = elementTypeProperty.GetString();

            return elementType switch
            {
                "Text" => JsonSerializer.Deserialize<TextElement>(root.GetRawText(), options),
                "Image" => JsonSerializer.Deserialize<ImageElement>(root.GetRawText(), options),
                "Shape" => JsonSerializer.Deserialize<ShapeElement>(root.GetRawText(), options),
                "QrCode" => JsonSerializer.Deserialize<QrCodeElement>(root.GetRawText(), options),
                "DynamicField" => JsonSerializer.Deserialize<DynamicFieldElement>(root.GetRawText(), options),
                "Line" => JsonSerializer.Deserialize<LineElement>(root.GetRawText(), options),
                "Button" => JsonSerializer.Deserialize<ButtonElement>(root.GetRawText(), options),
                "Table" => JsonSerializer.Deserialize<TableElement>(root.GetRawText(), options),
                _ => throw new JsonException($"Unknown element type: {elementType}")
            };
        }

        public override void Write(Utf8JsonWriter writer, LayoutElementBase value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }
}
