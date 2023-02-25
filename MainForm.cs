namespace RTK
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
            if (Directory.Exists(FolderTextBox.Text))
            {
                LabelCheck.ForeColor = Color.Green;
                LabelCheck.Text = "��";
                ButtonENGIsert.Enabled = true;
                ButtonUniqueNames.Enabled = true;
                ButtonDictionary.Enabled = true;
                ButtonLanguageUpdate.Enabled = true;
            }
            else
            {
                LabelCheck.ForeColor = Color.Red;
                LabelCheck.Text = "������: ����� �� �������";
                ButtonENGIsert.Enabled = false;
                ButtonUniqueNames.Enabled = false;
                ButtonDictionary.Enabled = false;
                ButtonLanguageUpdate.Enabled = false;
            }
        }

        private void ButtonENGIsert_Click(object sender, EventArgs e)
        {
            InfoTextBox.Text = string.Empty;
            InfoTextBox.AppendText("�����");
            (int, int) Count = (0, 0);
            (int, int) tempCount;
            //��������� ������ ���� ������ � ������� ���������� � �� ���� ��������� ��������� �� ���� SearchOption
            string[] AllFiles = Directory.GetFiles(DirectoryPath, "*.xml", SearchOption.AllDirectories);

            foreach (string TempFile in AllFiles)
            {
                tempCount = ResultProcessing(CommentInsert.CommentsInsertProcessing(TempFile));
                Count.Item1 += tempCount.Item1;
                Count.Item2 += tempCount.Item2;
            }
            InfoTextBox.AppendText(Environment.NewLine + "����������: " + Count.Item1 + ". ���������: " + Count.Item2);
        }

        private void ButtonUniqueNames_Click(object sender, EventArgs e)
        {
            InfoTextBox.Text = string.Empty;
            InfoTextBox.AppendText("�����");
            (int, int) Count = (0, 0);
            (int, int) tempCount;
            //��������� ������ ���� ������ � ������� ���������� � �� ���� ��������� ��������� �� ���� SearchOption
            string[] AllFiles = Directory.GetFiles(DirectoryPath, "*.xml", SearchOption.AllDirectories);

            foreach (string TempFile in AllFiles)
            {
                tempCount = ResultProcessing(UniqueNames.UniqueNamesProcessing(TempFile));
                Count.Item1 += tempCount.Item1;
                Count.Item2 += tempCount.Item2;
            }
            InfoTextBox.AppendText(Environment.NewLine + "����������: " + Count.Item1 + ". ���������: " + Count.Item2);
        }

        private void ButtonDictionary_Click(object sender, EventArgs e)
        {

            DialogResult answer = MessageBox.Show("������� ��� ��� ����������� � ����������� �� ������� � ���� ��� ��������.", "����� �����", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            bool mode = answer == DialogResult.Yes;
            InfoTextBox.Text = string.Empty;
            InfoTextBox.AppendText("�����");
            (int, int) Count = (0, 0);
            (int, int) tempCount;
            //��������� ������ ���� ������ � ������� ���������� � �� ���� ��������� ��������� �� ���� SearchOption
            string[] AllFiles = Directory.GetFiles(DirectoryPath, "*.txt", SearchOption.AllDirectories);

            foreach (string TempFile in AllFiles)
            {
                tempCount = ResultProcessing(TextTranslit.TextTranslitProcessing(TempFile, mode));
                Count.Item1 += tempCount.Item1;
                Count.Item2 += tempCount.Item2;
            }
            InfoTextBox.AppendText(Environment.NewLine + "����������: " + Count.Item1 + ". ���������: " + Count.Item2);
        }

        private void ButtonLanguageUpdate_Click(object sender, EventArgs e)
        {
            InfoTextBox.Text = string.Empty;
            InfoTextBox.AppendText("�����");

            (bool, string) result;
            result = LanguageUpdate.LanguageUpdateProcessing(DirectoryPath);
            if (result.Item1)
            {
                InfoTextBox.AppendText(Environment.NewLine + "������� ���������!");
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
    }
}