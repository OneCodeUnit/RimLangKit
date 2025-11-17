using RimLangKit.Checks;
using RimLangKit.Parsers;
using RimLangKit.Repositories;

namespace RimLangKit.Modules.AutoTranslation
{
    /// <summary>
    /// Модуль автоматического перевода с использованием базы данных переводов.
    /// </summary>
    /// <remarks>
    /// Предоставляет высокоуровневые методы для создания и использования базы данных переводов,
    /// объединяя функциональность парсера и репозитория.
    /// </remarks>
    public static class AutoTranslator
    {
        /// <summary>
        /// Загружает базу данных переводов и анализирует ее содержимое.
        /// </summary>
        /// <param name="databasePath">Полный путь к файлу базы данных.</param>
        /// <returns>
        /// Объект XmlError с результатом анализа:
        /// - IsValid=true и количество тегов при наличии данных.
        /// - IsValid=false и сообщение об ошибке при отсутствии данных.
        /// </returns>
        /// <remarks>
        /// Анализирует коллекцию "translated_tags" в базе данных.
        /// </remarks>
        public static XmlError LoadDatabase(string databasePath)
        {
            var repository = new TranslationRepository(databasePath);
            var analysisResult = repository.Analyze("translated_tags");
            return analysisResult;
        }

        /// <summary>
        /// Создает новую пустую базу данных переводов.
        /// </summary>
        /// <param name="databasePath">Полный путь к файлу базы данных.</param>
        /// <remarks>
        /// Если файл базы данных уже существует, он будет удален и создан заново.
        /// </remarks>
        public static void NewDatabase(string databasePath)
        {
            if (File.Exists(databasePath))
                File.Delete(databasePath);
            var repository = new TranslationRepository(databasePath);
        }

        /// <summary>
        /// Добавляет данные из XML-файла в базу данных переводов.
        /// </summary>
        /// <param name="databasePath">Полный путь к файлу базы данных.</param>
        /// <param name="currentFile">Полный путь к XML-файлу перевода для обработки.</param>
        /// <param name="collectionName">Имя коллекции в базе данных для сохранения тегов.</param>
        /// <param name="rewrite">true - перезаписывать существующие теги, false - добавлять только новые.</param>
        /// <returns>
        /// Объект XmlError с результатом операции:
        /// - IsValid=true и количество сохраненных тегов при успехе.
        /// - IsValid=false и сообщение об ошибке при неудаче.
        /// </returns>
        public static XmlError CreateDatabase(string databasePath, string currentFile, string collectionName, bool rewrite)
        {
            var repository = new TranslationRepository(databasePath);
            XmlError result = TranslationParser.ParseAndSaveToDatabase(currentFile, repository, collectionName, rewrite);
            return result;
        }

        /// <summary>
        /// Автоматически переводит XML-файл, используя базу данных переводов.
        /// </summary>
        /// <param name="databasePath">Полный путь к файлу базы данных.</param>
        /// <param name="currentFile">Полный путь к XML-файлу для перевода.</param>
        /// <param name="collectionName">Имя коллекции в базе данных для поиска переводов.</param>
        /// <returns>
        /// Объект XmlError с результатом операции:
        /// - IsValid=true и количество переведенных тегов при успехе.
        /// - IsValid=false и сообщение об ошибке при неудаче.
        /// </returns>
        public static XmlError TranslateFile(string databasePath, string currentFile, string collectionName)
        {
            var repository = new TranslationRepository(databasePath);
            XmlError result = TranslationParser.ParseAndTranslateTags(currentFile, repository, collectionName);
            return result;
        }
    }
}
