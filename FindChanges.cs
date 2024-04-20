using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace RimLangKit
{
    internal sealed class FindChanges
    {
        static Dictionary<string, string> TranslationData = new();
        static Dictionary<string, string> ModData = new();
        static Dictionary<string, string> ChangedData = new();

        internal static bool GetTranslationData(string currentFile, TextBox textBox)
        {
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

            if (xDoc.Element("LanguageData") is null)
            {
                textBox.AppendText($"{Environment.NewLine}Не удалось найти LanguageData {currentFile}");
                return false;
            }

            XElement? root = xDoc.Element("LanguageData");
            if (root?.Elements() is null)
            {
                textBox.AppendText($"{Environment.NewLine}Тег LanguageData пуст  {currentFile}");
                return false;
            }

            XmlReaderSettings settings = new(){ DtdProcessing = DtdProcessing.Parse };
            XmlReader reader = XmlReader.Create(currentFile, settings);
            reader.MoveToContent();

            string key = string.Empty;
            string value = string.Empty; 
            bool hasValue = false;
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        key = reader.Name.ToString();
                        if (hasValue)
                        {
                            bool unique = TranslationData.TryAdd(key, value);
                            if (!unique)
                            {
                                DateTime time = DateTime.Now;
                                string result = time.ToString("ffff", CultureInfo.InvariantCulture);
                                TranslationData.TryAdd(key + result, value);
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
            return true;
        }

        internal static bool GetModData(string currentFile, TextBox textBox)
        {
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

            if (xDoc.Element("LanguageData") is null)
            {
                textBox.AppendText($"{Environment.NewLine}Не удалось найти LanguageData {currentFile}");
                return false;
            }

            XElement? root = xDoc.Element("LanguageData");
            if (root?.Elements() is null)
            {
                textBox.AppendText($"{Environment.NewLine}Тег LanguageData пуст  {currentFile}");
                return false;
            }

            foreach (XElement node in root.Elements())
            {
                bool unique = ModData.TryAdd(node.Name.ToString(), node.Value);
                if (!unique)
                {
                    DateTime time = DateTime.Now;
                    string result = time.ToString("ffff", CultureInfo.InvariantCulture);
                    TranslationData.TryAdd(node.Name.ToString() + result, node.Value);
                }
            }

            return true;
        }

        internal static void FindChangesInFiles()
        {
            foreach (var line in TranslationData)
            {
                if (ModData.TryGetValue(line.Key, out string? value))
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


        internal static void WriteChanges(TextBox textBox)
        {
            if (ChangedData.Count == 0)
            {
                textBox.AppendText($"{Environment.NewLine}Не найдено сломанных файлов.");
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
                textBox.AppendText($"{Environment.NewLine}Найдено {ChangedData.Count} изменений. Результат записан в файл ChangedData.txt папки {Directory.GetCurrentDirectory()}");
                ChangedData.Clear();
            }

            if (ModData.Count != 0)
            {
                StreamWriter sw = new(Path.Combine(Directory.GetCurrentDirectory() + "\\ModData.txt"), false);
                sw.WriteLine("==Строки только в моде (не обязательно ошибка)==");
                foreach (var line in ModData)
                {
                    sw.WriteLine($"{line.Key}: {line.Value}");
                }
                sw.Close();
                textBox.AppendText($"{Environment.NewLine}Найдено {ModData.Count} изменений. Результат записан в файл ModData.txt папки {Directory.GetCurrentDirectory()}");
                ModData.Clear();
            }

            if (TranslationData.Count != 0)
            {
                StreamWriter sw = new(Path.Combine(Directory.GetCurrentDirectory() + "\\TranslationData.txt"), false);
                sw.WriteLine("==Строки только в переводе (не обязательно ошибка)==");
                foreach (var line in TranslationData)
                {
                    sw.WriteLine($"{line.Key}: {line.Value}");
                }
                sw.Close();
                textBox.AppendText($"{Environment.NewLine}Найдено {TranslationData.Count} изменений. Результат записан в файл TranslationData.txt папки {Directory.GetCurrentDirectory()}");
                TranslationData.Clear();
            }
        }
    }
}
