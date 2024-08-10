using System.Diagnostics;
using System.Globalization;
using BlueMystic;
using RimLangKit.Properties;
using RimLanguageCore.Activities;
using RimLanguageCore.Misc;

namespace RimLangKit
{
    public partial class MainForm : Form
    {
        private DarkModeCS? DM;
        private static string DirectoryPath = string.Empty;
        private static string GamePath = string.Empty;
        bool FindChangesFirst = true;
        string TranslationPath = string.Empty;
        string ModPath = string.Empty;
        string DownloadPath = string.Empty;
        string SteamCmdPath = string.Empty;

        public MainForm()
        {
            InitializeComponent();
            // Установка темной темы
            if (Settings.Default.darkTheme)
                DM = new DarkModeCS(this);

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
                    version = version[..^2];
            }
            VersionLabel.Text = "Версия " + version;

            // Восстановление последней вкладки
            MainTabs.SelectTab(Settings.Default.lastTab);

            // Установка размера вкладок
            MainTabs.TabPages.Remove(tabPage3);
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
                LabelCheck.ForeColor = Color.Green;
                LabelCheck.Text = "ОК";
                ButtonENGIsert.Enabled = true;
                FileRenamerButton.Enabled = true;
                NamesTranslatorButton.Enabled = true;
                ButtonCase.Enabled = true;
                ButtonEncoding.Enabled = true;
                TagCollectorButton.Enabled = true;
                FileFixerButton.Enabled = true;
                ButtonFindChanges.Enabled = true;
            }
            else
            {
                LabelCheck.ForeColor = Color.Red;
                LabelCheck.Text = "Ошибка: некорректная папка";
                ButtonENGIsert.Enabled = false;
                FileRenamerButton.Enabled = false;
                NamesTranslatorButton.Enabled = false;
                ButtonCase.Enabled = false;
                ButtonEncoding.Enabled = false;
                TagCollectorButton.Enabled = false;
                FileFixerButton.Enabled = false;
                ButtonFindChanges.Enabled = false;
            }
        }

        #region жертвы для рефакторинга
        private void ButtonEncoding_Click(object sender, EventArgs e)
        {
            ActionHandler("Исправление кодировки", 1);
        }
        private void ButtonENGIsert_Click(object sender, EventArgs e)
        {
            ActionHandler("Добавление комментариев", 2);
        }
        private void ButtonCase_Click(object sender, EventArgs e)
        {
            InfoTextBox.Text = $"{PlaceTime()}Создание вспомогательных файлов";
            string[] defTypeList = ["AbilityDef", "BodyDef", "BodyPartDef", "BodyPartGroupDef", "ChemicalDef", "FactionDef", "HediffDef", "MemeDef", "MentalBreakDef", "MentalFitDef", "MentalStateDef", "OrderedTakeGroupDef", "PawnCapacityDef", "PawnKindDef", "ScenarioDef", "SitePartDef", "SkillDef", "StyleCategoryDef", "ThingDef", "ToolCapacityDef", "WorldObjectDef", "XenotypeDef"];
            //Получение списка всех файлов в заданой директории и во всех вложенных подпапках за счёт SearchOption
            string[] allFiles = Directory.GetFiles(DirectoryPath, "*.xml", SearchOption.AllDirectories);
            Dictionary<string, string> words = new();
            //List<string> words = new();

            // Поиск подходящей директории
            string directory;
            if (Directory.Exists(DirectoryPath + "\\Common"))
            {
                directory = DirectoryPath + "\\Common";
            }
            else
            {
                directory = DirectoryPath;
            }

            int typeCount = 0;
            // Проверяется каждый подходящий DefType из списка
            foreach (string defType in defTypeList)
            {
                int count = 0;
                foreach (string tempFile in allFiles)
                {
                    // Каждый файл проверяется на соотвествие этому типу. Если не соотвествует, возвращает пустой список
                    List<string> tempWords = CaseCreate.FindWordsProcessing(tempFile, defType);
                    foreach (string word in tempWords)
                    {
                        bool result = words.TryAdd(word, defType);
                        if (result) { count++; }
                    }
                }
                // Если нашлись слова в данном DefType, то создаются файлы
                if (count > 0)
                {
                    CaseCreate.CreateCase(directory, words, defType);
                    CaseCreate.CreateGender(directory, words, defType);
                    InfoTextBox.AppendText($"{Environment.NewLine}{PlaceTime()}Обработано {count} объектов типа {defType}");
                    typeCount++;
                }
            }
            if (typeCount > 0)
            {
                InfoTextBox.AppendText($"{Environment.NewLine}{PlaceTime()}Созданы файлы по адресу {directory}\\Languages\\Russian\\WordInfo");
            }
            else
            {
                InfoTextBox.AppendText($"{Environment.NewLine}{PlaceTime()}Файлы не созданы");
            }
            InfoTextBox.AppendText($"{Environment.NewLine}{PlaceTime()}Завершено");
        }

        private void ActionHandler(string name, int code)
        {
            InfoTextBox.Text = $"{PlaceTime()}{name}";
            // Получение списка всех файлов в заданой папке и во всех вложенных подпапках за счёт SearchOption
            string[] allFiles = Directory.GetFiles(DirectoryPath, "*.xml", SearchOption.AllDirectories);
            int count = 0;
            int errCount = 0;
            bool result = true;
            foreach (string tempFile in allFiles)
            {
                switch (code)
                {
                    case 1:
                        result = EncodingFix.EncodingFixProcessing(tempFile);
                        break;
                    case 2:
                        result = CommentInsert.CommentsInsertProcessing(tempFile, InfoTextBox);
                        break;
                    default:
                        break;
                }
                if (result) count++;
                else errCount++;
            }

            InfoTextBox.AppendText($"{Environment.NewLine}{PlaceTime()}Успешно завершено. Обработано файлов - {count}");
            if (errCount != 0)
                InfoTextBox.AppendText($"{Environment.NewLine}Пропущено файлов - {errCount}");
        }
        public static string PlaceTime()
        {
            DateTime time = DateTime.Now;
            string result = time.ToString("HH:mm:ss", CultureInfo.InvariantCulture) + " - ";
            return result;
        }
        #endregion

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
            if (Settings.Default.darkTheme == true)
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

        // Выбор текстового поля в зависимости от выбранной вкладки
        private void SendToInfoTextBox(string text)
        {
            if (Settings.Default.lastTab == 0)
                InfoTextBox.AppendText($"{Environment.NewLine}{text}");
            else if (Settings.Default.lastTab == 1)
                InfoTextBox2.AppendText($"{Environment.NewLine}{text}");
            else
                InfoTextBox3.AppendText($"{Environment.NewLine}{text}"); ;

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
            string message = string.Empty;
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
                InfoTextBox.AppendText($"{Environment.NewLine}Пропущено файлов - {errCount}");

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
        private void ButtonFindChanges_Click(object sender, EventArgs e)
        {
            if (FindChangesFirst)
            {
                TranslationPath = DirectoryPath;
                InfoTextBox.AppendText($"{Environment.NewLine}Выбраны файлы перевода - {TranslationPath}.{Environment.NewLine}Теперь выберите путь к файлам мода.");
                DirectoryPath = string.Empty;
                FolderTextBox.Text = string.Empty;
                FindChangesFirst = false;
            }
            else
            {
                ModPath = DirectoryPath;
                InfoTextBox.AppendText($"{Environment.NewLine}Файлы перевода - {TranslationPath}.{Environment.NewLine}Выбраны файлы мода - {ModPath}");
                DirectoryPath = TranslationPath;
                FindChangesFirst = true;

                InfoTextBox.Text = $"{PlaceTime()}Поиск изменений в переводе";
                string[] allFiles = Directory.GetFiles(TranslationPath, "*.xml", SearchOption.AllDirectories);
                int count = 0;
                int errCount = 0;
                bool result;
                foreach (string tempFile in allFiles)
                {
                    result = FindChanges.GetTranslationData(tempFile, InfoTextBox);
                    if (result) count++;
                    else errCount++;
                }
                InfoTextBox.AppendText($"{Environment.NewLine}{PlaceTime()}Собраны данные перевода. Обработано файлов - {count}.{Environment.NewLine}Пропущено файлов - {errCount}.");

                allFiles = Directory.GetFiles(ModPath, "*.xml", SearchOption.AllDirectories);
                count = 0;
                errCount = 0;
                foreach (string tempFile in allFiles)
                {
                    result = FindChanges.GetModData(tempFile, InfoTextBox);
                    if (result) count++;
                    else errCount++;
                }
                InfoTextBox.AppendText($"{Environment.NewLine}{PlaceTime()}Собраны исходные данные. Обработано файлов - {count}.{Environment.NewLine}Пропущено файлов - {errCount}.");

                FindChanges.FindChangesInFiles();
                FindChanges.WriteChanges(InfoTextBox);
            }
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
        #endregion





        #region обновление локализации
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
                FolderButton2.BackColor = Color.FromArgb(154, 185, 115);
            }
            else
            {
                ButtonLanguageUpdate.Enabled = false;
                ResetButton.Enabled = false;
                FolderButton2.BackColor = Color.FromArgb(205, 92, 92);
            }
        }
        #endregion


        #region загрузка модов
        private void ButtonSteam_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new();
            ofd.Filter = "steamcmd (*.exe)|*.exe";
            ofd.ShowDialog();
            SteamCmdPath = ofd.FileName;
            SteamTextBox.Text = SteamCmdPath;

            //Settings.Default["steamCmdFile"] = SteamCmdPath;
            //Settings.Default.Save();   
        }

        private void ButtonResultFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog ofd = new();
            DialogResult dr = ofd.ShowDialog();
            if (dr == DialogResult.OK)
            {
                DownloadPath = ofd.SelectedPath;
                FolderTextBox3.Text = DownloadPath;


                //Settings.Default["downloadDirectory"] = DownloadPath;
                //Settings.Default.Save();          
            }
        }

        private void ButtonDownload_Click(object sender, EventArgs e)
        {
            //System.Diagnostics.Process.Start(SteamCmdPath);
            //C:\steamcmd\steamcmd.exe +force_install_dir C:\Users\inqui\Desktop\1 +login anonymous +workshop_download_item 294100 3228047321 +quit 
        }
        #endregion


    }
}