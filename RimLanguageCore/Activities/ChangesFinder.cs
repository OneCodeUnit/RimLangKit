using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using RimLanguageCore.Misc;

namespace RimLanguageCore.Activities
{
    public class ChangesFinder
    {
        private static Dictionary<string, string> TranslationData = new();
        private static Dictionary<string, string> ModData = new();
        private static Dictionary<string, string> ChangedData = new();

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

        public static (bool, string) GetModData(string currentFile)
        {
            (bool, string) result = XmlErrorChecker.XmlErrorCheck(currentFile);
            if (!result.Item1)
            {
                return result;
            }

            XDocument xDoc = XDocument.Load(currentFile, LoadOptions.PreserveWhitespace);
            XElement root = xDoc.Element("LanguageData");

            foreach (XElement node in root.Elements())
            {
                bool unique = ModData.TryAdd(node.Name.ToString(), node.Value);
                if (!unique)
                {
                    DateTime time = DateTime.Now;
                    string uniqueString = time.ToString("ffff", CultureInfo.InvariantCulture);
                    TranslationData.TryAdd(node.Name.ToString() + uniqueString, node.Value);
                }
            }

            return (true, string.Empty);
        }

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