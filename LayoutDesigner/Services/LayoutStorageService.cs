using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LayoutDesigner.Models;
using LayoutDesigner.Models.Base;
using LayoutDesigner.Services.Interfaces;
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
        private const string RecentFilesKey = "RecentFiles";
        private const int MaxRecentFiles = 10;
        private readonly string _appDataPath;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly IErrorHandlingService _errorHandlingService;

        public LayoutStorageService(IErrorHandlingService errorHandlingService)
        {
            _errorHandlingService = errorHandlingService;
            _appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "LayoutDesigner");

            Directory.CreateDirectory(_appDataPath);

            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true,
                Converters = { new LayoutElementConverter() }
            };
        }

        public async Task<bool> SaveLayoutAsync(LayoutDocument document, string filePath)
        {
            try
            {
                document.ModifiedDate = DateTime.Now;

                // Embed images as Base64 before saving
                await EmbedImagesAsync(document);

                var json = JsonSerializer.Serialize(document, _jsonOptions);
                await File.WriteAllTextAsync(filePath, json);

                AddRecentFile(filePath);
                return true;
            }
            catch (Exception ex)
            {
                _errorHandlingService.HandleError(ex, "Error saving layout", showDialog: false);
                return false;
            }
        }

        /// <summary>
        /// Embeds all images from ImageElements as Base64 data
        /// </summary>
        private async Task EmbedImagesAsync(LayoutDocument document)
        {
            foreach (var element in document.Elements.OfType<ImageElement>())
            {
                // Only embed if we have a valid ImagePath and no ImageData yet
                if (!string.IsNullOrWhiteSpace(element.ImagePath) &&
                    File.Exists(element.ImagePath))
                {
                    try
                    {
                        // Read image file and convert to Base64
                        var imageBytes = await File.ReadAllBytesAsync(element.ImagePath);
                        element.ImageData = Convert.ToBase64String(imageBytes);
                    }
                    catch (Exception ex)
                    {
                        // If embedding fails, just skip this image (it will use the path instead)
                        _errorHandlingService.HandleError(ex,
                            $"Could not embed image: {element.ImagePath}",
                            showDialog: false);
                    }
                }
            }
        }

        public async Task<LayoutDocument?> LoadLayoutAsync(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    return null;

                var json = await File.ReadAllTextAsync(filePath);
                var document = JsonSerializer.Deserialize<LayoutDocument>(json, _jsonOptions);

                if (document != null)
                {
                    AddRecentFile(filePath);
                }

                return document;
            }
            catch (Exception ex)
            {
                _errorHandlingService.HandleError(ex, "Error loading layout", showDialog: false);
                return null;
            }
        }

        public List<string> GetRecentFiles()
        {
            try
            {
                var settingsFile = Path.Combine(_appDataPath, "settings.json");
                if (!File.Exists(settingsFile))
                    return new List<string>();

                var json = File.ReadAllText(settingsFile);
                var settings = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

                if (settings != null && settings.ContainsKey(RecentFilesKey))
                {
                    var recentFiles = settings[RecentFilesKey].Deserialize<List<string>>();
                    return recentFiles?.Where(File.Exists).ToList() ?? new List<string>();
                }
            }
            catch (Exception ex)
            {
                _errorHandlingService.HandleError(ex, "Error loading recent files", showDialog: false);
            }

            return new List<string>();
        }

        public void AddRecentFile(string filePath)
        {
            try
            {
                var recentFiles = GetRecentFiles();

                // Remove if already exists
                recentFiles.Remove(filePath);

                // Add to beginning
                recentFiles.Insert(0, filePath);

                // Keep only max recent files
                if (recentFiles.Count > MaxRecentFiles)
                {
                    recentFiles = recentFiles.Take(MaxRecentFiles).ToList();
                }

                // Save
                var settingsFile = Path.Combine(_appDataPath, "settings.json");
                var settings = new Dictionary<string, object>
                {
                    { RecentFilesKey, recentFiles }
                };

                var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(settingsFile, json);
            }
            catch (Exception ex)
            {
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
