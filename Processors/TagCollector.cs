using RimLangKit.Checks;
using System.Text;
using System.Xml.Linq;

namespace RimLangKit.Processors
{
    public static class TagCollector
    {
        // Уникальные Tags
        private static List<string> Tags = new();
        // Уникальные Defs
        private static List<string> Defs = new();
        // Список Tags для каждого Defs
        private static Dictionary<string, List<string>> TagList = new();
        // Распространенность Tags длятекущего перевода
        private static Dictionary<string, int> TagSpread = new();
        // Распространенность Defs для текущего перевода
        private static Dictionary<string, int> DefsSpread = new();

        // Предобработка файла. Сбор данных о тегах в переводе
        private static string ParseDefName(string currentFile)
        {
            string[] path = currentFile.Split('\\');
            string def = path[^2];
            if (!Defs.Contains(def))
            {
                Defs.Add(def);
                DefsSpread.Add(def, 1);
                TagList.Add(def, []);
            }
            else
            {
                DefsSpread[def]++;
            }
            return def;
        }

        // Поиск тегов в текущем файле
        public static (bool, string) TagCollectorActivity(string currentFile)
        {
            if (currentFile.Contains("LoadFolders.xml", StringComparison.OrdinalIgnoreCase) || currentFile.Contains("About.xml", StringComparison.OrdinalIgnoreCase) || currentFile.Contains("Keyed.xml", StringComparison.OrdinalIgnoreCase))
            {
                return (false, "Пропуск файла без defs");
            }

            (bool, string) result = XmlErrorChecker.XmlErrorCheck(currentFile);
            if (!result.Item1)
            {
                return result;
            }

            XDocument xDoc = XDocument.Load(currentFile, LoadOptions.PreserveWhitespace);
            XElement? root = xDoc.Element("LanguageData");

            // DefName текущего файла
            string def = ParseDefName(currentFile);
            if (def == "Keyed")
            {
                return (false, "Keyed-файл");
            }

            foreach (XElement node in root.Elements())
            {
                // Получение текущего тега
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

                    if (!TagList[def].Contains(content))
                    {
                        TagList[def].Add(content);
                    }
                }
                else
                {
                    return (false, $"{content} - Неправильный тег?");
                }
            }
            return (true, string.Empty);
        }

        public static string TagWriterActivity()
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

            string directory = Directory.GetCurrentDirectory();

            // Запись в файл
            StreamWriter sw = new(Path.Combine(directory, "TagsByDefs.txt"), false, Encoding.UTF8);
            foreach (string def in Defs)
            {
                sw.WriteLine($"**{def}**");
                List<string> tag = TagList[def];
                tag.ForEach(sw.WriteLine);
                sw.WriteLine();
            }
            sw.Close();
            StreamWriter swTags = new(Path.Combine(directory, "UniqueTags.txt"), false, Encoding.UTF8);
            Tags.ForEach(swTags.WriteLine);
            swTags.Close();
            StreamWriter swDefs = new(Path.Combine(directory, "UniqueDefs.txt"), false, Encoding.UTF8);
            Defs.ForEach(swDefs.WriteLine);
            swDefs.Close();

            // Статистика по тегам
            var spreadList = TagSpread.ToList();
            spreadList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
            spreadList.Reverse();
            StreamWriter swSpread = new(Path.Combine(directory, "SpreadTags.txt"), false, Encoding.UTF8);
            foreach (var spread in spreadList)
            {
                if (spread.Value > 1)
                {
                    swSpread.WriteLine($"{spread.Key} - {spread.Value}");
                }
            }
            swSpread.Close();

            // Статистика по дефам
            var defSpreadList = DefsSpread.ToList();
            defSpreadList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
            defSpreadList.Reverse();
            StreamWriter swDefSpread = new(Path.Combine(directory, "SpreadDefs.txt"), false, Encoding.UTF8);
            foreach (var spread in defSpreadList)
            {
                if (spread.Value > 1)
                {
                    swDefSpread.WriteLine($"{spread.Key} - {spread.Value}");
                }
            }

