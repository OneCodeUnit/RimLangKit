using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace RimLanguageCore.Activities
{
    public static class FileFixer
    {
        private static Dictionary<string, string> BrokenFiles = new();

        public static (bool, string) FileFixerActivity(string currentFile)
        {
            string error;

            // Позволяет избежать падения при загрузке сломанного .xml файла
            try
            {
                XDocument.Load(currentFile, LoadOptions.PreserveWhitespace);
            }
            catch
            {
                error = "Ошибка загрузки файла";
                BrokenFiles.Add(currentFile, error);
                return (false, error);
            }

            XDocument xDoc = XDocument.Load(currentFile, LoadOptions.PreserveWhitespace);

            // Проверка декларации
            string Declaration = xDoc.Declaration?.ToString();
            if (Declaration is null)
            {
                error = "Отсутствует декларация";
                BrokenFiles.Add(currentFile, error);
                return (false, error);
            }
            else if (!Declaration.StartsWith("<?xml version=\"1.0\" encoding=\"utf-8\"", StringComparison.OrdinalIgnoreCase))
            {
                error = "Ошибка декларации";
                BrokenFiles.Add(currentFile, error);
                return (false, error);
            }

            // Технические файлы
            if (currentFile.EndsWith("About.xml", StringComparison.OrdinalIgnoreCase) || currentFile.EndsWith("LoadFolders.xml", StringComparison.OrdinalIgnoreCase))
            {
                return (true, string.Empty);
            }

            // Позволяет избежать обработки пустого или не содержащего нужных тегов файла
            if (xDoc.Element("LanguageData") is null)
            {
                error = "Отсутствует тег LanguageData";
                BrokenFiles.Add(currentFile, error);
                return (false, error);
            }

            //Перевод контекста в содержимое тега LanguageData
            XElement? root = xDoc.Element("LanguageData");
            if (root?.Elements() is null)
            {
                error = "Пустой тег LanguageData";
                BrokenFiles.Add(currentFile, error);
                return (false, error);
            }
            return (true, string.Empty);
        }

        // Запись изменений
        public static string BrokenFilesWriterActivity()
        {
            if (BrokenFiles.Count == 0)
                return ("Не найдено сломанных файлов");

            string currentDirectory = Directory.GetCurrentDirectory();
            StreamWriter sw = new(Path.Combine(currentDirectory + "\\BrokenFiles.txt"), false);
            foreach (var file in BrokenFiles)
            {
                sw.WriteLine($"{file.Value}: {file.Key}");
            }
            sw.Close();
            string result = $"Найдено {BrokenFiles.Count} сломанных файлов. Результат записан в файл BrokenFiles.txt папки {currentDirectory}";
            BrokenFiles.Clear();
            return result;
        }
    }
}