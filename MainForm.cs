using Microsoft.Extensions.Logging;
using RimLangKit.Checks;
using RimLangKit.Common;
using RimLangKit.Modules.AutoTranslation;
using RimLangKit.Modules.GameLocalization;
using RimLangKit.Processors;
using RimLangKit.Properties;
using RimLangKit.Services;
using RimLangKit.Utilities;
using System.Diagnostics;
using System.Reflection;

namespace RimLangKit
{
    public partial class MainForm : Form
    {
        private readonly IGitHubService _githubService;
        private readonly IMorpherService _morpherService;
        private readonly ILogger<MainForm> _logger;

        // Управление отменой длительных операций
        private CancellationTokenSource? _currentOperationCts;

        private static string DirectoryPath = string.Empty;
        private static string GamePath = string.Empty;
        private static string AdditionalFolder = string.Empty;
        private static string SelectedDBPath = string.Empty;
        private static string TranslationFolder = string.Empty;
        private static string AutoTranslateModFolder = string.Empty;
        private readonly Color goodColor = Color.FromArgb(0, 130, 0);
        private readonly Color badColor = Color.FromArgb(130, 0, 0);

        public MainForm(IGitHubService githubService, IMorpherService morpherService, ILogger<MainForm> logger)
        {
            _githubService = githubService ?? throw new ArgumentNullException(nameof(githubService));
            _morpherService = morpherService ?? throw new ArgumentNullException(nameof(morpherService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _logger.LogInformation("Инициализация главной формы");

            InitializeComponent();
            FolderTextBox.Text = DirectoryPath;

            // Обновление настроек при первом запуске
            if (Settings.Default.firstLaunch)
            {
                Settings.Default.Upgrade();
                Settings.Default.firstLaunch = false;
                Settings.Default.Save();
                SendToInfoTextBox("Первый запуск. Обновление настроек завершено");
            }

            // Инициализация меню "О программе"
            ToolStripMenuItemVersion.Text += Assembly.GetEntryAssembly()?.GetName().Version?.ToString()[..^2];

            // Инициализация меню "Автор"
            ToolStripMenuItemCreator.Text += " OliveWizard";

            // Автоматическая проверка обновлений (перенесено в Load событие)
            if (Settings.Default.isAutoUpdateActive)
            {
                ToolStripMenuItemAutoUpdateCheck.Checked = true;
                this.Load += async (s, e) => await PerformAutoUpdateCheckAsync();
            }

            // Восстановление последней вкладки
            MainTabs.SelectTab(Settings.Default.lastTab);

            // Установка размера вкладок
            MainTabs.SizeMode = TabSizeMode.Fixed;
            MainTabs.ItemSize = new Size((MainTabs.Width / MainTabs.TabPages.Count) - 2, MainTabs.ItemSize.Height);

            // Восстановление пути к папке игры
            GamePath = Settings.Default.savedDirectory;
            if (!Directory.Exists(GamePath))
                GamePath = string.Empty;
            FolderTextBox2.Text = GamePath;

            LanguageInput.Text = Settings.Default.language;
            RepoInput.Text = Settings.Default.repo;
        }

        // Выбор папки
        private void FolderButton_Click(object sender, EventArgs e)
        {
            using FolderBrowserDialog ofd = new();
            DialogResult dr = ofd.ShowDialog();
            if (dr == DialogResult.OK)
            {
                string selectedPath = ofd.SelectedPath;

                // Валидация пути
                if (!PathValidator.IsPathSafe(selectedPath, out string? errorMessage))
                {
                    _logger.LogWarning("Выбран небезопасный путь: {Path}. Причина: {Error}", selectedPath, errorMessage);
                    MessageBox.Show(
                        $"Выбранный путь небезопасен:\n{errorMessage}",
                        "Ошибка выбора папки",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                DirectoryPath = selectedPath;
                FolderTextBox.Text = DirectoryPath;
                _logger.LogInformation("Выбрана папка для работы: {Path}", DirectoryPath);
            }
        }

        // Обработка изменения адреса папки
        private void FolderTextBox_TextChanged(object sender, EventArgs e)
        {
            DirectoryPath = FolderTextBox.Text;
            // Кнопки доступны только тогда, когда директория существует
            // Проверка что это не папка Steam (294100 - RimWorld App ID)
            if (Directory.Exists(FolderTextBox.Text) && !FolderTextBox.Text.Contains(Constants.Paths.SteamRimWorldAppId))
            {
                LabelCheck.ForeColor = goodColor;
                LabelCheck.Text = "ОК";
                CommentInserterButton.Enabled = true;
                FileRenamerButton.Enabled = true;
                NamesTranslatorButton.Enabled = true;
                CaseCreatorButton.Enabled = true;
                EncodingFixerButton.Enabled = true;
                TagCollectorButton.Enabled = true;
                FileFixerButton.Enabled = true;
                FindChangesButton.Enabled = true;
                PreTranslatorButton.Enabled = true;
                AdditionalFolderButton.Enabled = true;
            }
            else
            {
                LabelCheck.ForeColor = badColor;
                LabelCheck.Text = "Ошибка: некорректная папка";
                CommentInserterButton.Enabled = false;
                FileRenamerButton.Enabled = false;
                NamesTranslatorButton.Enabled = false;
                CaseCreatorButton.Enabled = false;
                EncodingFixerButton.Enabled = false;
                TagCollectorButton.Enabled = false;
                FileFixerButton.Enabled = false;
                FindChangesButton.Enabled = false;
                PreTranslatorButton.Enabled = false;
                AdditionalFolderButton.Enabled = false;
            }
        }

        #region прочие объекты

        private void AdditionalFolderButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog ofd = new();
            DialogResult dr = ofd.ShowDialog();
            if (dr == DialogResult.OK)
            {
                string selectedPath = ofd.SelectedPath;

                // Валидация пути
                if (!PathValidator.IsPathSafe(selectedPath, out string? errorMessage))
                {
                    _logger.LogWarning("Выбран небезопасный дополнительный путь: {Path}. Причина: {Error}", selectedPath, errorMessage);
                    MessageBox.Show(
                        $"Выбранный путь небезопасен:\n{errorMessage}",
                        "Ошибка выбора папки",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                AdditionalFolder = selectedPath;
                _logger.LogInformation("Выбрана дополнительная папка: {Path}", AdditionalFolder);
            }
            AdditionalFolderButton.BackColor = goodColor;
        }

        /// <summary>
        /// Добавляет текст в соответствующее текстовое поле в зависимости от выбранной вкладки
        /// </summary>
        /// <param name="text">Текст для добавления</param>
        /// <param name="tabId">ID вкладки (0 или 1). Если null, используется текущая выбранная вкладка</param>
        private void SendToInfoTextBox(string text, int? tabId = null)
        {
            // Определяем ID вкладки: если не указан явно, берем из настроек
            int targetTab = tabId ?? Settings.Default.lastTab;

            // Выбираем целевой TextBox
            var targetTextBox = targetTab == 0 ? InfoTextBox : InfoTextBox2;

            // Добавляем текст с переносом строки, если уже есть содержимое
            if (targetTextBox.Text == string.Empty)
                targetTextBox.AppendText(text);
            else
                targetTextBox.AppendText($"{Environment.NewLine}{text}");
        }

        private void MainTabs_IndexChange(object sender, EventArgs e)
        {
            Settings.Default.lastTab = MainTabs.SelectedIndex;
            Settings.Default.Save();
        }
        #endregion

        #region кнопки функций

        /// <summary>
        /// Async обработчик файлов с поддержкой прогресса и отмены
        /// </summary>
        private async Task ActionHandlerAsync(
            string operationName,
            string fileMask,
            FileProcessorType processorType,
            IProgress<ProcessingProgress>? progress = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Запуск операции: {OperationName}", operationName);
                SendToInfoTextBox($"{TimeSetter.PlaceTime()}Запуск: {operationName}");

                // Получение списка файлов
                var allFiles = await Task.Run(() =>
                    Directory.GetFiles(DirectoryPath, fileMask, SearchOption.AllDirectories),
                    cancellationToken);

                if (allFiles.Length == 0)
                {
                    SendToInfoTextBox($"{TimeSetter.PlaceTime()}Не найдено файлов с маской {fileMask}");
                    _logger.LogWarning("Не найдено файлов с маской {FileMask} в {DirectoryPath}", fileMask, DirectoryPath);
                    return;
                }

                int processedCount = 0;
                int errorCount = 0;
                var errors = new List<string>();

                // Обработка файлов
                for (int i = 0; i < allFiles.Length; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var currentFile = allFiles[i];
                    var fileName = Path.GetFileName(currentFile);

                    // Отчет о прогрессе
                    progress?.Report(ProcessingProgress.Create(
                        processedCount: i,
                        totalCount: allFiles.Length,
                        skippedCount: errorCount,
                        currentFile: fileName,
                        message: $"Обработка {i + 1} из {allFiles.Length}"));

                    // Обработка файла в фоновом потоке
                    var result = await Task.Run(() => ProcessFile(currentFile, processorType), cancellationToken);

                    if (result.Item1)
                    {
                        processedCount++;
                    }
                    else
                    {
                        errorCount++;
                        var errorMsg = $"{result.Item2} ({currentFile})";
                        errors.Add(errorMsg);
                        _logger.LogWarning("Ошибка обработки файла: {Error}", errorMsg);
                    }
                }

                // Постобработка
                await PerformPostProcessingAsync(processorType, cancellationToken);

                // Финальный отчет
                var completionMessage = $"Завершено. Обработано файлов - {processedCount}";
                if (errorCount > 0)
                {
                    completionMessage += $"{Environment.NewLine}Пропущено файлов - {errorCount}";
                }

                SendToInfoTextBox($"{TimeSetter.PlaceTime()}{completionMessage}");
                _logger.LogInformation("Операция завершена: {Message}", completionMessage);

                // Вывод первых ошибок
                foreach (var error in errors.Take(Constants.UI.MaxErrorsToDisplay))
                {
                    SendToInfoTextBox($"{TimeSetter.PlaceTime()}{error}");
                }

                if (errors.Count > Constants.UI.MaxErrorsToDisplay)
                {
                    SendToInfoTextBox($"{TimeSetter.PlaceTime()}...и еще {errors.Count - Constants.UI.MaxErrorsToDisplay} ошибок");
                }

                progress?.Report(ProcessingProgress.Completed(
                    processedCount, errorCount, allFiles.Length, completionMessage));
            }
            catch (OperationCanceledException)
            {
                var message = $"Операция '{operationName}' отменена пользователем";
                SendToInfoTextBox($"{TimeSetter.PlaceTime()}{message}");
                _logger.LogInformation(message);
                progress?.Report(ProcessingProgress.Cancelled(0, 0, message));
            }
            catch (Exception ex)
            {
                var message = $"Критическая ошибка при выполнении '{operationName}': {ex.Message}";
                SendToInfoTextBox($"{TimeSetter.PlaceTime()}{message}");
                _logger.LogError(ex, "Критическая ошибка в ActionHandlerAsync");
                MessageBox.Show(message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Обработка одного файла
        /// </summary>
        private (bool, string) ProcessFile(string filePath, FileProcessorType processorType)
        {
            return processorType switch
            {
                FileProcessorType.FileRenamer => FileRenamer.FileRenamerActivity(filePath),
                FileProcessorType.NamesTranslator => NamesTranslator.NamesTranslatorActivity(filePath),
                FileProcessorType.TagCollector => TagCollector.TagCollectorActivity(filePath),
                FileProcessorType.FileFixer => FileFixer.FileFixerActivity(filePath),
                FileProcessorType.EncodingFixer => EncodingFixer.EncodingFixerActivity(filePath),
                FileProcessorType.CommentInserter => CommentInserter.InsertComments(filePath),
                _ => (false, "Неизвестный тип процессора")
            };
        }

        /// <summary>
        /// Постобработка после завершения основной операции
        /// </summary>
        private async Task PerformPostProcessingAsync(FileProcessorType processorType, CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                string message;
                switch (processorType)
                {
                    case FileProcessorType.TagCollector:
                        message = TagCollector.TagWriterActivity();
                        SendToInfoTextBox($"{TimeSetter.PlaceTime()}{message}");
                        message = TagCollector.DefsClassGeneratorActivity();
                        SendToInfoTextBox($"{TimeSetter.PlaceTime()}{message}");
                        TagCollector.DataCleanerActivity();
                        break;

                    case FileProcessorType.FileFixer:
                        message = FileFixer.BrokenFilesWriterActivity();
                        SendToInfoTextBox($"{TimeSetter.PlaceTime()}{message}");
                        break;
                }
            }, cancellationToken);
        }

        // УСТАРЕВШИЙ МЕТОД - будет удален в версии 4.0
        // Обработчик нажатий кнопок
        [Obsolete("Используйте ActionHandlerAsync() вместо этого метода")]
        private void ActionHandler(string name, string mask, string code)
        {
            InfoTextBox.AppendText($"{Environment.NewLine}{TimeSetter.PlaceTime()}Запуск: {name}");
            // Получение списка всех файлов в заданой папке и во всех вложенных подпапках за счёт SearchOption
            string[] allFiles = Directory.GetFiles(DirectoryPath, mask, SearchOption.AllDirectories);
            int count = 0;
            int errCount = 0;
            (bool, string) result = (false, string.Empty);
            string message;
            foreach (string currentFile in allFiles)
            {
                switch (code)
                {
                    case "FileRenamer":
                        result = FileRenamer.FileRenamerActivity(currentFile);
                        break;
                    case "NamesTranslator":
                        result = NamesTranslator.NamesTranslatorActivity(currentFile);
                        break;
                    case "TagCollector":
                        result = TagCollector.TagCollectorActivity(currentFile);
                        break;
                    case "FileFixer":
                        result = FileFixer.FileFixerActivity(currentFile);
                        break;
                    case "EncodingFixer":
                        result = EncodingFixer.EncodingFixerActivity(currentFile);
                        break;
                    case "CommentInserter":
                        result = CommentInserter.InsertComments(currentFile);
                        break;
                    default:
                        break;
                }
                if (result.Item1)
                    count++;
                else
                {
                    errCount++;
                    InfoTextBox.AppendText($"{Environment.NewLine}{TimeSetter.PlaceTime()}{result.Item2} ({currentFile})");
                }
            }

            InfoTextBox.AppendText($"{Environment.NewLine}{TimeSetter.PlaceTime()}Завершено. Обработано файлов - {count}");
            if (errCount != 0)
            {
                InfoTextBox.AppendText($"{Environment.NewLine}Пропущено файлов - {errCount}");
            }

            // Постобработка
            switch (code)
            {
                case "TagCollector":
                    message = TagCollector.TagWriterActivity();
                    InfoTextBox.AppendText($"{Environment.NewLine}{TimeSetter.PlaceTime()}{message}");
                    message = TagCollector.DefsClassGeneratorActivity();
                    InfoTextBox.AppendText($"{Environment.NewLine}{TimeSetter.PlaceTime()}{message}");
                    TagCollector.DataCleanerActivity();
                    break;
                case "FileFixer":
                    message = FileFixer.BrokenFilesWriterActivity();
                    InfoTextBox.AppendText($"{Environment.NewLine}{TimeSetter.PlaceTime()}{message}");
                    break;
                default:
                    break;
            }
        }

        // Поиск изменений в переводе (async версия)
        private async void FindChangesButton_Click(object sender, EventArgs e)
        {
            if (AdditionalFolder == string.Empty)
            {
                SendToInfoTextBox($"{TimeSetter.PlaceTime()}Ошибка. Не выбран источник данных");
                MessageBox.Show("Выберите папку с исходными файлами мода", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _currentOperationCts?.Cancel();
            _currentOperationCts?.Dispose();
            _currentOperationCts = new CancellationTokenSource();

            try
            {
                _logger.LogInformation("Запуск поиска изменений в переводе");
                SendToInfoTextBox($"Файлы перевода - {DirectoryPath}.{Environment.NewLine}Исходные файлы мода - {AdditionalFolder}");

                // Фаза 1: Сбор данных перевода
                SendToInfoTextBox($"{TimeSetter.PlaceTime()}Поиск изменений в переводе");

                var translationFiles = await Task.Run(() =>
                    Directory.GetFiles(DirectoryPath, Constants.Files.XmlMask, SearchOption.AllDirectories),
                    _currentOperationCts.Token);

                int count = 0;
                int errCount = 0;

                for (int i = 0; i < translationFiles.Length; i++)
                {
                    _currentOperationCts.Token.ThrowIfCancellationRequested();

                    var result = await Task.Run(() =>
                        ChangesFinder.GetTranslationData(translationFiles[i]),
                        _currentOperationCts.Token);

                    if (result.Item1)
                        count++;
                    else
                    {
                        errCount++;
                        SendToInfoTextBox($"{TimeSetter.PlaceTime()}{result.Item2} ({translationFiles[i]})");
                    }
                }

                SendToInfoTextBox($"{TimeSetter.PlaceTime()}Собраны данные перевода. Обработано файлов - {count}. Пропущено файлов - {errCount}.");

                // Фаза 2: Сбор исходных данных
                var modFiles = await Task.Run(() =>
                    Directory.GetFiles(AdditionalFolder, Constants.Files.XmlMask, SearchOption.AllDirectories),
                    _currentOperationCts.Token);

                count = 0;
                errCount = 0;

                for (int i = 0; i < modFiles.Length; i++)
                {
                    _currentOperationCts.Token.ThrowIfCancellationRequested();

                    var result = await Task.Run(() =>
                        ChangesFinder.GetModData(modFiles[i]),
                        _currentOperationCts.Token);

                    if (result.Item1)
                        count++;
                    else
                    {
                        errCount++;
                        SendToInfoTextBox($"{TimeSetter.PlaceTime()}{result.Item2} ({modFiles[i]})");
                    }
                }

                SendToInfoTextBox($"{TimeSetter.PlaceTime()}Собраны исходные данные. Обработано файлов - {count}. Пропущено файлов - {errCount}.");

                // Фаза 3: Поиск изменений и запись
                await Task.Run(() =>
                {
                    ChangesFinder.FindChangesInFiles();
                    var writeResult = ChangesFinder.WriteChanges();
                    SendToInfoTextBox($"{TimeSetter.PlaceTime()}{writeResult}");
                }, _currentOperationCts.Token);

                _logger.LogInformation("Поиск изменений завершен успешно");
            }
            catch (OperationCanceledException)
            {
                SendToInfoTextBox($"{TimeSetter.PlaceTime()}Операция отменена пользователем");
                _logger.LogInformation("Поиск изменений отменен пользователем");
            }
            catch (Exception ex)
            {
                var message = $"Ошибка при поиске изменений: {ex.Message}";
                SendToInfoTextBox($"{TimeSetter.PlaceTime()}{message}");
                _logger.LogError(ex, "Ошибка в FindChangesButton_Click");
                MessageBox.Show(message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _currentOperationCts?.Dispose();
                _currentOperationCts = null;
            }
        }

        // TODO: Переписать на async с использованием _morpherService.GetRequestLimitAsync() и GetWordFormsAsync()
        // TODO: Добавить CancellationToken для возможности отмены
        // TODO: Добавить Progress<ProcessingProgress> для отображения прогресса
        private void CaseCreatorButton_Click(object sender, EventArgs e)
        {
            _logger.LogInformation("Запуск создания вспомогательных файлов");
            InfoTextBox.AppendText($"{TimeSetter.PlaceTime()}Создание вспомогательных файлов");
            string[] defTypeList = Constants.DefTypes.SupportedTypes;
            //Получение списка всех файлов в заданой директории и во всех вложенных подпапках за счёт SearchOption
            string[] allFiles = Directory.GetFiles(DirectoryPath, Constants.Files.XmlMask, SearchOption.AllDirectories);
            Dictionary<string, string> words = [];

            // Поиск подходящей директории
            string directory = Directory.Exists(DirectoryPath + $"\\{Constants.Paths.CommonFolderName}") ? DirectoryPath + $"\\{Constants.Paths.CommonFolderName}" : DirectoryPath;
            int typeCount = 0;
            // Проверяется каждый подходящий DefType из списка
            foreach (string defType in defTypeList)
            {
                int count = 0;
                foreach (string tempFile in allFiles)
                {
                    // Каждый файл проверяется на соотвествие этому типу. Если не соотвествует, возвращает пустой список
                    List<string> tempWords = CaseCreator.FindWordsProcessing(tempFile, defType);
                    foreach (string word in tempWords)
                    {
                        bool result = words.TryAdd(word, defType);
                        if (result) { count++; }
                    }
                }
                // Если нашлись слова в данном DefType, то создаются файлы
                if (count > 0)
                {
                    int? limit = MorpherService.GetMorpherRequestLimit();
                    if (!limit.HasValue)
                    {
                        InfoTextBox.AppendText($"{Environment.NewLine}{TimeSetter.PlaceTime()}Ошибка получения лимита слов для {defType}. Проблемы с интернетом?");
                        continue;
                    }
                    else if (limit < words.Count)
                    {
                        InfoTextBox.AppendText($"{Environment.NewLine}{TimeSetter.PlaceTime()}Превышен лимит. Доступно сегодня {limit}, требуется для {defType} - {words.Count}. Стоит убрать уже обработанные папки и попробовать завтра. Или сменить IP.");
                        continue;
                    }
                    else
                    {
                        InfoTextBox.AppendText($"{Environment.NewLine}{TimeSetter.PlaceTime()}Доступно запросов - {limit}");
                    }
                    CaseCreator.CreateCase(directory, words, defType);
                    CaseCreator.CreateGender(directory, words, defType);
                    InfoTextBox.AppendText($"{Environment.NewLine}{TimeSetter.PlaceTime()}Обработано {count} объектов типа {defType}");
                    typeCount++;
                }
            }
            if (typeCount > 0)
            {
                InfoTextBox.AppendText($"{Environment.NewLine}{TimeSetter.PlaceTime()}Созданы файлы по адресу {directory}\\Languages\\Russian\\WordInfo");
            }
            else
            {
                InfoTextBox.AppendText($"{Environment.NewLine}{TimeSetter.PlaceTime()}Файлы не созданы");
            }
            InfoTextBox.AppendText($"{Environment.NewLine}{TimeSetter.PlaceTime()}Завершено");
        }

        // TODO: Переписать на async с CancellationToken
        // TODO: Добавить Progress<ProcessingProgress> для отображения прогресса
        // TODO: Вынести длительные операции в фоновый поток
        private void PreTranslatorButton_Click(object sender, EventArgs e)
        {
            _logger.LogInformation("Запуск предварительного перевода");

            if (AdditionalFolder == string.Empty)
            {
                InfoTextBox.AppendText($"{Environment.NewLine}{TimeSetter.PlaceTime()}Ошибка. Не выбран источник данных");
                MessageBox.Show("Выберите папку с исходными файлами для перевода", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            InfoTextBox.AppendText($"{Environment.NewLine}Переводимые файлы - {DirectoryPath}.{Environment.NewLine}Исходные файлы для предварительного перевода - {AdditionalFolder}");
            InfoTextBox.AppendText($"{Environment.NewLine}{TimeSetter.PlaceTime()}Сбор данных для перевода");
            string[] allFiles = Directory.GetFiles(AdditionalFolder, Constants.Files.XmlMask, SearchOption.AllDirectories);
            int count = 0;
            int errCount = 0;
            (bool, string) result = (false, string.Empty);
            foreach (string tempFile in allFiles)
            {
                if (tempFile.StartsWith(DirectoryPath, StringComparison.OrdinalIgnoreCase))
                {
                    InfoTextBox.AppendText($"{Environment.NewLine}{TimeSetter.PlaceTime()}Включать переводимый мод в исходные файлы - плохая идея.");
                    return;
                }
                result = PreTranslator.BuildDatabase(tempFile);
                if (result.Item1)
                {
                    count++;
                }
                else
                {
                    errCount++;
                    InfoTextBox.AppendText($"{Environment.NewLine}{TimeSetter.PlaceTime()}{result.Item2} ({tempFile})");
                }
            }
            InfoTextBox.AppendText($"{Environment.NewLine}{TimeSetter.PlaceTime()}Собрано {result.Item2} пар перевода. Обработано файлов - {count}. Пропущено файлов - {errCount}.");


            InfoTextBox.AppendText($"{Environment.NewLine}{TimeSetter.PlaceTime()}Начат предварительный перевод");
            allFiles = Directory.GetFiles(DirectoryPath, Constants.Files.XmlMask, SearchOption.AllDirectories);
            count = 0;
            errCount = 0;
            foreach (string tempFile in allFiles)
            {
                result = PreTranslator.Translation(tempFile);
                if (result.Item1)
                {
                    count++;
                }
                else
                {
                    errCount++;
                    InfoTextBox.AppendText($"{Environment.NewLine}{TimeSetter.PlaceTime()}{result.Item2} ({tempFile})");
                }
            }
            InfoTextBox.AppendText($"{Environment.NewLine}{TimeSetter.PlaceTime()}Обработано файлов - {count}. Пропущено файлов - {errCount}.");
        }

        // Вызов обработчика из кнопок (новая async версия)
        private async void FileRenamerButton_Click(object sender, EventArgs e)
        {
            await RunOperationWithCancellationAsync(
                "Переименование файлов",
                Constants.Files.XmlMask,
                FileProcessorType.FileRenamer);
        }

        private async void NamesTranslatorButton_Click(object sender, EventArgs e)
        {
            await RunOperationWithCancellationAsync(
                "Транскрипция имён",
                Constants.Files.TxtMask,
                FileProcessorType.NamesTranslator);
        }

        private async void TagCollectorButton_Click(object sender, EventArgs e)
        {
            await RunOperationWithCancellationAsync(
                "Сбор статистики тегов",
                Constants.Files.XmlMask,
                FileProcessorType.TagCollector);
        }

        private async void FileFixerButton_Click(object sender, EventArgs e)
        {
            await RunOperationWithCancellationAsync(
                "Поиск сломанных файлов",
                Constants.Files.XmlMask,
                FileProcessorType.FileFixer);
        }

        private async void EncodingFixerButton_Click(object sender, EventArgs e)
        {
            await RunOperationWithCancellationAsync(
                "Исправление кодировки",
                Constants.Files.XmlMask,
                FileProcessorType.EncodingFixer);
        }

        private async void CommentInserterButton_Click(object sender, EventArgs e)
        {
            await RunOperationWithCancellationAsync(
                "Добавление комментариев",
                Constants.Files.XmlMask,
                FileProcessorType.CommentInserter);
        }

        /// <summary>
        /// Запуск операции с возможностью отмены
        /// </summary>
        private async Task RunOperationWithCancellationAsync(
            string operationName,
            string fileMask,
            FileProcessorType processorType)
        {
            // Отменяем предыдущую операцию если она еще выполняется
            _currentOperationCts?.Cancel();
            _currentOperationCts?.Dispose();

            // Создаем новый токен отмены
            _currentOperationCts = new CancellationTokenSource();

            try
            {
                // TODO: Здесь можно добавить прогресс-бар в UI
                // var progress = new Progress<ProcessingProgress>(p => UpdateProgressBar(p));

                await ActionHandlerAsync(
                    operationName,
                    fileMask,
                    processorType,
                    progress: null, // Пока без прогресс-бара
                    cancellationToken: _currentOperationCts.Token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при выполнении операции {OperationName}", operationName);
                MessageBox.Show(
                    $"Ошибка: {ex.Message}",
                    "Ошибка выполнения",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                _currentOperationCts?.Dispose();
                _currentOperationCts = null;
            }
        }
        #endregion

        #region LanguageUpdate
        // Обновление локализации
        private void ButtonLanguageUpdate_Click(object sender, EventArgs e)
        {
            (bool, string) result;
            SendToInfoTextBox("Проверка обновления");
            result = LanguageUpdater.TranslationVersionCheckActivity(Settings.Default.sha, Settings.Default.repo);
            if (result.Item1 == false)
            {
                SendToInfoTextBox(result.Item2);
                return;
            }
            SendToInfoTextBox("Найдена новая версия");
            var dr = MessageBox.Show($"{result.Item2}.\nОбновить?", "Обновление", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (dr != DialogResult.Yes)
            {
                return;
            }

            SendToInfoTextBox("Загрузка обновления");
            result = LanguageUpdater.LanguageUpdateDownload(Settings.Default.repo);
            if (result.Item1 == false)
            {
                SendToInfoTextBox(result.Item2);
                return;
            }
            SendToInfoTextBox("Обновление загружено");

            SendToInfoTextBox("Установка обновления");
            result = LanguageUpdater.LanguageUpdateActivity(Settings.Default.savedDirectory, Settings.Default.language);
            if (result.Item1 == false)
            {
                SendToInfoTextBox(result.Item2);
                return;
            }
            SendToInfoTextBox(result.Item2);

            Settings.Default.sha = LanguageUpdater.GetSha();
            Settings.Default.Save();
        }

        // Удаление скачанного перевода
        private void ResetButton_Click(object sender, EventArgs e)
        {
            // Получение списка дополнений
            if (Directory.Exists($"{Settings.Default.savedDirectory}\\{Constants.Paths.DataFolderName}"))
            {
                string[] modules = Directory.GetDirectories($"{Settings.Default.savedDirectory}\\{Constants.Paths.DataFolderName}");
                if (modules.Length == 0)
                {
                    MessageBox.Show("В указанной папке нет дополнений", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    string modulesList = string.Empty;
                    List<string> languagePathList = [];
                    foreach (var module in modules)
                    {
                        string moduleName = module[module.LastIndexOf('\\')..];
                        moduleName = moduleName[1..];
                        string languagePath = $"{module}\\Languages\\{Settings.Default.language}";
                        if (Directory.Exists(languagePath))
                        {
                            modulesList += $"- {moduleName}: {languagePath}\n";
                            languagePathList.Add(languagePath);
                        }
                    }
                    if (languagePathList.Count == 0)
                    {
                        MessageBox.Show("Перевод модулей не найден", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        var dr = MessageBox.Show($"Вы собираетесь удалить перевод, созданный в этой программе. Другие переводы затронуты не будут.\nБудут удалены следующие папки:\n\n{modulesList}", "Удаление", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                        if (dr == DialogResult.Yes)
                        {
                            Settings.Default.sha = Constants.Repository.DefaultSha;
                            Settings.Default.Save();
                            foreach (var path in languagePathList)
                            {
                                Directory.Delete(path, true);
                            }
                            MessageBox.Show("Удаление завершено", "Удаление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("В указанной папке нет перевода", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Сброс настроек к значеним по умолчанию
        private void DefaultButton_Click(object sender, EventArgs e)
        {
            var dr = MessageBox.Show("Вы действительно хотите вернуть все параметры в этом окне к изначальным?", "Сброс настроек", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (dr == DialogResult.Yes)
            {
                Settings.Default.sha = Constants.Repository.DefaultSha;
                Settings.Default.repo = Constants.Repository.DefaultRepo;
                Settings.Default.language = Constants.Repository.DefaultLanguage;
                Settings.Default.savedDirectory = string.Empty;
                Settings.Default.Save();

                LanguageInput.Text = Constants.Repository.DefaultLanguage;
                RepoInput.Text = Constants.Repository.DefaultRepo;
                FolderTextBox2.Text = string.Empty;
                MessageBox.Show("Параметры сброшены", "Сброс настроек", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void RepoInput_TextChanged(object sender, EventArgs e)
        {
            Settings.Default.repo = RepoInput.Text;
            Settings.Default.Save();
            CheckTextBoxes();
        }

        private void LanguageInput_TextChanged(object sender, EventArgs e)
        {
            Settings.Default.language = LanguageInput.Text;
            Settings.Default.Save();
            CheckTextBoxes();
        }

        // Доступ к обновлению, когда введены данные
        private void CheckTextBoxes()
        {
            if ((FolderButton2.BackColor == goodColor) && (RepoInput.Text != string.Empty) && (LanguageInput.Text != string.Empty))
            {
                ButtonLanguageUpdate.Enabled = true;
                ResetButton.Enabled = true;
            }
            else
            {
                ButtonLanguageUpdate.Enabled = false;
                ResetButton.Enabled = false;
            }
        }

        // Выбор папки игры
        private void FolderButton2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog ofd = new();
            DialogResult dr = ofd.ShowDialog();
            if (dr == DialogResult.OK)
            {
                string selectedPath = ofd.SelectedPath;

                // Валидация пути
                if (!PathValidator.IsPathSafe(selectedPath, out string? errorMessage))
                {
                    _logger.LogWarning("Выбран небезопасный путь игры: {Path}. Причина: {Error}", selectedPath, errorMessage);
                    MessageBox.Show(
                        $"Выбранный путь небезопасен:\n{errorMessage}",
                        "Ошибка выбора папки",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                GamePath = selectedPath;
                FolderTextBox2.Text = GamePath;
                _logger.LogInformation("Выбрана папка игры: {Path}", GamePath);
            }
        }

        private void FolderTextBox2_TextChanged(object sender, EventArgs e)
        {
            string tempPath = FolderTextBox2.Text;
            FolderButton2.BackColor = badColor;
            if (Directory.Exists(tempPath))
            {
                FolderButton2.BackColor = goodColor;
                if (Directory.Exists($"{tempPath}\\{Constants.Paths.DataFolderName}"))
                {
                    string[] modules = Directory.GetDirectories($"{tempPath}\\{Constants.Paths.DataFolderName}");
                    if (modules.Length == 0)
                    {
                        SendToInfoTextBox("Нет модулей", 1);
                        FolderButton2.BackColor = Color.MediumBlue;
                    }
                    else
                    {
                        SendToInfoTextBox("Найдены модули:", 1);
                        foreach (var module in modules)
                        {
                            string moduleName = module[module.LastIndexOf('\\')..];
                            moduleName = moduleName[1..];
                            SendToInfoTextBox(moduleName, 1);
                        }
                        GamePath = tempPath;
                        Settings.Default.savedDirectory = tempPath;
                        Settings.Default.Save();
                    }
                }
                else
                {
                    SendToInfoTextBox("Нет модулей", 1);
                    FolderButton2.BackColor = Color.MediumBlue;
                }
            }
            CheckTextBoxes();
        }
        #endregion

        #region Верхнее меню
        // Переключение состояния автообновления
        private void ToolStripMenuItemAutoUpdateCheck_Click(object sender, EventArgs e)
        {
            // Я уже не помню, почему инвертирую, но именно так работает
            ToolStripMenuItemAutoUpdateCheck.Checked = !ToolStripMenuItemAutoUpdateCheck.Checked;
            Settings.Default.isAutoUpdateActive = ToolStripMenuItemAutoUpdateCheck.Checked;
            Settings.Default.Save();
        }

        // Ручная проверка обновлений
        private async void ToolStripMenuItemCheckUpdate_Click(object sender, EventArgs e)
        {
            await CheckVersionAsync();
        }

        private async Task<bool> CheckVersionAsync()
        {
            _logger.LogInformation("Проверка обновлений приложения");

            var result = await _githubService.GetLatestReleaseAsync();

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Не удалось проверить обновления: {Error}", result.ErrorMessage);
                MessageBox.Show(
                    result.ErrorMessage ?? "Не удалось проверить обновления. Проверьте подключение к интернету.",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }

            var json = result.Value;
            var newVersion = json?.TagName?.ToString()[1..];
            var oldVersion = Assembly.GetEntryAssembly()?.GetName().Version?.ToString()[..^2];

            if (oldVersion == newVersion)
            {
                _logger.LogInformation("Приложение обновлено до последней версии {Version}", oldVersion);
                MessageBox.Show("Обновление не требуется", "Обновление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            else
            {
                _logger.LogInformation("Доступна новая версия {NewVersion} (текущая: {OldVersion})", newVersion, oldVersion);
                var dr = MessageBox.Show(
                    $"Доступно обновление до версии {newVersion}!\nПерейти на страницу загрузки?\n\nВ новой версии:\n{json?.Body}",
                    "Обновление",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information);

                if (dr == DialogResult.Yes)
                {
                    var url = json?.HtmlUrl?.ToString();
                    if (!string.IsNullOrEmpty(url))
                    {
                        _logger.LogInformation("Открытие страницы релиза: {Url}", url);
                        Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
                    }
                }
                return true;
            }
        }

        // Вывод информации об авторе
        private void ToolStripMenuItemCreator_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Это полностью бесплатная программа от великого OliveWizard для сообщества RimWorld.\n\n", "Автор", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Переход на страницу с руководством
        private void ToolStripMenuItemGuide_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo { FileName = @"https://github.com/OneCodeUnit/RimLangKit/blob/master/README.md", UseShellExecute = true });
        }

        // Автоматическая проверка обновлений при запуске
        private async Task PerformAutoUpdateCheckAsync()
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
        #endregion

        #region Загрузка базы данных
        // Выбор базы данных для загрузки
        private void SelectDatabaseButton_Click(object sender, EventArgs e)
        {
            FileDialog fileDialog = new OpenFileDialog
            {
                Filter = $"База данных (*{Constants.Files.DatabaseExtension})|*{Constants.Files.DatabaseExtension}|All files (*.*)|*.*",
            };
            DialogResult dr = fileDialog.ShowDialog();
            if (dr == DialogResult.OK)
            {
                SelectedDBPath = fileDialog.FileName;
                SelectDatabaseTextBox.Text = SelectedDBPath;
            }

        }

        // Обработка изменения пути к базе данных
        private void SelectDatabaseTextBox_TextChanged(object sender, EventArgs e)
        {
            string path = SelectDatabaseTextBox.Text;
            SelectedDBPath = path;
            if (File.Exists(path) && path.EndsWith(Constants.Files.DatabaseExtension, StringComparison.OrdinalIgnoreCase))
            {
                // Загрузка базы данных (проверка её содержимого)
                var result = AutoTranslator.LoadDatabase(SelectedDBPath);
                CheckDatabaseLabel.Text = $"Сейчас в базе: {result.Message}";
            }
            else
            {

            }
        }

        #endregion

        // Создание новой базы данных
        private void CreateDatabaseButton_Click(object sender, EventArgs e)
        {
            string executableDirectory;
            if (SelectedDBPath == string.Empty)
                executableDirectory = Path.Combine(AppContext.BaseDirectory, Constants.Files.DefaultDatabaseName);
            else
                executableDirectory = SelectedDBPath;

            var dr = MessageBox.Show($"База данных будет создана по адресу {executableDirectory}.\nСоздать?", "Создание базы данных", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (dr == DialogResult.Yes)
            {
                AutoTranslator.NewDatabase(executableDirectory);
                MessageBox.Show("База данных создана", "Создание базы данных", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SelectedDBPath = executableDirectory;
                SelectDatabaseTextBox.Text = SelectedDBPath;
                // Загрузка базы данных (проверка её содержимого)
                var result = AutoTranslator.LoadDatabase(SelectedDBPath);
                CheckDatabaseLabel.Text = $"Сейчас в базе: {result.Message}";
            }
        }

        // Добавление файлов из папки в базу данных
        private void UpdateDatabaseButton_Click(object sender, EventArgs e)
        {
            bool rewrite = false;
            if (RewriteRadioButtonTrue.Checked)
                rewrite = true;
            if (RewriteRadioButtonFalse.Checked)
                rewrite = false;

            string[] allFiles = Directory.GetFiles(TranslationFolder, Constants.Files.XmlMask, SearchOption.AllDirectories);
            List<XmlError> results = [];
            foreach (string currentFile in allFiles)
            {
                var result = AutoTranslator.CreateDatabase(SelectedDBPath, currentFile, "translated_tags", rewrite);
                results.Add(new XmlError(result.IsValid, $"{currentFile}: {result.Message}"));
            }

            MessageBox.Show("Данные добавлены", "Обновление базы данных", MessageBoxButtons.OK, MessageBoxIcon.Information);
            // Загрузка базы данных (проверка её содержимого)
            var checkResult = AutoTranslator.LoadDatabase(SelectedDBPath);
            CheckDatabaseLabel.Text = $"Сейчас в базе: {checkResult.Message}";
        }

        // Выбор папки с переводами
        private void SelectForUpdateDatabaseButton_Click(object sender, EventArgs e)
        {
            using FolderBrowserDialog ofd = new();
            DialogResult dr = ofd.ShowDialog();
            if (dr == DialogResult.OK)
            {
                string selectedPath = ofd.SelectedPath;

                // Валидация пути
                if (!PathValidator.IsPathSafe(selectedPath, out string? errorMessage))
                {
                    _logger.LogWarning("Выбран небезопасный путь для БД: {Path}. Причина: {Error}", selectedPath, errorMessage);
                    MessageBox.Show(
                        $"Выбранный путь небезопасен:\n{errorMessage}",
                        "Ошибка выбора папки",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                TranslationFolder = selectedPath;
                UpdateDatabaseTextBox.Text = TranslationFolder;
                _logger.LogInformation("Выбрана папка с переводами: {Path}", TranslationFolder);
            }
        }

        // Обработка изменения адреса папки с переводами
        private void UpdateDatabaseTextBox_TextChanged(object sender, EventArgs e)
        {
            string path = UpdateDatabaseTextBox.Text;
            if (Directory.Exists(path))
            {
                TranslationFolder = path;
            }
        }

        // Автоперевод файлов из папки
        private void AutoTranslateButton_Click(object sender, EventArgs e)
        {
            string[] allFiles = Directory.GetFiles(AutoTranslateModFolder, Constants.Files.XmlMask, SearchOption.AllDirectories);
            List<XmlError> results = [];
            foreach (string currentFile in allFiles)
            {
                var result = AutoTranslator.TranslateFile(SelectedDBPath, currentFile, "translated_tags");
                results.Add(new XmlError(result.IsValid, $"{currentFile}: {result.Message}"));
            }
            MessageBox.Show("Файлы переведы", "Автоперевод", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Выбор папки с модом для автоперевода
        private void SelectModButton_Click(object sender, EventArgs e)
        {
            using FolderBrowserDialog ofd = new();
            DialogResult dr = ofd.ShowDialog();
            if (dr == DialogResult.OK)
            {
                string selectedPath = ofd.SelectedPath;

                // Валидация пути
                if (!PathValidator.IsPathSafe(selectedPath, out string? errorMessage))
                {
                    _logger.LogWarning("Выбран небезопасный путь мода: {Path}. Причина: {Error}", selectedPath, errorMessage);
                    MessageBox.Show(
                        $"Выбранный путь небезопасен:\n{errorMessage}",
                        "Ошибка выбора папки",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                AutoTranslateModFolder = selectedPath;
                SelectModTextBox.Text = AutoTranslateModFolder;
                _logger.LogInformation("Выбрана папка с модом: {Path}", AutoTranslateModFolder);
            }
        }

        private void SelectModTextBox_TextChanged(object sender, EventArgs e)
        {
            string path = SelectModTextBox.Text;
            if (Directory.Exists(path))
            {
                AutoTranslateModFolder = path;
            }
        }
    }
}