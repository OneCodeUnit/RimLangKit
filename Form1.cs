namespace RTK
{
    public partial class MainForm : Form
    {
        private string DirectoryPath = string.Empty;

        public MainForm()
        {
            InitializeComponent();
        }

        private void FolderButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog ofd = new();
            if (Directory.Exists("C:\\Program Files(x86)\\Steam\\steamapps\\common\\RimWorld"))
            {
                ofd.SelectedPath = "C:\\Program Files(x86)\\Steam\\steamapps\\common\\RimWorld";
            }
            if (Directory.Exists(FolderTextBox.Text))
            {
                ofd.SelectedPath = FolderTextBox.Text;
            }
            DialogResult dr = ofd.ShowDialog();

            if (dr == DialogResult.OK)
            {
                DirectoryPath = ofd.SelectedPath;
                FolderTextBox.Text = DirectoryPath;
            }
        }

        private void FolderTextBox_TextChanged(object sender, EventArgs e)
        {
            ChangeSize(FolderTextBox);
            DirectoryPath = FolderTextBox.Text;
            if (Directory.Exists(FolderTextBox.Text))
            {
                LabelCheck.ForeColor = Color.Green;
                LabelCheck.Text = "ОК";
                ButtonENGIsert.Enabled = true;
                ButtonUniqueNames.Enabled = true;
                ButtonDictionary.Enabled = true;
                ButtonLanguageUpdate.Enabled = true;
            }
            else
            {
                LabelCheck.ForeColor = Color.Red;
                LabelCheck.Text = "Ошибка: папка не найдена";
                ButtonENGIsert.Enabled = false;
                ButtonUniqueNames.Enabled = false;
                ButtonDictionary.Enabled = false;
                ButtonLanguageUpdate.Enabled = false;
            }
        }

        private void LabelCheck_TextChanged(object sender, EventArgs e)
        {
            ChangeSize(LabelCheck);
        }

        private void InfoTextBox_TextChanged(object sender, EventArgs e)
        {
            ChangeSize(InfoTextBox);
        }

        private void ButtonENGIsert_Click(object sender, EventArgs e)
        {
            XMLProcessing("Comments");
        }

        private void ButtonUniqueNames_Click(object sender, EventArgs e)
        {
            XMLProcessing("UniqueNames");
        }

        private void ButtonDictionary_Click(object sender, EventArgs e)
        {

            DialogResult answer = MessageBox.Show("Нажмите «Да» для транкрипции с английского на русский и «Нет» для обратной.", "Выбор языка", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            bool mode = answer == DialogResult.Yes;
            TXTProcessing(mode);
        }

        private void ButtonLanguageUpdate_Click(object sender, EventArgs e)
        {
            LanguageProcessing();
        }
    }
}