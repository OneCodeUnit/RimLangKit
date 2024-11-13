using RimLanguageCore.Misc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace RimLanguageCore.Activities
{
    public static class FileFixer
    {
        private static readonly Dictionary<string, string> BrokenFiles = new();

        public static (bool, string) FileFixerActivity(string currentFile)
        {
            // Технические файлы
            if (currentFile.EndsWith("About.xml", StringComparison.OrdinalIgnoreCase) || currentFile.EndsWith("LoadFolders.xml", StringComparison.OrdinalIgnoreCase))
            {
                return (true, string.Empty);
            }

            (bool, string) result = XmlErrorChecker.XmlErrorCheck(currentFile);
            if (!result.Item1)
            {
                BrokenFiles.Add(currentFile, result.Item2);
                return (true, string.Empty);
            }

            XDocument xDoc = XDocument.Load(currentFile, LoadOptions.PreserveWhitespace);

            // Проверка декларации
            string Declaration = xDoc.Declaration?.ToString();
            if (Declaration is null)
            {
                BrokenFiles.Add(currentFile, "Отсутствует декларация");
                return (true, string.Empty);
            }
            else if (!Declaration.StartsWith("<?xml version=\"1.0\" encoding=\"utf-8\"", StringComparison.OrdinalIgnoreCase))
            {
                BrokenFiles.Add(currentFile, "Ошибка декларации");
                return (true, string.Empty);
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