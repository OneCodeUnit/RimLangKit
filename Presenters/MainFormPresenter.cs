using Microsoft.Extensions.Logging;
using RimLangKit.Models;
using RimLangKit.Properties;
using RimLangKit.Services;
using RimLangKit.Views.Interfaces;
using System.Diagnostics;
using System.Reflection;

namespace RimLangKit.Presenters
{
    /// <summary>
    /// Главный presenter, координирующий работу всего приложения
    /// </summary>
    public class MainFormPresenter
    {
        private readonly IMainView _mainView;
        private readonly ApplicationState _state;
        private readonly IGitHubService _githubService;
        private readonly ILogger<MainFormPresenter> _logger;

        // Специализированные презентеры
        public FileProcessingPresenter FileProcessing { get; }
        public DatabasePresenter Database { get; }
        public LanguageUpdatePresenter LanguageUpdate { get; }

        public MainFormPresenter(
            IMainView mainView,
            ApplicationState state,
            IGitHubService githubService,
            ILogger<MainFormPresenter> logger,
            FileProcessingPresenter fileProcessingPresenter,
            DatabasePresenter databasePresenter,
            LanguageUpdatePresenter languageUpdatePresenter)
        {
            _mainView = mainView ?? throw new ArgumentNullException(nameof(mainView));
            _state = state ?? throw new ArgumentNullException(nameof(state));
            _githubService = githubService ?? throw new ArgumentNullException(nameof(githubService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            FileProcessing = fileProcessingPresenter ?? throw new ArgumentNullException(nameof(fileProcessingPresenter));
            Database = databasePresenter ?? throw new ArgumentNullException(nameof(databasePresenter));
            LanguageUpdate = languageUpdatePresenter ?? throw new ArgumentNullException(nameof(languageUpdatePresenter));
        }

        /// <summary>
        /// Инициализация при загрузке формы
        /// </summary>
        public void Initialize()
        {
            _logger.LogInformation("Инициализация главной формы через presenter");

            // Загрузка сохраненного состояния
            _state.GamePath = Settings.Default.savedDirectory;
            if (!Directory.Exists(_state.GamePath))
            {
                _state.GamePath = string.Empty;
            }

            _state.LastSelectedTab = Settings.Default.lastTab;
        }

        /// <summary>
        /// Обработка изменения выбранной вкладки
        /// </summary>
        public void HandleTabChange(int selectedIndex)
        {
            _state.LastSelectedTab = selectedIndex;
            Settings.Default.lastTab = selectedIndex;
            Settings.Default.Save();
        }

        /// <summary>
        /// Переключение состояния автообновления
        /// </summary>
        public void ToggleAutoUpdate(bool isChecked)
        {
            Settings.Default.isAutoUpdateActive = isChecked;
            Settings.Default.Save();
            _logger.LogInformation("Автообновление {Status}", isChecked ? "включено" : "выключено");
        }

        /// <summary>
        /// Ручная проверка обновлений приложения
        /// </summary>
        public async Task CheckForUpdatesAsync()
        {
            await CheckVersionAsync();
        }

        /// <summary>
        /// Автоматическая проверка обновлений при запуске
        /// </summary>
        public async Task PerformAutoUpdateCheckAsync()
        {
            try
            {
                var oldCheckDate = Settings.Default.lastCheckDate;
                var newCheckDate = DateTime.Now;

                if (oldCheckDate.Month != newCheckDate.Month)
                {
                    _logger.LogInformation("Выполнение автоматической проверки обновлений");
                    bool result = await CheckVersionAsync();

                    if (result)
                    {
                        Settings.Default.lastCheckDate = newCheckDate;
                        Settings.Default.Save();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при автоматической проверке обновлений");
                // Не показываем ошибку пользователю при автоматической проверке
            }
        }

        /// <summary>
        /// Проверка версии приложения
        /// </summary>
        private async Task<bool> CheckVersionAsync()
        {
            _logger.LogInformation("Проверка обновлений приложения");

            var result = await _githubService.GetLatestReleaseAsync();

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Не удалось проверить обновления: {Error}", result.ErrorMessage);
                _mainView.ShowMessageBox(
                    result.ErrorMessage ?? "Не удалось проверить обновления. Проверьте подключение к интернету.",
                    "Ошибка",
                    MessageBoxIcon.Error);
                return false;
            }

            var json = result.Value;
            var newVersion = json?.TagName?.ToString()[1..];
            var oldVersion = Assembly.GetEntryAssembly()?.GetName().Version?.ToString()[..^2];

            if (oldVersion == newVersion)
            {
                _logger.LogInformation("Приложение обновлено до последней версии {Version}", oldVersion);
                _mainView.ShowMessageBox("Обновление не требуется", "Обновление");
                return true;
            }
            else
            {
                _logger.LogInformation("Доступна новая версия {NewVersion} (текущая: {OldVersion})", newVersion, oldVersion);

                if (_mainView.ShowConfirmation(
                    $"Доступно обновление до версии {newVersion}!\nПерейти на страницу загрузки?\n\nВ новой версии:\n{json?.Body}",
                    "Обновление"))
                {
                    var url = json?.HtmlUrl?.ToString();
                    if (!string.IsNullOrEmpty(url))
                    {
                        _logger.LogInformation("Открытие страницы релиза: {Url}", url);
                        _mainView.OpenUrl(url);
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// Показать информацию об авторе
        /// </summary>
        public void ShowAbout()
        {
            _mainView.ShowMessageBox(
                "Это полностью бесплатная программа от великого OliveWizard для сообщества RimWorld.\n\n",
                "Автор");
        }

        /// <summary>
        /// Открыть руководство
        /// </summary>
        public void OpenGuide()
        {
            _mainView.OpenUrl("https://github.com/OneCodeUnit/RimLangKit/blob/master/README.md");
        }

        /// <summary>
        /// Получить состояние приложения
        /// </summary>
        public ApplicationState GetApplicationState() => _state;
    }
}
