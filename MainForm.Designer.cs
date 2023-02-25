namespace RTK
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
            FolderButton = new Button();
            FolderTextBox = new TextBox();
            LabelCheck = new Label();
            ButtonENGIsert = new Button();
            InfoTextBox = new TextBox();
            ButtonUniqueNames = new Button();
            ButtonDictionary = new Button();
            ButtonLanguageUpdate = new Button();
            SuspendLayout();
            // 
            // FolderButton
            // 
            FolderButton.Location = new Point(322, 47);
            FolderButton.Name = "FolderButton";
            FolderButton.Size = new Size(230, 60);
            FolderButton.TabIndex = 0;
            FolderButton.Text = "Выбрать папку";
            FolderButton.UseVisualStyleBackColor = true;
            FolderButton.Click += FolderButton_Click;
            // 
            // FolderTextBox
            // 
            FolderTextBox.AllowDrop = true;
            FolderTextBox.Location = new Point(12, 12);
            FolderTextBox.MaximumSize = new Size(540, 27);
            FolderTextBox.MinimumSize = new Size(220, 27);
            FolderTextBox.Name = "FolderTextBox";
            FolderTextBox.Size = new Size(540, 27);
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
            InfoTextBox.Location = new Point(248, 113);
            InfoTextBox.Multiline = true;
            InfoTextBox.Name = "InfoTextBox";
            InfoTextBox.ReadOnly = true;
            InfoTextBox.ScrollBars = ScrollBars.Vertical;
            InfoTextBox.Size = new Size(304, 222);
            InfoTextBox.TabIndex = 4;
            InfoTextBox.Text = "Прогресс выполнения";
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
            // ButtonLanguageUpdate
            // 
            ButtonLanguageUpdate.Enabled = false;
            ButtonLanguageUpdate.Location = new Point(12, 275);
            ButtonLanguageUpdate.Name = "ButtonLanguageUpdate";
            ButtonLanguageUpdate.Size = new Size(230, 60);
            ButtonLanguageUpdate.TabIndex = 8;
            ButtonLanguageUpdate.Text = "Обновить локализацию";
            ButtonLanguageUpdate.UseVisualStyleBackColor = true;
            ButtonLanguageUpdate.Click += ButtonLanguageUpdate_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            ClientSize = new Size(566, 347);
            Controls.Add(ButtonLanguageUpdate);
            Controls.Add(ButtonDictionary);
            Controls.Add(ButtonUniqueNames);
            Controls.Add(InfoTextBox);
            Controls.Add(ButtonENGIsert);
            Controls.Add(LabelCheck);
            Controls.Add(FolderTextBox);
            Controls.Add(FolderButton);
            MaximumSize = new Size(584, 394);
            MinimumSize = new Size(584, 394);
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
        private Button ButtonLanguageUpdate;
    }
}