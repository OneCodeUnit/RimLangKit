using System.Text;
using System.Text.RegularExpressions;

namespace RimLangKit.Processors
{
    /// <summary>
    /// Исправляет кодировку и форматирование текстовых файлов.
    /// </summary>
    /// <remarks>
    /// Приводит файлы к стандарту UTF-8 с BOM, заменяет табуляции на пробелы и окончания строк на CRLF.
    /// </remarks>
    public static class EncodingFixer
    {
        /// <summary>
        /// Исправляет кодировку и форматирование указанного файла.
        /// </summary>
        /// <param name="currentFile">Полный путь к файлу для обработки.</param>
        /// <returns>
        /// Кортеж, содержащий:
        /// - bool: всегда true после успешной обработки.
        /// - string: всегда пустая строка.
        /// </returns>
        /// <remarks>
        /// Метод выполняет следующие операции:
        /// - Заменяет все табуляции на два пробела
        /// - Приводит окончания строк к формату CRLF (\r\n)
        /// - Сохраняет файл в кодировке UTF-8 с BOM (Byte Order Mark)
        /// </remarks>
        public static (bool, string) EncodingFixerActivity(string currentFile)
        {
            string text = File.ReadAllText(currentFile);
            // Замена табов на пробелы
            text = Regex.Replace(text, "\t+", "  ");
            // Замена окончания строки на CRLF
            text = Regex.Replace(text, "(?<!\r)\n", "\r\n");

            // Сохранение c кодировкой utf-8 + BOM
            var data = Encoding.UTF8.GetBytes(text);
            var result = Encoding.UTF8.GetPreamble().Concat(data).ToArray();
            var encoder = new UTF8Encoding(true);
            text = encoder.GetString(result);

            File.WriteAllText(currentFile, text);

            return (true, string.Empty);
        }
    }
}