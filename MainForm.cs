using System.Globalization;

namespace RimLangKit
{
    public partial class MainForm : Form
    {
        private static string DirectoryPath = string.Empty;

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
            }
        }

        private void ButtonENGIsert_Click(object sender, EventArgs e)
        {
            InfoTextBox.Text = $"{PlaceTime()}Добавление комментариев";
            //Получение списка всех файлов в заданой директории и во всех вложенных подпапках за счёт SearchOption
            string[] allFiles = Directory.GetFiles(DirectoryPath, "*.xml", SearchOption.AllDirectories);
            int count = 0;
            int errCount = 0;
            bool result;
            foreach (string tempFile in allFiles)
            {
                result = CommentInsert.CommentsInsertProcessing(tempFile, InfoTextBox);
                if (result) count++;
                else errCount++;
            }
            InfoTextBox.AppendText($"{Environment.NewLine}{PlaceTime()}Успешно завершено. Обработано файлов - {count}, пропущено - {errCount}");
        }

        private void ButtonUniqueNames_Click(object sender, EventArgs e)
        {
            InfoTextBox.Text = $"{PlaceTime()}Переименование файлов";
            //Получение списка всех файлов в заданой директории и во всех вложенных подпапках за счёт SearchOption
            string[] allFiles = Directory.GetFiles(DirectoryPath, "*.xml", SearchOption.AllDirectories);
            int count = 0;
            int errCount = 0;
            bool result;
            foreach (string tempFile in allFiles)
            {
                result = UniqueNames.UniqueNamesProcessing(tempFile, InfoTextBox);
                if (result) count++;
                else errCount++;
            }
            InfoTextBox.AppendText($"{Environment.NewLine}{PlaceTime()}Успешно завершено. Обработано файлов - {count}, пропущено - {errCount}");
        }

        private void ButtonDictionary_Click(object sender, EventArgs e)
        {

            DialogResult answer = MessageBox.Show("Нажмите «Да» для транкрипции с английского на русский и «Нет» для обратной.", "Выбор языка", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            bool mode = answer == DialogResult.Yes;
            InfoTextBox.Text = $"{PlaceTime()}Транскрипция файлов";
            int count = 0;
            int errCount = 0;
            bool result;
            //Получение списка всех файлов в заданой директории и во всех вложенных подпапках за счёт SearchOption
            string[] allFiles = Directory.GetFiles(DirectoryPath, "*.txt", SearchOption.AllDirectories);

            foreach (string tempFile in allFiles)
            {
                result = TextTranslit.TextTranslitProcessing(tempFile, mode, InfoTextBox);
                if (result) count++;
                else errCount++;
            }
            InfoTextBox.AppendText($"{Environment.NewLine}{PlaceTime()}Успешно завершено. Обработано файлов - {count}, пропущено - {errCount}");
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

        private void ButtonEncoding_Click(object sender, EventArgs e)
        {
            InfoTextBox.Text = $"{PlaceTime()}Исправление кодировки";
            string[] allFiles = Directory.GetFiles(DirectoryPath, "*.xml", SearchOption.AllDirectories);
            int count = 0;
            foreach (string tempFile in allFiles)
            {
                EncodingFix.EncodingFixProcessing(tempFile, InfoTextBox);
                count++;
            }
            InfoTextBox.AppendText($"{Environment.NewLine}{PlaceTime()}Успешно завершено. Обработано файлов - {count}");
        }

        public static string PlaceTime()
        {
            DateTime time = DateTime.Now;
            string result = time.ToString("HH:mm:ss", CultureInfo.InvariantCulture) + " - ";
            return result;
        }
    }
}