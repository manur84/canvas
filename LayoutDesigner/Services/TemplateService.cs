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

namespace LayoutDesigner.Services
{
    /// <summary>
    /// Service for managing layout templates
    /// </summary>
    public class TemplateService : ITemplateService
    {
        private readonly List<LayoutTemplate> _builtInTemplates;
        private readonly List<LayoutTemplate> _customTemplates;
        private readonly string _customTemplatesPath;
        private readonly IAppLogger? _logger;

        public TemplateService(IAppLogger logger)
        {
            ArgumentNullException.ThrowIfNull(logger);

            _logger = logger;
            _customTemplatesPath = ConfigurationDefaults.TemplatesPath;

            Directory.CreateDirectory(_customTemplatesPath);

            _builtInTemplates = CreateBuiltInTemplates();
            _customTemplates = LoadCustomTemplates();
        }

        public List<LayoutTemplate> GetAllTemplates()
        {
            return _builtInTemplates.Concat(_customTemplates).ToList();
        }

        public List<LayoutTemplate> GetTemplatesByCategory(string category)
        {
            if (category == TemplateCategories.All)
            {
                return GetAllTemplates();
            }

            return GetAllTemplates()
                .Where(t => t.Category == category)
                .ToList();
        }

        public LayoutTemplate? GetTemplateById(string id)
        {
            return GetAllTemplates().FirstOrDefault(t => t.Id == id);
        }

