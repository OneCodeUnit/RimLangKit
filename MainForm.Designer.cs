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
            ButtonUniqueNames = new Button();
            ButtonDictionary = new Button();
            ButtonCase = new Button();
            ButtonEncoding = new Button();
            UpdateButton = new Button();
            UpdateLabel = new Label();
            LinkLabelGithub = new LinkLabel();
            ButtonTagCollector = new Button();
            LinkLabelInfo = new LinkLabel();
            ButtonFileFix = new Button();
            ButtonFindChanges = new Button();
            SuspendLayout();
            // 
            // VersionLabel
            // 
            VersionLabel.AutoSize = true;
            VersionLabel.Location = new Point(654, 9);
            VersionLabel.Name = "VersionLabel";
            VersionLabel.Size = new Size(82, 20);
            VersionLabel.TabIndex = 12;
            VersionLabel.Text = "Версия 3.3";
            // 
            // FolderButton
            // 
            FolderButton.Location = new Point(248, 45);
            FolderButton.Name = "FolderButton";
            FolderButton.Size = new Size(400, 60);
            FolderButton.TabIndex = 0;
            FolderButton.Text = "Выбрать папку";
            FolderButton.UseVisualStyleBackColor = true;
            FolderButton.Click += FolderButton_Click;
            // 
            // FolderTextBox
            // 
            FolderTextBox.AllowDrop = true;
            FolderTextBox.Location = new Point(12, 12);
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
            LabelCheck.Location = new Point(12, 42);
            LabelCheck.Name = "LabelCheck";
            LabelCheck.Size = new Size(122, 20);
            LabelCheck.TabIndex = 2;
            LabelCheck.Text = "Выберите папку";
            // 
            // ButtonENGIsert
            // 
            ButtonENGIsert.Enabled = false;
            ButtonENGIsert.Location = new Point(12, 77);
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
            InfoTextBox.Location = new Point(248, 111);
            InfoTextBox.MaximumSize = new Size(543, 488);
            InfoTextBox.MinimumSize = new Size(543, 488);
            InfoTextBox.Multiline = true;
            InfoTextBox.Name = "InfoTextBox";
            InfoTextBox.ReadOnly = true;
            InfoTextBox.ScrollBars = ScrollBars.Vertical;
            InfoTextBox.Size = new Size(543, 488);
            InfoTextBox.TabIndex = 4;
            // 
            // ButtonUniqueNames
            // 
            ButtonUniqueNames.Enabled = false;
            ButtonUniqueNames.Location = new Point(12, 143);
            ButtonUniqueNames.Name = "ButtonUniqueNames";
            ButtonUniqueNames.Size = new Size(230, 60);
            ButtonUniqueNames.TabIndex = 6;
            ButtonUniqueNames.Text = "Избавиться от одинаковых имён файлов";
            ButtonUniqueNames.UseVisualStyleBackColor = true;
            ButtonUniqueNames.Click += ButtonUniqueNames_Click;
            // 
            // ButtonDictionary
            // 
            ButtonDictionary.Enabled = false;
            ButtonDictionary.Location = new Point(12, 209);
            ButtonDictionary.Name = "ButtonDictionary";
            ButtonDictionary.Size = new Size(230, 60);
            ButtonDictionary.TabIndex = 7;
            ButtonDictionary.Text = "Транскрипция имён";
            ButtonDictionary.UseVisualStyleBackColor = true;
            ButtonDictionary.Click += ButtonDictionary_Click;
            // 
            // ButtonCase
            // 
            ButtonCase.Enabled = false;
            ButtonCase.Location = new Point(12, 275);
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
            ButtonEncoding.Location = new Point(12, 341);
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
            UpdateButton.Location = new Point(654, 52);
            UpdateButton.Name = "UpdateButton";
            UpdateButton.Size = new Size(40, 40);
            UpdateButton.TabIndex = 11;
            UpdateButton.UseVisualStyleBackColor = false;
            UpdateButton.Click += UpdateButton_Click;
            // 
            // UpdateLabel
            // 
            UpdateLabel.AutoSize = true;
            UpdateLabel.Location = new Point(654, 29);
            UpdateLabel.Name = "UpdateLabel";
            UpdateLabel.Size = new Size(92, 20);
            UpdateLabel.TabIndex = 13;
            UpdateLabel.Text = "Последняя?";
            // 
            // LinkLabelGithub
            // 
            LinkLabelGithub.AutoSize = true;
            LinkLabelGithub.Location = new Point(700, 52);
            LinkLabelGithub.Name = "LinkLabelGithub";
            LinkLabelGithub.Size = new Size(63, 20);
            LinkLabelGithub.TabIndex = 14;
            LinkLabelGithub.TabStop = true;
            LinkLabelGithub.Text = "Скачать";
            LinkLabelGithub.Visible = false;
            LinkLabelGithub.LinkClicked += LinkLabelGithub_LinkClicked;
            // 
            // ButtonTagCollector
            // 
            ButtonTagCollector.Enabled = false;
            ButtonTagCollector.Location = new Point(12, 407);
            ButtonTagCollector.Name = "ButtonTagCollector";
            ButtonTagCollector.Size = new Size(230, 60);
            ButtonTagCollector.TabIndex = 15;
            ButtonTagCollector.Text = "Проанализировать теги";
            ButtonTagCollector.UseVisualStyleBackColor = true;
            ButtonTagCollector.Click += ButtonTagCollector_Click;
            // 
            // LinkLabelInfo
            // 
            LinkLabelInfo.AutoSize = true;
            LinkLabelInfo.Location = new Point(700, 72);
            LinkLabelInfo.Name = "LinkLabelInfo";
            LinkLabelInfo.Size = new Size(91, 20);
            LinkLabelInfo.TabIndex = 16;
            LinkLabelInfo.TabStop = true;
            LinkLabelInfo.Text = "Инструкция";
            LinkLabelInfo.LinkClicked += LinkLabelInfo_LinkClicked;
            // 
            // ButtonFileFix
            // 
            ButtonFileFix.Enabled = false;
            ButtonFileFix.Location = new Point(12, 473);
            ButtonFileFix.Name = "ButtonFileFix";
            ButtonFileFix.Size = new Size(230, 60);
            ButtonFileFix.TabIndex = 17;
            ButtonFileFix.Text = "Найти сломанные файлы";
            ButtonFileFix.UseVisualStyleBackColor = true;
            ButtonFileFix.Click += ButtonFileFix_Click;
            // 
            // ButtonFindChanges
            // 
            ButtonFindChanges.Enabled = false;
            ButtonFindChanges.Location = new Point(12, 539);
            ButtonFindChanges.Name = "ButtonFindChanges";
            ButtonFindChanges.Size = new Size(230, 60);
            ButtonFindChanges.TabIndex = 18;
            ButtonFindChanges.Text = "Найти изменения текста";
            ButtonFindChanges.UseVisualStyleBackColor = true;
            ButtonFindChanges.Click += ButtonFindChanges_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(120F, 120F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoScroll = true;
            ClientSize = new Size(805, 611);
            Controls.Add(ButtonFindChanges);
            Controls.Add(ButtonFileFix);
            Controls.Add(LinkLabelInfo);
            Controls.Add(ButtonTagCollector);
            Controls.Add(LinkLabelGithub);
            Controls.Add(UpdateLabel);
            Controls.Add(VersionLabel);
            Controls.Add(UpdateButton);
            Controls.Add(ButtonEncoding);
            Controls.Add(ButtonCase);
            Controls.Add(ButtonDictionary);
            Controls.Add(ButtonUniqueNames);
            Controls.Add(InfoTextBox);
            Controls.Add(ButtonENGIsert);
            Controls.Add(LabelCheck);
            Controls.Add(FolderTextBox);
            Controls.Add(FolderButton);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximumSize = new Size(823, 658);
            MinimumSize = new Size(823, 658);
            Name = "MainForm";
            Text = "RimWorld Language Kit by OliveWizard";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button FolderButton;
        private TextBox FolderTextBox;
        private Label LabelCheck;
        private Button ButtonENGIsert;
        private TextBox InfoTextBox;
        private Button ButtonUniqueNames;
        private Button ButtonDictionary;
        private Button ButtonCase;
        private Button ButtonEncoding;
        private Button UpdateButton;
        private Label UpdateLabel;
        private Label VersionLabel;
        private LinkLabel LinkLabelGithub;
        private Button ButtonTagCollector;
        private LinkLabel LinkLabelInfo;
        private Button ButtonFileFix;
        private Button ButtonFindChanges;
    }
}