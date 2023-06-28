namespace LanguageUpdate
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
            FolderButton = new Button();
            FolderTextBox = new TextBox();
            InfoTextBox = new TextBox();
            ButtonLanguageUpdate = new Button();
            RepoInput = new TextBox();
            LanguageInput = new TextBox();
            DefaultButton = new Button();
            ResetButton = new Button();
            SuspendLayout();
            // 
            // FolderButton
            // 
            FolderButton.Location = new Point(13, 59);
            FolderButton.Margin = new Padding(4);
            FolderButton.Name = "FolderButton";
            FolderButton.Size = new Size(200, 50);
            FolderButton.TabIndex = 0;
            FolderButton.Text = "Выбрать папку";
            FolderButton.UseVisualStyleBackColor = true;
            FolderButton.Click += FolderButton_Click;
            // 
            // FolderTextBox
            // 
            FolderTextBox.AllowDrop = true;
            FolderTextBox.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            FolderTextBox.Location = new Point(16, 17);
            FolderTextBox.Margin = new Padding(4);
            FolderTextBox.Name = "FolderTextBox";
            FolderTextBox.Size = new Size(753, 34);
            FolderTextBox.TabIndex = 1;
            FolderTextBox.TextChanged += FolderTextBox_TextChanged;
            // 
            // InfoTextBox
            // 
            InfoTextBox.AcceptsReturn = true;
            InfoTextBox.AcceptsTab = true;
            InfoTextBox.Location = new Point(221, 59);
            InfoTextBox.Margin = new Padding(4);
            InfoTextBox.Multiline = true;
            InfoTextBox.Name = "InfoTextBox";
            InfoTextBox.ReadOnly = true;
            InfoTextBox.ScrollBars = ScrollBars.Vertical;
            InfoTextBox.Size = new Size(548, 431);
            InfoTextBox.TabIndex = 4;
            InfoTextBox.Text = "Прогресс выполнения";
            // 
            // ButtonLanguageUpdate
            // 
            ButtonLanguageUpdate.Enabled = false;
            ButtonLanguageUpdate.Location = new Point(13, 117);
            ButtonLanguageUpdate.Margin = new Padding(4);
            ButtonLanguageUpdate.Name = "ButtonLanguageUpdate";
            ButtonLanguageUpdate.Size = new Size(200, 70);
            ButtonLanguageUpdate.TabIndex = 8;
            ButtonLanguageUpdate.Text = "Обновить локализацию";
            ButtonLanguageUpdate.UseVisualStyleBackColor = true;
            ButtonLanguageUpdate.Click += ButtonLanguageUpdate_Click;
            // 
            // RepoInput
            // 
            RepoInput.AllowDrop = true;
            RepoInput.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            RepoInput.Location = new Point(13, 298);
            RepoInput.Margin = new Padding(4);
            RepoInput.Name = "RepoInput";
            RepoInput.Size = new Size(200, 34);
            RepoInput.TabIndex = 9;
            RepoInput.TextChanged += RepoInput_TextChanged;
            // 
            // LanguageInput
            // 
            LanguageInput.AllowDrop = true;
            LanguageInput.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            LanguageInput.Location = new Point(13, 340);
            LanguageInput.Margin = new Padding(4);
            LanguageInput.Name = "LanguageInput";
            LanguageInput.Size = new Size(200, 34);
            LanguageInput.TabIndex = 10;
            LanguageInput.TextChanged += LanguageInput_TextChanged;
            // 
            // DefaultButton
            // 
            DefaultButton.Location = new Point(13, 440);
            DefaultButton.Margin = new Padding(4);
            DefaultButton.Name = "DefaultButton";
            DefaultButton.Size = new Size(200, 50);
            DefaultButton.TabIndex = 11;
            DefaultButton.Text = "Вернуть значения";
            DefaultButton.UseVisualStyleBackColor = true;
            DefaultButton.Click += DefaultButton_Click;
            // 
            // ResetButton
            // 
            ResetButton.Location = new Point(13, 382);
            ResetButton.Margin = new Padding(4);
            ResetButton.Name = "ResetButton";
            ResetButton.Size = new Size(200, 50);
            ResetButton.TabIndex = 12;
            ResetButton.Text = "Сбросить перевод";
            ResetButton.UseVisualStyleBackColor = true;
            ResetButton.Click += ResetButton_Click;
            // 
            // MainForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(782, 503);
            Controls.Add(ResetButton);
            Controls.Add(DefaultButton);
            Controls.Add(LanguageInput);
            Controls.Add(RepoInput);
            Controls.Add(ButtonLanguageUpdate);
            Controls.Add(InfoTextBox);
            Controls.Add(FolderTextBox);
            Controls.Add(FolderButton);
            Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4);
            MaximizeBox = false;
            Name = "MainForm";
            Text = "Обновление перевода игры от OliveWizard";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button FolderButton;
        private TextBox FolderTextBox;
        private TextBox InfoTextBox;
        private Button ButtonLanguageUpdate;
        private TextBox RepoInput;
        private TextBox LanguageInput;
        private Button DefaultButton;
        private Button ResetButton;
    }
}