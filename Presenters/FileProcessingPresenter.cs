using Microsoft.Extensions.Logging;
using RimLangKit.Common;
using RimLangKit.Models;
using RimLangKit.Processors;
using RimLangKit.Services;
using RimLangKit.Utilities;
using RimLangKit.Views.Interfaces;

namespace RimLangKit.Presenters
{
    /// <summary>
    /// Presenter для обработки файлов
    /// </summary>
    public class FileProcessingPresenter
    {
        private readonly IMainView _mainView;
        private readonly IFileProcessingView _fileProcessingView;
        private readonly ApplicationState _state;
        private readonly IMorpherService _morpherService;
        private readonly ILogger<FileProcessingPresenter> _logger;
        private readonly TagCollectorData _tagCollectorData;

        private CancellationTokenSource? _currentOperationCts;

        public FileProcessingPresenter(
            IMainView mainView,
            IFileProcessingView fileProcessingView,
            ApplicationState state,
            IMorpherService morpherService,
            ILogger<FileProcessingPresenter> logger)
        {
            _mainView = mainView ?? throw new ArgumentNullException(nameof(mainView));
            _fileProcessingView = fileProcessingView ?? throw new ArgumentNullException(nameof(fileProcessingView));
            _state = state ?? throw new ArgumentNullException(nameof(state));
            _morpherService = morpherService ?? throw new ArgumentNullException(nameof(morpherService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tagCollectorData = new TagCollectorData();
        }

        /// <summary>
        /// Обработка выбора основной папки
        /// </summary>
        public void HandleFolderSelection()
        {
            if (_fileProcessingView.ShowFolderBrowserDialog(out string selectedPath))
            {
                // Валидация пути
                if (!PathValidator.IsPathSafe(selectedPath, out string? errorMessage))
                {
                    _logger.LogWarning("Выбран небезопасный путь: {Path}. Причина: {Error}", selectedPath, errorMessage);
                    _mainView.ShowMessageBox(
                        $"Выбранный путь небезопасен:\n{errorMessage}",
                        "Ошибка выбора папки",
                        MessageBoxIcon.Warning);
                    return;
                }

                _state.DirectoryPath = selectedPath;
                _logger.LogInformation("Выбрана папка для работы: {Path}", selectedPath);
            }
        }

        /// <summary>
        /// Обработка выбора дополнительной папки
        /// </summary>
        public void HandleAdditionalFolderSelection()
        {
            if (_fileProcessingView.ShowFolderBrowserDialog(out string selectedPath))
            {
                // Валидация пути
                if (!PathValidator.IsPathSafe(selectedPath, out string? errorMessage))
                {
                    _logger.LogWarning("Выбран небезопасный дополнительный путь: {Path}. Причина: {Error}", selectedPath, errorMessage);
                    _mainView.ShowMessageBox(
                        $"Выбранный путь небезопасен:\n{errorMessage}",
                        "Ошибка выбора папки",
                        MessageBoxIcon.Warning);
                    return;
                }

                _state.AdditionalFolder = selectedPath;
                _logger.LogInformation("Выбрана дополнительная папка: {Path}", selectedPath);
                _mainView.SetAdditionalFolderButtonColor(true);
            }
        }

        /// <summary>
        /// Валидация пути к директории
        /// </summary>
        public void ValidateDirectoryPath(string path)
        {
            _state.DirectoryPath = path;

            // Проверка что это не папка Steam (294100 - RimWorld App ID)
            if (Directory.Exists(path) && !path.Contains(Constants.Paths.SteamRimWorldAppId))
            {
                _mainView.SetDirectoryCheckStatus("ОК", true);
                _mainView.SetProcessingButtonsEnabled(true);
            }
            else
            {
                _mainView.SetDirectoryCheckStatus("Ошибка: некорректная папка", false);
                _mainView.SetProcessingButtonsEnabled(false);
            }
        }

        /// <summary>
        /// Запуск обработки файлов
        /// </summary>
        public async Task ProcessFilesAsync(
            string operationName,
            string fileMask,
            FileProcessorType processorType)
        {
            // Отменяем предыдущую операцию
            _currentOperationCts?.Cancel();
            _currentOperationCts?.Dispose();
            _currentOperationCts = new CancellationTokenSource();

            try
            {
                var progress = _fileProcessingView.GetProgressReporter();

                await ActionHandlerAsync(
                    operationName,
                    fileMask,
                    processorType,
                    progress,
                    _currentOperationCts.Token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при выполнении операции {OperationName}", operationName);
                _mainView.ShowMessageBox(
                    $"Ошибка: {ex.Message}",
                    "Ошибка выполнения",
                    MessageBoxIcon.Error);
            }
            finally
            {
                _currentOperationCts?.Dispose();
                _currentOperationCts = null;
            }
        }

        /// <summary>
        /// Async обработчик файлов с поддержкой прогресса и отмены
        /// </summary>
        private async Task ActionHandlerAsync(
            string operationName,
            string fileMask,
            FileProcessorType processorType,
            IProgress<ProcessingProgress>? progress,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Запуск операции: {OperationName}", operationName);
                _mainView.ShowMessage($"{TimeSetter.PlaceTime()}Запуск: {operationName}");

                // Получение списка файлов
                var allFiles = await Task.Run(() =>
                    Directory.GetFiles(_state.DirectoryPath, fileMask, SearchOption.AllDirectories),
                    cancellationToken);

                if (allFiles.Length == 0)
                {
                    _mainView.ShowMessage($"{TimeSetter.PlaceTime()}Не найдено файлов с маской {fileMask}");
                    _logger.LogWarning("Не найдено файлов с маской {FileMask} в {DirectoryPath}", fileMask, _state.DirectoryPath);
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

                _mainView.ShowMessage($"{TimeSetter.PlaceTime()}{completionMessage}");
                _logger.LogInformation("Операция завершена: {Message}", completionMessage);

                // Вывод первых ошибок
                foreach (var error in errors.Take(Constants.UI.MaxErrorsToDisplay))
                {
                    _mainView.ShowMessage($"{TimeSetter.PlaceTime()}{error}");
                }

                if (errors.Count > Constants.UI.MaxErrorsToDisplay)
                {
                    _mainView.ShowMessage($"{TimeSetter.PlaceTime()}...и еще {errors.Count - Constants.UI.MaxErrorsToDisplay} ошибок");
                }

                progress?.Report(ProcessingProgress.Completed(
                    processedCount, errorCount, allFiles.Length, completionMessage));
            }
            catch (OperationCanceledException)
            {
                var message = $"Операция '{operationName}' отменена пользователем";
                _mainView.ShowMessage($"{TimeSetter.PlaceTime()}{message}");
                _logger.LogInformation(message);
                progress?.Report(ProcessingProgress.Cancelled(0, 0, message));
            }
            catch (Exception ex)
            {
                var message = $"Критическая ошибка при выполнении '{operationName}': {ex.Message}";
                _mainView.ShowMessage($"{TimeSetter.PlaceTime()}{message}");
                _logger.LogError(ex, "Критическая ошибка в ActionHandlerAsync");
                _mainView.ShowMessageBox(message, "Ошибка", MessageBoxIcon.Error);
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
                FileProcessorType.TagCollector => TagCollector.TagCollectorActivity(filePath, _tagCollectorData),
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
                        message = TagCollector.TagWriterActivity(_tagCollectorData);
                        _mainView.ShowMessage($"{TimeSetter.PlaceTime()}{message}");
                        message = TagCollector.DefsClassGeneratorActivity(_tagCollectorData);
                        _mainView.ShowMessage($"{TimeSetter.PlaceTime()}{message}");
                        _tagCollectorData.Clear();
                        break;

                    case FileProcessorType.FileFixer:
                        message = FileFixer.BrokenFilesWriterActivity();
                        _mainView.ShowMessage($"{TimeSetter.PlaceTime()}{message}");
                        break;
                }
            }, cancellationToken);
        }

        /// <summary>
        /// Поиск изменений в переводе
        /// </summary>
        public async Task FindChangesAsync()
        {
            if (string.IsNullOrEmpty(_state.AdditionalFolder))
            {
                _mainView.ShowMessage($"{TimeSetter.PlaceTime()}Ошибка. Не выбран источник данных");
                _mainView.ShowMessageBox("Выберите папку с исходными файлами мода", "Ошибка", MessageBoxIcon.Warning);
                return;
            }

            _currentOperationCts?.Cancel();
            _currentOperationCts?.Dispose();
            _currentOperationCts = new CancellationTokenSource();

            try
            {
                _logger.LogInformation("Запуск поиска изменений в переводе");
                _mainView.ShowMessage($"Файлы перевода - {_state.DirectoryPath}.{Environment.NewLine}Исходные файлы мода - {_state.AdditionalFolder}");

                // Фаза 1: Сбор данных перевода
                _mainView.ShowMessage($"{TimeSetter.PlaceTime()}Поиск изменений в переводе");

                var translationFiles = await Task.Run(() =>
                    Directory.GetFiles(_state.DirectoryPath, Constants.Files.XmlMask, SearchOption.AllDirectories),
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
                        _mainView.ShowMessage($"{TimeSetter.PlaceTime()}{result.Item2} ({translationFiles[i]})");
                    }
                }

                _mainView.ShowMessage($"{TimeSetter.PlaceTime()}Собраны данные перевода. Обработано файлов - {count}. Пропущено файлов - {errCount}.");

                // Фаза 2: Сбор исходных данных
                var modFiles = await Task.Run(() =>
                    Directory.GetFiles(_state.AdditionalFolder, Constants.Files.XmlMask, SearchOption.AllDirectories),
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
                        _mainView.ShowMessage($"{TimeSetter.PlaceTime()}{result.Item2} ({modFiles[i]})");
                    }
                }

                _mainView.ShowMessage($"{TimeSetter.PlaceTime()}Собраны исходные данные. Обработано файлов - {count}. Пропущено файлов - {errCount}.");

                // Фаза 3: Поиск изменений и запись
                await Task.Run(() =>
                {
                    ChangesFinder.FindChangesInFiles();
                    var writeResult = ChangesFinder.WriteChanges();
                    _mainView.ShowMessage($"{TimeSetter.PlaceTime()}{writeResult}");
                }, _currentOperationCts.Token);

                _logger.LogInformation("Поиск изменений завершен успешно");
            }
            catch (OperationCanceledException)
            {
                _mainView.ShowMessage($"{TimeSetter.PlaceTime()}Операция отменена пользователем");
                _logger.LogInformation("Поиск изменений отменен пользователем");
            }
            catch (Exception ex)
            {
                var message = $"Ошибка при поиске изменений: {ex.Message}";
                _mainView.ShowMessage($"{TimeSetter.PlaceTime()}{message}");
                _logger.LogError(ex, "Ошибка в FindChangesAsync");
                _mainView.ShowMessageBox(message, "Ошибка", MessageBoxIcon.Error);
            }
            finally
            {
                _currentOperationCts?.Dispose();
                _currentOperationCts = null;
            }
        }

