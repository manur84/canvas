using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LayoutDesigner.Models;

namespace LayoutDesigner.Services.Interfaces
{
    /// <summary>
    /// Service for managing layout templates
    /// </summary>
    public interface ITemplateService
    {
        /// <summary>
        /// Gets all available templates
        /// </summary>
        List<LayoutTemplate> GetAllTemplates();

        /// <summary>
        /// Gets templates by category
        /// </summary>
        List<LayoutTemplate> GetTemplatesByCategory(string category);

        /// <summary>
        /// Gets a template by ID
        /// </summary>
        LayoutTemplate? GetTemplateById(string id);

        /// <summary>
        /// Saves a custom template
        /// </summary>
        Task<bool> SaveTemplateAsync(LayoutTemplate template, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a custom template
        /// </summary>
        Task<bool> DeleteTemplateAsync(string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all template categories
        /// </summary>
        List<string> GetCategories();
    }
}
