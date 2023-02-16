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
            this.FolderButton = new System.Windows.Forms.Button();
            this.FolderTextBox = new System.Windows.Forms.TextBox();
            this.LabelCheck = new System.Windows.Forms.Label();
            this.ButtonENGIsert = new System.Windows.Forms.Button();
            this.InfoTextBox = new System.Windows.Forms.TextBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.ButtonUniqueNames = new System.Windows.Forms.Button();
            this.ButtonDictionary = new System.Windows.Forms.Button();
            this.ButtonLanguageUpdate = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // FolderButton
            // 
            this.FolderButton.Location = new System.Drawing.Point(560, 12);
            this.FolderButton.Name = "FolderButton";
            this.FolderButton.Size = new System.Drawing.Size(230, 60);
            this.FolderButton.TabIndex = 0;
            this.FolderButton.Text = "Выбрать папку";
            this.FolderButton.UseVisualStyleBackColor = true;
            this.FolderButton.Click += new System.EventHandler(this.FolderButton_Click);
            // 
            // FolderTextBox
            // 
            this.FolderTextBox.AllowDrop = true;
            this.FolderTextBox.Location = new System.Drawing.Point(12, 12);
            this.FolderTextBox.MaximumSize = new System.Drawing.Size(540, 27);
            this.FolderTextBox.MinimumSize = new System.Drawing.Size(220, 27);
            this.FolderTextBox.Name = "FolderTextBox";
            this.FolderTextBox.Size = new System.Drawing.Size(540, 27);
            this.FolderTextBox.TabIndex = 1;
            this.FolderTextBox.TextChanged += new System.EventHandler(this.FolderTextBox_TextChanged);
            // 
            // LabelCheck
            // 
            this.LabelCheck.AutoSize = true;
            this.LabelCheck.Location = new System.Drawing.Point(12, 42);
            this.LabelCheck.Name = "LabelCheck";
            this.LabelCheck.Size = new System.Drawing.Size(122, 20);
            this.LabelCheck.TabIndex = 2;
            this.LabelCheck.Text = "Выберите папку";
            this.LabelCheck.TextChanged += new System.EventHandler(this.LabelCheck_TextChanged);
            // 
            // ButtonENGIsert
            // 
            this.ButtonENGIsert.Enabled = false;
            this.ButtonENGIsert.Location = new System.Drawing.Point(12, 77);
            this.ButtonENGIsert.Name = "ButtonENGIsert";
            this.ButtonENGIsert.Size = new System.Drawing.Size(230, 60);
            this.ButtonENGIsert.TabIndex = 3;
            this.ButtonENGIsert.Text = "Добавить комментарии";
            this.ButtonENGIsert.UseVisualStyleBackColor = true;
            this.ButtonENGIsert.Click += new System.EventHandler(this.ButtonENGIsert_Click);
            // 
            // InfoTextBox
            // 
            this.InfoTextBox.AcceptsReturn = true;
            this.InfoTextBox.AcceptsTab = true;
            this.InfoTextBox.Location = new System.Drawing.Point(560, 112);
            this.InfoTextBox.MaximumSize = new System.Drawing.Size(230, 329);
            this.InfoTextBox.Multiline = true;
            this.InfoTextBox.Name = "InfoTextBox";
            this.InfoTextBox.ReadOnly = true;
            this.InfoTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.InfoTextBox.Size = new System.Drawing.Size(230, 329);
            this.InfoTextBox.TabIndex = 4;
            this.InfoTextBox.Text = "Прогресс выполнения";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(560, 77);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(230, 30);
            this.progressBar.TabIndex = 5;
            // 
            // ButtonUniqueNames
            // 
            this.ButtonUniqueNames.Enabled = false;
            this.ButtonUniqueNames.Location = new System.Drawing.Point(12, 143);
            this.ButtonUniqueNames.Name = "ButtonUniqueNames";
            this.ButtonUniqueNames.Size = new System.Drawing.Size(230, 60);
            this.ButtonUniqueNames.TabIndex = 6;
            this.ButtonUniqueNames.Text = "Избавиться от одинаковых имён файлов";
            this.ButtonUniqueNames.UseVisualStyleBackColor = true;
            this.ButtonUniqueNames.Click += new System.EventHandler(this.ButtonUniqueNames_Click);
            // 
            // ButtonDictionary
            // 
            this.ButtonDictionary.Enabled = false;
            this.ButtonDictionary.Location = new System.Drawing.Point(12, 209);
            this.ButtonDictionary.Name = "ButtonDictionary";
            this.ButtonDictionary.Size = new System.Drawing.Size(230, 60);
            this.ButtonDictionary.TabIndex = 7;
            this.ButtonDictionary.Text = "Транскрипция имён";
            this.ButtonDictionary.UseVisualStyleBackColor = true;
            this.ButtonDictionary.Click += new System.EventHandler(this.ButtonDictionary_Click);
            // 
            // ButtonLanguageUpdate
            // 
            this.ButtonLanguageUpdate.Enabled = false;
            this.ButtonLanguageUpdate.Location = new System.Drawing.Point(12, 275);
            this.ButtonLanguageUpdate.Name = "ButtonLanguageUpdate";
            this.ButtonLanguageUpdate.Size = new System.Drawing.Size(230, 60);
            this.ButtonLanguageUpdate.TabIndex = 8;
            this.ButtonLanguageUpdate.Text = "Обновить локализацию";
            this.ButtonLanguageUpdate.UseVisualStyleBackColor = true;
            this.ButtonLanguageUpdate.Click += new System.EventHandler(this.ButtonLanguageUpdate_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(802, 453);
            this.Controls.Add(this.ButtonLanguageUpdate);
            this.Controls.Add(this.ButtonDictionary);
            this.Controls.Add(this.ButtonUniqueNames);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.InfoTextBox);
            this.Controls.Add(this.ButtonENGIsert);
            this.Controls.Add(this.LabelCheck);
            this.Controls.Add(this.FolderTextBox);
            this.Controls.Add(this.FolderButton);
            this.MaximumSize = new System.Drawing.Size(820, 500);
            this.MinimumSize = new System.Drawing.Size(410, 250);
            this.Name = "MainForm";
            this.Text = "RimWorld Language Kit by OliveWizard";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button FolderButton;
        private TextBox FolderTextBox;
        private Label LabelCheck;
        private Button ButtonENGIsert;
        private TextBox InfoTextBox;
        private ProgressBar progressBar;
        private Button ButtonUniqueNames;
        private Button ButtonDictionary;
        private Button ButtonLanguageUpdate;
    }
}