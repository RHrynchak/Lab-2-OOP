using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Lab_2.Models;
using Microsoft.Win32;

namespace Lab_2.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Fields

        private readonly Logger _logger;
        private GoogleDriveService _googleDriveService;

        private string _currentXmlPath;
        private string _currentXslPath;
        private string _statusMessage;
        private string _resultsHeader;
        private string _searchTime;
        private string _googleDriveStatus;
        private bool _isGoogleDriveConnected;

        private string _selectedPlatform;
        private string _selectedMultiplayer;
        private string _selectedRating;
        private string _titleKeyword;
        private string _genreKeyword;
        private string _developerKeyword;
        private string _publisherKeyword;
        private decimal? _priceFrom;
        private decimal? _priceTo;
        private DateTime? _releaseDateFrom;
        private DateTime? _releaseDateTo;
        private string _descriptionKeyword;

        private int _selectedParserIndex;

        #endregion

        #region Properties

        public string CurrentXmlPath
        {
            get => _currentXmlPath;
            set => SetProperty(ref _currentXmlPath, value);
        }

        public string CurrentXslPath
        {
            get => _currentXslPath;
            set => SetProperty(ref _currentXslPath, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public string ResultsHeader
        {
            get => _resultsHeader;
            set => SetProperty(ref _resultsHeader, value);
        }

        public string SearchTime
        {
            get => _searchTime;
            set => SetProperty(ref _searchTime, value);
        }

        public string GoogleDriveStatus
        {
            get => _googleDriveStatus;
            set => SetProperty(ref _googleDriveStatus, value);
        }

        public bool IsGoogleDriveConnected
        {
            get => _isGoogleDriveConnected;
            set
            {
                SetProperty(ref _isGoogleDriveConnected, value);
                GoogleDriveStatus = value
                    ? "☁️ Google Drive: Підключено ✅"
                    : "☁️ Google Drive: Не підключено";
            }
        }

        public ObservableCollection<Game> SearchResults { get; set; }
        public ObservableCollection<string> AvailablePlatforms { get; set; }
        public ObservableCollection<string> AvailableRatings { get; set; }

        // Search Criteria Properties
        public string SelectedPlatform
        {
            get => _selectedPlatform;
            set => SetProperty(ref _selectedPlatform, value);
        }

        public string SelectedMultiplayer
        {
            get => _selectedMultiplayer;
            set => SetProperty(ref _selectedMultiplayer, value);
        }

        public string SelectedRating
        {
            get => _selectedRating;
            set => SetProperty(ref _selectedRating, value);
        }

        public string TitleKeyword
        {
            get => _titleKeyword;
            set => SetProperty(ref _titleKeyword, value);
        }

        public string GenreKeyword
        {
            get => _genreKeyword;
            set => SetProperty(ref _genreKeyword, value);
        }

        public string DeveloperKeyword
        {
            get => _developerKeyword;
            set => SetProperty(ref _developerKeyword, value);
        }

        public string PublisherKeyword
        {
            get => _publisherKeyword;
            set => SetProperty(ref _publisherKeyword, value);
        }

        public decimal? PriceFrom
        {
            get => _priceFrom;
            set => SetProperty(ref _priceFrom, value);
        }

        public decimal? PriceTo
        {
            get => _priceTo;
            set => SetProperty(ref _priceTo, value);
        }

        public DateTime? ReleaseDateFrom
        {
            get => _releaseDateFrom;
            set => SetProperty(ref _releaseDateFrom, value);
        }

        public DateTime? ReleaseDateTo
        {
            get => _releaseDateTo;
            set => SetProperty(ref _releaseDateTo, value);
        }

        public string DescriptionKeyword
        {
            get => _descriptionKeyword;
            set => SetProperty(ref _descriptionKeyword, value);
        }

        public int SelectedParserIndex
        {
            get => _selectedParserIndex;
            set => SetProperty(ref _selectedParserIndex, value);
        }

        #endregion

        #region Commands

        public ICommand SelectXmlFileCommand { get; }
        public ICommand SelectXslFileCommand { get; }
        public ICommand LoadXmlFromDriveCommand { get; }
        public ICommand LoadXslFromDriveCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand SaveLocalXmlCommand { get; }
        public ICommand SaveLocalHtmlCommand { get; }
        public ICommand SaveDriveXmlCommand { get; }
        public ICommand SaveDriveHtmlCommand { get; }
        public ICommand TransformFullCommand { get; }
        public ICommand ConnectGoogleDriveCommand { get; }
        public ICommand ShowDriveFilesCommand { get; }
        public ICommand OpenLogCommand { get; }

        #endregion

        #region Constructor

        public MainViewModel()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return;
            }

            _logger = Logger.GetInstance();
            _logger.LogInfo("ViewModel ініціалізовано");

            SearchResults = new ObservableCollection<Game>();
            AvailablePlatforms = new ObservableCollection<string> { "(Всі)", "PC", "PlayStation", "Xbox", "Nintendo Switch" };
            AvailableRatings = new ObservableCollection<string> { "(Всі)", "E", "E10+", "T", "M" };

            SelectXmlFileCommand = new RelayCommand(SelectXmlFile);
            SelectXslFileCommand = new RelayCommand(SelectXslFile);
            LoadXmlFromDriveCommand = new RelayCommand(LoadXmlFromDrive, () => IsGoogleDriveConnected);
            LoadXslFromDriveCommand = new RelayCommand(LoadXslFromDrive, () => IsGoogleDriveConnected);
            SearchCommand = new RelayCommand(ExecuteSearch, CanExecuteSearch);
            ClearCommand = new RelayCommand(Clear);
            SaveLocalXmlCommand = new RelayCommand(SaveLocalXml, () => SearchResults.Count > 0);
            SaveLocalHtmlCommand = new RelayCommand(SaveLocalHtml, () => SearchResults.Count > 0);
            SaveDriveXmlCommand = new RelayCommand(SaveDriveXml, () => IsGoogleDriveConnected && SearchResults.Count > 0);
            SaveDriveHtmlCommand = new RelayCommand(SaveDriveHtml, () => IsGoogleDriveConnected && SearchResults.Count > 0);
            TransformFullCommand = new RelayCommand(TransformFull, () => !string.IsNullOrEmpty(CurrentXmlPath) && !string.IsNullOrEmpty(CurrentXslPath));
            ConnectGoogleDriveCommand = new RelayCommand(ConnectGoogleDrive);
            ShowDriveFilesCommand = new RelayCommand(ShowDriveFiles, () => IsGoogleDriveConnected);
            OpenLogCommand = new RelayCommand(OpenLog);

            StatusMessage = "Готовий до роботи";
            ResultsHeader = "📊 Результати пошуку: 0 ігор";
            GoogleDriveStatus = "☁️ Google Drive: Не підключено";
            SelectedParserIndex = 0;

            LoadDefaultFiles();
        }

        #endregion

        #region Methods

        private void LoadDefaultFiles()
        {
            if (File.Exists("GameLibrary.xml"))
            {
                CurrentXmlPath = Path.GetFullPath("GameLibrary.xml");
                LoadAvailableAttributes();
            }

            if (File.Exists("GameLibrary.xsl"))
            {
                CurrentXslPath = Path.GetFullPath("GameLibrary.xsl");
            }
        }

        private void LoadAvailableAttributes()
        {
            try
            {
                var context = new XmlParserContext();
                context.SetStrategy(new LinqParserStrategy());
                var allGames = context.Parse(CurrentXmlPath, new SearchCriteria());

                var platforms = allGames.Select(g => g.Platform).Distinct().Where(p => !string.IsNullOrEmpty(p)).OrderBy(p => p).ToList();
                AvailablePlatforms.Clear();
                AvailablePlatforms.Add("(Всі)");
                foreach (var platform in platforms)
                {
                    AvailablePlatforms.Add(platform);
                }

                var ratings = allGames.Select(g => g.Rating).Distinct().Where(r => !string.IsNullOrEmpty(r)).OrderBy(r => r).ToList();
                AvailableRatings.Clear();
                AvailableRatings.Add("(Всі)");
                foreach (var rating in ratings)
                {
                    AvailableRatings.Add(rating);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Завантаження атрибутів", ex);
            }
        }

        private SearchCriteria BuildSearchCriteria()
        {
            bool? multiplayerValue = SelectedMultiplayer switch
            {
                "Так" => true,
                "Ні" => false,
                _ => null
            };

            return new SearchCriteria
            {
                Platform = SelectedPlatform == "(Всі)" ? string.Empty : SelectedPlatform,
                Multiplayer = multiplayerValue,
                Rating = SelectedRating == "(Всі)" ? string.Empty : SelectedRating,
                TitleKeyword = TitleKeyword,
                GenreKeyword = GenreKeyword,
                DeveloperKeyword = DeveloperKeyword,
                PublisherKeyword = PublisherKeyword,
                PriceFrom = PriceFrom,
                PriceTo = PriceTo,
                ReleaseDateFrom = ReleaseDateFrom,
                ReleaseDateTo = ReleaseDateTo,
                DescriptionKeyword = DescriptionKeyword
            };
        }

        private IXmlParserStrategy GetSelectedParser()
        {
            return SelectedParserIndex switch
            {
                0 => new SaxParserStrategy(),
                1 => new DomParserStrategy(),
                2 => new LinqParserStrategy(),
                _ => new LinqParserStrategy()
            };
        }

        private string GetParserName()
        {
            return SelectedParserIndex switch
            {
                0 => "SAX",
                1 => "DOM",
                2 => "LINQ",
                _ => "LINQ"
            };
        }

        #endregion

        #region Command Implementations
        private void ResetResults()
        {
            SearchResults.Clear();
            ResultsHeader = "📊 Результати пошуку: 0 ігор";
            SearchTime = string.Empty;
        }

        private void SelectXmlFile()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "XML Files (*.xml)|*.xml",
                Title = "Виберіть XML файл"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                if (Path.GetExtension(openFileDialog.FileName).ToLower() != ".xml")
                {
                    MessageBox.Show("Будь ласка, оберіть файл з розширенням .xml",
                                    "Невірний формат",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Warning);
                    return;
                }
                CurrentXmlPath = openFileDialog.FileName;
                LoadAvailableAttributes();
                ResetResults();
                StatusMessage = $"XML файл завантажено: {Path.GetFileName(CurrentXmlPath)}";
                _logger.LogInfo($"Вибрано XML файл: {CurrentXmlPath}");
            }
        }

        private void SelectXslFile()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "XSL Files (*.xslt;*.xsl)|*.xslt;*.xsl",
                Title = "Виберіть XSL файл"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                if (Path.GetExtension(openFileDialog.FileName).ToLower() != ".xsl" && Path.GetExtension(openFileDialog.FileName).ToLower() != ".xslt")
                {
                    MessageBox.Show("Будь ласка, оберіть файл з розширенням .xsl або .xslt",
                                    "Невірний формат",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Warning);
                    return;
                }
                CurrentXslPath = openFileDialog.FileName;
                StatusMessage = $"XSL файл завантажено: {Path.GetFileName(CurrentXslPath)}";
                _logger.LogInfo($"Вибрано XSL файл: {CurrentXslPath}");
            }
        }

        private void LoadXmlFromDrive()
        {
            try
            {
                var picker = new DriveFilePickerWindow(_googleDriveService);
                picker.FileTypeFilterComboBox.SelectedIndex = 1;

                if (picker.ShowDialog() == true && picker.SelectedFile != null)
                {
                    bool isXml = picker.SelectedFile.FileType == "XML" ||
                         picker.SelectedFile.Name.EndsWith(".xml", StringComparison.OrdinalIgnoreCase);

                    if (!isXml)
                    {
                        MessageBox.Show("Ви вибрали невірний файл! Будь ласка, оберіть XML файл.",
                                        "Невірний формат",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Warning);
                        return;
                    }

                    StatusMessage = "Завантаження XML з Google Drive...";

                    string tempPath = Path.Combine(Path.GetTempPath(), picker.SelectedFile.Name);
                    _googleDriveService.DownloadFile(picker.SelectedFile.Id, tempPath);

                    CurrentXmlPath = tempPath;
                    LoadAvailableAttributes();
                    ResetResults();

                    StatusMessage = $"XML завантажено з Google Drive: {picker.SelectedFile.Name}";
                    _logger.LogInfo($"XML завантажено з Google Drive: {picker.SelectedFile.Name}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження XML: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                _logger.LogError("Завантаження XML з Google Drive", ex);
            }
        }

        private void LoadXslFromDrive()
        {
            try
            {
                var picker = new DriveFilePickerWindow(_googleDriveService);
                picker.FileTypeFilterComboBox.SelectedIndex = 2;

                if (picker.ShowDialog() == true && picker.SelectedFile != null)
                {
                    bool isXsl = picker.SelectedFile.FileType == "XSLT" ||
                         picker.SelectedFile.Name.EndsWith(".xsl", StringComparison.OrdinalIgnoreCase) ||
                         picker.SelectedFile.Name.EndsWith(".xslt", StringComparison.OrdinalIgnoreCase);

                    if (!isXsl)
                    {
                        MessageBox.Show("Ви вибрали невірний файл! Будь ласка, оберіть XSL/XSLT файл.",
                                        "Невірний формат",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Warning);
                        return;
                    }

                    StatusMessage = "Завантаження XSL з Google Drive...";

                    string tempPath = Path.Combine(Path.GetTempPath(), picker.SelectedFile.Name);
                    _googleDriveService.DownloadFile(picker.SelectedFile.Id, tempPath);

                    CurrentXslPath = tempPath;
                    StatusMessage = $"XSL завантажено з Google Drive: {picker.SelectedFile.Name}";
                    _logger.LogInfo($"XSL завантажено з Google Drive: {picker.SelectedFile.Name}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження XSL: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                _logger.LogError("Завантаження XSL з Google Drive", ex);
            }
        }

        private bool CanExecuteSearch()
        {
            return !string.IsNullOrEmpty(CurrentXmlPath);
        }

        private void ExecuteSearch()
        {
            try
            {
                var criteria = BuildSearchCriteria();
                var parserName = GetParserName();

                StatusMessage = $"Пошук з {parserName}...";
                _logger.LogInfo($"Початок пошуку: {parserName}");

                var stopwatch = Stopwatch.StartNew();

                var context = new XmlParserContext();
                context.SetStrategy(GetSelectedParser());
                var results = context.Parse(CurrentXmlPath, criteria);

                stopwatch.Stop();

                SearchResults.Clear();
                foreach (var game in results)
                {
                    SearchResults.Add(game);
                }

                ResultsHeader = $"📊 Результати пошуку: {SearchResults.Count} ігор";
                SearchTime = $"⏱️ {stopwatch.ElapsedMilliseconds} мс";
                StatusMessage = $"Знайдено {SearchResults.Count} ігор за {stopwatch.ElapsedMilliseconds} мс";

                _logger.LogInfo($"Пошук завершено: знайдено {SearchResults.Count} ігор");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка пошуку: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                _logger.LogError("Пошук", ex);
            }
        }

        private void Clear()
        {
            var result = MessageBox.Show(
                "Ви дійсно хочете очистити всі поля пошуку?",
                "Підтвердження",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                SelectedPlatform = "(Всі)";
                SelectedMultiplayer = "(Всі)";
                SelectedRating = "(Всі)";
                TitleKeyword = string.Empty;
                GenreKeyword = string.Empty;
                DeveloperKeyword = string.Empty;
                PublisherKeyword = string.Empty;
                PriceFrom = null;
                PriceTo = null;
                ReleaseDateFrom = null;
                ReleaseDateTo = null;
                DescriptionKeyword = string.Empty;

                SearchResults.Clear();
                ResultsHeader = "📊 Результати пошуку: 0 ігор";
                SearchTime = string.Empty;
                StatusMessage = "Поля очищено";

                _logger.LogInfo("Поля пошуку очищено");
            }
        }

        private void SaveLocalXml()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "XML Files (*.xml)|*.xml",
                FileName = "FilteredGames.xml"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    FileExporter exporter = new XmlExporter();
                    exporter.Export(SearchResults.ToList(), saveFileDialog.FileName);

                    StatusMessage = $"XML збережено: {Path.GetFileName(saveFileDialog.FileName)}";
                    MessageBox.Show($"Файл збережено успішно!\n{saveFileDialog.FileName}", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка збереження: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    _logger.LogError("Збереження локального XML", ex);
                }
            }
        }

        private void SaveLocalHtml()
        {
            if (string.IsNullOrEmpty(CurrentXslPath))
            {
                MessageBox.Show("XSL файл не вибрано!", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "HTML Files (*.html)|*.html",
                FileName = "FilteredGames.html"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    FileExporter exporter = new HtmlExporter(CurrentXslPath);
                    exporter.Export(SearchResults.ToList(), saveFileDialog.FileName);

                    StatusMessage = $"HTML збережено: {Path.GetFileName(saveFileDialog.FileName)}";

                    var result = MessageBox.Show(
                        $"Файл збережено успішно!\n{saveFileDialog.FileName}\n\nВідкрити в браузері?",
                        "Успіх", MessageBoxButton.YesNo, MessageBoxImage.Information);

                    if (result == MessageBoxResult.Yes)
                    {
                        Process.Start(new ProcessStartInfo { FileName = saveFileDialog.FileName, UseShellExecute = true });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка збереження: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    _logger.LogError("Збереження локального HTML", ex);
                }
            }
        }

        private void SaveDriveXml()
        {
            try
            {
                StatusMessage = "Завантаження XML на Google Drive...";

                FileExporter exporter = new GoogleDriveXmlExporter(_googleDriveService);
                string fileName = $"FilteredGames_{DateTime.Now:dd-MM-yyyy_HH-mm-ss}.xml";
                exporter.Export(SearchResults.ToList(), fileName);

                StatusMessage = "XML завантажено на Google Drive!";
                MessageBox.Show("XML файл успішно завантажено на Google Drive!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                _logger.LogError("Завантаження XML на Google Drive", ex);
            }
        }

        private void SaveDriveHtml()
        {
            if (string.IsNullOrEmpty(CurrentXslPath))
            {
                MessageBox.Show("XSL файл не вибрано!", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                StatusMessage = "Завантаження HTML на Google Drive...";

                FileExporter exporter = new GoogleDriveHtmlExporter(_googleDriveService, CurrentXslPath);
                string fileName = $"FilteredGames_{DateTime.Now:dd-MM-yyyy_HH-mm-ss}.html";
                exporter.Export(SearchResults.ToList(), fileName);

                StatusMessage = "HTML завантажено на Google Drive!";
                MessageBox.Show("HTML файл успішно завантажено на Google Drive!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                _logger.LogError("Завантаження HTML на Google Drive", ex);
            }
        }

        private void TransformFull()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "HTML Files (*.html)|*.html",
                FileName = "FullGameLibrary.html"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    StatusMessage = "Трансформація XML → HTML...";

                    var transformer = new XslTransformer();
                    transformer.Transform(CurrentXmlPath, CurrentXslPath, saveFileDialog.FileName);

                    StatusMessage = $"HTML створено: {Path.GetFileName(saveFileDialog.FileName)}";

                    var result = MessageBox.Show(
                        $"Трансформація завершена!\n{saveFileDialog.FileName}\n\nВідкрити в браузері?",
                        "Успіх", MessageBoxButton.YesNo, MessageBoxImage.Information);

                    if (result == MessageBoxResult.Yes)
                    {
                        Process.Start(new ProcessStartInfo { FileName = saveFileDialog.FileName, UseShellExecute = true });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка трансформації: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    _logger.LogError("Трансформація повного XML", ex);
                }
            }
        }

        private void ConnectGoogleDrive()
        {
            try
            {
                StatusMessage = "Підключення до Google Drive...";

                _googleDriveService = new GoogleDriveService();
                _googleDriveService.Initialize("credentials.json");

                IsGoogleDriveConnected = true;
                StatusMessage = "Google Drive підключено успішно!";
                MessageBox.Show("Google Drive підключено успішно!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                IsGoogleDriveConnected = false;
                MessageBox.Show(
                    $"Не вдалося підключитися до Google Drive:\n{ex.Message}\n\n" +
                    "Переконайтеся, що файл credentials.json існує та Google Drive API увімкнено.",
                    "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                _logger.LogError("Підключення Google Drive", ex);
            }
        }

        private void ShowDriveFiles()
        {
            try
            {
                var picker = new DriveFilePickerWindow(_googleDriveService);
                picker.Title = "Файли на Google Drive";
                picker.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка відображення файлів: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenLog()
        {
            try
            {
                _logger.OpenLogFile();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не вдалося відкрити файл логу: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            _logger.LogInfo("Програма закрита");
        }

        #endregion
    }
}
