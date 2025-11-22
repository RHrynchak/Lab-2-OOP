using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Lab_2.Models;

namespace Lab_2
{
    public partial class DriveFilePickerWindow : Window
    {
        private GoogleDriveService _driveService;
        private List<DriveFileInfo> _allFiles;
        private List<DriveFileInfo> _filteredFiles;

        public DriveFileInfo SelectedFile { get; private set; }

        public DriveFilePickerWindow(GoogleDriveService driveService)
        {
            InitializeComponent();
            _driveService = driveService;
            LoadFiles();
        }

        private void LoadFiles()
        {
            try
            {
                StatusTextBlock.Text = "Завантаження файлів...";

                var files = _driveService.ListFiles(100);
                _allFiles = files.Select(f => new DriveFileInfo
                {
                    Id = f.Id,
                    Name = f.Name,
                    MimeType = f.MimeType,
                    Size = f.Size,
                    ModifiedTime = f.ModifiedTime,
                    FileType = GetFileTypeFromMime(f.MimeType)
                }).ToList();

                _filteredFiles = new List<DriveFileInfo>(_allFiles);
                FilesDataGrid.ItemsSource = _filteredFiles;

                StatusTextBlock.Text = $"Завантажено {_allFiles.Count} файлів";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження файлів: {ex.Message}",
                    "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusTextBlock.Text = "Помилка завантаження";
            }
        }

        private string GetFileTypeFromMime(string mimeType)
        {
            if (mimeType == null) return "Unknown";

            if (mimeType.Contains("xml")) return "XML";
            if (mimeType.Contains("xsl") || mimeType.Contains("xsl")) return "XSL";
            if (mimeType.Contains("html")) return "HTML";
            if (mimeType.Contains("text")) return "Text";

            return "Other";
        }

        private void FileTypeFilter_Changed(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void SearchText_Changed(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            if (_allFiles == null) return;

            _filteredFiles = new List<DriveFileInfo>(_allFiles);

            var selectedType = (FileTypeFilterComboBox.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content.ToString();
            if (selectedType != "Всі файли")
            {
                string typeFilter = selectedType switch
                {
                    "XML файли" => "XML",
                    "XSL файли" => "XSL",
                    "HTML файли" => "HTML",
                    _ => null
                };

                if (typeFilter != null)
                {
                    _filteredFiles = _filteredFiles.Where(f => f.FileType == typeFilter).ToList();
                }
            }

            var searchText = SearchTextBox.Text?.Trim();
            if (!string.IsNullOrEmpty(searchText))
            {
                _filteredFiles = _filteredFiles.Where(f =>
                    f.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            FilesDataGrid.ItemsSource = _filteredFiles;
            StatusTextBlock.Text = $"Показано {_filteredFiles.Count} з {_allFiles.Count} файлів";
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadFiles();
            SearchTextBox.Clear();
            FileTypeFilterComboBox.SelectedIndex = 0;
        }

        private void FilesDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Select_Click(sender, e);
        }

        private void Select_Click(object sender, RoutedEventArgs e)
        {
            if (FilesDataGrid.SelectedItem is DriveFileInfo selected)
            {
                SelectedFile = selected;
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Виберіть файл зі списку!",
                    "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }

    public class DriveFileInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string MimeType { get; set; }
        public long? Size { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public string FileType { get; set; }

        public string SizeFormatted
        {
            get
            {
                if (!Size.HasValue) return "-";

                double bytes = Size.Value;
                if (bytes < 1024) return $"{bytes} B";
                if (bytes < 1024 * 1024) return $"{bytes / 1024:F1} KB";
                if (bytes < 1024 * 1024 * 1024) return $"{bytes / (1024 * 1024):F1} MB";
                return $"{bytes / (1024 * 1024 * 1024):F1} GB";
            }
        }
    }
}
