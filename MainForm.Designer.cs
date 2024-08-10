namespace RimLangKit
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            VersionLabel = new Label();
            FolderButton = new Button();
            FolderTextBox = new TextBox();
            LabelCheck = new Label();
            ButtonENGIsert = new Button();
            InfoTextBox = new TextBox();
            FileRenamerButton = new Button();
            NamesTranslatorButton = new Button();
            ButtonCase = new Button();
            ButtonEncoding = new Button();
            UpdateButton = new Button();
            UpdateLabel = new Label();
            LinkLabelGithub = new LinkLabel();
            TagCollectorButton = new Button();
            LinkLabelInfo = new LinkLabel();
            FileFixerButton = new Button();
            ButtonFindChanges = new Button();
            MainTabs = new TabControl();
            tabPage1 = new TabPage();
            tabPage2 = new TabPage();
            ResetButton = new Button();
            DefaultButton = new Button();
            LanguageInput = new TextBox();
            RepoInput = new TextBox();
            ButtonLanguageUpdate = new Button();
            InfoTextBox2 = new TextBox();
            FolderTextBox2 = new TextBox();
            FolderButton2 = new Button();
            tabPage3 = new TabPage();
            ButtonDownload = new Button();
            ButtonResultFolder = new Button();
            FolderTextBox3 = new TextBox();
            ButtonSteam = new Button();
            SteamTextBox = new TextBox();
            InfoTextBox3 = new TextBox();
            ButtonDarkMode = new Button();
            MainTabs.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            tabPage3.SuspendLayout();
            SuspendLayout();
            // 
            // VersionLabel
            // 
            VersionLabel.AutoSize = true;
            VersionLabel.Location = new Point(826, 9);
            VersionLabel.Name = "VersionLabel";
            VersionLabel.Size = new Size(66, 20);
            VersionLabel.TabIndex = 12;
            VersionLabel.Text = "Версия?";
            // 
            // FolderButton
            // 
            FolderButton.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 204);
            FolderButton.Location = new Point(653, 6);
            FolderButton.Name = "FolderButton";
            FolderButton.Size = new Size(141, 60);
            FolderButton.TabIndex = 0;
            FolderButton.Text = "Выбрать папку";
            FolderButton.UseVisualStyleBackColor = true;
            FolderButton.Click += FolderButton_Click;
            // 
            // FolderTextBox
            // 
            FolderTextBox.AllowDrop = true;
            FolderTextBox.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 204);
            FolderTextBox.Location = new Point(11, 13);
            FolderTextBox.MaximumSize = new Size(636, 27);
            FolderTextBox.MinimumSize = new Size(636, 27);
            FolderTextBox.Name = "FolderTextBox";
            FolderTextBox.Size = new Size(636, 27);
            FolderTextBox.TabIndex = 1;
            FolderTextBox.TextChanged += FolderTextBox_TextChanged;
            // 
            // LabelCheck
            // 
            LabelCheck.AutoSize = true;
            LabelCheck.Location = new Point(11, 43);
            LabelCheck.Name = "LabelCheck";
            LabelCheck.Size = new Size(160, 28);
            LabelCheck.TabIndex = 2;
            LabelCheck.Text = "Выберите папку";
            // 
            // ButtonENGIsert
            // 
            ButtonENGIsert.Enabled = false;
            ButtonENGIsert.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 204);
            ButtonENGIsert.Location = new Point(11, 78);
            ButtonENGIsert.Name = "ButtonENGIsert";
            ButtonENGIsert.Size = new Size(230, 60);
            ButtonENGIsert.TabIndex = 3;
            ButtonENGIsert.Text = "Добавить комментарии";
            ButtonENGIsert.UseVisualStyleBackColor = true;
            ButtonENGIsert.Click += ButtonENGIsert_Click;
            // 
            // InfoTextBox
            // 
            InfoTextBox.AcceptsReturn = true;
            InfoTextBox.AcceptsTab = true;
            InfoTextBox.Location = new Point(247, 78);
            InfoTextBox.MaximumSize = new Size(543, 522);
            InfoTextBox.MinimumSize = new Size(543, 522);
            InfoTextBox.Multiline = true;
            InfoTextBox.Name = "InfoTextBox";
            InfoTextBox.ReadOnly = true;
            InfoTextBox.ScrollBars = ScrollBars.Vertical;
            InfoTextBox.Size = new Size(543, 522);
            InfoTextBox.TabIndex = 4;
            // 
            // FileRenamerButton
            // 
            FileRenamerButton.Enabled = false;
            FileRenamerButton.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 204);
            FileRenamerButton.Location = new Point(11, 144);
            FileRenamerButton.Name = "FileRenamerButton";
            FileRenamerButton.Size = new Size(230, 60);
            FileRenamerButton.TabIndex = 6;
            FileRenamerButton.Text = "Избавиться от одинаковых имён файлов";
            FileRenamerButton.UseVisualStyleBackColor = true;
            FileRenamerButton.Click += FileRenamerButton_Click;
            // 
            // NamesTranslatorButton
            // 
            NamesTranslatorButton.Enabled = false;
            NamesTranslatorButton.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 204);
            NamesTranslatorButton.Location = new Point(11, 210);
            NamesTranslatorButton.Name = "NamesTranslatorButton";
            NamesTranslatorButton.Size = new Size(230, 60);
            NamesTranslatorButton.TabIndex = 7;
            NamesTranslatorButton.Text = "Транскрипция имён";
            NamesTranslatorButton.UseVisualStyleBackColor = true;
            NamesTranslatorButton.Click += NamesTranslatorButton_Click;
            // 
            // ButtonCase
            // 
            ButtonCase.Enabled = false;
            ButtonCase.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 204);
            ButtonCase.Location = new Point(11, 276);
            ButtonCase.Name = "ButtonCase";
            ButtonCase.Size = new Size(230, 60);
            ButtonCase.TabIndex = 9;
            ButtonCase.Text = "Создать Case, Plural и Gender";
            ButtonCase.UseVisualStyleBackColor = true;
            ButtonCase.Click += ButtonCase_Click;
            // 
            // ButtonEncoding
            // 
            ButtonEncoding.Enabled = false;
            ButtonEncoding.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 204);
            ButtonEncoding.Location = new Point(11, 342);
            ButtonEncoding.Name = "ButtonEncoding";
            ButtonEncoding.Size = new Size(230, 60);
            ButtonEncoding.TabIndex = 10;
            ButtonEncoding.Text = "Исправить кодировку";
            ButtonEncoding.UseVisualStyleBackColor = true;
            ButtonEncoding.Click += ButtonEncoding_Click;
            // 
            // UpdateButton
            // 
            UpdateButton.BackColor = SystemColors.Control;
            UpdateButton.BackgroundImage = Properties.Resources.wait_c;
            UpdateButton.BackgroundImageLayout = ImageLayout.Center;
            UpdateButton.FlatAppearance.BorderSize = 0;
            UpdateButton.Location = new Point(826, 52);
            UpdateButton.Name = "UpdateButton";
            UpdateButton.Size = new Size(40, 40);
            UpdateButton.TabIndex = 11;
            UpdateButton.UseVisualStyleBackColor = false;
            UpdateButton.Click += UpdateButton_Click;
            // 
            // UpdateLabel
            // 
            UpdateLabel.AutoSize = true;
            UpdateLabel.Location = new Point(826, 29);
            UpdateLabel.Name = "UpdateLabel";
            UpdateLabel.Size = new Size(92, 20);
            UpdateLabel.TabIndex = 13;
            UpdateLabel.Text = "Последняя?";
            // 
            // LinkLabelGithub
            // 
            LinkLabelGithub.AutoSize = true;
            LinkLabelGithub.Location = new Point(872, 52);
            LinkLabelGithub.Name = "LinkLabelGithub";
            LinkLabelGithub.Size = new Size(63, 20);
            LinkLabelGithub.TabIndex = 14;
            LinkLabelGithub.TabStop = true;
            LinkLabelGithub.Text = "Скачать";
            LinkLabelGithub.Visible = false;
            LinkLabelGithub.LinkClicked += LinkLabelGithub_LinkClicked;
            // 
            // TagCollectorButton
            // 
            TagCollectorButton.Enabled = false;
            TagCollectorButton.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 204);
            TagCollectorButton.Location = new Point(11, 408);
            TagCollectorButton.Name = "TagCollectorButton";
            TagCollectorButton.Size = new Size(230, 60);
            TagCollectorButton.TabIndex = 15;
            TagCollectorButton.Text = "Проанализировать теги";
            TagCollectorButton.UseVisualStyleBackColor = true;
            TagCollectorButton.Click += TagCollectorButton_Click;
            // 
            // LinkLabelInfo
            // 
            LinkLabelInfo.AutoSize = true;
            LinkLabelInfo.Location = new Point(872, 72);
            LinkLabelInfo.Name = "LinkLabelInfo";
            LinkLabelInfo.Size = new Size(91, 20);
            LinkLabelInfo.TabIndex = 16;
            LinkLabelInfo.TabStop = true;
            LinkLabelInfo.Text = "Инструкция";
            LinkLabelInfo.LinkClicked += LinkLabelInfo_LinkClicked;
            // 
            // FileFixerButton
            // 
            FileFixerButton.Enabled = false;
            FileFixerButton.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 204);
            FileFixerButton.Location = new Point(11, 474);
            FileFixerButton.Name = "FileFixerButton";
            FileFixerButton.Size = new Size(230, 60);
            FileFixerButton.TabIndex = 17;
            FileFixerButton.Text = "Найти сломанные файлы";
            FileFixerButton.UseVisualStyleBackColor = true;
            FileFixerButton.Click += FileFixerButton_Click;
            // 
            // ButtonFindChanges
            // 
            ButtonFindChanges.Enabled = false;
            ButtonFindChanges.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 204);
            ButtonFindChanges.Location = new Point(11, 540);
            ButtonFindChanges.Name = "ButtonFindChanges";
            ButtonFindChanges.Size = new Size(230, 60);
            ButtonFindChanges.TabIndex = 18;
            ButtonFindChanges.Text = "Найти изменения текста";
            ButtonFindChanges.UseVisualStyleBackColor = true;
            ButtonFindChanges.Click += ButtonFindChanges_Click;
            // 
            // MainTabs
            // 
            MainTabs.Controls.Add(tabPage1);
            MainTabs.Controls.Add(tabPage2);
            MainTabs.Controls.Add(tabPage3);
            MainTabs.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
            MainTabs.Location = new Point(12, 12);
            MainTabs.Name = "MainTabs";
            MainTabs.SelectedIndex = 0;
            MainTabs.Size = new Size(808, 651);
            MainTabs.TabIndex = 19;
            MainTabs.SelectedIndexChanged += MainTabs_IndexChange;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(InfoTextBox);
            tabPage1.Controls.Add(ButtonFindChanges);
            tabPage1.Controls.Add(FolderButton);
            tabPage1.Controls.Add(FileFixerButton);
            tabPage1.Controls.Add(FolderTextBox);
            tabPage1.Controls.Add(LabelCheck);
            tabPage1.Controls.Add(TagCollectorButton);
            tabPage1.Controls.Add(ButtonENGIsert);
            tabPage1.Controls.Add(FileRenamerButton);
            tabPage1.Controls.Add(NamesTranslatorButton);
            tabPage1.Controls.Add(ButtonCase);
            tabPage1.Controls.Add(ButtonEncoding);
            tabPage1.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
            tabPage1.Location = new Point(4, 37);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(800, 610);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Перевод мода";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(ResetButton);
            tabPage2.Controls.Add(DefaultButton);
            tabPage2.Controls.Add(LanguageInput);
            tabPage2.Controls.Add(RepoInput);
            tabPage2.Controls.Add(ButtonLanguageUpdate);
            tabPage2.Controls.Add(InfoTextBox2);
            tabPage2.Controls.Add(FolderTextBox2);
            tabPage2.Controls.Add(FolderButton2);
            tabPage2.Location = new Point(4, 37);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(800, 610);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Перевод игры";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // ResetButton
            // 
            ResetButton.Enabled = false;
            ResetButton.Location = new Point(4, 495);
            ResetButton.Margin = new Padding(4);
            ResetButton.Name = "ResetButton";
            ResetButton.Size = new Size(200, 50);
            ResetButton.TabIndex = 25;
            ResetButton.Text = "Удалить перевод";
            ResetButton.UseVisualStyleBackColor = true;
            ResetButton.Click += ResetButton_Click;
            // 
            // DefaultButton
            // 
            DefaultButton.Location = new Point(4, 553);
            DefaultButton.Margin = new Padding(4);
            DefaultButton.Name = "DefaultButton";
            DefaultButton.Size = new Size(200, 50);
            DefaultButton.TabIndex = 24;
            DefaultButton.Text = "Сброс настроек";
            DefaultButton.UseVisualStyleBackColor = true;
            DefaultButton.Click += DefaultButton_Click;
            // 
            // LanguageInput
            // 
            LanguageInput.AllowDrop = true;
            LanguageInput.Font = new Font("Segoe UI", 12F);
            LanguageInput.Location = new Point(4, 453);
            LanguageInput.Margin = new Padding(4);
            LanguageInput.Name = "LanguageInput";
            LanguageInput.Size = new Size(200, 34);
            LanguageInput.TabIndex = 23;
            LanguageInput.TextChanged += LanguageInput_TextChanged;
            // 
            // RepoInput
            // 
            RepoInput.AllowDrop = true;
            RepoInput.Font = new Font("Segoe UI", 12F);
            RepoInput.Location = new Point(4, 411);
            RepoInput.Margin = new Padding(4);
            RepoInput.Name = "RepoInput";
            RepoInput.Size = new Size(200, 34);
            RepoInput.TabIndex = 22;
            RepoInput.TextChanged += RepoInput_TextChanged;
            // 
            // ButtonLanguageUpdate
            // 
            ButtonLanguageUpdate.Enabled = false;
            ButtonLanguageUpdate.Location = new Point(4, 107);
            ButtonLanguageUpdate.Margin = new Padding(4);
            ButtonLanguageUpdate.Name = "ButtonLanguageUpdate";
            ButtonLanguageUpdate.Size = new Size(200, 70);
            ButtonLanguageUpdate.TabIndex = 21;
            ButtonLanguageUpdate.Text = "Обновить локализацию";
            ButtonLanguageUpdate.UseVisualStyleBackColor = true;
            ButtonLanguageUpdate.Click += ButtonLanguageUpdate_Click;
            // 
            // InfoTextBox2
            // 
            InfoTextBox2.AcceptsReturn = true;
            InfoTextBox2.AcceptsTab = true;
            InfoTextBox2.Location = new Point(212, 49);
            InfoTextBox2.Margin = new Padding(4);
            InfoTextBox2.Multiline = true;
            InfoTextBox2.Name = "InfoTextBox2";
            InfoTextBox2.ReadOnly = true;
            InfoTextBox2.ScrollBars = ScrollBars.Vertical;
            InfoTextBox2.Size = new Size(581, 554);
            InfoTextBox2.TabIndex = 20;
            // 
            // FolderTextBox2
            // 
            FolderTextBox2.AllowDrop = true;
            FolderTextBox2.Font = new Font("Segoe UI", 12F);
            FolderTextBox2.Location = new Point(7, 7);
            FolderTextBox2.Margin = new Padding(4);
            FolderTextBox2.Name = "FolderTextBox2";
            FolderTextBox2.Size = new Size(753, 34);
            FolderTextBox2.TabIndex = 19;
            FolderTextBox2.TextChanged += FolderTextBox2_TextChanged;
            // 
            // FolderButton2
            // 
            FolderButton2.Location = new Point(4, 49);
            FolderButton2.Margin = new Padding(4);
            FolderButton2.Name = "FolderButton2";
            FolderButton2.Size = new Size(200, 50);
            FolderButton2.TabIndex = 18;
            FolderButton2.Text = "Выбрать папку";
            FolderButton2.UseVisualStyleBackColor = true;
            FolderButton2.Click += FolderButton2_Click;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(ButtonDownload);
            tabPage3.Controls.Add(ButtonResultFolder);
            tabPage3.Controls.Add(FolderTextBox3);
            tabPage3.Controls.Add(ButtonSteam);
            tabPage3.Controls.Add(SteamTextBox);
            tabPage3.Controls.Add(InfoTextBox3);
            tabPage3.Location = new Point(4, 37);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new Padding(3);
            tabPage3.Size = new Size(800, 610);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "Загрузка мода";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // ButtonDownload
            // 
            ButtonDownload.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
            ButtonDownload.Location = new Point(609, 91);
            ButtonDownload.Name = "ButtonDownload";
            ButtonDownload.Size = new Size(184, 36);
            ButtonDownload.TabIndex = 26;
            ButtonDownload.Text = "Скачать";
            ButtonDownload.UseVisualStyleBackColor = true;
            ButtonDownload.Click += ButtonDownload_Click;
            // 
            // ButtonResultFolder
            // 
            ButtonResultFolder.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 204);
            ButtonResultFolder.Location = new Point(609, 49);
            ButtonResultFolder.Name = "ButtonResultFolder";
            ButtonResultFolder.Size = new Size(184, 36);
            ButtonResultFolder.TabIndex = 25;
            ButtonResultFolder.Text = "Место сохранения";
            ButtonResultFolder.UseVisualStyleBackColor = true;
            ButtonResultFolder.Click += ButtonResultFolder_Click;
            // 
            // FolderTextBox3
            // 
            FolderTextBox3.AllowDrop = true;
            FolderTextBox3.Font = new Font("Segoe UI", 12F);
            FolderTextBox3.Location = new Point(7, 49);
            FolderTextBox3.Margin = new Padding(4);
            FolderTextBox3.Name = "FolderTextBox3";
            FolderTextBox3.Size = new Size(595, 34);
            FolderTextBox3.TabIndex = 24;
            // 
            // ButtonSteam
            // 
            ButtonSteam.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 204);
            ButtonSteam.Location = new Point(609, 7);
            ButtonSteam.Name = "ButtonSteam";
            ButtonSteam.Size = new Size(184, 36);
            ButtonSteam.TabIndex = 23;
            ButtonSteam.Text = "Выбрать SteamCMD";
            ButtonSteam.UseVisualStyleBackColor = true;
            ButtonSteam.Click += ButtonSteam_Click;
            // 
            // SteamTextBox
            // 
            SteamTextBox.AllowDrop = true;
            SteamTextBox.Font = new Font("Segoe UI", 12F);
            SteamTextBox.Location = new Point(7, 7);
            SteamTextBox.Margin = new Padding(4);
            SteamTextBox.Name = "SteamTextBox";
            SteamTextBox.Size = new Size(595, 34);
            SteamTextBox.TabIndex = 22;
            // 
            // InfoTextBox3
            // 
            InfoTextBox3.AcceptsReturn = true;
            InfoTextBox3.AcceptsTab = true;
            InfoTextBox3.Location = new Point(7, 134);
            InfoTextBox3.Margin = new Padding(4);
            InfoTextBox3.Multiline = true;
            InfoTextBox3.Name = "InfoTextBox3";
            InfoTextBox3.ReadOnly = true;
            InfoTextBox3.ScrollBars = ScrollBars.Vertical;
            InfoTextBox3.Size = new Size(786, 469);
            InfoTextBox3.TabIndex = 21;
            // 
            // ButtonDarkMode
            // 
            ButtonDarkMode.Image = Properties.Resources.darkmode;
            ButtonDarkMode.Location = new Point(826, 98);
            ButtonDarkMode.Name = "ButtonDarkMode";
            ButtonDarkMode.Size = new Size(40, 40);
            ButtonDarkMode.TabIndex = 20;
            ButtonDarkMode.UseVisualStyleBackColor = true;
            ButtonDarkMode.Click += ButtonDarkMode_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(120F, 120F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoScroll = true;
            ClientSize = new Size(975, 669);
            Controls.Add(ButtonDarkMode);
            Controls.Add(MainTabs);
            Controls.Add(VersionLabel);
            Controls.Add(UpdateLabel);
            Controls.Add(UpdateButton);
            Controls.Add(LinkLabelGithub);
            Controls.Add(LinkLabelInfo);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximumSize = new Size(993, 716);
            MinimumSize = new Size(993, 716);
            Name = "MainForm";
            Text = "RimWorld Language Kit by OliveWizard";
            MainTabs.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            tabPage3.ResumeLayout(false);
            tabPage3.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button FolderButton;
        private TextBox FolderTextBox;
        private Label LabelCheck;
        private Button ButtonENGIsert;
        private TextBox InfoTextBox;
        private Button FileRenamerButton;
        private Button NamesTranslatorButton;
        private Button ButtonCase;
        private Button ButtonEncoding;
        private Button UpdateButton;
        private Label UpdateLabel;
        private Label VersionLabel;
        private LinkLabel LinkLabelGithub;
        private Button TagCollectorButton;
        private LinkLabel LinkLabelInfo;
        private Button FileFixerButton;
        private Button ButtonFindChanges;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private Button ResetButton;
        private Button DefaultButton;
        private TextBox LanguageInput;
        private TextBox RepoInput;
        private Button ButtonLanguageUpdate;
        private TextBox InfoTextBox2;
        private TextBox FolderTextBox2;
        private Button FolderButton2;
        private Button ButtonDarkMode;
        private TabControl MainTabs;
        private TabPage tabPage3;
        private Button ButtonSteam;
        private TextBox SteamTextBox;
        private TextBox InfoTextBox3;
        private TextBox FolderTextBox3;
        private Button ButtonDownload;
        private Button ButtonResultFolder;
    }
}