using Microsoft.Extensions.Logging;
using RimLangKit.Common;
using RimLangKit.Models;
using RimLangKit.Modules.GameLocalization;
using RimLangKit.Properties;
using RimLangKit.Views.Interfaces;

namespace RimLangKit.Presenters
{
    /// <summary>
    /// Presenter для обновления локализации игры
    /// </summary>
    public class LanguageUpdatePresenter
    {
        private readonly IMainView _mainView;
        private readonly ILanguageUpdateView _languageUpdateView;
        private readonly IFileProcessingView _fileProcessingView;
        private readonly ApplicationState _state;
        private readonly ILogger<LanguageUpdatePresenter> _logger;

        public LanguageUpdatePresenter(
            IMainView mainView,
            ILanguageUpdateView languageUpdateView,
            IFileProcessingView fileProcessingView,
            ApplicationState state,
            ILogger<LanguageUpdatePresenter> logger)
        {
            _mainView = mainView ?? throw new ArgumentNullException(nameof(mainView));
            _languageUpdateView = languageUpdateView ?? throw new ArgumentNullException(nameof(languageUpdateView));
            _fileProcessingView = fileProcessingView ?? throw new ArgumentNullException(nameof(fileProcessingView));
            _state = state ?? throw new ArgumentNullException(nameof(state));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Проверка и применение обновления локализации
        /// </summary>
        public void UpdateLanguage()
        {
            try
            {
                _mainView.ShowMessage("Проверка обновления");

                var result = LanguageUpdater.TranslationVersionCheckActivity(
                    Settings.Default.sha,
                    Settings.Default.repo);

                if (!result.Item1)
                {
                    _mainView.ShowMessage(result.Item2);
                    return;
                }

                _mainView.ShowMessage("Найдена новая версия");

                if (!_mainView.ShowConfirmation($"{result.Item2}.\nОбновить?", "Обновление"))
                {
                    return;
                }

                _mainView.ShowMessage("Загрузка обновления");
                result = LanguageUpdater.LanguageUpdateDownload(Settings.Default.repo);

                if (!result.Item1)
                {
                    _mainView.ShowMessage(result.Item2);
                    return;
                }

                _mainView.ShowMessage("Обновление загружено");
                _mainView.ShowMessage("Установка обновления");

                result = LanguageUpdater.LanguageUpdateActivity(
                    Settings.Default.savedDirectory,
                    Settings.Default.language);

                if (!result.Item1)
                {
                    _mainView.ShowMessage(result.Item2);
                    return;
                }

                _mainView.ShowMessage(result.Item2);

                Settings.Default.sha = LanguageUpdater.GetSha();
                Settings.Default.Save();

                _logger.LogInformation("Обновление локализации завершено успешно");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка обновления локализации");
                _mainView.ShowMessageBox(
                    $"Ошибка обновления локализации: {ex.Message}",
                    "Ошибка",
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Удаление скачанного перевода
        /// </summary>
        public void ResetLanguage()
        {
            try
            {
                string dataPath = $"{Settings.Default.savedDirectory}\\{Constants.Paths.DataFolderName}";

                if (!Directory.Exists(dataPath))
                {
                    _mainView.ShowMessageBox("В указанной папке нет перевода", "Ошибка", MessageBoxIcon.Error);
                    return;
                }

                string[] modules = Directory.GetDirectories(dataPath);

                if (modules.Length == 0)
                {
                    _mainView.ShowMessageBox("В указанной папке нет дополнений", "Ошибка", MessageBoxIcon.Error);
                    return;
                }

                string modulesList = string.Empty;
                List<string> languagePathList = [];

                foreach (var module in modules)
                {
                    string moduleName = module[module.LastIndexOf('\\')..][1..];
                    string languagePath = $"{module}\\Languages\\{Settings.Default.language}";

                    if (Directory.Exists(languagePath))
                    {
                        modulesList += $"- {moduleName}: {languagePath}\n";
                        languagePathList.Add(languagePath);
                    }
                }

                if (languagePathList.Count == 0)
                {
                    _mainView.ShowMessageBox("Перевод модулей не найден", "Ошибка", MessageBoxIcon.Error);
                    return;
                }

                if (!_mainView.ShowConfirmation(
                    $"Вы собираетесь удалить перевод, созданный в этой программе. Другие переводы затронуты не будут.\nБудут удалены следующие папки:\n\n{modulesList}",
                    "Удаление"))
                {
                    return;
                }

                Settings.Default.sha = Constants.Repository.DefaultSha;
                Settings.Default.Save();

                foreach (var path in languagePathList)
                {
                    Directory.Delete(path, true);
                }

                _mainView.ShowMessageBox("Удаление завершено", "Удаление");
                _logger.LogInformation("Удаление перевода завершено, удалено папок: {Count}", languagePathList.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка удаления перевода");
                _mainView.ShowMessageBox(
                    $"Ошибка удаления перевода: {ex.Message}",
                    "Ошибка",
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Сброс настроек к значениям по умолчанию
        /// </summary>
        public void ResetSettings()
        {
            if (!_mainView.ShowConfirmation(
                "Вы действительно хотите вернуть все параметры в этом окне к изначальным?",
                "Сброс настроек"))
            {
                return;
            }

            Settings.Default.sha = Constants.Repository.DefaultSha;
            Settings.Default.repo = Constants.Repository.DefaultRepo;
            Settings.Default.language = Constants.Repository.DefaultLanguage;
            Settings.Default.savedDirectory = string.Empty;
            Settings.Default.Save();

            _languageUpdateView.SetLanguageValue(Constants.Repository.DefaultLanguage);
            _languageUpdateView.SetRepositoryValue(Constants.Repository.DefaultRepo);
            _state.GamePath = string.Empty;

            _mainView.ShowMessageBox("Параметры сброшены", "Сброс настроек");
            _logger.LogInformation("Настройки локализации сброшены к значениям по умолчанию");
        }

        /// <summary>
        /// Обработка изменения репозитория
        /// </summary>
        public void HandleRepositoryChange(string value)
        {
            Settings.Default.repo = value;
            Settings.Default.Save();
            CheckTextBoxes();
        }

        /// <summary>
        /// Обработка изменения языка
        /// </summary>
        public void HandleLanguageChange(string value)
        {
            Settings.Default.language = value;
            Settings.Default.Save();
            CheckTextBoxes();
        }

        /// <summary>
        /// Проверка доступности кнопок обновления
        /// </summary>
        private void CheckTextBoxes()
        {
            bool isValid = _state.IsGamePathValid &&
                          !string.IsNullOrEmpty(_languageUpdateView.GetRepositoryValue()) &&
                          !string.IsNullOrEmpty(_languageUpdateView.GetLanguageValue());

            _mainView.SetLanguageUpdateButtonsEnabled(isValid);
        }

        /// <summary>
        /// Обработка выбора папки игры
        /// </summary>
        public void HandleGameFolderSelection()
        {
            if (_fileProcessingView.ShowFolderBrowserDialog(out string selectedPath))
            {
                // Валидация пути
                if (!PathValidator.IsPathSafe(selectedPath, out string? errorMessage))
                {
                    _logger.LogWarning("Выбран небезопасный путь игры: {Path}. Причина: {Error}", selectedPath, errorMessage);
                    _mainView.ShowMessageBox(
                        $"Выбранный путь небезопасен:\n{errorMessage}",
                        "Ошибка выбора папки",
                        MessageBoxIcon.Warning);
                    return;
                }

                _state.GamePath = selectedPath;
                _logger.LogInformation("Выбрана папка игры: {Path}", selectedPath);
            }
        }

        /// <summary>
        /// Валидация пути к игре
        /// </summary>
        public void ValidateGamePath(string path)
        {
            _mainView.SetGameFolderButtonColor(_mainView.BadColor);

            if (!Directory.Exists(path))
            {
                CheckTextBoxes();
                return;
            }

            _mainView.SetGameFolderButtonColor(_mainView.GoodColor);

            string dataPath = $"{path}\\{Constants.Paths.DataFolderName}";

            if (!Directory.Exists(dataPath))
            {
                _mainView.ShowMessage("Нет модулей", 1);
                _mainView.SetGameFolderButtonColor(Color.MediumBlue);
                CheckTextBoxes();
                return;
            }

            string[] modules = Directory.GetDirectories(dataPath);

            if (modules.Length == 0)
            {
                _mainView.ShowMessage("Нет модулей", 1);
                _mainView.SetGameFolderButtonColor(Color.MediumBlue);
            }
            else
            {
                _mainView.ShowMessage("Найдены модули:", 1);
                foreach (var module in modules)
                {
                    string moduleName = module[module.LastIndexOf('\\')..][1..];
                    _mainView.ShowMessage(moduleName, 1);
                }

                _state.GamePath = path;
                Settings.Default.savedDirectory = path;
                Settings.Default.Save();
            }

            CheckTextBoxes();
        }
    }
}
