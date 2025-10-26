using System.Xml.Linq;

namespace RimLangKit.Checks
{
    public static class XmlErrorChecker
    {
        public static (bool, string) XmlErrorCheck(string currentFile)
        {
            XDocument xDoc;
            // Позволяет избежать падения при загрузке сломанного .xml файла
            try
            {
                xDoc = XDocument.Load(currentFile, LoadOptions.PreserveWhitespace);
            }
            catch (Exception ex)
            {
                return (false, $"Не удалось загрузить файл: {ex.Message}");
            }

            // Позволяет избежать обработки пустого или не содержащего нужных тегов файла
            XElement? root = xDoc.Element("LanguageData");
            if (root is null)
            {
                return (false, "Не удалось найти корневой элемент 'LanguageData'");
            }

            // Позволяет избежать обработки пустого тега LanguageData
            if (root.Elements().Any())
            {
                return (true, string.Empty);
            }
            else
            {
                return (false, "Корневой элемент 'LanguageData' не содержит дочерних элементов.");
            }
        }

        // Новая версия, использующая XmlError вместо кортежей. Перевести логику на неё
        public static XmlError CheckXml(string currentFile)
        {
            XDocument xDoc;
            // Позволяет избежать падения при загрузке сломанного .xml файла
            try
            {
                xDoc = XDocument.Load(currentFile, LoadOptions.PreserveWhitespace);
            }
            catch (Exception ex)
            {
                return new XmlError (false, $"Не удалось загрузить файл: {ex.Message}");
            }

            // Позволяет избежать обработки пустого или не содержащего нужных тегов файла
            XElement? root = xDoc.Element("LanguageData");
            if (root is null)
            {
                return new XmlError(false, "Не удалось найти корневой элемент 'LanguageData'");
            }

            // Позволяет избежать обработки пустого тега LanguageData
            if (root.Elements().Any())
            {
                return new XmlError (true, string.Empty);
            }
            else
            {
                return new XmlError (false, "Корневой элемент 'LanguageData' не содержит дочерних элементов.");
            }
        }
    }
}