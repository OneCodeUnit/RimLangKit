using System.Diagnostics;
using System.Globalization;

namespace RimLangKit
{
    public partial class MainForm : Form
    {
        private static string DirectoryPath = string.Empty;
        bool FindChangesFirst = true;
        string TranslationPath = string.Empty;
        string ModPath = string.Empty;

        public MainForm()
        {
            InitializeComponent();
        }

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
            if (Directory.Exists(FolderTextBox.Text))
            {
                LabelCheck.ForeColor = Color.Green;
                LabelCheck.Text = "ОК";
                ButtonENGIsert.Enabled = true;
                ButtonUniqueNames.Enabled = true;
                ButtonDictionary.Enabled = true;
                ButtonCase.Enabled = true;
                ButtonEncoding.Enabled = true;
                ButtonTagCollector.Enabled = true;
                ButtonFileFix.Enabled = true;
                ButtonFindChanges.Enabled = true;
            }
            else
            {
                LabelCheck.ForeColor = Color.Red;
                LabelCheck.Text = "Ошибка: папка не найдена";
                ButtonENGIsert.Enabled = false;
                ButtonUniqueNames.Enabled = false;
                ButtonDictionary.Enabled = false;
                ButtonCase.Enabled = false;
                ButtonEncoding.Enabled = false;
                ButtonTagCollector.Enabled = false;
                ButtonFileFix.Enabled = false;
                ButtonFindChanges.Enabled = false;
            }
        }

        private void ButtonEncoding_Click(object sender, EventArgs e)
        {
            ActionHandler("Исправление кодировки", 1);
        }
        private void ButtonENGIsert_Click(object sender, EventArgs e)
        {
            ActionHandler("Добавление комментариев", 2);
        }
        private void ButtonUniqueNames_Click(object sender, EventArgs e)
        {
            ActionHandler("Переименование файлов", 3);
        }
        private void ButtonDictionary_Click(object sender, EventArgs e)
        {
            ActionHandler("Транскрипция имён", 4);
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
        private void ButtonTagCollector_Click(object sender, EventArgs e)
        {
            ActionHandler("Сбор статистики тегов", 5);
        }
        private void ButtonFileFix_Click(object sender, EventArgs e)
        {
            ActionHandler("Поиск сломанных файлов", 6);
        }

        private void ActionHandler(string name, int code)
        {
            InfoTextBox.Text = $"{PlaceTime()}{name}";
            bool mode = false;
            if (code == 4)
            {
                DialogResult answer = MessageBox.Show("Нажмите «Да» для транкрипции с английского на русский и «Нет» для обратной.", "Выбор языка", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                mode = answer == DialogResult.Yes;
            }
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
                    case 3:
                        result = UniqueNames.UniqueNamesProcessing(tempFile, InfoTextBox);
                        break;
                    case 4:
                        result = TextTranslit.TextTranslitProcessing(tempFile, mode, InfoTextBox);
                        break;
                    case 5:
                        string def = TagCollector.CheckDef(tempFile);
                        result = TagCollector.TagSearch(tempFile, def, InfoTextBox);
                        break;
                    case 6:
                        result = FileFix.FileFixProcessing(tempFile);
                        break;
                    default:
                        break;
                }
                if (result) count++;
                else errCount++;
            }
            if (code == 5)
            {
                TagCollector.WriteTags(InfoTextBox);
            }

            InfoTextBox.AppendText($"{Environment.NewLine}{PlaceTime()}Успешно завершено. Обработано файлов - {count}");
            if (errCount != 0)
                InfoTextBox.AppendText($"{Environment.NewLine}Пропущено файлов - {errCount}");

            if (code == 6)
            {
                FileFix.WriteBrokenFiles(InfoTextBox);
            }
        }

        public static string PlaceTime()
        {
            DateTime time = DateTime.Now;
            string result = time.ToString("HH:mm:ss", CultureInfo.InvariantCulture) + " - ";
            return result;
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            Root? json = HttpClient.GetGithubJson();
            if (json != null)
            {
                string newVersion = json.tag_name[1..];
                UpdateLabel.Text = $"Последняя {newVersion}";
                string oldVersion = VersionLabel.Text[7..];
                if (oldVersion == newVersion)
                {
                    UpdateButton.BackgroundImage = Properties.Resources.yes_c;
                }
                else
                {
                    UpdateButton.BackgroundImage = Properties.Resources.warn_c;
                    LinkLabelGithub.Visible = true;
                }

            }
            else
            {
                UpdateButton.BackgroundImage = Properties.Resources.no_c;
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

        private void ButtonFindChanges_Click(object sender, EventArgs e)
        {
            if (FindChangesFirst)
            {
                TranslationPath = DirectoryPath;
                InfoTextBox.AppendText($"{Environment.NewLine}Файлы перевода - {TranslationPath}.{Environment.NewLine}Выберите пусть к файлам мода.");
                DirectoryPath = string.Empty;
                FolderTextBox.Text = string.Empty;
                FindChangesFirst = false;
            }
            else
            {
                ModPath = DirectoryPath;
                InfoTextBox.AppendText($"{Environment.NewLine}Файлы перевода - {TranslationPath}.{Environment.NewLine}Файлы мода - {ModPath}");
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
    }
}