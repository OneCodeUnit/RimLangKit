using RimLanguageCore.Misc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace RimLanguageCore.Activities
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
        private static string CheckDef(string currentFile)
        {
            string[] path = currentFile.Split('\\');
            string def = path[^2];
            if (!Defs.Contains(def))
            {
                Defs.Add(def);
                DefsSpread.Add(def, 1);
                TagList.Add(def, new List<string>());
            }
            else
                DefsSpread[def]++;
            return def;
        }

        // Поиск тегов в текущем файле
        public static (bool, string) TagCollectorActivity(string currentFile)
        {
            if (currentFile.Contains("LoadFolders.xml", StringComparison.OrdinalIgnoreCase) || currentFile.Contains("About.xml", StringComparison.OrdinalIgnoreCase) || currentFile.Contains("Keyed.xml", StringComparison.OrdinalIgnoreCase))
                return (false, "Пропуск файла без defs");

            (bool, string) result = XmlErrorChecker.XmlErrorCheck(currentFile);
            if (!result.Item1)
                return result;

            XDocument xDoc = XDocument.Load(currentFile, LoadOptions.PreserveWhitespace);
            XElement root = xDoc.Element("LanguageData");

            // DefName текущего файла
            string def = CheckDef(currentFile);
            if (def == "Keyed")
                return (false, "Keyed-файл");

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
                        TagSpread[content]++;
                    if (!TagList[def].Contains(content))
                        TagList[def].Add(content);
                }
                else
                    return (false, $"{content} - Неправильный тег?");
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
            StreamWriter sw = new(Path.Combine(directory + "\\TagsByDefs.txt"), false);
            foreach (string def in Defs)
            {
                sw.WriteLine($"**{def}**");
                List<string> tag = TagList[def];
                tag.ForEach(sw.WriteLine);
                sw.WriteLine();
            }
            sw.Close();
            StreamWriter swTags = new(Path.Combine(directory + "\\UniqueTags.txt"), false);
            Tags.ForEach(swTags.WriteLine);
            swTags.Close();
            StreamWriter swDefs = new(Path.Combine(directory + "\\UniqueDefs.txt"), false);
            Defs.ForEach(swDefs.WriteLine);
            swDefs.Close();

            // Статистика по тегам
            var spreadList = TagSpread.ToList();
            spreadList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
            spreadList.Reverse();
            StreamWriter swSpread = new(Path.Combine(directory + "\\SpreadTags.txt"), false);
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
            StreamWriter swDefSpread = new(Path.Combine(directory + "\\SpreadDefs.txt"), false);
            foreach (var spread in defSpreadList)
            {
                if (spread.Value > 1)
                    swDefSpread.WriteLine($"{spread.Key} - {spread.Value}");
            }

            swDefSpread.Close();
            return $"Результат записан в файлы TagsByDefs.txt, UniqueTags.txt, UniqueDefs.txt папки {directory}";
        }

        public static string DefsClassGeneratorActivity()
        {
            // Удаление составных (временно)
            Defs.Remove("RulePackDef");
            List<string> deleted = new();
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
            StreamWriter sw = new(Path.Combine(directory + "\\defsClass.cs"), false);

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
    }
}