        /// <summary>
        /// Создание вспомогательных файлов (Case/Gender)
        /// </summary>
        public async Task CreateCaseFilesAsync()
        {
            _currentOperationCts?.Cancel();
            _currentOperationCts?.Dispose();
            _currentOperationCts = new CancellationTokenSource();

            try
            {
                _logger.LogInformation("Запуск создания вспомогательных файлов");
                _mainView.ShowMessage($"{TimeSetter.PlaceTime()}Создание вспомогательных файлов");

                string[] defTypeList = Constants.DefTypes.SupportedTypes;

                string[] allFiles = await Task.Run(() =>
                    Directory.GetFiles(_state.DirectoryPath, Constants.Files.XmlMask, SearchOption.AllDirectories),
                    _currentOperationCts.Token);

                Dictionary<string, string> words = [];

                string directory = Directory.Exists(_state.DirectoryPath + $"\\{Constants.Paths.CommonFolderName}")
                    ? _state.DirectoryPath + $"\\{Constants.Paths.CommonFolderName}"
                    : _state.DirectoryPath;

                int typeCount = 0;

                foreach (string defType in defTypeList)
                {
                    _currentOperationCts.Token.ThrowIfCancellationRequested();

                    int count = 0;
                    words.Clear();

                    // Сбор слов
                    await Task.Run(() =>
                    {
                        foreach (string tempFile in allFiles)
                        {
                            _currentOperationCts.Token.ThrowIfCancellationRequested();

                            List<string> tempWords = CaseCreator.FindWordsProcessing(tempFile, defType);
                            foreach (string word in tempWords)
                            {
                                bool result = words.TryAdd(word, defType);
                                if (result) { count++; }
                            }
                        }
                    }, _currentOperationCts.Token);

                    if (count > 0)
                    {
                        // Получение лимита
                        var limitResult = await _morpherService.GetRequestLimitAsync();

                        if (!limitResult.IsSuccess || limitResult.Value == null)
                        {
                            _mainView.ShowMessage($"{TimeSetter.PlaceTime()}Ошибка получения лимита слов для {defType}: {limitResult.ErrorMessage ?? "Проблемы с интернетом?"}");
                            _logger.LogWarning("Не удалось получить лимит Morpher для {DefType}: {Error}", defType, limitResult.ErrorMessage);
                            continue;
                        }

                        int limit = limitResult.Value.Value;

                        if (limit < words.Count)
                        {
                            _mainView.ShowMessage($"{TimeSetter.PlaceTime()}Превышен лимит. Доступно сегодня {limit}, требуется для {defType} - {words.Count}. Стоит убрать уже обработанные папки и попробовать завтра. Или сменить IP.");
                            _logger.LogWarning("Превышен лимит Morpher: доступно {Limit}, требуется {Required} для {DefType}", limit, words.Count, defType);
                            continue;
                        }

                        _mainView.ShowMessage($"{TimeSetter.PlaceTime()}Доступно запросов - {limit}");
                        _logger.LogInformation("Доступно запросов Morpher: {Limit} для {DefType}", limit, defType);

                        // Создание файлов
                        await CaseCreator.CreateCaseAsync(directory, words, defType, _morpherService, _currentOperationCts.Token);
                        await Task.Run(() =>
                            CaseCreator.CreateGender(directory, words, defType),
                            _currentOperationCts.Token);

                        _mainView.ShowMessage($"{TimeSetter.PlaceTime()}Обработано {count} объектов типа {defType}");
                        _logger.LogInformation("Обработано {Count} объектов типа {DefType}", count, defType);
                        typeCount++;
                    }
                }

                if (typeCount > 0)
                {
                    _mainView.ShowMessage($"{TimeSetter.PlaceTime()}Созданы файлы по адресу {directory}\\Languages\\Russian\\WordInfo");
                    _logger.LogInformation("Созданы файлы WordInfo для {TypeCount} типов DefType", typeCount);
                }
                else
                {
                    _mainView.ShowMessage($"{TimeSetter.PlaceTime()}Файлы не созданы");
                    _logger.LogInformation("Файлы WordInfo не созданы - не найдено подходящих слов");
                }

                _mainView.ShowMessage($"{TimeSetter.PlaceTime()}Завершено");
            }
            catch (OperationCanceledException)
            {
                _mainView.ShowMessage($"{TimeSetter.PlaceTime()}Операция создания файлов отменена пользователем");
                _logger.LogInformation("Создание вспомогательных файлов отменено пользователем");
            }
            catch (Exception ex)
            {
                var message = $"Ошибка при создании вспомогательных файлов: {ex.Message}";
                _mainView.ShowMessage($"{TimeSetter.PlaceTime()}{message}");
                _logger.LogError(ex, "Ошибка в CreateCaseFilesAsync");
                _mainView.ShowMessageBox(message, "Ошибка", MessageBoxIcon.Error);
            }
            finally
            {
                _currentOperationCts?.Dispose();
                _currentOperationCts = null;
            }
        }