        public async Task<bool> SaveTemplateAsync(LayoutTemplate template, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(template);

            try
            {
                template.IsBuiltIn = false;
                template.CreatedDate = DateTime.Now;

                // Remove existing if updating
                var existing = _customTemplates.FirstOrDefault(t => t.Id == template.Id);
                if (existing != null)
                {
                    _customTemplates.Remove(existing);
                }

                _customTemplates.Add(template);

                // Save to file
                var filePath = Path.Combine(_customTemplatesPath, $"{template.Id}.json");
                var json = JsonSerializer.Serialize(template, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                await File.WriteAllTextAsync(filePath, json, cancellationToken);
                _logger?.LogInfo($"Template saved successfully: {template.Name}");
                return true;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Failed to save template: {template.Name}");
                return false;
            }
        }

        public async Task<bool> DeleteTemplateAsync(string id, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(id);

            try
            {
                var template = _customTemplates.FirstOrDefault(t => t.Id == id);
                if (template == null)
                {
                    _logger?.LogWarning($"Template not found for deletion: {id}");
                    return false;
                }

                _customTemplates.Remove(template);

                var filePath = Path.Combine(_customTemplatesPath, $"{id}.json");
                if (File.Exists(filePath))
                {
                    // File.Delete is synchronous but fast for single file operations
                    // Using Task.Run to keep async pattern consistent
                    await Task.Run(() => File.Delete(filePath), cancellationToken);
                }

                _logger?.LogInfo($"Template deleted successfully: {template.Name}");
                return true;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Failed to delete template: {id}");
                return false;
            }
        }

        public List<string> GetCategories()
        {
            return new List<string>
            {
                TemplateCategories.All,
                TemplateCategories.BusinessCards,
                TemplateCategories.Badges,
                TemplateCategories.Labels,
                TemplateCategories.Signs,
                TemplateCategories.Certificates,
                TemplateCategories.Custom
            };
        }

        private List<LayoutTemplate> LoadCustomTemplates()
        {
            var templates = new List<LayoutTemplate>();

            try
            {
                var files = Directory.GetFiles(_customTemplatesPath, "*.json");

                foreach (var file in files)
                {
                    try
                    {
                        // Use synchronous read for initialization to avoid blocking startup
                        // This is acceptable as it's called once during service construction
                        var json = File.ReadAllText(file);
                        var template = JsonSerializer.Deserialize<LayoutTemplate>(json);

                        if (template != null)
                        {
                            templates.Add(template);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Skip invalid templates
                        _logger?.LogWarning($"Failed to load template from {file}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Directory doesn't exist or can't be read
                _logger?.LogError(ex, "Failed to load custom templates directory");
            }

            return templates;
        }

        private List<LayoutTemplate> CreateBuiltInTemplates()
        {
            var templates = new List<LayoutTemplate>();

            // Business Card Template
            templates.Add(new LayoutTemplate
            {
                Id = "builtin-business-card-1",
                Name = "Standard Business Card",
                Description = "85x55mm standard business card with name, title, and contact info",
                Category = TemplateCategories.BusinessCards,
                Width = 340, // 85mm * 4 pixels/mm
                Height = 220, // 55mm * 4 pixels/mm
                IsBuiltIn = true,
                Elements = new List<LayoutElementBase>
                {
                    new TextElement
                    {
                        Name = "Company Name",
                        X = 20,
                        Y = 20,
                        Width = 300,
                        Height = 40,
                        Text = "COMPANY NAME",
                        FontSize = 24,
                        IsBold = true,
                        ForegroundColor = "#FF0078D7"
                    },
                    new TextElement
                    {
                        Name = "Person Name",
                        X = 20,
                        Y = 70,
                        Width = 300,
                        Height = 30,
                        Text = "John Doe",
                        FontSize = 18,
                        IsBold = true,
                        ForegroundColor = "#FF212529"
                    },
                    new TextElement
                    {
                        Name = "Title",
                        X = 20,
                        Y = 105,
                        Width = 300,
                        Height = 25,
                        Text = "Senior Developer",
                        FontSize = 14,
                        ForegroundColor = "#FF6C757D"
                    },
                    new TextElement
                    {
                        Name = "Contact",
                        X = 20,
                        Y = 145,
                        Width = 300,
                        Height = 60,
                        Text = "Email: john@company.com\nPhone: +1 234 567 8900\nWebsite: www.company.com",
                        FontSize = 10,
                        ForegroundColor = "#FF212529"
                    }
                }
            });

            // Name Badge Template
            templates.Add(new LayoutTemplate
            {
                Id = "builtin-badge-1",
                Name = "Conference Name Badge",
                Description = "Conference badge with name and company",
                Category = TemplateCategories.Badges,
                Width = 300,
                Height = 200,
                IsBuiltIn = true,
                Elements = new List<LayoutElementBase>
                {
                    new ShapeElement
                    {
                        Name = "Background",
                        X = 0,
                        Y = 0,
                        Width = 300,
                        Height = 200,
                        ShapeType = ShapeType.RoundedRectangle,
                        FillColor = "#FFFFFFFF",
                        StrokeColor = "#FF0078D7",
                        StrokeThickness = 3,
                        CornerRadius = 10
                    },
                    new TextElement
                    {
                        Name = "Hello Label",
                        X = 20,
                        Y = 30,
                        Width = 260,
                        Height = 30,
                        Text = "HELLO",
                        FontSize = 20,
                        IsBold = true,
                        ForegroundColor = "#FF0078D7",
                        TextAlignment = System.Windows.TextAlignment.Center
                    },
                    new TextElement
                    {
                        Name = "Name",
                        X = 20,
                        Y = 70,
                        Width = 260,
                        Height = 60,
                        Text = "Your Name",
                        FontSize = 32,
                        IsBold = true,
                        ForegroundColor = "#FF212529",
                        TextAlignment = System.Windows.TextAlignment.Center
                    },
                    new TextElement
                    {
                        Name = "Company",
                        X = 20,
                        Y = 140,
                        Width = 260,
                        Height = 30,
                        Text = "Company Name",
                        FontSize = 14,
                        ForegroundColor = "#FF6C757D",
                        TextAlignment = System.Windows.TextAlignment.Center
                    }
                }
            });

            // Product Label Template
            templates.Add(new LayoutTemplate
            {
                Id = "builtin-label-1",
                Name = "Product Label",
                Description = "Product label with title, description, and price",
                Category = TemplateCategories.Labels,
                Width = 250,
                Height = 150,
                IsBuiltIn = true,
                Elements = new List<LayoutElementBase>
                {
                    new ShapeElement
                    {
                        Name = "Border",
                        X = 0,
                        Y = 0,
                        Width = 250,
                        Height = 150,
                        ShapeType = ShapeType.Rectangle,
                        FillColor = "#FFFEFEFE",
                        StrokeColor = "#FFCCCCCC",
                        StrokeThickness = 2
                    },
                    new TextElement
                    {
                        Name = "Product Name",
                        X = 15,
                        Y = 15,
                        Width = 220,
                        Height = 35,
                        Text = "Product Name",
                        FontSize = 18,
                        IsBold = true,
                        ForegroundColor = "#FF212529"
                    },
                    new TextElement
                    {
                        Name = "Description",
                        X = 15,
                        Y = 55,
                        Width = 220,
                        Height = 50,
                        Text = "Product description goes here. Features and benefits.",
                        FontSize = 11,
                        ForegroundColor = "#FF495057"
                    },
                    new TextElement
                    {
                        Name = "Price",
                        X = 15,
                        Y = 110,
                        Width = 220,
                        Height = 30,
                        Text = "$19.99",
                        FontSize = 20,
                        IsBold = true,
                        ForegroundColor = "#FF28A745"
                    }
                }
            });

            // Room Sign Template
            templates.Add(new LayoutTemplate
            {
                Id = "builtin-sign-1",
                Name = "Room Sign",
                Description = "Simple room/door sign",
                Category = TemplateCategories.Signs,
                Width = 400,
                Height = 150,
                IsBuiltIn = true,
                Elements = new List<LayoutElementBase>
                {
                    new ShapeElement
                    {
                        Name = "Background",
                        X = 0,
                        Y = 0,
                        Width = 400,
                        Height = 150,
                        ShapeType = ShapeType.RoundedRectangle,
                        FillColor = "#FF0078D7",
                        CornerRadius = 8
                    },
                    new TextElement
                    {
                        Name = "Room Name",
                        X = 20,
                        Y = 40,
                        Width = 360,
                        Height = 70,
                        Text = "Conference Room A",
                        FontSize = 36,
                        IsBold = true,
                        ForegroundColor = "#FFFFFFFF",
                        TextAlignment = System.Windows.TextAlignment.Center,
                        VerticalAlignment = System.Windows.VerticalAlignment.Center
                    }
                }
            });

            // Certificate Template
            templates.Add(new LayoutTemplate
            {
                Id = "builtin-certificate-1",
                Name = "Simple Certificate",
                Description = "Achievement certificate with border",
                Category = TemplateCategories.Certificates,
                Width = 800,
                Height = 600,
                IsBuiltIn = true,
                Elements = new List<LayoutElementBase>
                {
                    new ShapeElement
                    {
                        Name = "Outer Border",
                        X = 0,
                        Y = 0,
                        Width = 800,
                        Height = 600,
                        ShapeType = ShapeType.Rectangle,
                        FillColor = "#FFFEFEFE",
                        StrokeColor = "#FFD4AF37",
                        StrokeThickness = 8
                    },
                    new ShapeElement
                    {
                        Name = "Inner Border",
                        X = 30,
                        Y = 30,
                        Width = 740,
                        Height = 540,
                        ShapeType = ShapeType.Rectangle,
                        FillColor = "#00FFFFFF",
                        StrokeColor = "#FFD4AF37",
                        StrokeThickness = 2
                    },
                    new TextElement
                    {
                        Name = "Certificate Title",
                        X = 50,
                        Y = 100,
                        Width = 700,
                        Height = 60,
                        Text = "CERTIFICATE OF ACHIEVEMENT",
                        FontSize = 36,
                        IsBold = true,
                        ForegroundColor = "#FFD4AF37",
                        TextAlignment = System.Windows.TextAlignment.Center
                    },
                    new TextElement
                    {
                        Name = "Presented To",
                        X = 100,
                        Y = 200,
                        Width = 600,
                        Height = 40,
                        Text = "This is to certify that",
                        FontSize = 18,
                        ForegroundColor = "#FF212529",
                        TextAlignment = System.Windows.TextAlignment.Center
                    },
                    new TextElement
                    {
                        Name = "Recipient Name",
                        X = 100,
                        Y = 260,
                        Width = 600,
                        Height = 70,
                        Text = "Recipient Name",
                        FontSize = 42,
                        IsBold = true,
                        ForegroundColor = "#FF212529",
                        TextAlignment = System.Windows.TextAlignment.Center
                    },
                    new TextElement
                    {
                        Name = "Achievement",
                        X = 100,
                        Y = 360,
                        Width = 600,
                        Height = 60,
                        Text = "has successfully completed the requirements for",
                        FontSize = 16,
                        ForegroundColor = "#FF495057",
                        TextAlignment = System.Windows.TextAlignment.Center
                    },
                    new TextElement
                    {
                        Name = "Course/Award",
                        X = 100,
                        Y = 430,
                        Width = 600,
                        Height = 50,
                        Text = "Advanced Layout Design",
                        FontSize = 24,
                        IsBold = true,
                        ForegroundColor = "#FF212529",
                        TextAlignment = System.Windows.TextAlignment.Center
                    }
                }
            });

            return templates;
        }
    }
}
