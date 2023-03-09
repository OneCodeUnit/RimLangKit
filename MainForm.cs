namespace RimLangKit
{
    public partial class MainForm : Form
    {
        private static string DirectoryPath = string.Empty;
        private static readonly string[] DefTypeList = { "AbilityDef", "BodyDef", "BodyPartDef", "BodyPartGroupDef", "FactionDef", "HediffDef", "MemeDef", "OrderedTakeGroupDef", "PawnCapacityDef", "PawnKindDef", "SitePartDef", "StyleCategoryDef", "ThingDef", "ToolCapacityDef", "WorldObjectDef", "SkillDef" };

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
            if (Directory.Exists(FolderTextBox.Text))
            {
                LabelCheck.ForeColor = Color.Green;
                LabelCheck.Text = "ОК";
                ButtonENGIsert.Enabled = true;
                ButtonUniqueNames.Enabled = true;
                ButtonDictionary.Enabled = true;
                ButtonLanguageUpdate.Enabled = true;
                ButtonCase.Enabled = true;
            }
            else
            {
                LabelCheck.ForeColor = Color.Red;
                LabelCheck.Text = "Ошибка: папка не найдена";
                ButtonENGIsert.Enabled = false;
                ButtonUniqueNames.Enabled = false;
                ButtonDictionary.Enabled = false;
                ButtonLanguageUpdate.Enabled = false;
                ButtonCase.Enabled = false;
            }
        }

        private void ButtonENGIsert_Click(object sender, EventArgs e)
        {
            InfoTextBox.Text = string.Empty;
            InfoTextBox.AppendText("Старт");
            (int, int) Count = (0, 0);
            (int, int) tempCount;
            //Получение списка всех файлов в заданой директории и во всех вложенных подпапках за счёт SearchOption
            string[] AllFiles = Directory.GetFiles(DirectoryPath, "*.xml", SearchOption.AllDirectories);

            foreach (string TempFile in AllFiles)
            {
                tempCount = ResultProcessing(CommentInsert.CommentsInsertProcessing(TempFile));
                Count.Item1 += tempCount.Item1;
                Count.Item2 += tempCount.Item2;
            }
            InfoTextBox.AppendText(Environment.NewLine + "Обработано: " + Count.Item1 + ". Пропущено: " + Count.Item2);
        }

        private void ButtonUniqueNames_Click(object sender, EventArgs e)
        {
            InfoTextBox.Text = string.Empty;
            InfoTextBox.AppendText("Старт");
            (int, int) Count = (0, 0);
            (int, int) tempCount;
            //Получение списка всех файлов в заданой директории и во всех вложенных подпапках за счёт SearchOption
            string[] AllFiles = Directory.GetFiles(DirectoryPath, "*.xml", SearchOption.AllDirectories);

            foreach (string TempFile in AllFiles)
            {
                tempCount = ResultProcessing(UniqueNames.UniqueNamesProcessing(TempFile));
                Count.Item1 += tempCount.Item1;
                Count.Item2 += tempCount.Item2;
            }
            InfoTextBox.AppendText(Environment.NewLine + "Обработано: " + Count.Item1 + ". Пропущено: " + Count.Item2);
        }

        private void ButtonDictionary_Click(object sender, EventArgs e)
        {

            DialogResult answer = MessageBox.Show("Нажмите «Да» для транкрипции с английского на русский и «Нет» для обратной.", "Выбор языка", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            bool mode = answer == DialogResult.Yes;
            InfoTextBox.Text = string.Empty;
            InfoTextBox.AppendText("Старт");
            (int, int) Count = (0, 0);
            (int, int) tempCount;
            //Получение списка всех файлов в заданой директории и во всех вложенных подпапках за счёт SearchOption
            string[] AllFiles = Directory.GetFiles(DirectoryPath, "*.txt", SearchOption.AllDirectories);

            foreach (string TempFile in AllFiles)
            {
                tempCount = ResultProcessing(TextTranslit.TextTranslitProcessing(TempFile, mode));
                Count.Item1 += tempCount.Item1;
                Count.Item2 += tempCount.Item2;
            }
            InfoTextBox.AppendText(Environment.NewLine + "Обработано: " + Count.Item1 + ". Пропущено: " + Count.Item2);
        }

        private void ButtonLanguageUpdate_Click(object sender, EventArgs e)
        {
            InfoTextBox.Text = string.Empty;
            InfoTextBox.AppendText("Старт");

            (bool, string) result;
            result = LanguageUpdate.LanguageUpdateProcessing(DirectoryPath);
            if (result.Item1)
            {
                InfoTextBox.AppendText(Environment.NewLine + "Успешно обновлено!");
            }
            else
            {
                InfoTextBox.AppendText(Environment.NewLine + result.Item2);
            }
        }

        private (int, int) ResultProcessing((bool, string) result)
        {
            int ProcessedCount = 0;
            int SkipCount = 0;
            if (result.Item1)
            {
                ProcessedCount++;
            }
            else
            {
                SkipCount++;
                InfoTextBox.AppendText(Environment.NewLine);
                InfoTextBox.AppendText(result.Item2);
            }
            return (ProcessedCount, SkipCount);
        }

        private void ButtonCase_Click(object sender, EventArgs e)
        {
            InfoTextBox.Text = string.Empty;
            InfoTextBox.AppendText("Старт");
            int Count = 0;
            string[] AllFiles = Directory.GetFiles(DirectoryPath, "*.xml", SearchOption.AllDirectories);
            List<string> words = new();
            foreach (string DefType in DefTypeList)
            {
                foreach (string TempFile in AllFiles)
                {
                    List<string> tempWords = CaseCreate.FindWordsProcessing(TempFile, DefType);
                    words.AddRange(tempWords);
                    Count += tempWords.Count;
                }
                if (Count > 0)
                {
                    InfoTextBox.AppendText(Environment.NewLine + $"Обработано {Count} объектов типа {DefType}");
                    CaseCreate.CreateCase(DirectoryPath, words, DefType);
                    CaseCreate.CreateGender(DirectoryPath, words, DefType);
                }
                Count = 0;
            }
            InfoTextBox.AppendText(Environment.NewLine + $"Созданы файлы по адресу {DirectoryPath}\\Languages\\Russian\\WordInfo");
        }
    }
}