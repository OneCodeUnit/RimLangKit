using System.Xml.Linq;

namespace RimLangKit
{
    internal sealed class TagCollector
    {
        static List<string> Tags = new();
        static List<string> Defs = new();
        static Dictionary<string, List<string>> TagList = new();
        static Dictionary<string, int> TagSpread = new();
        static Dictionary<string, int> DefsSpread = new();

        internal static void WriteTags(TextBox textBox)
        {
            // Удаление пустых
            foreach (KeyValuePair<string, List<string>> tag in TagList)
            {
                if (tag.Value.Count < 1)
                {
                    TagList.Remove(tag.Key);
                    Defs.Remove(tag.Key);
                }
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

            // Статистика по тегам
            var spreadList = TagSpread.ToList();
            spreadList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
            spreadList.Reverse();
            StreamWriter swSpread = new(Path.Combine(Directory.GetCurrentDirectory() + "\\SpreadTags.txt"), false);
            foreach (var spread in spreadList)
            {
                if (spread.Value > 1)
                    swSpread.WriteLine($"{spread.Key} - {spread.Value}");
            }
            swSpread.Close();

            // Статистика по дефам
            var defSpreadList = DefsSpread.ToList();
            defSpreadList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
            defSpreadList.Reverse();
            StreamWriter swDefSpread = new(Path.Combine(Directory.GetCurrentDirectory() + "\\SpreadDefs.txt"), false);
            foreach (var spread in defSpreadList)
            {
                if (spread.Value > 1)
                    swDefSpread.WriteLine($"{spread.Key} - {spread.Value}");
            }
            swDefSpread.Close();

            textBox.AppendText($"{Environment.NewLine}Результат записан в файлы TagsByDefs.txt, UniqueTags.txt, UniqueDefs.txt папки {Directory.GetCurrentDirectory()}");
            TagList.Clear();
            TagSpread.Clear();
            DefsSpread.Clear();
            Tags.Clear();
            Defs.Clear();
        }

        internal static string CheckDef(string currentFile)
        {
            string[] path = currentFile.Split('\\');
            string def = path[^2];
            if (!Defs.Contains(def))
            {
                Defs.Add(def);
                DefsSpread.Add(def, 1);
            }
            else
            {
                DefsSpread[def]++;
            }
            if (!TagList.ContainsKey(def))
            {
                TagList.Add(def, new List<string>());
            }
            return def;
        }

        // Поиск тегов в текущем файле
        internal static bool TagSearch(string currentFile, string def, TextBox textBox)
        {
            if (currentFile.Contains("LoadFolders.xml", StringComparison.OrdinalIgnoreCase) || currentFile.Contains("About.xml", StringComparison.OrdinalIgnoreCase) || currentFile.Contains("Keyed.xml", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            // Позволяет избежать падения при загрузке сломанного .xml файла
            try
            {
                XDocument.Load(currentFile, LoadOptions.PreserveWhitespace);
            }
            catch
            {
                textBox.AppendText($"{Environment.NewLine}Не удалось загрузить файл {currentFile}");
                return false;
            }
            XDocument xDoc = XDocument.Load(currentFile, LoadOptions.PreserveWhitespace);

            //Позволяет избежать обработки пустого или не содержащего нужных тегов файла
            if (xDoc.Element("LanguageData") is null)
            {
                textBox.AppendText($"{Environment.NewLine}Не удалось найти LanguageData {currentFile}");
                return false;
            }
            //Перевод контекста в содержимое тега LanguageData
            XElement? root = xDoc.Element("LanguageData");
            if (root?.Elements() is null)
            {
                textBox.AppendText($"{Environment.NewLine}Тег LanguageData пуст {currentFile}");
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
                        TagSpread.Add(content, 1);
                    }
                    else
                    {
                        TagSpread[content]++;
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
                    textBox.AppendText($"{Environment.NewLine}Неправильный тег? {content} в {currentFile}");
                }
            }
            return true;
        }
    }
}