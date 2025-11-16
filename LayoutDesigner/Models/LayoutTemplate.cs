using System;
using System.Collections.Generic;
using LayoutDesigner.Models.Base;

namespace LayoutDesigner.Models
{
    /// <summary>
    /// Represents a layout template that can be instantiated on the canvas
    /// </summary>
    public class LayoutTemplate
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = "Untitled Template";
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = "General";
        public string? ThumbnailPath { get; set; }
        public double Width { get; set; } = 400;
        public double Height { get; set; } = 200;
        public List<LayoutElementBase> Elements { get; set; } = new();
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public bool IsBuiltIn { get; set; } = false;

        /// <summary>
        /// Creates a deep copy of all elements in this template
        /// </summary>
        public List<LayoutElementBase> InstantiateElements(double offsetX = 0, double offsetY = 0)
        {
            var instances = new List<LayoutElementBase>();

            foreach (var element in Elements)
            {
                var clone = element.Clone();
                clone.X += offsetX;
                clone.Y += offsetY;
                instances.Add(clone);
            }

            return instances;
        }
    }

    /// <summary>
    /// Template categories for organization
    /// </summary>
    public static class TemplateCategories
    {
        public const string BusinessCards = "Business Cards";
        public const string Badges = "Badges";
        public const string Labels = "Labels";
        public const string Signs = "Signs";
        public const string Certificates = "Certificates";
        public const string Custom = "Custom";
        public const string All = "All";
    }
}
