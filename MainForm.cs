using RimLangKit.Properties;
using RimLanguageCore.Activities;
using RimLanguageCore.Misc;
using System.Diagnostics;
using System.Reflection;

namespace RimLangKit
{
    public partial class MainForm : Form
    {
        private static string DirectoryPath = string.Empty;
        private static string GamePath = string.Empty;
        private static string AdditionalFolder = string.Empty;
        private readonly Color goodColor = Color.FromArgb(0, 130, 0);
        private readonly Color badColor = Color.FromArgb(130, 0, 0);

        public MainForm()
        {
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

            // Автоматическая проверка обновлений
            if (Settings.Default.isAutoUpdateActive)
            {
                ToolStripMenuItemAutoUpdateCheck.Checked = true;
                var oldCheckDate = Settings.Default.lastCheckDate;
                var newCheckDate = DateTime.Now;
                if (oldCheckDate.Month != newCheckDate.Month)
                {
                    bool result = CheckVersion();
                    if (result)
                    {
                        Settings.Default.lastCheckDate = newCheckDate;
                        Settings.Default.Save();
                    }
                }
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
                DirectoryPath = ofd.SelectedPath;
                FolderTextBox.Text = DirectoryPath;
            }
        }

        // Обработка изменения адреса папки
        private void FolderTextBox_TextChanged(object sender, EventArgs e)
        {
            DirectoryPath = FolderTextBox.Text;
            // Кнопки доступны только тогда, когда директория существует
            if (Directory.Exists(FolderTextBox.Text) && !FolderTextBox.Text.Contains("294100"))
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
                AdditionalFolder = ofd.SelectedPath;
            }
            AdditionalFolderButton.BackColor = goodColor;
        }

        // Выбор текстового поля в зависимости от выбранной вкладки
        private void SendToInfoTextBox(string text)
        {
            if (Settings.Default.lastTab == 0)
            {
                if (InfoTextBox.Text == string.Empty)
                    InfoTextBox.AppendText($"{text}");
                else
                    InfoTextBox.AppendText($"{Environment.NewLine}{text}");
            }
            else
            {
                if (InfoTextBox2.Text == string.Empty)
                    InfoTextBox2.AppendText($"{text}");
                else
                    InfoTextBox2.AppendText($"{Environment.NewLine}{text}");
            }
        }

        private void MainTabs_IndexChange(object sender, EventArgs e)
        {
            Settings.Default.lastTab = MainTabs.SelectedIndex;
            Settings.Default.Save();
        }
        #endregion

        #region кнопки функций
        // Обработчик нажатий кнопок
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