        /// <summary>
        /// Предварительный перевод
        /// </summary>
        public async Task PreTranslateAsync()
        {
            _currentOperationCts?.Cancel();
            _currentOperationCts?.Dispose();
            _currentOperationCts = new CancellationTokenSource();

            try
            {
                _logger.LogInformation("Запуск предварительного перевода");

                if (string.IsNullOrEmpty(_state.AdditionalFolder))
                {
                    _mainView.ShowMessage($"{TimeSetter.PlaceTime()}Ошибка. Не выбран источник данных");
                    _mainView.ShowMessageBox("Выберите папку с исходными файлами для перевода", "Ошибка", MessageBoxIcon.Warning);
                    return;
                }

                PreTranslator.ClearTranslationData();

                _mainView.ShowMessage($"Переводимые файлы - {_state.DirectoryPath}.{Environment.NewLine}Исходные файлы для предварительного перевода - {_state.AdditionalFolder}");
                _mainView.ShowMessage($"{TimeSetter.PlaceTime()}Сбор данных для перевода");

                // Фаза 1: Сбор данных
                string[] allFiles = await Task.Run(() =>
                    Directory.GetFiles(_state.AdditionalFolder, Constants.Files.XmlMask, SearchOption.AllDirectories),
                    _currentOperationCts.Token);

                int count = 0;
                int errCount = 0;
                (bool, string) result = (false, string.Empty);

                for (int i = 0; i < allFiles.Length; i++)
                {
                    _currentOperationCts.Token.ThrowIfCancellationRequested();

                    string tempFile = allFiles[i];

                    if (tempFile.StartsWith(_state.DirectoryPath, StringComparison.OrdinalIgnoreCase))
                    {
                        _mainView.ShowMessage($"{TimeSetter.PlaceTime()}Включать переводимый мод в исходные файлы - плохая идея.");
                        _logger.LogWarning("Попытка включить переводимый мод в исходные файлы: {File}", tempFile);
                        return;
                    }

                    result = await PreTranslator.BuildDatabaseAsync(tempFile, _currentOperationCts.Token);
                    if (result.Item1)
                    {
                        count++;
                    }
                    else
                    {
                        errCount++;
                        _mainView.ShowMessage($"{TimeSetter.PlaceTime()}{result.Item2} ({tempFile})");
                        _logger.LogWarning("Ошибка сбора данных из {File}: {Error}", tempFile, result.Item2);
                    }
                }

                _mainView.ShowMessage($"{TimeSetter.PlaceTime()}Собрано {result.Item2} пар перевода. Обработано файлов - {count}. Пропущено файлов - {errCount}.");
                _logger.LogInformation("Фаза 1 завершена: собрано {Pairs} пар, обработано {Count} файлов, пропущено {Errors}", result.Item2, count, errCount);

                // Фаза 2: Применение
                _mainView.ShowMessage($"{TimeSetter.PlaceTime()}Начат предварительный перевод");

                allFiles = await Task.Run(() =>
                    Directory.GetFiles(_state.DirectoryPath, Constants.Files.XmlMask, SearchOption.AllDirectories),
                    _currentOperationCts.Token);

                count = 0;
                errCount = 0;

                for (int i = 0; i < allFiles.Length; i++)
                {
                    _currentOperationCts.Token.ThrowIfCancellationRequested();

                    string tempFile = allFiles[i];
                    result = await PreTranslator.TranslationAsync(tempFile, _currentOperationCts.Token);

                    if (result.Item1)
                    {
                        count++;
                    }
                    else
                    {
                        errCount++;
                        _mainView.ShowMessage($"{TimeSetter.PlaceTime()}{result.Item2} ({tempFile})");
                        _logger.LogWarning("Ошибка перевода {File}: {Error}", tempFile, result.Item2);
                    }
                }

                _mainView.ShowMessage($"{TimeSetter.PlaceTime()}Обработано файлов - {count}. Пропущено файлов - {errCount}.");
                _logger.LogInformation("Фаза 2 завершена: обработано {Count} файлов, пропущено {Errors}", count, errCount);
                _mainView.ShowMessage($"{TimeSetter.PlaceTime()}Предварительный перевод завершен");
            }
            catch (OperationCanceledException)
            {
                _mainView.ShowMessage($"{TimeSetter.PlaceTime()}Операция предварительного перевода отменена пользователем");
                _logger.LogInformation("Предварительный перевод отменен пользователем");
            }
            catch (Exception ex)
            {
                var message = $"Ошибка при предварительном переводе: {ex.Message}";
                _mainView.ShowMessage($"{TimeSetter.PlaceTime()}{message}");
                _logger.LogError(ex, "Ошибка в PreTranslateAsync");
                _mainView.ShowMessageBox(message, "Ошибка", MessageBoxIcon.Error);
            }
            finally
            {
                _currentOperationCts?.Dispose();
                _currentOperationCts = null;
            }
        }
    }
}
