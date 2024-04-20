using System.Xml.Linq;

namespace RimLangKit
{
    internal sealed class FileFix
    {
        static Dictionary<string, string> BrokenFiles = new();

        internal static bool FileFixProcessing(string currentFile)
        {
            // Позволяет избежать падения при загрузке сломанного .xml файла
            try
            {
                XDocument.Load(currentFile, LoadOptions.PreserveWhitespace);
            }
            catch
            {
                BrokenFiles.Add(currentFile, "Ошибка загрузки файла");
                return false;
            }

            XDocument xDoc = XDocument.Load(currentFile, LoadOptions.PreserveWhitespace);

            // Позволяет проверка декларации
            string? Declaration = xDoc.Declaration?.ToString();
            if (Declaration is null)
            {
                BrokenFiles.Add(currentFile, "Отсутствует декларация");
            }
            else if (!Declaration.StartsWith("<?xml version=\"1.0\" encoding=\"utf-8\"", StringComparison.OrdinalIgnoreCase))
            {
                BrokenFiles.Add(currentFile, "Ошибка декларации");
                return false;
            }

            // Технические файлы
            if (currentFile.EndsWith("About.xml", StringComparison.OrdinalIgnoreCase) || currentFile.EndsWith("LoadFolders.xml", StringComparison.OrdinalIgnoreCase)) 
            {
                return true;
            }

            // Позволяет избежать обработки пустого или не содержащего нужных тегов файла
            if (xDoc.Element("LanguageData") is null)
            {
                BrokenFiles.Add(currentFile, "Отсутствует тег LanguageData");
                return false;
            }

            //Перевод контекста в содержимое тега LanguageData
            XElement? root = xDoc.Element("LanguageData");
            if (root?.Elements() is null)
            {
                BrokenFiles.Add(currentFile, "Пустой тег LanguageData");
                return false;
            }
            return true;
        }

        internal static void WriteBrokenFiles(TextBox textBox)
        {
            if (BrokenFiles.Count == 0)
            {
                textBox.AppendText($"{Environment.NewLine}Не найдено сломанных файлов.");
                return;
            }

            StreamWriter sw = new(Path.Combine(Directory.GetCurrentDirectory() + "\\BrokenFiles.txt"), false);
            foreach (var file in BrokenFiles)
            {
                sw.WriteLine($"{file.Value}: {file.Key}");
            }
            sw.Close();
            textBox.AppendText($"{Environment.NewLine}Найдено {BrokenFiles.Count} сломанных файлов. Результат записан в файл BrokenFiles.txt папки {Directory.GetCurrentDirectory()}");
            BrokenFiles.Clear();
        }
    }
}