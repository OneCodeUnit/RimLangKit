using System.Xml.Linq;

namespace RimLangKit
{
    internal static class UniqueNames
    {
        internal static (bool, string) UniqueNamesProcessing(string currentFile)
        {
            string error = string.Empty;
            //Позволяет избежать падения при загрузке сломанного .xml файла
            try
            {
                XDocument.Load(currentFile, LoadOptions.PreserveWhitespace);
            }
            catch
            {
                error = "Не удалось загрузить файл " + currentFile;
                return (false, error);
            }

            XDocument xDoc = XDocument.Load(currentFile, LoadOptions.PreserveWhitespace);
            //Позволяет избежать обработки пустого или не содержащего нужных тегов файла
            if (xDoc.Element("LanguageData") is null)
            {
                error = "Не удалось найти LanguageData " + currentFile;
                return (false, error);
            }
            //Перевод контекста в содержимое тега LanguageData
            XElement? root = xDoc.Element("LanguageData");
            if (root?.Elements() is null)
            {
                error = "Тег LanguageData пуст " + currentFile;
                return (false, error);
            }

            //Разделение пути до файла на части и поиск нужной
            string[] path = currentFile.Split('\\');
            StringComparison condition = StringComparison.OrdinalIgnoreCase;
            string folderName = string.Empty;
            for (int i = 0; i < path.Length; i++)
            {
                if (path[i].Equals("Keyed", condition) || path[i].Equals("DefInjected", condition))
                {
                    folderName = path[i - 3];
                    break;
                }
            }

            //Сохранение с добавлением имени папки. Имя папки то, которое до "Languages"
            string newPath = $"{currentFile.Remove(currentFile.Length - 4)}_{folderName}.xml";
            File.Move(currentFile, newPath);
            return (true, error);
        }
    }
}
