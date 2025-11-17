using LiteDB;
using RimLangKit.Checks;
using RimLangKit.Models;
using RimLangKit.Repositories;
using System.Xml.Linq;

namespace RimLangKit.Parsers
{
    /// <summary>
    /// Парсер XML-файлов переводов для извлечения и обработки тегов.
    /// </summary>
    /// <remarks>
    /// Извлекает теги из XML-файлов, связывает их с английскими комментариями
    /// и сохраняет в базу данных или применяет переводы к файлам.
    /// </remarks>
    public static class TranslationParser
    {
        /// <summary>
        /// Парсит XML-файл перевода и сохраняет теги в базу данных.
        /// </summary>
        /// <param name="currentFile">Полный путь к XML-файлу перевода.</param>
        /// <param name="repository">Репозиторий для сохранения тегов.</param>
        /// <param name="collectionName">Имя коллекции в базе данных.</param>
        /// <param name="rewrite">true - перезаписывать существующие теги, false - добавлять только новые.</param>
        /// <returns>
        /// Объект XmlError с результатом операции:
        /// - IsValid=true и количество сохраненных тегов при успехе.
        /// - IsValid=false и сообщение об ошибке при неудаче.
        /// </returns>
        /// <remarks>
        /// Тип тега (TagType) определяется по имени родительской директории файла.
        /// Извлекает английские комментарии формата "<!-- EN: ... -->" и связывает их с тегами.
        /// </remarks>
        public static XmlError ParseAndSaveToDatabase(string currentFile, TranslationRepository repository, string collectionName, bool rewrite)
        {
            var tagType = Path.GetFileName(Path.GetDirectoryName(currentFile));
            if (string.IsNullOrEmpty(tagType))
                tagType = "Unknown";
            var tags = ParseXmlFile(currentFile, tagType);
            if (tags.Count == 0)
                return new XmlError(false, "не найдено тегов или XML файл содержит ошибки");

            repository.SaveTags(tags, collectionName, rewrite);
            return new XmlError(true, $"{tags.Count} тегов сохранено в базу данных.");
        }

        /// <summary>
        /// Автоматически переводит теги в XML-файле, используя базу данных переводов.
        /// </summary>
        /// <param name="currentFile">Полный путь к XML-файлу для перевода.</param>
        /// <param name="repository">Репозиторий с базой данных переводов.</param>
        /// <param name="collectionName">Имя коллекции для поиска переводов.</param>
        /// <returns>
        /// Объект XmlError с результатом операции:
        /// - IsValid=true и количество переведенных тегов при успехе.
        /// - IsValid=false и сообщение об ошибке при неудаче.
        /// </returns>
        /// <remarks>
        /// Метод читает английский текст из тегов и ищет их переводы в базе данных.
        /// Найденные переводы применяются к файлу, который затем сохраняется.
        /// Тип тега (TagType) определяется по имени родительской директории файла.
        /// </remarks>
        public static XmlError ParseAndTranslateTags(string currentFile, TranslationRepository repository, string collectionName)
        {
            var tagType = Path.GetFileName(Path.GetDirectoryName(currentFile));
            if (string.IsNullOrEmpty(tagType))
                tagType = "Unknown";

            // Проверка XML на ошибки
            XmlError error = XmlErrorChecker.CheckXml(currentFile);
            if (!error.IsValid)
            {
                return error;
            }
            XDocument doc = XDocument.Load(currentFile);

            var nodes = doc.Root?.Nodes().ToList();
            if (nodes is null)
                return new XmlError(false, "Файл пустой или содержит ошибки");

            int counter = 0;
            foreach (var node in nodes)
            {
                // Если это элемент с данными и имеет значение
                if (node is XElement element)
                {
                    if (!string.IsNullOrWhiteSpace(element.Value))
                    {
                        string tagDef = element.Name.LocalName;
                        string tagText = element.Value;

                        RimTag tag = new(tagDef, tagText, tagType);
                        string result = repository.GetTag(tag, collectionName);
                        if (!string.IsNullOrEmpty(result))
                        {
                            element.Value = result;
                            counter++;
                        }
                    }
                }
            }
            doc.Save(currentFile);
            return new XmlError(true, $"Автоматически переведено {counter} тегов");
        }

        /// <summary>
        /// Парсит XML-файл и извлекает теги с английскими комментариями.
        /// </summary>
        /// <param name="currentFile">Полный путь к XML-файлу.</param>
        /// <param name="tagType">Тип тега (обычно имя родительской директории).</param>
        /// <returns>Список объектов RimTag с извлеченными данными.</returns>
        /// <remarks>
        /// Метод извлекает связки: английский комментарий (из <!-- EN: ... -->) → русский перевод (из тега).
        /// Минимальная длина комментария для обработки - 13 символов.
        /// Возвращает пустой список при ошибках XML.
        /// </remarks>
        private static List<RimTag> ParseXmlFile(string currentFile, string tagType)
        {
            var rimTags = new List<RimTag>();

            // Проверка XML на ошибки
            XmlError error = XmlErrorChecker.CheckXml(currentFile);
            if (!error.IsValid)
            {
                return rimTags;
            }

            XDocument doc = XDocument.Load(currentFile);
            string previousComment = string.Empty;

            var nodes = doc.Root?.Nodes().ToList();
            if (nodes is null)
                return rimTags;
            foreach (var node in nodes)
            {
                // Если это комментарий
                if (node is XComment comment)
                {
                    previousComment = comment.ToString();
                }
                // Если это элемент с данными и имеет значение
                else if (node is XElement element)
                {
                    if (!string.IsNullOrWhiteSpace(element.Value))
                    {
                        string tagDef = element.Name.LocalName;
                        string tagText = element.Value;
                        string tagComment = string.Empty;
                        if (previousComment.Length > 13) // Минимальная длина комментария <!-- EN: ... -->
                        {
                            tagComment = previousComment[9..^4].Trim();
                            var rimTag = new RimTag(tagDef, tagText, tagComment, tagType);
                            rimTags.Add(rimTag);
                        }
                    }

                    // Сброс комментария для следующего тега.
                    previousComment = string.Empty;
                }
            }
            return rimTags;
        }
    }
}