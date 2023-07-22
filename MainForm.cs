using System.IO;
using System.Windows.Forms;
using TranslationKitLib;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace RimLangKit
{
    public partial class MainForm : Form
    {
        private static string DirectoryPath = string.Empty;
        private static readonly string[] DefTypeList = { "AbilityDef", "BodyDef", "BodyPartDef", "BodyPartGroupDef", "FactionDef", "HediffDef", "MemeDef", "OrderedTakeGroupDef", "PawnCapacityDef", "PawnKindDef", "SitePartDef", "StyleCategoryDef", "ThingDef", "ToolCapacityDef", "WorldObjectDef", "SkillDef" };
        private static bool IsStarted = false;

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
            InfoTextBox.Text = string.Empty;
            InfoTextBox.AppendText("Старт");
            (int, int) count = (0, 0);
            (int, int) tempCount;
            //Получение списка всех файлов в заданой директории и во всех вложенных подпапках за счёт SearchOption
            string[] allFiles = Directory.GetFiles(DirectoryPath, "*.xml", SearchOption.AllDirectories);

            foreach (string tempFile in allFiles)
            {
                tempCount = ResultProcessing(CommentInsert.CommentsInsertProcessing(tempFile));
                count.Item1 += tempCount.Item1;
                count.Item2 += tempCount.Item2;
            }
            InfoTextBox.AppendText(Environment.NewLine + "Обработано: " + count.Item1 + ". Пропущено: " + count.Item2);
        }

        private void ButtonUniqueNames_Click(object sender, EventArgs e)
        {
            InfoTextBox.Text = string.Empty;
            InfoTextBox.AppendText("Старт");
            (int, int) count = (0, 0);
            (int, int) tempCount;
            //Получение списка всех файлов в заданой директории и во всех вложенных подпапках за счёт SearchOption
            string[] allFiles = Directory.GetFiles(DirectoryPath, "*.xml", SearchOption.AllDirectories);

            foreach (string tempFile in allFiles)
            {
                tempCount = ResultProcessing(UniqueNames.UniqueNamesProcessing(tempFile));
                count.Item1 += tempCount.Item1;
                count.Item2 += tempCount.Item2;
            }
            InfoTextBox.AppendText(Environment.NewLine + "Обработано: " + count.Item1 + ". Пропущено: " + count.Item2);
        }

        private void ButtonDictionary_Click(object sender, EventArgs e)
        {

            DialogResult answer = MessageBox.Show("Нажмите «Да» для транкрипции с английского на русский и «Нет» для обратной.", "Выбор языка", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            bool mode = answer == DialogResult.Yes;
            InfoTextBox.Text = string.Empty;
            InfoTextBox.AppendText("Старт");
            (int, int) count = (0, 0);
            (int, int) tempCount;
            //Получение списка всех файлов в заданой директории и во всех вложенных подпапках за счёт SearchOption
            string[] allFiles = Directory.GetFiles(DirectoryPath, "*.txt", SearchOption.AllDirectories);

            foreach (string tempFile in allFiles)
            {
                tempCount = ResultProcessing(TextTranslit.TextTranslitProcessing(tempFile, mode));
                count.Item1 += tempCount.Item1;
                count.Item2 += tempCount.Item2;
            }
            InfoTextBox.AppendText(Environment.NewLine + "Обработано: " + count.Item1 + ". Пропущено: " + count.Item2);
        }

        // Обработка ошибок. Позволяется выводить полученные от функции сообщения
        private (int, int) ResultProcessing((bool, string) result)
        {
            int processedCount = 0;
            int skipCount = 0;
            if (result.Item1)
            {
                processedCount++;
            }
            else
            {
                skipCount++;
                InfoTextBox.AppendText(Environment.NewLine);
                InfoTextBox.AppendText(result.Item2);
            }
            return (processedCount, skipCount);
        }

        private void ButtonCase_Click(object sender, EventArgs e)
        {
            InfoTextBox.Text = string.Empty;
            InfoTextBox.AppendText("Старт");
            //Получение списка всех файлов в заданой директории и во всех вложенных подпапках за счёт SearchOption
            string[] allFiles = Directory.GetFiles(DirectoryPath, "*.xml", SearchOption.AllDirectories);
            List<string> words = new();

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

            int globalCount = 0;
            // Проверяется каждый подходящий DefType из списка
            foreach (string defType in DefTypeList)
            {
                int count = 0;
                foreach (string tempFile in allFiles)
                {
                    // Каждый файл проверяется на соотвествие этому типу. Если не соотвествует, возвращает пустой список
                    List<string> tempWords = CaseCreate.FindWordsProcessing(tempFile, defType);
                    words.AddRange(tempWords);
                    count += tempWords.Count;
                }
                // Если нашлись слова в данном DefType, то создаются файлы
                if (count > 0)
                {
                    globalCount++;
                    InfoTextBox.AppendText(Environment.NewLine + $"Обработано {count} объектов типа {defType}");
                    CaseCreate.CreateCase(directory, words, defType);
                    CaseCreate.CreateGender(directory, words, defType);
                }
            }
            if (globalCount > 0)
            {
                InfoTextBox.AppendText(Environment.NewLine + $"Созданы файлы по адресу {directory}\\Languages\\Russian\\WordInfo");
            }
            else
            {
                InfoTextBox.AppendText(Environment.NewLine + "Ничего не создано");
            }
            InfoTextBox.AppendText(Environment.NewLine + "Завершено");
        }

        private void ButtonEncoding_Click(object sender, EventArgs e)
        {
            InfoTextBox.Text = $"{Messages.PlaceTime()}Исправление кодировки";
            string[] allFiles = Directory.GetFiles(DirectoryPath, "*.xml", SearchOption.AllDirectories);
            int count = 0;
            foreach (string tempFile in allFiles)
            {
                EncodingFix.EncodingFixProcessing(tempFile, InfoTextBox);
                count++;
            }
            InfoTextBox.AppendText($"{Environment.NewLine}{Messages.PlaceTime()}Успешно завершено. Обработано файлов - {count}");
        }
    }
}