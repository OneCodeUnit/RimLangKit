using System.Xml.Linq;

namespace TagCollector
{
    internal class Program
    {
        static List<string> Tags = new();
        static List<string> Defs = new();
        static Dictionary<string, List<string>> TagList = new();

        static void Main()
        {
            Console.WriteLine("Путь к папке с извлечёнными xml:");
            string? DirectoryPath = Console.ReadLine();
            if (DirectoryPath == null)
            {
                Console.WriteLine("Путь не введён");
                return;
            }
            if (!Directory.Exists(DirectoryPath))
            {
                Console.WriteLine("Папка не существует");
                return;
            }
            // Получение списка всех файлов в заданой директории и во всех вложенных подпапках за счёт SearchOption
            string[] allFiles = Directory.GetFiles(DirectoryPath, "*.xml", SearchOption.AllDirectories);
            int count = 0;
            int errCount = 0;
            bool result;
            foreach (string tempFile in allFiles)
            {
                string def = CheckDef(tempFile);
                result = TagSearch(tempFile, def);
                if (result) count++;
                else errCount++;
            }
            Console.WriteLine($"Успешно завершено. Обработано файлов - {count}, пропущено - {errCount}");

            // Удаление пустых
            foreach (KeyValuePair<string, List<string>> tag in TagList)
            {
                if (tag.Value.Count < 1)
                {
                    TagList.Remove(tag.Key);
                    Defs.Remove(tag.Key);
                }
            }

            // Вывод в консоль
            foreach (string def in Defs)
            {
                Console.WriteLine($"**{def}**");
                List<string> tag = TagList[def];
                tag.ForEach(Console.WriteLine);
                Console.WriteLine();
            }

            // Запись в файл
            StreamWriter sw = new(Path.Combine(Directory.GetCurrentDirectory() + "\\TagsByDefs.txt"), false);
            foreach (string def in Defs)
            {
                sw.WriteLine($"**{def}**");
                List<string> tag = TagList[def];
                tag.ForEach(sw.WriteLine);
                sw.WriteLine();
            }
            sw.Close();
            StreamWriter swTags = new(Path.Combine(Directory.GetCurrentDirectory() + "\\UniqueTags.txt"), false);
            Tags.ForEach(swTags.WriteLine);
            swTags.Close();
            StreamWriter swDefs = new(Path.Combine(Directory.GetCurrentDirectory() + "\\UniqueDefs.txt"), false);
            Defs.ForEach(swDefs.WriteLine);
            swDefs.Close();
            Console.WriteLine($"Результат записан в файлы TagsByDefs.txt, UniqueTags.txt, UniqueDefs.txt папки {Directory.GetCurrentDirectory()}");
            Console.ReadKey();
        }

        static string CheckDef(string currentFile)
        {
            string[] path = currentFile.Split('\\');
            string def = path[^2];
            if (!Defs.Contains(def))
            {
                Defs.Add(def);
            }
            if (!TagList.ContainsKey(def))
            {
                TagList.Add(def, new List<string>());
            }
            return def;
        }

        // Поиск тегов в текущем файле
        static bool TagSearch(string currentFile, string def)
        {
            // Позволяет избежать падения при загрузке сломанного .xml файла
            try
            {
                XDocument.Load(currentFile, LoadOptions.PreserveWhitespace);
            }
            catch
            {
                Console.WriteLine($"Не удалось загрузить файл {currentFile}");
                return false;
            }
            XDocument xDoc = XDocument.Load(currentFile, LoadOptions.PreserveWhitespace);

            //Позволяет избежать обработки пустого или не содержащего нужных тегов файла
            if (xDoc.Element("LanguageData") is null)
            {
                if (currentFile.Contains("LoadFolders.xml", StringComparison.OrdinalIgnoreCase) || currentFile.Contains("About.xml", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
                else
                {
                    Console.WriteLine($"Не удалось найти LanguageData {currentFile}");
                    return false;
                }
            }
            //Перевод контекста в содержимое тега LanguageData
            XElement? root = xDoc.Element("LanguageData");
            if (root?.Elements() is null)
            {
                Console.WriteLine($"Тег LanguageData пуст {currentFile}");
                return false;
            }

            foreach (XElement node in root.Elements())
            {
                //Получение текущего тега
                string content = node.Name.ToString();
                if (content.Contains('.'))
                {
                    content = content[content.IndexOf('.')..];
                    if (!Tags.Contains(content))
                    {
                        Tags.Add(content);
                    }
                    List<string> checkTag = TagList[def];
                    if (!checkTag.Contains(content))
                    {
                        checkTag.Add(content);
                        TagList[def] = checkTag;
                    }
                }
                else if (currentFile.Contains("Keyed", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
                else
                {
                    Console.WriteLine($"Неправильный тег? {content} в {currentFile}");
                }
            }
            return true;
        }
    }
}