            swDefSpread.Close();
            return $"Результат записан в файлы TagsByDefs.txt, UniqueTags.txt, UniqueDefs.txt папки {directory}";
        }

        public static string DefsClassGeneratorActivity()
        {
            // Удаление составных (временно)
            Defs.Remove("RulePackDef");
            List<string> deleted = [];
            foreach (string def in Defs)
            {
                if (def.Contains('.'))
                {
                    deleted.Add(def);
                }
            }
            foreach (string def in deleted)
            {
                Defs.Remove(def);
            }

            string directory = Directory.GetCurrentDirectory();
            string text = string.Empty;
            // Создание файла
            StreamWriter sw = new(Path.Combine(directory, "defsClass.cs"), false, Encoding.UTF8);

            // Заголовок
            text += "using System.Xml.Serialization;\n\n";
            text += "namespace TextExporter\n{\n";

            text += "\t[XmlRoot(ElementName = \"Defs\")]\n";
            text += "\tpublic class Defs\n\t{\n";

            // Список классов
            //text += "\t\tpublic readonly List<string> defTypes = [";
            //foreach (string def in Defs)
            //{
            //    text += $"\"{def}\", ";
            //}
            //text = text[..^2];
            //text += "];\n\n";

            // Списки классов
            foreach (string def in Defs)
            {
                text += $"\t\t[XmlElement(ElementName = \"{def}\")]\n";
                text += $"\t\tpublic List<{def}>? {def} {{ get; set; }}\n";
            }
            text += "\t}\n";
            sw.WriteLine(text);
            text = string.Empty;

            // Классы
            foreach (string def in Defs)
            {
                text += $"\t[XmlRoot(ElementName = \"{def}\")]\n";
                text += $"\tpublic class {def}\n\t{{\n";

                // Список тегов
                //text += "\t\tpublic readonly List<string> tagTypes = [";
                //foreach (string tag in TagList[def])
                //{
                //    string trimedTag = tag[1..];
                //    if (!trimedTag.Contains('.'))
                //        text += $"\"{trimedTag}\", ";
                //}
                //text = text[..^2];
                //text += "];\n\n";

                text += "\t\t[XmlElement(ElementName = \"defName\")]\n";
                text += "\t\tpublic string? DefName { get; set; }\n\n";
                foreach (string tag in TagList[def])
                {
                    string trimedTag = tag[1..];
                    if (!trimedTag.Contains('.'))
                    {
                        text += $"\t\t[XmlElement(ElementName = \"{trimedTag}\")]\n";
                        text += $"\t\tpublic string? {trimedTag[0].ToString().ToUpperInvariant() + trimedTag[1..]} {{ get; set; }}\n\n";
                    }
                }

                text += "\t}\n";
                sw.WriteLine(text);
                text = string.Empty;
            }
            text += "}";
            sw.WriteLine(text);
            sw.Close();
            return $"Создан класс для извлечения текста в программе и записан в файл defsClass.cs папки {directory}";
        }

        public static void DataCleanerActivity()
        {
            TagList.Clear();
            TagSpread.Clear();
            DefsSpread.Clear();
            Tags.Clear();
            Defs.Clear();
        }

        #region Новые instance методы без статического состояния

        /// <summary>
        /// Предобработка файла. Сбор данных о тегах в переводе (без статических коллекций)
        /// </summary>
        private static string ParseDefName(string currentFile, TagCollectorData data)
        {
            string[] path = currentFile.Split('\\');
            string def = path[^2];
            if (!data.Defs.Contains(def))
            {
                data.Defs.Add(def);
                data.DefsSpread.Add(def, 1);
                data.TagList.Add(def, []);
            }
            else
            {
                data.DefsSpread[def]++;
            }
            return def;
        }

        /// <summary>
        /// Поиск тегов в текущем файле (новая версия без статического состояния)
        /// </summary>
        public static (bool, string) TagCollectorActivity(string currentFile, TagCollectorData data)
        {
            if (currentFile.Contains("LoadFolders.xml", StringComparison.OrdinalIgnoreCase) ||
                currentFile.Contains("About.xml", StringComparison.OrdinalIgnoreCase) ||
                currentFile.Contains("Keyed.xml", StringComparison.OrdinalIgnoreCase))
            {
                return (false, "Пропуск файла без defs");
            }

            (bool, string) result = XmlErrorChecker.XmlErrorCheck(currentFile);
            if (!result.Item1)
            {
                return result;
            }

            XDocument xDoc = XDocument.Load(currentFile, LoadOptions.PreserveWhitespace);
            XElement? root = xDoc.Element("LanguageData");

            // DefName текущего файла
            string def = ParseDefName(currentFile, data);
            if (def == "Keyed")
            {
                return (false, "Keyed-файл");
            }

            foreach (XElement node in root.Elements())
            {
                // Получение текущего тега
                string content = node.Name.ToString();
                if (content.Contains('.'))
                {
                    content = content[content.IndexOf('.')..];
                    if (!data.Tags.Contains(content))
                    {
                        data.Tags.Add(content);
                        data.TagSpread.Add(content, 1);
                    }
                    else
                    {
                        data.TagSpread[content]++;
                    }

                    if (!data.TagList[def].Contains(content))
                    {
                        data.TagList[def].Add(content);
                    }
                }
                else
                {
                    return (false, $"{content} - Неправильный тег?");
                }
            }
            return (true, string.Empty);
        }

        /// <summary>
        /// Запись собранных данных в файлы (новая версия без статического состояния)
        /// </summary>
        public static string TagWriterActivity(TagCollectorData data)
        {
            // Удаление пустых
            var keysToRemove = data.TagList.Where(kvp => kvp.Value.Count < 1).Select(kvp => kvp.Key).ToList();
            foreach (var key in keysToRemove)
            {
                data.TagList.Remove(key);
                data.Defs.Remove(key);
            }

            string directory = Directory.GetCurrentDirectory();

            // Запись в файл
            using (StreamWriter sw = new(Path.Combine(directory, "TagsByDefs.txt"), false, Encoding.UTF8))
            {
                foreach (string def in data.Defs)
                {
                    sw.WriteLine($"**{def}**");
                    List<string> tag = data.TagList[def];
                    tag.ForEach(sw.WriteLine);
                    sw.WriteLine();
                }
            }

            using (StreamWriter swTags = new(Path.Combine(directory, "UniqueTags.txt"), false, Encoding.UTF8))
            {
                data.Tags.ForEach(swTags.WriteLine);
            }

            using (StreamWriter swDefs = new(Path.Combine(directory, "UniqueDefs.txt"), false, Encoding.UTF8))
            {
                data.Defs.ForEach(swDefs.WriteLine);
            }

            // Статистика по тегам
            var spreadList = data.TagSpread.ToList();
            spreadList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
            spreadList.Reverse();
            using (StreamWriter swSpread = new(Path.Combine(directory, "SpreadTags.txt"), false, Encoding.UTF8))
            {
                foreach (var spread in spreadList)
                {
                    if (spread.Value > 1)
                    {
                        swSpread.WriteLine($"{spread.Key} - {spread.Value}");
                    }
                }
            }

            // Статистика по дефам
            var defSpreadList = data.DefsSpread.ToList();
            defSpreadList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
            defSpreadList.Reverse();
            using (StreamWriter swDefSpread = new(Path.Combine(directory, "SpreadDefs.txt"), false, Encoding.UTF8))
            {
                foreach (var spread in defSpreadList)
                {
                    if (spread.Value > 1)
                    {
                        swDefSpread.WriteLine($"{spread.Key} - {spread.Value}");
                    }
                }
            }

            return $"Результат записан в файлы TagsByDefs.txt, UniqueTags.txt, UniqueDefs.txt папки {directory}";
        }

        /// <summary>
        /// Генерация C# классов на основе собранных Defs (новая версия без статического состояния)
        /// </summary>
        public static string DefsClassGeneratorActivity(TagCollectorData data)
        {
            // Удаление составных (временно)
            data.Defs.Remove("RulePackDef");
            List<string> deleted = [];
            foreach (string def in data.Defs)
            {
                if (def.Contains('.'))
                {
                    deleted.Add(def);
                }
            }
            foreach (string def in deleted)
            {
                data.Defs.Remove(def);
            }

            string directory = Directory.GetCurrentDirectory();
            string text = string.Empty;

            using (StreamWriter sw = new(Path.Combine(directory, "defsClass.cs"), false, Encoding.UTF8))
            {
                // Заголовок
                text += "using System.Xml.Serialization;\n\n";
                text += "namespace TextExporter\n{\n";

                text += "\t[XmlRoot(ElementName = \"Defs\")]\n";
                text += "\tpublic class Defs\n\t{\n";

                // Списки классов
                foreach (string def in data.Defs)
                {
                    text += $"\t\t[XmlElement(ElementName = \"{def}\")]\n";
                    text += $"\t\tpublic List<{def}>? {def} {{ get; set; }}\n";
                }
                text += "\t}\n";
                sw.WriteLine(text);
                text = string.Empty;

                // Классы
                foreach (string def in data.Defs)
                {
                    text += $"\t[XmlRoot(ElementName = \"{def}\")]\n";
                    text += $"\tpublic class {def}\n\t{{\n";

                    text += "\t\t[XmlElement(ElementName = \"defName\")]\n";
                    text += "\t\tpublic string? DefName { get; set; }\n\n";
                    foreach (string tag in data.TagList[def])
                    {
                        string trimedTag = tag[1..];
                        if (!trimedTag.Contains('.'))
                        {
                            text += $"\t\t[XmlElement(ElementName = \"{trimedTag}\")]\n";
                            text += $"\t\tpublic string? {trimedTag[0].ToString().ToUpperInvariant() + trimedTag[1..]} {{ get; set; }}\n\n";
                        }
                    }

                    text += "\t}\n";
                    sw.WriteLine(text);
                    text = string.Empty;
                }
                text += "}";
                sw.WriteLine(text);
            }

            return $"Создан класс для извлечения текста в программе и записан в файл defsClass.cs папки {directory}";
        }

        #endregion
    }
}