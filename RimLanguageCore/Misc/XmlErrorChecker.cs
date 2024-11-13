using System.Xml.Linq;

namespace RimLanguageCore.Misc
{
    public static class XmlErrorChecker
    {
        public static (bool, string) XmlErrorCheck(string currentFile)
        {
            // Позволяет избежать падения при загрузке сломанного .xml файла
            try
            {
                XDocument.Load(currentFile, LoadOptions.PreserveWhitespace);
            }
            catch
            {
                return (false, "Не удалось загрузить файл");
            }

            XDocument xDoc = XDocument.Load(currentFile, LoadOptions.PreserveWhitespace);
            // Позволяет избежать обработки пустого или не содержащего нужных тегов файла
            if (xDoc.Element("LanguageData") is null)
            {
                return (false, "Не удалось найти LanguageData");
            }
            // Перевод контекста в содержимое тега LanguageData
            XElement root = xDoc.Element("LanguageData");
            return root?.Elements() is null ? (false, "Тег LanguageData пуст") : (true, string.Empty);
        }
    }
}
