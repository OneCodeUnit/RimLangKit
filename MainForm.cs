﻿using System.Diagnostics;
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
        private readonly Color goodColor = Color.FromArgb(0, 130, 0);
        private readonly Color badColor = Color.FromArgb(130, 0, 0);

        public MainForm()
        {
            InitializeComponent();
            FolderTextBox.Text = DirectoryPath;

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
            using FolderBrowserDialog ofd = new();
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
            string newVersion = json.TagName.ToString()[1..];
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
                SendToInfoTextBox($"{json.Name}. Что нового?{Environment.NewLine}{json.Body}");
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
            InfoTextBox2.AppendText($"{TimeSetter.PlaceTime()}Запуск: обновление перевода игры");
            (bool, string) result;
            result = LanguageUpdater.TranslationVersionCheckActivity(Settings.Default.sha, Settings.Default.repo);
            InfoTextBox2.AppendText($"{Environment.NewLine}{TimeSetter.PlaceTime()}{result.Item2}");
            if (result.Item1 == false)
                return;
            result = LanguageUpdater.LanguageUpdateDownload(Settings.Default.repo);
            InfoTextBox2.AppendText($"{Environment.NewLine}{TimeSetter.PlaceTime()}{result.Item2}");
            if (result.Item1 == false)
                return;

            result = LanguageUpdater.LanguageUpdateActivity(GamePath, Settings.Default.language);
            InfoTextBox2.AppendText($"{Environment.NewLine}{TimeSetter.PlaceTime()}{result.Item2}");
            if (result.Item1 == false)
                return;

            Settings.Default["sha"] = LanguageUpdater.GetSha();
            Settings.Default.Save();
        }

        // Удаление скачанного перевода
        private void ResetButton_Click(object sender, EventArgs e)
        {
            // Получения списка дополнений
            string[] modules = Directory.GetDirectories($"{GamePath}\\Data");
            if (modules.Length == 0)
            {
                InfoTextBox2.AppendText($"{Environment.NewLine}{TimeSetter.PlaceTime()}В указанной папке не обнаружены модули");
                return;
            }
            string resultString = "Будут удалены следующие папки:";
            string language = Settings.Default.language;
            for (int i = 0; i < modules.Length; i++)
            {
                modules[i] += $"\\Languages\\{language}";
                resultString += $"{Environment.NewLine}{modules[i]}";
            }
            DialogResult res = MessageBox.Show(resultString, "Подтверждение", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (res == DialogResult.OK)
            {
                Settings.Default["sha"] = "00000000";
                Settings.Default.Save();
                for (int i = 0; i < modules.Length; i++)
                {
                    if (Directory.Exists(modules[i]))
                    {
                        Directory.Delete(modules[i], true);
                        InfoTextBox2.AppendText($"{Environment.NewLine}{TimeSetter.PlaceTime()}Папка {modules[i]} удалена");
                    }
                    else
                    {
                        InfoTextBox2.AppendText($"{Environment.NewLine}{TimeSetter.PlaceTime()}Папка {modules[i]} не найдена");
                    }
                }
                InfoTextBox2.AppendText($"{Environment.NewLine}{TimeSetter.PlaceTime()}Удаление перевода завершено");
            }
            InfoTextBox2.AppendText($"{Environment.NewLine}{TimeSetter.PlaceTime()}Удаление перевода отменено");
        }

        // Сброс настроек к значеним по умолчанию
        private void DefaultButton_Click(object sender, EventArgs e)
        {
            Settings.Default["sha"] = "00000000";
            Settings.Default["repo"] = "Ludeon/RimWorld-ru";
            Settings.Default["language"] = "Russian (GitHub)";
            Settings.Default["savedDirectory"] = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\RimWorld";
            Settings.Default.Save();

            if (Directory.Exists("C:\\Program Files (x86)\\Steam\\steamapps\\common\\RimWorld"))
            {
                GamePath = Settings.Default.savedDirectory;
            }
            else
            {
                GamePath = string.Empty;
            }
            RepoInput.Text = Settings.Default.repo;
            LanguageInput.Text = Settings.Default.language;
            FolderTextBox2.Text = GamePath;
            InfoTextBox2.AppendText($"{Environment.NewLine}{TimeSetter.PlaceTime()}Настройки программы (перевода) сброшены к значениям по умолчанию");
        }

        private void RepoInput_TextChanged(object sender, EventArgs e)
        {
            Settings.Default["repo"] = "Ludeon/RimWorld-ru";
            Settings.Default.Save();
        }

        private void LanguageInput_TextChanged(object sender, EventArgs e)
        {
            Settings.Default["language"] = "Russian (GitHub)";
            Settings.Default.Save();
        }

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
            string path = $"{tempPath}\\Version.txt";
            if (File.Exists(path))
            {
                ButtonLanguageUpdate.Enabled = true;
                ResetButton.Enabled = true;
                GamePath = tempPath;
                Settings.Default["savedDirectory"] = tempPath;
                Settings.Default.Save();
                FolderButton2.BackColor = goodColor;
            }
            else
            {
                ButtonLanguageUpdate.Enabled = false;
                ResetButton.Enabled = false;
                FolderButton2.BackColor = badColor;
            }
        }
#endregion
    }
}