using RimLangKit.Checks;
using System.Xml;
using System.Xml.Linq;

namespace RimLangKit.Processors
{
    /// <summary>
    /// Создает базу переводов из существующих файлов и применяет их к новым файлам.
    /// </summary>
    /// <remarks>
    /// Использует связку английский текст → русский перевод для автоматического перевода повторяющихся фраз.
    /// </remarks>
    public static class PreTranslator
    {
        /// <summary>База данных переводов (английский текст - русский перевод).</summary>
        private static Dictionary<string, string> TranslationData = [];

        /// <summary>
        /// Строит базу данных переводов из файла с английскими комментариями и русским переводом.
        /// </summary>
        /// <param name="currentFile">Полный путь к XML-файлу перевода.</param>
        /// <returns>
        /// Кортеж, содержащий:
        /// - bool: true при успешном построении базы, false при отсутствии новых переводов.
        /// - string: Количество найденных переводов при успехе, сообщение об ошибке при неудаче.
        /// </returns>
        /// <remarks>
        /// Метод извлекает связки английский текст (из комментариев) → русский перевод (из тегов).
        /// Пропускает теги с именами длиннее 30 символов для экономии памяти.
        /// Удаляет неоднозначные переводы (когда один английский текст имеет разные переводы).
        /// </remarks>
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

        /// <summary>
        /// Применяет переводы из базы данных к указанному файлу.
        /// </summary>
        /// <param name="currentFile">Полный путь к XML-файлу для перевода.</param>
        /// <returns>
        /// Кортеж, содержащий:
        /// - bool: true при успешном применении переводов, false при ошибке.
        /// - string: Пустая строка при успехе, сообщение об ошибке при неудаче.
        /// </returns>
        /// <remarks>
        /// Метод ищет английский текст в значениях тегов и заменяет его на русский перевод из базы данных.
        /// </remarks>
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
    }
}