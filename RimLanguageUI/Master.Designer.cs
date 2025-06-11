namespace RimLanguageUI
{
    partial class Master
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
            Label LabelLanguageName;
            Label LabelRepo;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Master));
            TabControlMain = new TabControl();
            TabPageGame = new TabPage();
            TextBoxInfo = new TextBox();
            TextBoxGamePath = new TextBox();
            ButtonLanguageUpdate = new Button();
            ButtonSelectGameFolder = new Button();
            ButtonReset = new Button();
            ButtonDefault = new Button();
            TextBoxLanguage = new TextBox();
            TextBoxRepo = new TextBox();
            TabPageMod = new TabPage();
            TabPageFile = new TabPage();
            textBox1 = new TextBox();
            ButtonCommentInsert = new Button();
            TabPageExport = new TabPage();
            MenuStripMaster = new MenuStrip();
            toolStripMenuItem1 = new ToolStripMenuItem();
            ToolStripMenuItemSettings = new ToolStripMenuItem();
            ToolStripMenuItemSettingsLocUpdate = new ToolStripMenuItem();
            ToolStripMenuItemSelectLanguage = new ToolStripMenuItem();
            ToolStripMenuItemLanguage = new ToolStripMenuItem();
            ToolStripMenuItemAbout = new ToolStripMenuItem();
            ToolStripMenuItemVersion = new ToolStripMenuItem();
            ToolStripMenuItemCreator = new ToolStripMenuItem();
            ToolStripMenuItemAutoUpdateCheck = new ToolStripMenuItem();
            ToolStripMenuItemCheckUpdate = new ToolStripMenuItem();
            ToolStripMenuItemGuide = new ToolStripMenuItem();
            LabelLanguageName = new Label();
            LabelRepo = new Label();
            TabControlMain.SuspendLayout();
            TabPageGame.SuspendLayout();
            TabPageFile.SuspendLayout();
            MenuStripMaster.SuspendLayout();
            SuspendLayout();
            // 
            // LabelLanguageName
            // 
            LabelLanguageName.AutoSize = true;
            LabelLanguageName.Location = new Point(7, 196);
            LabelLanguageName.Name = "LabelLanguageName";
            LabelLanguageName.Size = new Size(155, 25);
            LabelLanguageName.TabIndex = 33;
            LabelLanguageName.Text = "Название языка";
            // 
            // LabelRepo
            // 
            LabelRepo.AutoSize = true;
            LabelRepo.Location = new Point(7, 129);
            LabelRepo.Name = "LabelRepo";
            LabelRepo.Size = new Size(169, 25);
            LabelRepo.TabIndex = 32;
            LabelRepo.Text = "Имя репозитория";
            // 
            // TabControlMain
            // 
            TabControlMain.Alignment = TabAlignment.Left;
            TabControlMain.Controls.Add(TabPageGame);
            TabControlMain.Controls.Add(TabPageMod);
            TabControlMain.Controls.Add(TabPageFile);
            TabControlMain.Controls.Add(TabPageExport);
            TabControlMain.DrawMode = TabDrawMode.OwnerDrawFixed;
            TabControlMain.Font = new Font("Arial Nova", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
            TabControlMain.ItemSize = new Size(60, 120);
            TabControlMain.Location = new Point(12, 31);
            TabControlMain.Multiline = true;
            TabControlMain.Name = "TabControlMain";
            TabControlMain.Padding = new Point(5, 5);
            TabControlMain.SelectedIndex = 0;
            TabControlMain.Size = new Size(598, 390);
            TabControlMain.SizeMode = TabSizeMode.Fixed;
            TabControlMain.TabIndex = 0;
            TabControlMain.DrawItem += TabControlMain_DrawItem;
            TabControlMain.SelectedIndexChanged += TabControlMain_SelectedIndexChanged;
            // 
            // TabPageGame
            // 
            TabPageGame.Controls.Add(TextBoxInfo);
            TabPageGame.Controls.Add(TextBoxGamePath);
            TabPageGame.Controls.Add(ButtonLanguageUpdate);
            TabPageGame.Controls.Add(ButtonSelectGameFolder);
            TabPageGame.Controls.Add(LabelLanguageName);
            TabPageGame.Controls.Add(LabelRepo);
            TabPageGame.Controls.Add(ButtonReset);
            TabPageGame.Controls.Add(ButtonDefault);
            TabPageGame.Controls.Add(TextBoxLanguage);
            TabPageGame.Controls.Add(TextBoxRepo);
            TabPageGame.Location = new Point(124, 4);
            TabPageGame.Name = "TabPageGame";
            TabPageGame.Padding = new Padding(3);
            TabPageGame.Size = new Size(470, 382);
            TabPageGame.TabIndex = 0;
            TabPageGame.UseVisualStyleBackColor = true;
            // 
            // TextBoxInfo
            // 
            TextBoxInfo.AcceptsReturn = true;
            TextBoxInfo.AcceptsTab = true;
            TextBoxInfo.Location = new Point(214, 84);
            TextBoxInfo.Multiline = true;
            TextBoxInfo.Name = "TextBoxInfo";
            TextBoxInfo.ReadOnly = true;
            TextBoxInfo.ScrollBars = ScrollBars.Vertical;
            TextBoxInfo.Size = new Size(249, 291);
            TextBoxInfo.TabIndex = 38;
            // 
            // TextBoxGamePath
            // 
            TextBoxGamePath.Font = new Font("Arial Nova", 7.8F, FontStyle.Regular, GraphicsUnit.Point, 204);
            TextBoxGamePath.Location = new Point(7, 61);
            TextBoxGamePath.MaximumSize = new Size(200, 65);
            TextBoxGamePath.MinimumSize = new Size(200, 65);
            TextBoxGamePath.Multiline = true;
            TextBoxGamePath.Name = "TextBoxGamePath";
            TextBoxGamePath.ReadOnly = true;
            TextBoxGamePath.ScrollBars = ScrollBars.Vertical;
            TextBoxGamePath.Size = new Size(200, 65);
            TextBoxGamePath.TabIndex = 37;
            TextBoxGamePath.TextChanged += TextBoxGamePath_TextChanged;
            TextBoxGamePath.DoubleClick += TextBoxGamePath_DoubleClick;
            // 
            // ButtonLanguageUpdate
            // 
            ButtonLanguageUpdate.Enabled = false;
            ButtonLanguageUpdate.Location = new Point(214, 7);
            ButtonLanguageUpdate.Margin = new Padding(4);
            ButtonLanguageUpdate.Name = "ButtonLanguageUpdate";
            ButtonLanguageUpdate.Size = new Size(249, 70);
            ButtonLanguageUpdate.TabIndex = 35;
            ButtonLanguageUpdate.Text = "Обновить локализацию";
            ButtonLanguageUpdate.UseVisualStyleBackColor = true;
            ButtonLanguageUpdate.Click += ButtonLanguageUpdate_Click;
            // 
            // ButtonSelectGameFolder
            // 
            ButtonSelectGameFolder.Location = new Point(7, 7);
            ButtonSelectGameFolder.Margin = new Padding(4);
            ButtonSelectGameFolder.Name = "ButtonSelectGameFolder";
            ButtonSelectGameFolder.Size = new Size(200, 47);
            ButtonSelectGameFolder.TabIndex = 34;
            ButtonSelectGameFolder.Text = "Выбрать папку";
            ButtonSelectGameFolder.UseVisualStyleBackColor = true;
            ButtonSelectGameFolder.Click += ButtonSelectGameFolder_Click;
            // 
            // ButtonReset
            // 
            ButtonReset.Enabled = false;
            ButtonReset.Location = new Point(7, 267);
            ButtonReset.Margin = new Padding(4);
            ButtonReset.Name = "ButtonReset";
            ButtonReset.Size = new Size(200, 50);
            ButtonReset.TabIndex = 31;
            ButtonReset.Text = "Удалить перевод";
            ButtonReset.UseVisualStyleBackColor = true;
            ButtonReset.Click += ButtonReset_Click;
            // 
            // ButtonDefault
            // 
            ButtonDefault.Location = new Point(7, 325);
            ButtonDefault.Margin = new Padding(4);
            ButtonDefault.Name = "ButtonDefault";
            ButtonDefault.Size = new Size(200, 50);
            ButtonDefault.TabIndex = 30;
            ButtonDefault.Text = "Сброс настроек";
            ButtonDefault.UseVisualStyleBackColor = true;
            ButtonDefault.Click += ButtonDefault_Click;
            // 
            // TextBoxLanguage
            // 
            TextBoxLanguage.AllowDrop = true;
            TextBoxLanguage.Font = new Font("Segoe UI", 12F);
            TextBoxLanguage.Location = new Point(7, 225);
            TextBoxLanguage.Margin = new Padding(4);
            TextBoxLanguage.Name = "TextBoxLanguage";
            TextBoxLanguage.Size = new Size(200, 34);
            TextBoxLanguage.TabIndex = 29;
            TextBoxLanguage.TextChanged += TextBoxLanguage_TextChanged;
            // 
            // TextBoxRepo
            // 
            TextBoxRepo.AllowDrop = true;
            TextBoxRepo.Font = new Font("Segoe UI", 12F);
            TextBoxRepo.Location = new Point(7, 158);
            TextBoxRepo.Margin = new Padding(4);
            TextBoxRepo.Name = "TextBoxRepo";
            TextBoxRepo.Size = new Size(200, 34);
            TextBoxRepo.TabIndex = 28;
            TextBoxRepo.TextChanged += TextBoxRepo_TextChanged;
            // 
            // TabPageMod
            // 
            TabPageMod.Location = new Point(124, 4);
            TabPageMod.Name = "TabPageMod";
            TabPageMod.Padding = new Padding(3);
            TabPageMod.Size = new Size(470, 382);
            TabPageMod.TabIndex = 2;
            TabPageMod.UseVisualStyleBackColor = true;
            // 
            // TabPageFile
            // 
            TabPageFile.Controls.Add(textBox1);
            TabPageFile.Controls.Add(ButtonCommentInsert);
            TabPageFile.Location = new Point(124, 4);
            TabPageFile.Name = "TabPageFile";
            TabPageFile.Padding = new Padding(3);
            TabPageFile.Size = new Size(470, 382);
            TabPageFile.TabIndex = 1;
            TabPageFile.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(6, 6);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.ScrollBars = ScrollBars.Vertical;
            textBox1.Size = new Size(268, 65);
            textBox1.TabIndex = 38;
            textBox1.Text = "Двойной клик для выбора папки";
            // 
            // ButtonCommentInsert
            // 
            ButtonCommentInsert.Enabled = false;
            ButtonCommentInsert.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 204);
            ButtonCommentInsert.Location = new Point(3, 77);
            ButtonCommentInsert.Name = "ButtonCommentInsert";
            ButtonCommentInsert.Size = new Size(230, 60);
            ButtonCommentInsert.TabIndex = 4;
            ButtonCommentInsert.Text = "Добавить комментарии";
            ButtonCommentInsert.UseVisualStyleBackColor = true;
            // 
            // TabPageExport
            // 
            TabPageExport.Location = new Point(124, 4);
            TabPageExport.Name = "TabPageExport";
            TabPageExport.Padding = new Padding(3);
            TabPageExport.Size = new Size(470, 382);
            TabPageExport.TabIndex = 3;
            TabPageExport.UseVisualStyleBackColor = true;
            // 
            // MenuStripMaster
            // 
            MenuStripMaster.ImageScalingSize = new Size(20, 20);
            MenuStripMaster.Items.AddRange(new ToolStripItem[] { toolStripMenuItem1, ToolStripMenuItemSettings, ToolStripMenuItemAbout });
            MenuStripMaster.Location = new Point(0, 0);
            MenuStripMaster.Name = "MenuStripMaster";
            MenuStripMaster.Size = new Size(622, 28);
            MenuStripMaster.TabIndex = 1;
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(14, 24);
            // 
            // ToolStripMenuItemSettings
            // 
            ToolStripMenuItemSettings.DropDownItems.AddRange(new ToolStripItem[] { ToolStripMenuItemSettingsLocUpdate });
            ToolStripMenuItemSettings.Name = "ToolStripMenuItemSettings";
            ToolStripMenuItemSettings.Size = new Size(98, 24);
            ToolStripMenuItemSettings.Text = "Настройки";
            // 
            // ToolStripMenuItemSettingsLocUpdate
            // 
            ToolStripMenuItemSettingsLocUpdate.DropDownItems.AddRange(new ToolStripItem[] { ToolStripMenuItemSelectLanguage });
            ToolStripMenuItemSettingsLocUpdate.Name = "ToolStripMenuItemSettingsLocUpdate";
            ToolStripMenuItemSettingsLocUpdate.Size = new Size(224, 26);
            ToolStripMenuItemSettingsLocUpdate.Text = "Локализация";
            // 
            // ToolStripMenuItemSelectLanguage
            // 
            ToolStripMenuItemSelectLanguage.DropDownItems.AddRange(new ToolStripItem[] { ToolStripMenuItemLanguage });
            ToolStripMenuItemSelectLanguage.Name = "ToolStripMenuItemSelectLanguage";
            ToolStripMenuItemSelectLanguage.Size = new Size(189, 26);
            ToolStripMenuItemSelectLanguage.Text = "Выбрать язык";
            ToolStripMenuItemSelectLanguage.Click += ToolStripMenuItemSelectLanguage_Click;
            // 
            // ToolStripMenuItemLanguage
            // 
            ToolStripMenuItemLanguage.Name = "ToolStripMenuItemLanguage";
            ToolStripMenuItemLanguage.Size = new Size(155, 26);
            ToolStripMenuItemLanguage.Text = "Не задан";
            // 
            // ToolStripMenuItemAbout
            // 
            ToolStripMenuItemAbout.DropDownItems.AddRange(new ToolStripItem[] { ToolStripMenuItemVersion, ToolStripMenuItemCreator, ToolStripMenuItemAutoUpdateCheck, ToolStripMenuItemCheckUpdate, ToolStripMenuItemGuide });
            ToolStripMenuItemAbout.Name = "ToolStripMenuItemAbout";
            ToolStripMenuItemAbout.Size = new Size(118, 24);
            ToolStripMenuItemAbout.Text = "О программе";
            // 
            // ToolStripMenuItemVersion
            // 
            ToolStripMenuItemVersion.Name = "ToolStripMenuItemVersion";
            ToolStripMenuItemVersion.Size = new Size(283, 26);
            ToolStripMenuItemVersion.Text = "Версия ";
            // 
            // ToolStripMenuItemCreator
            // 
            ToolStripMenuItemCreator.Name = "ToolStripMenuItemCreator";
            ToolStripMenuItemCreator.Size = new Size(283, 26);
            ToolStripMenuItemCreator.Text = "Автор";
            ToolStripMenuItemCreator.Click += ToolStripMenuItemCreator_Click;
            // 
            // ToolStripMenuItemAutoUpdateCheck
            // 
            ToolStripMenuItemAutoUpdateCheck.Name = "ToolStripMenuItemAutoUpdateCheck";
            ToolStripMenuItemAutoUpdateCheck.Size = new Size(283, 26);
            ToolStripMenuItemAutoUpdateCheck.Text = "Автопроверка обновлений";
            ToolStripMenuItemAutoUpdateCheck.Click += ToolStripMenuItemAutoUpdateCheck_Click;
            // 
            // ToolStripMenuItemCheckUpdate
            // 
            ToolStripMenuItemCheckUpdate.BackgroundImageLayout = ImageLayout.None;
            ToolStripMenuItemCheckUpdate.Image = Properties.Resources.wait_c;
            ToolStripMenuItemCheckUpdate.Name = "ToolStripMenuItemCheckUpdate";
            ToolStripMenuItemCheckUpdate.Size = new Size(283, 26);
            ToolStripMenuItemCheckUpdate.Text = "Проверить обновления";
            ToolStripMenuItemCheckUpdate.Click += ToolStripMenuItemCheckUpdate_Click;
            // 
            // ToolStripMenuItemGuide
            // 
            ToolStripMenuItemGuide.Name = "ToolStripMenuItemGuide";
            ToolStripMenuItemGuide.Size = new Size(283, 26);
            ToolStripMenuItemGuide.Text = "Инструкция";
            ToolStripMenuItemGuide.Click += ToolStripMenuItemGuide_Click;
            // 
            // Master
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(622, 433);
            Controls.Add(TabControlMain);
            Controls.Add(MenuStripMaster);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = MenuStripMaster;
            MaximumSize = new Size(640, 480);
            MinimumSize = new Size(640, 480);
            Name = "Master";
            Text = "Переведи это с RimLangKit и OliveWizard!";
            TabControlMain.ResumeLayout(false);
            TabPageGame.ResumeLayout(false);
            TabPageGame.PerformLayout();
            TabPageFile.ResumeLayout(false);
            TabPageFile.PerformLayout();
            MenuStripMaster.ResumeLayout(false);
            MenuStripMaster.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TabControl TabControlMain;
        private TabPage TabPageGame;
        private TabPage TabPageFile;
        private MenuStrip MenuStripMaster;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem файлToolStripMenuItem;
        private ToolStripMenuItem ToolStripMenuItemSettings;
        private ToolStripMenuItem ToolStripMenuItemAbout;
        private ToolStripMenuItem ToolStripMenuItemVersion;
        private ToolStripMenuItem ToolStripMenuItemAutoUpdateCheck;
        private ToolStripMenuItem ToolStripMenuItemCheckUpdate;
        private TabPage TabPageMod;
        private TabPage TabPageExport;
        private ToolStripMenuItem ToolStripMenuItemGuide;
        private ToolStripMenuItem ToolStripMenuItemCreator;
        private Button ButtonReset;
        private Button ButtonDefault;
        private TextBox TextBoxLanguage;
        private TextBox TextBoxRepo;
        private Button ButtonLanguageUpdate;
        private Button ButtonSelectGameFolder;
        private TextBox TextBoxGamePath;
        private TextBox TextBoxInfo;
        private ToolStripMenuItem ToolStripMenuItemSettingsLocUpdate;
        private ToolStripMenuItem ToolStripMenuItemSelectLanguage;
        private ToolStripMenuItem ToolStripMenuItemLanguage;
        private Button ButtonCommentInsert;
        private TextBox textBox1;
    }
}
