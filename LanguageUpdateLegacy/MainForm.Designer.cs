using System.Drawing;
using System.Windows.Forms;

namespace LanguageUpdateLegacy
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.FolderButton = new System.Windows.Forms.Button();
            this.FolderTextBox = new System.Windows.Forms.TextBox();
            this.InfoTextBox = new System.Windows.Forms.TextBox();
            this.ButtonLanguageUpdate = new System.Windows.Forms.Button();
            this.RepoInput = new System.Windows.Forms.TextBox();
            this.LanguageInput = new System.Windows.Forms.TextBox();
            this.DefaultButton = new System.Windows.Forms.Button();
            this.ResetButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // FolderButton
            // 
            this.FolderButton.Location = new System.Drawing.Point(13, 59);
            this.FolderButton.Margin = new System.Windows.Forms.Padding(4);
            this.FolderButton.Name = "FolderButton";
            this.FolderButton.Size = new System.Drawing.Size(200, 50);
            this.FolderButton.TabIndex = 0;
            this.FolderButton.Text = "Выбрать папку";
            this.FolderButton.UseVisualStyleBackColor = true;
            this.FolderButton.Click += new System.EventHandler(this.FolderButton_Click);
            // 
            // FolderTextBox
            // 
            this.FolderTextBox.AllowDrop = true;
            this.FolderTextBox.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.FolderTextBox.Location = new System.Drawing.Point(16, 17);
            this.FolderTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.FolderTextBox.Name = "FolderTextBox";
            this.FolderTextBox.Size = new System.Drawing.Size(753, 34);
            this.FolderTextBox.TabIndex = 1;
            this.FolderTextBox.TextChanged += new System.EventHandler(this.FolderTextBox_TextChanged);
            // 
            // InfoTextBox
            // 
            this.InfoTextBox.AcceptsReturn = true;
            this.InfoTextBox.AcceptsTab = true;
            this.InfoTextBox.Location = new System.Drawing.Point(221, 59);
            this.InfoTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.InfoTextBox.Multiline = true;
            this.InfoTextBox.Name = "InfoTextBox";
            this.InfoTextBox.ReadOnly = true;
            this.InfoTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.InfoTextBox.Size = new System.Drawing.Size(548, 431);
            this.InfoTextBox.TabIndex = 4;
            this.InfoTextBox.Text = "Прогресс выполнения";
            // 
            // ButtonLanguageUpdate
            // 
            this.ButtonLanguageUpdate.Enabled = false;
            this.ButtonLanguageUpdate.Location = new System.Drawing.Point(13, 117);
            this.ButtonLanguageUpdate.Margin = new System.Windows.Forms.Padding(4);
            this.ButtonLanguageUpdate.Name = "ButtonLanguageUpdate";
            this.ButtonLanguageUpdate.Size = new System.Drawing.Size(200, 70);
            this.ButtonLanguageUpdate.TabIndex = 8;
            this.ButtonLanguageUpdate.Text = "Обновить локализацию";
            this.ButtonLanguageUpdate.UseVisualStyleBackColor = true;
            this.ButtonLanguageUpdate.Click += new System.EventHandler(this.ButtonLanguageUpdate_Click);
            // 
            // RepoInput
            // 
            this.RepoInput.AllowDrop = true;
            this.RepoInput.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.RepoInput.Location = new System.Drawing.Point(13, 298);
            this.RepoInput.Margin = new System.Windows.Forms.Padding(4);
            this.RepoInput.Name = "RepoInput";
            this.RepoInput.Size = new System.Drawing.Size(200, 34);
            this.RepoInput.TabIndex = 9;
            this.RepoInput.TextChanged += new System.EventHandler(this.RepoInput_TextChanged);
            // 
            // LanguageInput
            // 
            this.LanguageInput.AllowDrop = true;
            this.LanguageInput.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.LanguageInput.Location = new System.Drawing.Point(13, 340);
            this.LanguageInput.Margin = new System.Windows.Forms.Padding(4);
            this.LanguageInput.Name = "LanguageInput";
            this.LanguageInput.Size = new System.Drawing.Size(200, 34);
            this.LanguageInput.TabIndex = 10;
            this.LanguageInput.TextChanged += new System.EventHandler(this.LanguageInput_TextChanged);
            // 
            // DefaultButton
            // 
            this.DefaultButton.Location = new System.Drawing.Point(13, 440);
            this.DefaultButton.Margin = new System.Windows.Forms.Padding(4);
            this.DefaultButton.Name = "DefaultButton";
            this.DefaultButton.Size = new System.Drawing.Size(200, 50);
            this.DefaultButton.TabIndex = 11;
            this.DefaultButton.Text = "Вернуть значения";
            this.DefaultButton.UseVisualStyleBackColor = true;
            this.DefaultButton.Click += new System.EventHandler(this.DefaultButton_Click);
            // 
            // ResetButton
            // 
            this.ResetButton.Location = new System.Drawing.Point(13, 382);
            this.ResetButton.Margin = new System.Windows.Forms.Padding(4);
            this.ResetButton.Name = "ResetButton";
            this.ResetButton.Size = new System.Drawing.Size(200, 50);
            this.ResetButton.TabIndex = 12;
            this.ResetButton.Text = "Сбросить перевод";
            this.ResetButton.UseVisualStyleBackColor = true;
            this.ResetButton.Click += new System.EventHandler(this.ResetButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(782, 503);
            this.Controls.Add(this.ResetButton);
            this.Controls.Add(this.DefaultButton);
            this.Controls.Add(this.LanguageInput);
            this.Controls.Add(this.RepoInput);
            this.Controls.Add(this.ButtonLanguageUpdate);
            this.Controls.Add(this.InfoTextBox);
            this.Controls.Add(this.FolderTextBox);
            this.Controls.Add(this.FolderButton);
            this.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Обновление перевода игры от OliveWizard";
            this.ResumeLayout(false);
            this.PerformLayout();

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

