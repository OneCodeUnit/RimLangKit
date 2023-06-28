using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using TranslationKitLib;

namespace LanguageUpdateLegacy
{
    public partial class MainForm : Form
    {
        private static string DirectoryPath;

        public MainForm()
        {
            // Проверка сохранённого пути
            InitializeComponent();
            if (Directory.Exists(Properties.Settings.Default.savedDirectory))
            {
                DirectoryPath = Properties.Settings.Default.savedDirectory;
            }
            else
            {
                DirectoryPath = string.Empty;
            }
            FolderTextBox.Text = DirectoryPath;

            LanguageInput.Text = Properties.Settings.Default.language;
            RepoInput.Text = Properties.Settings.Default.repo;
        }

        // Диалог выбора папки
        private void FolderButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog ofd = new FolderBrowserDialog();
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
                Properties.Settings.Default["savedDirectory"] = tempPath;
                Properties.Settings.Default.Save();
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
            InfoTextBox.AppendText($"{Environment.NewLine}{PlaceTime()}Запуск");

            string sha = Properties.Settings.Default.sha;
            string repo = Properties.Settings.Default.repo;

            // Проверка обновлений
            List<Root> json = RimHttpClient.GetGithubJson(repo);
            string currentSha = json[0].sha;

            InfoTextBox.AppendText($"{Environment.NewLine}{PlaceTime()}Ваша версия - {sha.Substring(0, 6)}, доступная версия - {currentSha.Substring(0, 6)}");

            if (currentSha == sha)
            {
                InfoTextBox.AppendText($"{Environment.NewLine}{PlaceTime()}Обновление не требуется");
                return;
            }

            Stream stream = RimHttpClient.GetGithubArchive(repo);
            InfoTextBox.AppendText($"{Environment.NewLine}{PlaceTime()}Обновление загружено");
            FastZip archive = new FastZip();
            string tempDir = "temp";
            archive.ExtractZip(stream, tempDir, FastZip.Overwrite.Always, null, null, null, true, true, true);
            InfoTextBox.AppendText($"{Environment.NewLine}{PlaceTime()}Обновление извлечено");
            stream.Close();

            string[] baseDir = Directory.GetDirectories(tempDir);
            string[] dir = Directory.GetDirectories(baseDir[0]);
            StringComparison comparison = StringComparison.OrdinalIgnoreCase;
            foreach (string dirEntry in dir)
            {
                if (dirEntry.EndsWith("Biotech", comparison))
                {
                    FolderUpdate("Biotech", dirEntry);
                    InfoTextBox.AppendText($"{Environment.NewLine}{PlaceTime()}Biotech обновлено");
                }
                else if (dirEntry.EndsWith("Core", comparison))
                {
                    FolderUpdate("Core", dirEntry);
                    InfoTextBox.AppendText($"{Environment.NewLine}{PlaceTime()}Игра обновлена");
                }
                else if (dirEntry.EndsWith("Ideology", comparison))
                {
                    FolderUpdate("Ideology", dirEntry);
                    InfoTextBox.AppendText($"{Environment.NewLine}{PlaceTime()}Ideology обновлено");
                }
                else if (dirEntry.EndsWith("Royalty", comparison))
                {
                    FolderUpdate("Royalty", dirEntry);
                    InfoTextBox.AppendText($"{Environment.NewLine}{PlaceTime()}Royalty обновлено");
                }
            }
            Directory.Delete(tempDir, true);
            InfoTextBox.AppendText($"{Environment.NewLine}{PlaceTime()}Успешно обновлено!");

            Properties.Settings.Default["sha"] = currentSha;
            Properties.Settings.Default.Save();
        }

        private static string PlaceTime()
        {
            DateTime time = DateTime.Now;
            string result = time.ToString("HH:mm:ss", CultureInfo.InvariantCulture) + " - ";
            return result;
        }

        private static void FolderUpdate(string type, string tempPath)
        {
            string language = Properties.Settings.Default.language;
            string tempDir = $"{DirectoryPath}\\Data\\{type}\\Languages\\{language}";
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
            Directory.Move(tempPath, tempDir);
        }

        private void RepoInput_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default["repo"] = "Ludeon/RimWorld-ru";
            Properties.Settings.Default.Save();
        }

        private void LanguageInput_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default["language"] = "Russian (GitHub)";
            Properties.Settings.Default.Save();
        }

        private void DefaultButton_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default["sha"] = "00000000";
            Properties.Settings.Default["repo"] = "Ludeon/RimWorld-ru";
            Properties.Settings.Default["language"] = "Russian (GitHub)";
            Properties.Settings.Default["savedDirectory"] = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\RimWorld";
            Properties.Settings.Default.Save();

            if (Directory.Exists("C:\\Program Files (x86)\\Steam\\steamapps\\common\\RimWorld"))
            {
                DirectoryPath = Properties.Settings.Default.savedDirectory;
            }
            else
            {
                DirectoryPath = string.Empty;
            }
            RepoInput.Text = Properties.Settings.Default.repo;
            LanguageInput.Text = Properties.Settings.Default.language;
            FolderTextBox.Text = DirectoryPath;
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            string language = Properties.Settings.Default.language;
            string biotechFolder = $"{DirectoryPath}\\Data\\Biotech\\Languages\\{language}";
            string coreFolder = $"{DirectoryPath}\\Data\\Core\\Languages\\{language}";
            string ideologyFolder = $"{DirectoryPath}\\Data\\Ideology\\Languages\\{language}";
            string royaltyFolder = $"{DirectoryPath}\\Data\\Royalty\\Languages\\{language}";
            DialogResult res = MessageBox.Show($"Будут удалены следующие папки:{Environment.NewLine}{biotechFolder}{Environment.NewLine}{coreFolder}{Environment.NewLine}{ideologyFolder}{Environment.NewLine}{royaltyFolder}", "Подтверждение", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (res == DialogResult.OK)
            {
                Properties.Settings.Default["sha"] = "00000000";
                Directory.Delete(biotechFolder, true);
                Directory.Delete(coreFolder, true);
                Directory.Delete(ideologyFolder, true);
                Directory.Delete(royaltyFolder, true);
            }
        }
    }
}
