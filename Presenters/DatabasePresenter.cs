using Microsoft.Extensions.Logging;
using RimLangKit.Checks;
using RimLangKit.Common;
using RimLangKit.Models;
using RimLangKit.Modules.AutoTranslation;
using RimLangKit.Views.Interfaces;

namespace RimLangKit.Presenters
{
    /// <summary>
    /// Presenter для работы с базой данных переводов
    /// </summary>
    public class DatabasePresenter
    {
        private readonly IMainView _mainView;
        private readonly IDatabaseView _databaseView;
        private readonly IFileProcessingView _fileProcessingView;
        private readonly ApplicationState _state;
        private readonly ILogger<DatabasePresenter> _logger;

        public DatabasePresenter(
            IMainView mainView,
            IDatabaseView databaseView,
            IFileProcessingView fileProcessingView,
            ApplicationState state,
            ILogger<DatabasePresenter> logger)
        {
            _mainView = mainView ?? throw new ArgumentNullException(nameof(mainView));
            _databaseView = databaseView ?? throw new ArgumentNullException(nameof(databaseView));
            _fileProcessingView = fileProcessingView ?? throw new ArgumentNullException(nameof(fileProcessingView));
            _state = state ?? throw new ArgumentNullException(nameof(state));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Обработка выбора файла базы данных
        /// </summary>
        public void HandleDatabaseSelection()
        {
            if (_databaseView.ShowDatabaseFileDialog(out string selectedPath))
            {
                _state.SelectedDatabasePath = selectedPath;
                _logger.LogInformation("Выбран файл базы данных: {Path}", selectedPath);
            }
        }

        /// <summary>
        /// Валидация и загрузка базы данных
        /// </summary>
        public void ValidateDatabasePath(string path)
        {
            _state.SelectedDatabasePath = path;

            if (File.Exists(path) && path.EndsWith(Constants.Files.DatabaseExtension, StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var result = AutoTranslator.LoadDatabase(path);
                    _mainView.SetDatabaseCheckLabelText($"Сейчас в базе: {result.Message}");
                    _logger.LogInformation("База данных загружена: {Path}, содержит: {Message}", path, result.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка загрузки базы данных: {Path}", path);
                    _mainView.SetDatabaseCheckLabelText("Ошибка загрузки базы данных");
                }
            }
            else
            {
                _mainView.SetDatabaseCheckLabelText(string.Empty);
            }
        }

        /// <summary>
        /// Создание новой базы данных
        /// </summary>
        public void CreateDatabase()
        {
            string databasePath = string.IsNullOrEmpty(_state.SelectedDatabasePath)
                ? Path.Combine(AppContext.BaseDirectory, Constants.Files.DefaultDatabaseName)
                : _state.SelectedDatabasePath;

            if (_mainView.ShowConfirmation(
                $"База данных будет создана по адресу {databasePath}.\nСоздать?",
                "Создание базы данных"))
            {
                try
                {
                    AutoTranslator.NewDatabase(databasePath);
                    _mainView.ShowMessageBox("База данных создана", "Создание базы данных");

                    _state.SelectedDatabasePath = databasePath;
                    ValidateDatabasePath(databasePath);

                    _logger.LogInformation("Создана новая база данных: {Path}", databasePath);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка создания базы данных: {Path}", databasePath);
                    _mainView.ShowMessageBox(
                        $"Ошибка создания базы данных: {ex.Message}",
                        "Ошибка",
                        MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Обновление базы данных из папки с переводами
        /// </summary>
        public void UpdateDatabase()
        {
            if (string.IsNullOrEmpty(_state.SelectedDatabasePath))
            {
                _mainView.ShowMessageBox("Выберите файл базы данных", "Ошибка", MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(_state.TranslationFolder))
            {
                _mainView.ShowMessageBox("Выберите папку с переводами", "Ошибка", MessageBoxIcon.Warning);
                return;
            }

            try
            {
                bool rewrite = _databaseView.GetRewriteMode();

                string[] allFiles = Directory.GetFiles(_state.TranslationFolder, Constants.Files.XmlMask, SearchOption.AllDirectories);
                List<XmlError> results = [];

                foreach (string currentFile in allFiles)
                {
                    var result = AutoTranslator.CreateDatabase(_state.SelectedDatabasePath, currentFile, "translated_tags", rewrite);
                    results.Add(new XmlError(result.IsValid, $"{currentFile}: {result.Message}"));
                }

                _mainView.ShowMessageBox("Данные добавлены", "Обновление базы данных");
                ValidateDatabasePath(_state.SelectedDatabasePath);

                _logger.LogInformation("База данных обновлена, обработано файлов: {Count}", allFiles.Length);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка обновления базы данных");
                _mainView.ShowMessageBox(
                    $"Ошибка обновления базы данных: {ex.Message}",
                    "Ошибка",
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Обработка выбора папки с переводами для обновления БД
        /// </summary>
        public void HandleTranslationFolderSelection()
        {
            if (_fileProcessingView.ShowFolderBrowserDialog(out string selectedPath))
            {
                // Валидация пути
                if (!PathValidator.IsPathSafe(selectedPath, out string? errorMessage))
                {
                    _logger.LogWarning("Выбран небезопасный путь для БД: {Path}. Причина: {Error}", selectedPath, errorMessage);
                    _mainView.ShowMessageBox(
                        $"Выбранный путь небезопасен:\n{errorMessage}",
                        "Ошибка выбора папки",
                        MessageBoxIcon.Warning);
                    return;
                }

                _state.TranslationFolder = selectedPath;
                _logger.LogInformation("Выбрана папка с переводами: {Path}", selectedPath);
            }
        }

        /// <summary>
        /// Валидация папки с переводами
        /// </summary>
        public void ValidateTranslationFolder(string path)
        {
            if (Directory.Exists(path))
            {
                _state.TranslationFolder = path;
            }
        }

        /// <summary>
        /// Автоперевод файлов из папки
        /// </summary>
        public void AutoTranslateFiles()
        {
            if (string.IsNullOrEmpty(_state.SelectedDatabasePath))
            {
                _mainView.ShowMessageBox("Выберите файл базы данных", "Ошибка", MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(_state.AutoTranslateModFolder))
            {
                _mainView.ShowMessageBox("Выберите папку с модом для перевода", "Ошибка", MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string[] allFiles = Directory.GetFiles(_state.AutoTranslateModFolder, Constants.Files.XmlMask, SearchOption.AllDirectories);
                List<XmlError> results = [];

                foreach (string currentFile in allFiles)
                {
                    var result = AutoTranslator.TranslateFile(_state.SelectedDatabasePath, currentFile, "translated_tags");
                    results.Add(new XmlError(result.IsValid, $"{currentFile}: {result.Message}"));
                }

                _mainView.ShowMessageBox("Файлы переведены", "Автоперевод");
                _logger.LogInformation("Автоперевод завершен, обработано файлов: {Count}", allFiles.Length);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка автоперевода");
                _mainView.ShowMessageBox(
                    $"Ошибка автоперевода: {ex.Message}",
                    "Ошибка",
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Обработка выбора папки мода для автоперевода
        /// </summary>
        public void HandleModFolderSelection()
        {
            if (_fileProcessingView.ShowFolderBrowserDialog(out string selectedPath))
            {
                // Валидация пути
                if (!PathValidator.IsPathSafe(selectedPath, out string? errorMessage))
                {
                    _logger.LogWarning("Выбран небезопасный путь мода: {Path}. Причина: {Error}", selectedPath, errorMessage);
                    _mainView.ShowMessageBox(
                        $"Выбранный путь небезопасен:\n{errorMessage}",
                        "Ошибка выбора папки",
                        MessageBoxIcon.Warning);
                    return;
                }

                _state.AutoTranslateModFolder = selectedPath;
                _logger.LogInformation("Выбрана папка с модом: {Path}", selectedPath);
            }
        }

        /// <summary>
        /// Валидация папки мода
        /// </summary>
        public void ValidateModFolder(string path)
        {
            if (Directory.Exists(path))
            {
                _state.AutoTranslateModFolder = path;
            }
        }
    }
}
