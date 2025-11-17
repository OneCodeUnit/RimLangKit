using System.Globalization;
using System.Xml.Linq;
using System.Xml;
using RimLangKit.Checks;

namespace RimLangKit.Processors
{
    /// <summary>
    /// Находит различия между файлами переводов и файлами модов.
    /// </summary>
    /// <remarks>
    /// Сравнивает английские комментарии в файлах перевода с содержимым файлов мода,
    /// чтобы обнаружить изменения, добавления и удаления.
    /// </remarks>
    public class ChangesFinder
    {
        /// <summary>Словарь данных из файлов перевода (ключ тега - английский комментарий).</summary>
        private static Dictionary<string, string> TranslationData = [];
        /// <summary>Словарь данных из файлов мода (ключ тега - значение).</summary>
        private static Dictionary<string, string> ModData = [];
        /// <summary>Словарь измененных данных (ключ тега - новое значение из мода).</summary>
        private static Dictionary<string, string> ChangedData = [];

        /// <summary>
        /// Извлекает данные из файла перевода, читая английские комментарии.
        /// </summary>
        /// <param name="currentFile">Полный путь к XML-файлу перевода.</param>
        /// <returns>
        /// Кортеж, содержащий:
        /// - bool: true при успешном извлечении данных, false при ошибке.
        /// - string: Пустая строка при успехе, сообщение об ошибке при неудаче.
        /// </returns>
        /// <remarks>
        /// Метод извлекает текст из комментариев формата "<!-- EN: ... -->" и связывает его с именами тегов.
        /// </remarks>
        public static (bool, string) GetTranslationData(string currentFile)
        {
            (bool, string) result = XmlErrorChecker.XmlErrorCheck(currentFile);
            if (!result.Item1)
            {
                return result;
            }

            XmlReaderSettings settings = new() { DtdProcessing = DtdProcessing.Parse };
            XmlReader reader = XmlReader.Create(currentFile, settings);
            reader.MoveToContent();
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
                            bool unique = TranslationData.TryAdd(key, value);
                            if (!unique)
                            {
                                DateTime time = DateTime.Now;
                                string uniqueString = time.ToString("ffff", CultureInfo.InvariantCulture);
                                TranslationData.TryAdd(key + uniqueString, value);
                            }
                        }
                        hasValue = false;
                        break;
                    case XmlNodeType.Comment:
                        value = reader.Value.ToString()[4..].Trim();
                        hasValue = true;
                        break;
                }
            }
            return (true, string.Empty);
        }

        /// <summary>
        /// Извлекает данные из файла мода.
        /// </summary>
        /// <param name="currentFile">Полный путь к XML-файлу мода.</param>
        /// <returns>
        /// Кортеж, содержащий:
        /// - bool: true при успешном извлечении данных, false при ошибке.
        /// - string: Пустая строка при успехе, сообщение об ошибке при неудаче.
        /// </returns>
        /// <remarks>
        /// Метод извлекает имена тегов и их значения из файла мода.
        /// </remarks>
        public static (bool, string) GetModData(string currentFile)
        {
            (bool, string) result = XmlErrorChecker.XmlErrorCheck(currentFile);
            if (!result.Item1)
            {
                return result;
            }

            XDocument xDoc = XDocument.Load(currentFile, LoadOptions.PreserveWhitespace);
            XElement? root = xDoc.Element("LanguageData");

            foreach (XElement node in root.Elements())
            {
                bool unique = ModData.TryAdd(node.Name.ToString(), node.Value);
                if (!unique)
                {
                    TranslationData.TryAdd(node.Name.ToString() + Guid.NewGuid(), node.Value);
                }
            }

            return (true, string.Empty);
        }

        /// <summary>
        /// Сравнивает данные перевода с данными мода и находит изменения.
        /// </summary>
        /// <remarks>
        /// Метод заполняет словарь ChangedData изменениями, где английский текст отличается от значения в моде.
        /// </remarks>
        public static void FindChangesInFiles()
        {
            foreach (var line in TranslationData)
            {
                if (ModData.TryGetValue(line.Key, out string value))
                {
                    if (!line.Value.Equals(value, StringComparison.Ordinal))
                    {
                        ChangedData.Add(line.Key, value);
                    }
                    ModData.Remove(line.Key);
                    TranslationData.Remove(line.Key);
                }
            }
        }

        /// <summary>
        /// Записывает найденные изменения в текстовые файлы.
        /// </summary>
        /// <returns>Сообщение с информацией о найденных изменениях и созданных файлах.</returns>
        /// <remarks>
        /// Создает до трех файлов в текущей директории:
        /// - ChangedData.txt - теги с измененными значениями
        /// - ModData.txt - теги, присутствующие только в моде
        /// - TranslationData.txt - теги, присутствующие только в переводе
        /// </remarks>
        public static string WriteChanges()
        {
            string resultString = string.Empty;
            if (ChangedData.Count == 0)
            {
                resultString += "Не найдено сломанных файлов.";
            }
            else
            {
                StreamWriter sw = new(Path.Combine(Directory.GetCurrentDirectory() + "\\ChangedData.txt"), false);
                sw.WriteLine("==Различающиеся строки==");
                foreach (var line in ChangedData)
                {
                    sw.WriteLine($"{line.Key}: {line.Value}");
                }
                sw.Close();
                resultString += $"{Environment.NewLine}Найдено {ChangedData.Count} изменений. Результат записан в файл ChangedData.txt папки {Directory.GetCurrentDirectory()}";
                ChangedData.Clear();
            }

            if (ModData.Count != 0)
            {
                StreamWriter sw = new(Path.Combine(Directory.GetCurrentDirectory() + "\\ModData.txt"), false);
                sw.WriteLine("==Строки только в моде==");
                foreach (var line in ModData)
                {
                    sw.WriteLine($"{line.Key}: {line.Value}");
                }
                sw.Close();
                resultString += $"{Environment.NewLine}Найдено {ModData.Count} изменений. Результат записан в файл ModData.txt папки {Directory.GetCurrentDirectory()}";
                ModData.Clear();
            }

            if (TranslationData.Count != 0)
            {
                StreamWriter sw = new(Path.Combine(Directory.GetCurrentDirectory() + "\\TranslationData.txt"), false);
                sw.WriteLine("==Строки только в переводе==");
                foreach (var line in TranslationData)
                {
                    sw.WriteLine($"{line.Key}: {line.Value}");
                }
                sw.Close();
                resultString += $"{Environment.NewLine}Найдено {TranslationData.Count} изменений. Результат записан в файл TranslationData.txt папки {Directory.GetCurrentDirectory()}";
                TranslationData.Clear();
            }
            return resultString;
        }
    }
}