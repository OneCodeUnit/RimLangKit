using RimLanguageCore.Misc;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace RimLanguageCore.Activities
{
    public static class PreTranslator
    {
        private static Dictionary<string, string> TranslationData = new();

        public static (bool, string) BuildDatabase(string currentFile)
        {
            (bool, string) result = XmlErrorChecker.XmlErrorCheck(currentFile);
            if (!result.Item1)
                return result;

            XDocument xDoc = XDocument.Load(currentFile, LoadOptions.PreserveWhitespace);
            XElement root = xDoc.Element("LanguageData");

            XmlReaderSettings settings = new() { DtdProcessing = DtdProcessing.Parse };
            XmlReader reader = XmlReader.Create(currentFile, settings);
            reader.MoveToContent();

            // База defName - комментарий
            Dictionary<string, string> commentData = new();
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
            Dictionary<string, string> textData = new();
            foreach (XElement node in root.Elements())
            {
                textData.TryAdd(node.Name.ToString(), node.Value.ToString());
            }

            // База комментарий - перевод
            foreach (var text in textData)
            {
                // Для экономии памяти считаем, что по достаточно длинному тексту совпадений быть не может
                if (text.Key.Length > 30)
                    continue;
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

            if (TranslationData.Count > 0)
                return (true, $"{TranslationData.Count}");
            else 
                return (false, "Не найдено нового перевода");
        }

        public static (bool, string) Translation(string currentFile)
        {
            (bool, string) result = XmlErrorChecker.XmlErrorCheck(currentFile);
            if (!result.Item1)
                return result;

            XDocument xDoc = XDocument.Load(currentFile, LoadOptions.PreserveWhitespace);
            XElement root = xDoc.Element("LanguageData");

            foreach (XElement node in root.Elements())
            {
                // Поиск перевода в словаре
                if (TranslationData.TryGetValue(node.Value, out string textValue))
                    node.Value = textValue;
            }

            // Сохранение файла
            xDoc.Save(currentFile);
            return (true, string.Empty);
        }
    }
}
