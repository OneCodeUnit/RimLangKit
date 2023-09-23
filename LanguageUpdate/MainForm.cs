using System.IO.Compression;

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
            if (json is null)
            {
                InfoTextBox.AppendText($"{Environment.NewLine}{Messages.PlaceTime()}Ошибка получения данных с GitHub");
                return;
            }
            string currentSha = json[0].sha;

            InfoTextBox.AppendText($"{Environment.NewLine}{Messages.PlaceTime()}Ваша версия - {sha[..6]}, доступная версия - {currentSha[..6]}");

            if (currentSha == sha)
            {
                InfoTextBox.AppendText($"{Environment.NewLine}{Messages.PlaceTime()}Обновление не требуется");
                return;
            }
            // Получения списка дополнений
            string[] modules = Directory.GetDirectories($"{DirectoryPath}\\Data");
            if (modules.Length == 0) 
            {
                InfoTextBox.AppendText($"{Environment.NewLine}{Messages.PlaceTime()}В указанной папке не обнаружены модули");
                return;
            }
            for (int i = 0; i < modules.Length; i++)
            {
                modules[i] = modules[i].Replace($"{DirectoryPath}\\Data\\", string.Empty);
            }
            // Получение файлов перевода
            Stream? stream = RimHttpClient.GetGithubArchive(repo);
            if (stream is null)
            {
                InfoTextBox.AppendText($"{Environment.NewLine}{Messages.PlaceTime()}Ошибка получения файлов с GitHub");
                return;
            }
            InfoTextBox.AppendText($"{Environment.NewLine}{Messages.PlaceTime()}Обновление загружено");
            string tempDir = "temp";
            ZipArchive zipArchive = new (stream);
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
            zipArchive.ExtractToDirectory(tempDir);
            InfoTextBox.AppendText($"{Environment.NewLine}{Messages.PlaceTime()}Обновление извлечено");
            stream.Close();

            string[] baseDir = Directory.GetDirectories(tempDir);
            string[] dir = Directory.GetDirectories(baseDir[0]);
            StringComparison comparison = StringComparison.OrdinalIgnoreCase;
            bool ok;
            foreach (string dirEntry in dir)
            {
                ok = false;
                foreach (string module in modules)
                {
                    if (dirEntry.EndsWith(module, comparison))
                    {
                        FolderUpdate(module, dirEntry);
                        InfoTextBox.AppendText($"{Environment.NewLine}{Messages.PlaceTime()}Модуль {module} обновлён");
                        ok = true;
                        break;
                    }
                }
                if ((!dirEntry.EndsWith("RimWorldUniverse", comparison)) && (ok == false))
                {
                    string module = dirEntry.Replace($"{baseDir[0]}\\", string.Empty);
                    InfoTextBox.AppendText($"{Environment.NewLine}{Messages.PlaceTime()}Не найден модуль {module}");
                }
            }
            Directory.Delete(tempDir, true);
            InfoTextBox.AppendText($"{Environment.NewLine}{Messages.PlaceTime()}Обновление завершено!");

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
            InfoTextBox.AppendText($"{Environment.NewLine}{Messages.PlaceTime()}Сохранённые настройки возвращены к исходным значениям");
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            string language = Settings.Default.language;
            // Получения списка дополнений
            string[] modules = Directory.GetDirectories($"{DirectoryPath}\\Data");
            if (modules.Length == 0)
            {
                InfoTextBox.AppendText($"{Environment.NewLine}{Messages.PlaceTime()}В указанной папке не обнаружены модули");
                return;
            }
            string resultString = "Будут удалены следующие папки:";
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
                        InfoTextBox.AppendText($"{Environment.NewLine}{Messages.PlaceTime()}Папка {modules[i]} удалена");
                    }
                    else
                    {
                        InfoTextBox.AppendText($"{Environment.NewLine}{Messages.PlaceTime()}Папка {modules[i]} не найдена");
                    }
                }
            }
            InfoTextBox.AppendText($"{Environment.NewLine}{Messages.PlaceTime()}Удаление перевода завершено!");
        }
    }
}