using RimLangKit.Checks;
using System.Xml;
using System.Xml.Linq;

namespace RimLangKit.Processors
{
    public static class PreTranslator
    {
        private static Dictionary<string, string> TranslationData = [];

        public static (bool, string) BuildDatabase(string currentFile)
        {
            (bool, string) result = XmlErrorChecker.XmlErrorCheck(currentFile);
            if (!result.Item1)
            {
                return result;
            }

            XDocument xDoc = XDocument.Load(currentFile, LoadOptions.PreserveWhitespace);
            XElement? root = xDoc.Element("LanguageData");

            XmlReaderSettings settings = new() { DtdProcessing = DtdProcessing.Parse };
            XmlReader reader = XmlReader.Create(currentFile, settings);
            reader.MoveToContent();

            // База defName - комментарий
            Dictionary<string, string> commentData = [];
            string value = string.Empty;
            bool hasValue = false;
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        string key = reader.Name.ToString();
                        if (hasValue)
                        {
                            commentData.TryAdd(key, value);
                        }
                        hasValue = false;
                        break;
                    case XmlNodeType.Comment:
                        try
                        {
                            value = reader.Value.ToString()[4..].Trim();
                        }
                        catch
                        {
                            return (false, "Ошибка поиска комментариев.");
                        }
                        hasValue = true;
                        break;
                }
            }

            // База defName - перевод
            Dictionary<string, string> textData = [];
            foreach (XElement node in root.Elements())
            {
                textData.TryAdd(node.Name.ToString(), node.Value.ToString());
            }

            // База комментарий - перевод
            foreach (var text in textData)
            {
                // Для экономии памяти считаем, что по достаточно длинному тексту совпадений быть не может
                if (text.Key.Length > 30)
                {
                    continue;
                }

                if (commentData.TryGetValue(text.Key, out string commentValue))
                {
                    bool unique = TranslationData.TryAdd(commentValue, text.Value);
                    // Удаляем неоднозначный перевод
                    if (!unique)
                    {
                        TranslationData.Remove(text.Key);
                    }
                }
            }

            return TranslationData.Count > 0 ? (true, $"{TranslationData.Count}") : (false, "Не найдено нового перевода");
        }

        public static (bool, string) Translation(string currentFile)
        {
            (bool, string) result = XmlErrorChecker.XmlErrorCheck(currentFile);
            if (!result.Item1)
            {
                return result;
            }

            XDocument xDoc = XDocument.Load(currentFile, LoadOptions.PreserveWhitespace);
            XElement? root = xDoc.Element("LanguageData");

            foreach (XElement node in root.Elements())
            {
                // Поиск перевода в словаре
                if (TranslationData.TryGetValue(node.Value, out string textValue))
                {
                    node.Value = textValue;
                }
            }

            // Сохранение файла
            xDoc.Save(currentFile);
            return (true, string.Empty);
        }

        /// <summary>
        /// Async версия BuildDatabase с CancellationToken
        /// </summary>
        public static async Task<(bool, string)> BuildDatabaseAsync(string currentFile, CancellationToken cancellationToken = default)
        {
            return await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                return BuildDatabase(currentFile);
            }, cancellationToken);
        }

        /// <summary>
        /// Async версия Translation с CancellationToken
        /// </summary>
        public static async Task<(bool, string)> TranslationAsync(string currentFile, CancellationToken cancellationToken = default)
        {
            return await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                return Translation(currentFile);
            }, cancellationToken);
        }

        /// <summary>
        /// Очистка статического словаря переводов
        /// </summary>
        public static void ClearTranslationData()
        {
            TranslationData.Clear();
        }
    }
}