using RimLanguageCore.Activities;
using RimLanguageCore.Misc;
using RimLanguageUI.Properties;
using System.Diagnostics;
using System.Reflection;

namespace RimLanguageUI
{
    public partial class Master : Form
    {
        public Master()
        {
            InitializeComponent();
            ToolTip toolTipMaster = new();

            #region ������������� ���� "� ���������"
            ToolStripMenuItemVersion.Text += Assembly.GetEntryAssembly()?.GetName().Version?.ToString()[..^2];
            #endregion

            #region ������������� TabControlMain
            TabPageGame.Text = "�������\n����";
            TabPageMod.Text = "�������\n����";
            TabPageFile.Text = "������\n� �������";
            TabPageExport.Text = "����������\n������";
            TabControlMain.SelectTab(ProgramSettings.Default.LastSelectedTab);
            #endregion

            #region �������������� �������� ����������
            if (ProgramSettings.Default.IsAutoUpdateActive)
            {
                ToolStripMenuItemAutoUpdateCheck.Checked = true;
                var oldCheckDate = ProgramSettings.Default.LastCheckDate;
                var newCheckDate = DateTime.Now;
                if (oldCheckDate.Month != newCheckDate.Month)
                {
                    var json = GitHubHttpClient.GetGithubJson();
                    if (json is null)
                    {
                        MessageBox.Show("�� ������� ��������� ����������. ��������� ����������� � ���������.", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        var newVersion = json.TagName.ToString()[1..];
                        var oldVersion = Assembly.GetEntryAssembly()?.GetName().Version?.ToString()[..^2];
                        if (oldVersion != newVersion)
                        {
                            var dr = MessageBox.Show($"�������� ���������� �� ������ {newVersion}!\n������� �� �������� ��������?\n\n� ����� ������:\n{json.Body}", "����������", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                            if (dr == DialogResult.Yes)
                            {
                                Process.Start(new ProcessStartInfo { FileName = json.HtmlUrl.ToString(), UseShellExecute = true });
                            }
                        }
                        ProgramSettings.Default.LastCheckDate = newCheckDate;
                        ProgramSettings.Default.Save();
                    }
                }
            }
            #endregion

            #region ������������� ���� "������� ����"
            TextBoxLanguage.Text = ProgramSettings.Default.LangName;
            TextBoxRepo.Text = ProgramSettings.Default.RepoName;
            if (ProgramSettings.Default.GamePath != string.Empty)
            {
                TextBoxGamePath.Text = ProgramSettings.Default.GamePath;
            }
            #endregion
        }

        #region �����������
        private void TabControlMain_DrawItem(object sender, DrawItemEventArgs e)
        {
            var g = e.Graphics;
            var text = TabControlMain.TabPages[e.Index].Text;
            var sizeText = g.MeasureString(text, TabControlMain.Font);

            var x = e.Bounds.Left + 3;
            var y = e.Bounds.Top + (e.Bounds.Height - sizeText.Height) / 2;

            g.DrawString(text, TabControlMain.Font, Brushes.Black, x, y);
        }

        private void ToolStripMenuItemCheckUpdate_Click(object sender, EventArgs e)
        {
            var json = GitHubHttpClient.GetGithubJson();
            if (json is null)
            {
                MessageBox.Show("�� ������� ��������� ����������. ��������� ����������� � ���������.", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var newVersion = json.TagName.ToString()[1..];
            var oldVersion = Assembly.GetEntryAssembly()?.GetName().Version?.ToString()[..^2];
            if (oldVersion == newVersion)
            {
                MessageBox.Show("���������� �� ���������", "����������", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                var dr = MessageBox.Show($"�������� ���������� �� ������ {newVersion}!\n������� �� �������� ��������?\n\n� ����� ������:\n{json.Body}", "����������", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (dr == DialogResult.Yes)
                {
                    Process.Start(new ProcessStartInfo { FileName = json.HtmlUrl.ToString(), UseShellExecute = true });
                }
            }
            return;
        }

        private void ToolStripMenuItemGuide_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo { FileName = @"https://github.com/OneCodeUnit/RimLangKit/blob/master/README.md", UseShellExecute = true });
        }

        private void ToolStripMenuItemAutoUpdateCheck_Click(object sender, EventArgs e)
        {
            ToolStripMenuItemAutoUpdateCheck.Checked = !ToolStripMenuItemAutoUpdateCheck.Checked;
            if (ToolStripMenuItemAutoUpdateCheck.Checked)
            {
                ProgramSettings.Default.IsAutoUpdateActive = true;
            }
            else
            {
                ProgramSettings.Default.IsAutoUpdateActive = false;
            }
            ProgramSettings.Default.Save();
        }

        private void ToolStripMenuItemCreator_Click(object sender, EventArgs e)
        {
            MessageBox.Show("��� ��������� ���������� ��������� �� �������� OliveWizard ��� ���������� RimWorld.\n\n", "�����", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void TabControlMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            ProgramSettings.Default.LastSelectedTab = TabControlMain.SelectedIndex;
            ProgramSettings.Default.Save();
        }

        private void ButtonSelectGameFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new();
            var dr = fbd.ShowDialog();
            if (dr == DialogResult.OK)
            {
                ProgramSettings.Default.GamePath = fbd.SelectedPath;
                ProgramSettings.Default.Save();
                TextBoxGamePath.Text = fbd.SelectedPath;
            }
        }

        private void TextBoxGamePath_TextChanged(object sender, EventArgs e)
        {
            if (Directory.Exists(TextBoxGamePath.Text))
            {
                TextBoxGamePath.BackColor = Color.PaleGreen;
                if (Directory.Exists($"{TextBoxGamePath.Text}\\Data"))
                {
                    string[] modules = Directory.GetDirectories($"{TextBoxGamePath.Text}\\Data");
                    if (modules.Length == 0)
                    {
                        TextBoxInfo.Text = string.Empty;
                        TextBoxInfo.Text = "��� �������";
                        TextBoxGamePath.BackColor = Color.MediumBlue;
                    }
                    else
                    {
                        TextBoxInfo.Text = "��������� ������:" + Environment.NewLine;
                        foreach (var module in modules)
                        {
                            string moduleName = module[module.LastIndexOf('\\')..];
                            moduleName = moduleName[1..];
                            TextBoxInfo.Text += moduleName + Environment.NewLine;
                        }
                    }
                }
                else
                {
                    TextBoxInfo.Text = string.Empty;
                    TextBoxInfo.Text = "��� �������";
                    TextBoxGamePath.BackColor = Color.MediumBlue;
                }
            }
            else
            {
                TextBoxInfo.Text = string.Empty;
                TextBoxGamePath.BackColor = Color.PaleVioletRed;
            }
            CheckTextBoxes();
        }

        private void CheckTextBoxes()
        {
            if ((TextBoxGamePath.BackColor == Color.PaleGreen) && (TextBoxRepo.Text != string.Empty) && (TextBoxLanguage.Text != string.Empty))
            {
                ButtonLanguageUpdate.Enabled = true;
                ButtonReset.Enabled = true;
            }
            else
            {
                ButtonLanguageUpdate.Enabled = false;
                ButtonReset.Enabled = false;
            }
        }

        private void TextBoxRepo_TextChanged(object sender, EventArgs e)
        {
            ProgramSettings.Default.RepoName = TextBoxRepo.Text;
            ProgramSettings.Default.Save();
            CheckTextBoxes();
        }

        private void TextBoxLanguage_TextChanged(object sender, EventArgs e)
        {
            ProgramSettings.Default.LangName = TextBoxLanguage.Text;
            ProgramSettings.Default.Save();
            CheckTextBoxes();
        }

        private void ButtonReset_Click(object sender, EventArgs e)
        {
            // ��������� ������ ����������
            if (Directory.Exists($"{ProgramSettings.Default.GamePath}\\Data"))
            {
                string[] modules = Directory.GetDirectories($"{ProgramSettings.Default.GamePath}\\Data");
                if (modules.Length == 0)
                {
                    MessageBox.Show("� ��������� ����� ��� ����������", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    string modulesList = string.Empty;
                    List<string> languagePathList = [];
                    foreach (var module in modules)
                    {
                        string moduleName = module[module.LastIndexOf('\\')..];
                        moduleName = moduleName[1..];
                        string languagePath = $"{module}\\Languages\\{ProgramSettings.Default.LangName}";
                        if (Directory.Exists(languagePath))
                        {
                            modulesList += $"- {moduleName}: {languagePath}\n";
                            languagePathList.Add(languagePath);
                        }
                    }
                    if (modulesList == string.Empty)
                    {
                        MessageBox.Show("������� ������� �� ������", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        var dr = MessageBox.Show($"�� ����������� ������� �������, ��������� � ���� ���������. ������ �������� ��������� �� �����.\n����� ������� ��������� �����:\n\n{modulesList}", "��������", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                        if (dr == DialogResult.Yes)
                        {
                            ProgramSettings.Default.Sha = "00000000";
                            ProgramSettings.Default.Save();
                            foreach (var path in languagePathList)
                            {
                                Directory.Delete(path, true);
                            }
                            MessageBox.Show("�������� ���������", "��������", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("� ��������� ����� ��� ��������", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ButtonDefault_Click(object sender, EventArgs e)
        {
            var dr = MessageBox.Show("�� ������������� ������ ������� ��� ��������� � ���� ���� � �����������?", "����� ��������", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (dr == DialogResult.Yes)
            {
                ProgramSettings.Default.Sha = "00000000";
                ProgramSettings.Default.RepoName = "Ludeon/RimWorld-ru";
                ProgramSettings.Default.LangName = "Russian (GitHub)";
                ProgramSettings.Default.GamePath = string.Empty;
                ProgramSettings.Default.Save();

                TextBoxLanguage.Text = "Russian (GitHub)";
                TextBoxRepo.Text = "Ludeon/RimWorld-ru";
                TextBoxGamePath.Text = string.Empty;
                MessageBox.Show("��������� ��������", "����� ��������", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void TextBoxGamePath_DoubleClick(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo { FileName = ProgramSettings.Default.GamePath, UseShellExecute = true });
        }
        #endregion


        private void ButtonLanguageUpdate_Click(object sender, EventArgs e)
        {
            (bool, string) result;
            TextBoxInfo.Text = string.Empty;
            TextBoxInfo.Text = "�������� ���������� - %";
            result = LanguageUpdater.TranslationVersionCheckActivity(ProgramSettings.Default.Sha, ProgramSettings.Default.RepoName);
            if (result.Item1 == false)
            {
                MessageBox.Show(result.Item2, "����������", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            TextBoxInfo.Text = TextBoxInfo.Text[..^1] + "OK" + Environment.NewLine;
            var dr = MessageBox.Show($"{result.Item2}.\n��������?", "����������", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (dr != DialogResult.Yes)
            {
                return;
            }

            TextBoxInfo.Text = "�������� ���������� - %";
            result = LanguageUpdater.LanguageUpdateDownload(ProgramSettings.Default.RepoName);
            if (result.Item1 == false)
            {
                MessageBox.Show(result.Item2, "����������", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            TextBoxInfo.Text = TextBoxInfo.Text[..^1] + "OK" + Environment.NewLine;

            TextBoxInfo.Text = "��������� ���������� - %";
            result = LanguageUpdater.LanguageUpdateActivity(ProgramSettings.Default.GamePath, ProgramSettings.Default.LangName);
            if (result.Item1 == false)
            {
                MessageBox.Show(result.Item2, "����������", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            TextBoxInfo.Text = TextBoxInfo.Text[..^1] + "OK" + Environment.NewLine;
            TextBoxInfo.Text += result.Item2 + Environment.NewLine;

            ProgramSettings.Default.Sha = LanguageUpdater.GetSha();
            ProgramSettings.Default.Save();
        }
    }
}