using System.Xml.Linq;

namespace RimLangKit
{
    internal static class UniqueNames
    {
        internal static (bool, string) UniqueNamesProcessing(string CurrentFile)
        {
            string error = string.Empty;
            //Позволяет избежать падения при загрузке сломанного .xml файла
            try
            {
                XDocument.Load(CurrentFile, LoadOptions.PreserveWhitespace);
            }
            catch
            {
                error = "Не удалось загрузить файл " + CurrentFile;
                return (false, error);
            }

            XDocument xDoc = XDocument.Load(CurrentFile, LoadOptions.PreserveWhitespace);
            //Позволяет избежать обработки пустого или не содержащего нужных тегов файла
            if (xDoc.Element("LanguageData") is null)
            {
                error = "Не удалось найти LanguageData " + CurrentFile;
                return (false, error);
            }
            //Перевод контекста в содержимое тега LanguageData
            XElement? root = xDoc.Element("LanguageData");
            if (root?.Elements() is null)
            {
                error = "Тег LanguageData пуст " + CurrentFile;
                return (false, error);
            }

            //Разделение пути до файла на части и поиск нужной
            string[] Path = CurrentFile.Split('\\');
            StringComparison Condition = StringComparison.OrdinalIgnoreCase;
            string FolderName = string.Empty;
            for (int i = 0; i < Path.Length; i++)
            {
                if (Path[i].Equals("Keyed", Condition) || Path[i].Equals("DefInjected", Condition))
                {
                    FolderName = Path[i - 3];
                    break;
                }
            }

            //Сохранение с добавлением имени папки. Имя папки то, которое до "Languages"
            string NewPath = $"{CurrentFile.Remove(CurrentFile.Length - 4)}_{FolderName}.xml";
            File.Move(CurrentFile, NewPath);
            return (true, error);
        }
    }
}
