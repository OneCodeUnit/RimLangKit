using System.Diagnostics;
using DarkModeForms;
using RimLangKit.Properties;
using RimLanguageCore.Activities;
using RimLanguageCore.Misc;

namespace RimLangKit
{
    public partial class MainForm : Form
    {
        private static string DirectoryPath = string.Empty;
        private static string GamePath = string.Empty;
        private static string AdditionalFolder = string.Empty;
        private Color goodColor;
        private Color badColor;

        public MainForm()
        {
            InitializeComponent();
            FolderTextBox.Text = DirectoryPath;
            // Установка темной темы
            if (Settings.Default.darkTheme)
            {
                goodColor = Color.FromArgb(0, 180, 0);
                badColor = Color.FromArgb(180, 0, 0);
                _ = new DarkModeCS(this);
                ToolTip tooltip = new();
                tooltip.SetToolTip(UpdateButton, "Проверка обновлений");
                tooltip.SetToolTip(ButtonDarkMode, "Темная тема");
            }
            else
            {
                goodColor = Color.FromArgb(0, 130, 0);
                badColor = Color.FromArgb(130, 0, 0);
            }

            // Обновление настроек
            if (Settings.Default.firstLaunch)
            {
                Settings.Default.Upgrade();
                Settings.Default.firstLaunch = false;
                Settings.Default.Save();
                SendToInfoTextBox("Первый запуск. Обновление настроек завершено");
            }

            // Установка версии программы
            string? version = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Version?.ToString();
            if (version is null || version.Length < 5)
            {
                InfoTextBox.AppendText($"{Environment.NewLine}{TimeSetter.PlaceTime()}Неправильная версия программы");
            }
            else
            {
                version = version[..5];
                if (version.EndsWith(".0", StringComparison.OrdinalIgnoreCase))
                {
                    version = version[..^2];
                }
            }
            VersionLabel.Text = "Версия " + version;

            // Восстановление последней вкладки
            MainTabs.SelectTab(Settings.Default.lastTab);

            // Установка размера вкладок
            MainTabs.SizeMode = TabSizeMode.Fixed;
            MainTabs.ItemSize = new Size((MainTabs.Width / MainTabs.TabPages.Count) - 2, MainTabs.ItemSize.Height);

            // Восстановление пути к папке игры
            if (Directory.Exists(Settings.Default.savedDirectory))
            {
                GamePath = Settings.Default.savedDirectory;
            }
            else
            {
                GamePath = string.Empty;
            }
            FolderTextBox2.Text = GamePath;

            LanguageInput.Text = Settings.Default.language;
            RepoInput.Text = Settings.Default.repo;
        }

        // Выбор папки и обработка изменения адреса папки
        private void FolderButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog ofd = new();
            DialogResult dr = ofd.ShowDialog();
            if (dr == DialogResult.OK)
            {
                DirectoryPath = ofd.SelectedPath;
                FolderTextBox.Text = DirectoryPath;
            }
        }

        private void FolderTextBox_TextChanged(object sender, EventArgs e)
        {
            DirectoryPath = FolderTextBox.Text;
            // Кнопки доступны только, тогда, когда директория существует
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
        private void UpdateButton_Click(object sender, EventArgs e)
        {
            Root? json = GitHubHttpClient.GetGithubJson();
            if (json is null)
            {
                SendToInfoTextBox("Ошибка получения данных с GitHub");
                UpdateButton.BackgroundImage = Properties.Resources.no_c;
                LinkLabelGithub.Visible = true;
                return;
            }
            string newVersion = json.tag_name.ToString()[1..];
            string? oldVersion = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Version?.ToString();
            if (oldVersion is null || oldVersion.Length < 5)
            {
                SendToInfoTextBox("Неправильная версия программы");
                UpdateButton.BackgroundImage = Properties.Resources.warn_c;
                LinkLabelGithub.Visible = true;
                return;
            }
            else
            {
                oldVersion = oldVersion[..5];
            }
            if (oldVersion.EndsWith(".0", StringComparison.OrdinalIgnoreCase))
            {
                oldVersion = oldVersion[..^2];
            }
            SendToInfoTextBox($"Ваша версия - {oldVersion}, доступная версия - {newVersion}");
            UpdateLabel.Text = $"Последняя {newVersion}";

            if (oldVersion == newVersion)
            {
                SendToInfoTextBox("Обновление не требуется");
                UpdateButton.BackgroundImage = Properties.Resources.yes_c;
                LinkLabelGithub.Visible = false;
            }
            else
            {
                SendToInfoTextBox("Доступна новая версия программы. Её можно скачать на GitHub по ссылке справа");
                SendToInfoTextBox($"{json.name}. Что нового?{Environment.NewLine}{json.body}");
                UpdateButton.BackgroundImage = Properties.Resources.warn_c;
                LinkLabelGithub.Visible = true;
            }
        }

        private void LinkLabelGithub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LinkLabelGithub.LinkVisited = true;
            try
            {
                Process.Start(new ProcessStartInfo { FileName = @"https://github.com/OneCodeUnit/RimLangKit/releases/latest", UseShellExecute = true });
            }
            catch
            {
                MessageBox.Show("Сайт не открылся :(");
            }
        }

        private void LinkLabelInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LinkLabelGithub.LinkVisited = true;
            try
            {
                Process.Start(new ProcessStartInfo { FileName = @"https://github.com/OneCodeUnit/RimLangKit/blob/master/README.md", UseShellExecute = true });
            }
            catch
            {
                MessageBox.Show("Сайт не открылся :(");
            }
        }

        private void ButtonDarkMode_Click(object sender, EventArgs e)
        {
            if (Settings.Default.darkTheme)
            {
                Settings.Default["darkTheme"] = false;
                SendToInfoTextBox("Хе-хе бой. Это светлая тема. Чтобы развидеть тёмную тему, перезапусти приложение.");
            }
            else
            {
                Settings.Default["darkTheme"] = true;
                SendToInfoTextBox("Хе-хе бой. Это тёмная тема. Чтобы её увидеть, перезапусти приложение.");
            }
            Settings.Default.Save();
        }

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
                InfoTextBox.AppendText($"{Environment.NewLine}{text}");
            }
            else
            {
                InfoTextBox2.AppendText($"{Environment.NewLine}{text}");
            }
        }

        private void MainTabs_IndexChange(object sender, EventArgs e)
        {
            Settings.Default["lastTab"] = MainTabs.SelectedIndex;
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
                        result = CommentInserter.CommentInserterActivity(currentFile);
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
    }
}