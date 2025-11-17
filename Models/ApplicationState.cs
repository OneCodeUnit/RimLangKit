namespace RimLangKit.Models
{
    /// <summary>
    /// Состояние приложения, содержащее все пути и настройки
    /// </summary>
    public class ApplicationState
    {
        /// <summary>
        /// Путь к папке с файлами для обработки
        /// </summary>
        public string DirectoryPath { get; set; } = string.Empty;

        /// <summary>
        /// Путь к папке игры RimWorld
        /// </summary>
        public string GamePath { get; set; } = string.Empty;

        /// <summary>
        /// Дополнительная папка для некоторых операций
        /// </summary>
        public string AdditionalFolder { get; set; } = string.Empty;

        /// <summary>
        /// Путь к файлу базы данных переводов
        /// </summary>
        public string SelectedDatabasePath { get; set; } = string.Empty;

        /// <summary>
        /// Папка с переводами для обновления базы данных
        /// </summary>
        public string TranslationFolder { get; set; } = string.Empty;

        /// <summary>
        /// Папка с модом для автоматического перевода
        /// </summary>
        public string AutoTranslateModFolder { get; set; } = string.Empty;

        /// <summary>
        /// Индекс последней выбранной вкладки
        /// </summary>
        public int LastSelectedTab { get; set; } = 0;

        /// <summary>
        /// Проверяет, установлен ли корректный путь к директории
        /// </summary>
        public bool IsDirectoryPathValid =>
            !string.IsNullOrEmpty(DirectoryPath) &&
            Directory.Exists(DirectoryPath);

        /// <summary>
        /// Проверяет, установлен ли корректный путь к игре
        /// </summary>
        public bool IsGamePathValid =>
            !string.IsNullOrEmpty(GamePath) &&
            Directory.Exists(GamePath);

        /// <summary>
        /// Проверяет, установлена ли дополнительная папка
        /// </summary>
        public bool IsAdditionalFolderSet =>
            !string.IsNullOrEmpty(AdditionalFolder) &&
            Directory.Exists(AdditionalFolder);

        /// <summary>
        /// Проверяет, установлен ли путь к базе данных
        /// </summary>
        public bool IsDatabasePathValid =>
            !string.IsNullOrEmpty(SelectedDatabasePath) &&
            File.Exists(SelectedDatabasePath);
    }
}
