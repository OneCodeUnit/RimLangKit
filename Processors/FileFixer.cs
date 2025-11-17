using RimLangKit.Checks;
using System.Xml.Linq;

namespace RimLangKit.Processors
{
    /// <summary>
    /// Проверяет XML-файлы на наличие ошибок и собирает информацию о проблемных файлах.
    /// </summary>
    /// <remarks>
    /// Проверяет корректность XML-структуры и наличие правильной декларации в файлах переводов.
    /// </remarks>
    public static class FileFixer
    {
        /// <summary>Словарь сломанных файлов (путь к файлу - описание ошибки).</summary>
        private static readonly Dictionary<string, string> BrokenFiles = [];

        /// <summary>
        /// Проверяет указанный файл на наличие ошибок XML и корректность декларации.
        /// </summary>
        /// <param name="currentFile">Полный путь к XML-файлу для проверки.</param>
        /// <returns>
        /// Кортеж, содержащий:
        /// - bool: всегда true (метод не прерывает обработку при обнаружении ошибок).
        /// - string: всегда пустая строка.
        /// </returns>
        /// <remarks>
        /// Метод пропускает технические файлы About.xml и LoadFolders.xml.
        /// Проверяет наличие и корректность XML-декларации (должна быть "<?xml version="1.0" encoding="utf-8"?>").
        /// Сломанные файлы добавляются в словарь BrokenFiles для последующей записи.
        /// </remarks>
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
            string? Declaration = xDoc.Declaration?.ToString();
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

        /// <summary>
        /// Записывает информацию о сломанных файлах в текстовый файл.
        /// </summary>
        /// <returns>Сообщение о количестве найденных сломанных файлов и расположении отчета.</returns>
        /// <remarks>
        /// Создает файл BrokenFiles.txt в текущей директории, содержащий список проблемных файлов и их ошибок.
        /// Очищает словарь BrokenFiles после записи.
        /// </remarks>
        public static string BrokenFilesWriterActivity()
        {
            if (BrokenFiles.Count == 0)
            {
                return "Не найдено сломанных файлов";
            }

            string currentDirectory = Directory.GetCurrentDirectory();
            StreamWriter sw = new(Path.Combine(currentDirectory, "BrokenFiles.txt"), false);
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