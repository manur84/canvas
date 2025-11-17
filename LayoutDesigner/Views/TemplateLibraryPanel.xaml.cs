using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LayoutDesigner.Models;
using LayoutDesigner.Services.Interfaces;

namespace LayoutDesigner.Views
{
    /// <summary>
    /// Template Library Panel
    /// </summary>
    public partial class TemplateLibraryPanel : UserControl, INotifyPropertyChanged
    {
        private readonly ITemplateService _templateService;
        private string _selectedCategory = TemplateCategories.All;

        public event PropertyChangedEventHandler? PropertyChanged;

        public TemplateLibraryPanel()
        {
            InitializeComponent();

            _templateService = App.Services.Resolve<ITemplateService>();

            LoadCategories();
            LoadTemplates();
        }

        public string SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (_selectedCategory != value)
                {
                    _selectedCategory = value;
                    OnPropertyChanged();
                    LoadTemplates();
                }
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Event raised when a template is selected
        /// </summary>
        public event EventHandler<TemplateSelectedEventArgs>? TemplateSelected;

        private void LoadCategories()
        {
            var categories = _templateService.GetCategories();
            CategoryComboBox.ItemsSource = categories;
            CategoryComboBox.SelectedIndex = 0; // Select "All" by default
        }

        private void LoadTemplates()
        {
            var templates = _templateService.GetTemplatesByCategory(_selectedCategory);
            TemplatesList.ItemsSource = templates;
        }

        private void OnCategoryChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CategoryComboBox.SelectedItem is string category)
            {
                SelectedCategory = category;
            }
        }

        private void OnTemplateClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border { DataContext: LayoutTemplate template })
            {
                TemplateSelected?.Invoke(this, new TemplateSelectedEventArgs(template));
            }
        }
    }

    /// <summary>
    /// Event args for template selection
    /// </summary>
    public class TemplateSelectedEventArgs : EventArgs
    {
        public LayoutTemplate Template { get; }

        public TemplateSelectedEventArgs(LayoutTemplate template)
        {
            Template = template;
        }
    }
}