        // Работа без обработчика
        private void FindChangesButton_Click(object sender, EventArgs e)
        {
            if (AdditionalFolder == string.Empty)
            {
                InfoTextBox.AppendText($"{Environment.NewLine}{TimeSetter.PlaceTime()}Ошибка. Не выбран источник данных");
                return;
            }

            InfoTextBox.AppendText($"{Environment.NewLine}Файлы перевода - {DirectoryPath}.{Environment.NewLine}Исходные файлы мода - {AdditionalFolder}");

            InfoTextBox.AppendText($"{TimeSetter.PlaceTime()}Поиск изменений в переводе");
            string[] allFiles = Directory.GetFiles(DirectoryPath, "*.xml", SearchOption.AllDirectories);
            int count = 0;
            int errCount = 0;
            (bool, string) result;
            foreach (string tempFile in allFiles)
            {
                result = ChangesFinder.GetTranslationData(tempFile);
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
            InfoTextBox.AppendText($"{Environment.NewLine}{TimeSetter.PlaceTime()}Собраны данные перевода. Обработано файлов - {count}.{Environment.NewLine}Пропущено файлов - {errCount}.");

            allFiles = Directory.GetFiles(AdditionalFolder, "*.xml", SearchOption.AllDirectories);
            count = 0;
            errCount = 0;
            foreach (string tempFile in allFiles)
            {
                result = ChangesFinder.GetModData(tempFile);
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
            InfoTextBox.AppendText($"{Environment.NewLine}{TimeSetter.PlaceTime()}Собраны исходные данные. Обработано файлов - {count}.{Environment.NewLine}Пропущено файлов - {errCount}.");

            ChangesFinder.FindChangesInFiles();
            string writeResult = ChangesFinder.WriteChanges();
            InfoTextBox.AppendText($"{Environment.NewLine}{TimeSetter.PlaceTime()}{writeResult}");
        }

        private void CaseCreatorButton_Click(object sender, EventArgs e)
        {
            InfoTextBox.AppendText($"{TimeSetter.PlaceTime()}Создание вспомогательных файлов");
            string[] defTypeList = ["AbilityDef", "BodyDef", "BodyPartDef", "BodyPartGroupDef", "ChemicalDef", "FactionDef", "HediffDef", "MemeDef", "MentalBreakDef", "MentalFitDef", "MentalStateDef", "OrderedTakeGroupDef", "PawnCapacityDef", "PawnKindDef", "ScenarioDef", "SitePartDef", "SkillDef", "StyleCategoryDef", "ThingDef", "ToolCapacityDef", "WorldObjectDef", "XenotypeDef"];
            //Получение списка всех файлов в заданой директории и во всех вложенных подпапках за счёт SearchOption
            string[] allFiles = Directory.GetFiles(DirectoryPath, "*.xml", SearchOption.AllDirectories);
            Dictionary<string, string> words = [];

            // Поиск подходящей директории
            string directory = Directory.Exists(DirectoryPath + "\\Common") ? DirectoryPath + "\\Common" : DirectoryPath;
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
                    int? limit = MorpherHttpClient.GetMorpherRequestLimit();
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

        private void PreTranslatorButton_Click(object sender, EventArgs e)
        {
            if (AdditionalFolder == string.Empty)
            {
                InfoTextBox.AppendText($"{Environment.NewLine}{TimeSetter.PlaceTime()}Ошибка. Не выбран источник данных");
                return;
            }

            InfoTextBox.AppendText($"{Environment.NewLine}Переводимые файлы - {DirectoryPath}.{Environment.NewLine}Исходные файлы для предварительного перевода - {AdditionalFolder}");
            InfoTextBox.AppendText($"{Environment.NewLine}{TimeSetter.PlaceTime()}Сбор данных для перевода");
            string[] allFiles = Directory.GetFiles(AdditionalFolder, "*.xml", SearchOption.AllDirectories);
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
            allFiles = Directory.GetFiles(DirectoryPath, "*.xml", SearchOption.AllDirectories);
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

        // Вызов обработчика из кнопок
        private void FileRenamerButton_Click(object sender, EventArgs e)
        {
            ActionHandler("Переименование файлов", "*.xml", "FileRenamer");
        }

        private void NamesTranslatorButton_Click(object sender, EventArgs e)
        {
            ActionHandler("Транскрипция имён", "*.txt", "NamesTranslator");
        }

        private void TagCollectorButton_Click(object sender, EventArgs e)
        {
            ActionHandler("Сбор статистики тегов", "*.xml", "TagCollector");
        }

        private void FileFixerButton_Click(object sender, EventArgs e)
        {
            ActionHandler("Поиск сломанных файлов", "*.xml", "FileFixer");
        }

        private void EncodingFixerButton_Click(object sender, EventArgs e)
        {
            ActionHandler("Исправление кодировки", "*.xml", "EncodingFixer");
        }

        private void CommentInserterButton_Click(object sender, EventArgs e)
        {
            ActionHandler("Добавление комментариев", "*.xml", "CommentInserter");
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
            if (Directory.Exists($"{Settings.Default.savedDirectory}\\Data"))
            {
                string[] modules = Directory.GetDirectories($"{Settings.Default.savedDirectory}\\Data");
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
                            Settings.Default.sha = "00000000";
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
                Settings.Default.sha = "00000000";
                Settings.Default.repo = "Ludeon/RimWorld-ru";
                Settings.Default.language = "Russian (GitHub)";
                Settings.Default.savedDirectory = string.Empty;
                Settings.Default.Save();

                LanguageInput.Text = "Russian (GitHub)";
                RepoInput.Text = "Ludeon/RimWorld-ru";
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
                GamePath = ofd.SelectedPath;
                FolderTextBox2.Text = GamePath;
            }
        }

        private void FolderTextBox2_TextChanged(object sender, EventArgs e)
        {
            string tempPath = FolderTextBox2.Text;
            FolderButton2.BackColor = badColor;
            if (Directory.Exists(tempPath))
            {
                FolderButton2.BackColor = goodColor;
                if (Directory.Exists($"{tempPath}\\Data"))
                {
                    string[] modules = Directory.GetDirectories($"{tempPath}\\Data");
                    if (modules.Length == 0)
                    {
                        SendToInfoTextBox("Нет модулей");
                        FolderButton2.BackColor = Color.MediumBlue;
                    }
                    else
                    {
                        SendToInfoTextBox("Найдены модули:");
                        foreach (var module in modules)
                        {
                            string moduleName = module[module.LastIndexOf('\\')..];
                            moduleName = moduleName[1..];
                            SendToInfoTextBox(moduleName);
                        }
                        GamePath = tempPath;
                        Settings.Default.savedDirectory = tempPath;
                        Settings.Default.Save();
                    }
                }
                else
                {
                    SendToInfoTextBox("Нет модулей");
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
        private void ToolStripMenuItemCheckUpdate_Click(object sender, EventArgs e)
        {
            CheckVersion();
        }

        private static bool CheckVersion()
        {
            var json = GitHubHttpClient.GetGithubJson();
            if (json is null)
            {
                MessageBox.Show("Не удалось проверить обновления. Проверьте подключение к интернету.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            var newVersion = json.TagName.ToString()[1..];
            var oldVersion = Assembly.GetEntryAssembly()?.GetName().Version?.ToString()[..^2];
            if (oldVersion == newVersion)
            {
                MessageBox.Show("Обновление не требуется", "Обновление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            else
            {
                var dr = MessageBox.Show($"Доступно обновление до версии {newVersion}!\nПерейти на страницу загрузки?\n\nВ новой версии:\n{json.Body}", "Обновление", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (dr == DialogResult.Yes)
                {
                    Process.Start(new ProcessStartInfo { FileName = json.HtmlUrl.ToString(), UseShellExecute = true });
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
        #endregion
    }
}