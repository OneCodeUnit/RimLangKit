using ICSharpCode.SharpZipLib.Zip;
using System.Globalization;
using TranslationKitLib;

namespace LanguageUpdate
{
    public partial class MainForm : Form
    {
        private static string? DirectoryPath;

        public MainForm()
        {
            // Проверка сохранённого пути
            InitializeComponent();
            if (Directory.Exists(Settings.Default.savedDirectory))
            {
                DirectoryPath = Settings.Default.savedDirectory;
            }
            else
            {
                DirectoryPath = string.Empty;
            }
            FolderTextBox.Text = DirectoryPath;

            LanguageInput.Text = Settings.Default.language;
            RepoInput.Text = Settings.Default.repo;
        }

        // Диалог выбора папки
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

        // Проверка папки
        private void FolderTextBox_TextChanged(object sender, EventArgs e)
        {
            string tempPath = FolderTextBox.Text;
            string path = $"{tempPath}\\Version.txt";
            if (File.Exists(path))
            {
                ButtonLanguageUpdate.Enabled = true;
                DirectoryPath = tempPath;
                Settings.Default["savedDirectory"] = tempPath;
                Settings.Default.Save();
                FolderButton.BackColor = Color.FromArgb(154, 185, 115);
            }
            else
            {
                ButtonLanguageUpdate.Enabled = false;
                FolderButton.BackColor = Color.FromArgb(205, 92, 92);
            }
        }

        private void ButtonLanguageUpdate_Click(object sender, EventArgs e)
        {
            InfoTextBox.AppendText($"{Environment.NewLine}{Messages.PlaceTime()}Запуск");

            string sha = Settings.Default.sha;
            string repo = Settings.Default.repo;

            // Проверка обновлений
            List<Root>? json = RimHttpClient.GetGithubJson(repo);
            string currentSha = json[0].sha;

            InfoTextBox.AppendText($"{Environment.NewLine}{Messages.PlaceTime()}Ваша версия - {sha[..6]}, доступная версия - {currentSha[..6]}");

            if (currentSha == sha)
            {
                InfoTextBox.AppendText($"{Environment.NewLine}{Messages.PlaceTime()}Обновление не требуется");
                return;
            }

            Stream stream = RimHttpClient.GetGithubArchive(repo);
            InfoTextBox.AppendText($"{Environment.NewLine}{Messages.PlaceTime()}Обновление загружено");
            FastZip archive = new();
            string tempDir = "temp";
            archive.ExtractZip(stream, tempDir, FastZip.Overwrite.Always, null, null, null, true, true, true);
            InfoTextBox.AppendText($"{Environment.NewLine}{Messages.PlaceTime()}Обновление извлечено");
            stream.Close();

            string[] baseDir = Directory.GetDirectories(tempDir);
            string[] dir = Directory.GetDirectories(baseDir[0]);
            StringComparison comparison = StringComparison.OrdinalIgnoreCase;
            foreach (string dirEntry in dir)
            {
                if (dirEntry.EndsWith("Biotech", comparison))
                {
                    FolderUpdate("Biotech", dirEntry);
                    InfoTextBox.AppendText($"{Environment.NewLine}{Messages.PlaceTime()}Biotech обновлено");
                }
                else if (dirEntry.EndsWith("Core", comparison))
                {
                    FolderUpdate("Core", dirEntry);
                    InfoTextBox.AppendText($"{Environment.NewLine}{Messages.PlaceTime()}Игра обновлена");
                }
                else if (dirEntry.EndsWith("Ideology", comparison))
                {
                    FolderUpdate("Ideology", dirEntry);
                    InfoTextBox.AppendText($"{Environment.NewLine}{Messages.PlaceTime()}Ideology обновлено");
                }
                else if (dirEntry.EndsWith("Royalty", comparison))
                {
                    FolderUpdate("Royalty", dirEntry);
                    InfoTextBox.AppendText($"{Environment.NewLine}{Messages.PlaceTime()}Royalty обновлено");
                }
            }
            Directory.Delete(tempDir, true);
            InfoTextBox.AppendText($"{Environment.NewLine}{Messages.PlaceTime()}Успешно обновлено!");

            Settings.Default["sha"] = currentSha;
            Settings.Default.Save();
        }

        private static void FolderUpdate(string type, string tempPath)
        {
            string language = Settings.Default.language;
            string tempDir = $"{DirectoryPath}\\Data\\{type}\\Languages\\{language}";
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
            Directory.Move(tempPath, tempDir);
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

        private void DefaultButton_Click(object sender, EventArgs e)
        {
            Settings.Default["sha"] = "00000000";
            Settings.Default["repo"] = "Ludeon/RimWorld-ru";
            Settings.Default["language"] = "Russian (GitHub)";
            Settings.Default["savedDirectory"] = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\RimWorld";
            Settings.Default.Save();

            if (Directory.Exists("C:\\Program Files (x86)\\Steam\\steamapps\\common\\RimWorld"))
            {
                DirectoryPath = Settings.Default.savedDirectory;
            }
            else
            {
                DirectoryPath = string.Empty;
            }
            RepoInput.Text = Settings.Default.repo;
            LanguageInput.Text = Settings.Default.language;
            FolderTextBox.Text = DirectoryPath;
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            string language = Settings.Default.language;
            string biotechFolder = $"{DirectoryPath}\\Data\\Biotech\\Languages\\{language}";
            string coreFolder = $"{DirectoryPath}\\Data\\Core\\Languages\\{language}";
            string ideologyFolder = $"{DirectoryPath}\\Data\\Ideology\\Languages\\{language}";
            string royaltyFolder = $"{DirectoryPath}\\Data\\Royalty\\Languages\\{language}";
            DialogResult res = MessageBox.Show($"Будут удалены следующие папки:{Environment.NewLine}{biotechFolder}{Environment.NewLine}{coreFolder}{Environment.NewLine}{ideologyFolder}{Environment.NewLine}{royaltyFolder}", "Подтверждение", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (res == DialogResult.OK)
            {
                Settings.Default["sha"] = "00000000";
                Directory.Delete(biotechFolder, true);
                Directory.Delete(coreFolder, true);
                Directory.Delete(ideologyFolder, true);
                Directory.Delete(royaltyFolder, true);
            }
        }
    }
}