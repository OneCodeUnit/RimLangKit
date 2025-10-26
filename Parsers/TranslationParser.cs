using LiteDB;
using RimLangKit.Checks;
using RimLangKit.Models;
using RimLangKit.Repositories;
using System.Xml.Linq;

namespace RimLangKit.Parsers
{
    public static class TranslationParser
    {
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