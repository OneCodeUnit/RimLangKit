using RimLangKit.Properties;
using RimLanguageCore.Activities;
using RimLanguageCore.Misc;

namespace RimLangKit
{
    public partial class MainForm : Form
    {
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
                FolderButton2.BackColor = goodColor;
            }
            else
            {
                ButtonLanguageUpdate.Enabled = false;
                ResetButton.Enabled = false;
                FolderButton2.BackColor = badColor;
            }
        }
    }
}