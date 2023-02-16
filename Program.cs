using System.Globalization;
using System.Xml.Linq;
using ICSharpCode.SharpZipLib.Zip;

namespace RTK
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }

        internal static (bool, string) CommentsProcessing(string CurrentFile)
        {
            string error = string.Empty;
            //Позволяет избежать падения при загрузке сломанного .xml файла
            try
            {
                XDocument.Load(CurrentFile, LoadOptions.PreserveWhitespace);
            }
            catch
            {
                error = "Не удалось загрузить файл " + CurrentFile;
                return (false, error);
            }

            XDocument xDoc = XDocument.Load(CurrentFile, LoadOptions.PreserveWhitespace);
            //Позволяет избежать обработки пустого или не содержащего нужных тегов файла
            if (xDoc.Element("LanguageData") is null)
            {
                error = "Не удалось найти LanguageData " + CurrentFile;
                return (false, error);
            }
            //Перевод контекста в содержимое тега LanguageData
            XElement? root = xDoc.Element("LanguageData");
            if (root?.Elements() is null)
            {
                error = "Тег LanguageData пуст " + CurrentFile;
                return (false, error);
            }

            foreach (XElement node in root.Elements())
            {
                //Получение содержимого текущего тега
                string content = node.Value;
                //Создание комментария с ним
                XRaw comment = new("<!-- EN: " + content + " -->\n\t");
                //Добавление этого комментария перед текущим тегом
                node.AddBeforeSelf(comment);
            }
            //Перенос строки перед закрывающим тегом LanguageData
            root.LastNode?.AddAfterSelf("\n");

            //Сохранение файла
            xDoc.Save(CurrentFile);
            return (true, error);
        }

        internal static (bool, string) NamesProcessing(string CurrentFile)
        {
            string error = string.Empty;
            //Позволяет избежать падения при загрузке сломанного .xml файла
            try
            {
                XDocument.Load(CurrentFile, LoadOptions.PreserveWhitespace);
            }
            catch
            {
                error = "Не удалось загрузить файл " + CurrentFile;
                return (false, error);
            }

            XDocument xDoc = XDocument.Load(CurrentFile, LoadOptions.PreserveWhitespace);
            //Позволяет избежать обработки пустого или не содержащего нужных тегов файла
            if (xDoc.Element("LanguageData") is null)
            {
                error = "Не удалось найти LanguageData " + CurrentFile;
                return (false, error);
            }
            //Перевод контекста в содержимое тега LanguageData
            XElement? root = xDoc.Element("LanguageData");
            if (root?.Elements() is null)
            {
                error = "Тег LanguageData пуст " + CurrentFile;
                return (false, error);
            }

            //Разделение пути до файла на части и поиск нужной
            string[] Path = CurrentFile.Split('\\');
            StringComparison Condition = StringComparison.OrdinalIgnoreCase;
            string FolderName = string.Empty;
            for (int i = 0; i < Path.Length; i++)
            {
                if (Path[i].Equals("Keyed", Condition) || Path[i].Equals("DefInjected", Condition))
                {
                    FolderName = Path[i - 3];
                    break;
                }
            }

            //Сохранение с добавлением имени папки. Имя папки то, которое до "Languages"
            string NewPath = CurrentFile.Remove(CurrentFile.Length - 4) + "_" + FolderName + ".xml";
            File.Move(CurrentFile, NewPath);
            return (true, error);
        }

        internal static string SetLine(string? line)
        {
            return line is null ? string.Empty : line.Trim();
        }

        internal static Dictionary<string, string> SetDictionary(string line)
        {
            Dictionary<string, string> dictionary = new();
            foreach (string item in line.Split(","))
            {
                string[] symbols = item.Split(":");
                dictionary.Add(symbols[0], symbols[1]);
            }
            return dictionary;
        }

        internal static Dictionary<string, string> SetDictionary(string[] lines)
        {
            Dictionary<string, string> dictionary = new();
            foreach (string line in lines)
            {
                foreach (string item in line.Split(","))
                {
                    string[] symbols = item.Split(":");
                    dictionary.Add(symbols[0], symbols[1]);
                }
            }
            return dictionary;
        }

        internal static (bool, string) TranscriptionProcessing(string CurrentFile, bool mode)
        {
            string error = string.Empty;
            CultureInfo culture = CultureInfo.CurrentCulture;
            StringComparison comparison = StringComparison.OrdinalIgnoreCase;
            //Чтение файла словаря
            StreamReader DictionaryText;
            try
            {
                DictionaryText = new("dictionary.txt");
            }
            catch
            {
                error = "Не удалось открыть словарь.";
                return (false, error);
            }

            //Без повторной обработки файлов
            if (CurrentFile.EndsWith("_NEW.txt", comparison))
            {
                error = "Пропуск созданного ранее файла.";
                return (false, error);
            }

            //Перенос строк
            string line4 = SetLine(DictionaryText.ReadLine());
            string line3 = SetLine(DictionaryText.ReadLine());
            string line2 = SetLine(DictionaryText.ReadLine());
            string line1 = SetLine(DictionaryText.ReadLine());
            string[] lines = { line4, line3, line2, line1 };
            string lineClear = SetLine(DictionaryText.ReadLine());
            string lineStart = SetLine(DictionaryText.ReadLine());
            string lineEnd = SetLine(DictionaryText.ReadLine());

            //Заполнение словаря
            Dictionary<string, string> dictionary = new(SetDictionary(lines));
            Dictionary<string, string> dictionaryClear = new(SetDictionary(lineClear));
            Dictionary<string, string> dictionaryStart = new(SetDictionary(lineStart));
            Dictionary<string, string> dictionaryEnd = new(SetDictionary(lineEnd));

            //Открытие файлов
            StreamReader SourceText = new(CurrentFile);
            string TextFileRus = CurrentFile.Replace(".txt", "_NEW.txt");
            StreamWriter TranslatedText = new(TextFileRus);

            //Процесс транслитерации
            if (mode)
            {
                while (true)
                {
                    string? temp = SourceText.ReadLine();
                    if (temp == null) break;
                    string Source = temp.Trim().ToLower(culture);

                    foreach (KeyValuePair<string, string> item in dictionaryStart)
                    {
                        if (Source.StartsWith(item.Key, comparison))
                        {
                            Source = Source.Replace(item.Key, item.Value);
                            break;
                        }
                    }
                    foreach (KeyValuePair<string, string> item in dictionaryEnd)
                    {
                        if (Source.EndsWith(item.Key, comparison))
                        {
                            Source = Source.Replace(item.Key, item.Value);
                            break;
                        }
                    }
                    foreach (KeyValuePair<string, string> item in dictionaryClear)
                    {
                        if (Source.Contains(item.Key))
                        {
                            Source = Source.Replace(item.Key, item.Value);
                        }
                    }

                    foreach (KeyValuePair<string, string> item in dictionary)
                    {
                        if (Source.Contains(item.Key))
                        {
                            Source = Source.Replace(item.Key, item.Value);
                        }
                    }

                    for (int i = 0; i < Source.Length; i++)
                    {
                        if (Source[i] == '=')
                        {
                            Source = Source.Replace(Source[i].ToString(), string.Empty);
                        }
                    }
                    if (Source.Length > 1)
                    {
                        Source = string.Concat(Source[0].ToString().ToUpper(culture), Source.AsSpan(1));
                    }
                    else
                    {
                        Source = Source.ToUpper(culture);
                    }
                    TranslatedText.WriteLine(Source);
                }
            }
            else
            {
                while (true)
                {
                    string? temp = SourceText.ReadLine();
                    if (temp == null) break;
                    string Source = temp.Trim().ToLower(culture);

                    foreach (KeyValuePair<string, string> item in dictionaryStart)
                    {
                        if (Source.StartsWith(item.Value, comparison))
                        {
                            Source = Source.Replace(item.Value, item.Key);
                            break;
                        }
                    }
                    foreach (KeyValuePair<string, string> item in dictionaryEnd)
                    {
                        if (Source.EndsWith(item.Value, comparison))
                        {
                            Source = Source.Replace(item.Value, item.Key);
                            break;
                        }
                    }
                    foreach (KeyValuePair<string, string> item in dictionaryClear)
                    {
                        if (Source.Contains(item.Value))
                        {
                            Source = Source.Replace(item.Value, item.Key);
                        }
                    }

                    foreach (KeyValuePair<string, string> item in dictionary)
                    {
                        if (Source.Contains(item.Value))
                        {
                            Source = Source.Replace(item.Value, item.Key);
                        }
                    }

                    for (int i = 0; i < Source.Length; i++)
                    {
                        if (Source[i] == '=')
                        {
                            Source = Source.Replace(Source[i].ToString(), string.Empty);
                        }
                    }
                    if (Source.Length > 1)
                    {
                        Source = string.Concat(Source[0].ToString().ToUpper(culture), Source.AsSpan(1));
                    }
                    else
                    {
                        Source = Source.ToUpper(culture);
                    }
                    TranslatedText.WriteLine(Source);
                }
            }

            //Завершение работы
            SourceText.Close();
            TranslatedText.Close();
            return (true, error);
        }

        internal static (bool, string) CheckConfig(string CurrentFile)
        {
            string error = string.Empty;
            if (!File.Exists("config.xml"))
            {
                XDocument xdoc = new XDocument();
                XElement configuration = new XElement("configuration");
                XElement xmlsha = new XElement("sha", "0");
                XElement xmllanguage = new XElement("language", "Russian(GitHub)");
                XElement xmldirectory = new XElement("directory", CurrentFile);
                XElement xmlrepo= new XElement("repo", "RimWorld-ru");
                configuration.Add(xmlsha);
                configuration.Add(xmllanguage);
                configuration.Add(xmldirectory);
                configuration.Add(xmlrepo);
                xdoc.Add(configuration);
                xdoc.Save("config.xml");
                error = "Файл настроек не найден и создан новый с значениями по умолчанию. Запустите ещё раз.";
                return (false, error);
            }
            else
            {
                XDocument xdoc;
                try
                {
                    xdoc = XDocument.Load("config.xml");
                }
                catch
                {
                    error = "Файл настроек повреждён.";
                    return (false, error);
                }
                XElement? configuration = xdoc.Element("configuration");
                if (configuration.IsEmpty)
                {
                    error = "Файл настроек повреждён.";
                    return (false, error);
                }
                string? sha = configuration.Element("sha")?.Value;
                if (sha is null)
                {
                    error = "Файл настроек повреждён.";
                    return (false, error);
                }
                string? language = configuration.Element("language")?.Value;
                if (language is null)
                {
                    error = "Файл настроек повреждён.";
                    return (false, error);
                }
                string? directory = configuration.Element("directory")?.Value;
                if (directory is null)
                {
                    error = "Файл настроек повреждён.";
                    return (false, error);
                }
                string? repo = configuration.Element("repo")?.Value;
                if (repo is null)
                {
                    error = "Файл настроек повреждён.";
                    return (false, error);
                }
            }
            return (true, error);
        }
        internal static (bool, string) LoadConfig(string CurrentFile)
        {
            string error = string.Empty;
            XDocument xdoc = XDocument.Load("config.xml");
            XElement configuration = xdoc.Element("configuration");
            string sha = configuration.Element("sha").Value;
            string repo = configuration.Element("repo").Value;

            HttpClient client = new();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:10.0) Gecko/20100101 Firefox/10.0");
            HttpResponseMessage response = client.GetAsync("https://api.github.com/repos/Ludeon/" + repo + "/commits").Result;
            string json = response.Content.ReadAsStringAsync().Result;
            char[] separators = { ',', ':' };
            string[] temp = json.Split(separators, 2, StringSplitOptions.TrimEntries);
            int index = temp[1].IndexOf('"', 1);
            json = temp[1].Substring(1, index - 1);

            if (json != sha)
            {
                configuration.Element("sha").Value = json;
                configuration.Element("directory").Value = CurrentFile;
                xdoc.Save("config.xml");
                return (true, error);
            }
            else
            {
                error = "Обновление не требуется";
                return (false, error);
            }
        }

        internal static (bool, string) LoadFile()
        {
            string error = string.Empty;
            XDocument xdoc = XDocument.Load("config.xml");
            XElement configuration = xdoc.Element("configuration");
            string language = configuration.Element("language").Value;
            string directory = configuration.Element("directory").Value;
            string repo = configuration.Element("repo").Value;

            HttpClient client = new();
            //HttpResponseMessage responseFile = client.GetAsync("https://api.github.com/repos/Ludeon/" + repo + "/zipball/master").Result;
            HttpResponseMessage responseFile = client.GetAsync("https://github.com/Ludeon/" + repo + "/archive/refs/heads/master.zip").Result;
            Stream stream = responseFile.Content.ReadAsStreamAsync().Result;
            FastZip zip1 = new();
            string tempDir = "temp";
            zip1.ExtractZip(stream, tempDir, FastZip.Overwrite.Always, null, null, null, true, true, true);
            stream.Close();

            string[] basedir = Directory.GetDirectories(tempDir);
            string[] dir = Directory.GetDirectories(basedir[0]);
            StringComparison comparison = StringComparison.OrdinalIgnoreCase;
            foreach (string dirEntry in dir)
            {
                if (dirEntry.EndsWith("Biotech", comparison))
                {
                    FolderUpdate("Biotech", dirEntry, directory, language);
                }
                else if (dirEntry.EndsWith("Core", comparison))
                {
                    FolderUpdate("Core", dirEntry, directory, language);
                }
                else if (dirEntry.EndsWith("Ideology", comparison))
                {
                    FolderUpdate("Ideology", dirEntry, directory, language);
                }
                else if (dirEntry.EndsWith("Royalty", comparison))
                {
                    FolderUpdate("Royalty", dirEntry, directory, language);
                }
            }
            Directory.Delete(tempDir, true);
            return (true, error);
        }

        internal static void FolderUpdate(string type, string tempPath, string path, string? language)
        {
            string tempDir = path + "\\Data\\" + type + "\\Languages\\" + language;
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
            Directory.Move(tempPath, tempDir);
        }
    }
}