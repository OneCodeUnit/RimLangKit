using System.Globalization;

namespace RimLangKit
{
    internal static class TextTranslit
    {
        internal static (bool, string) TextTranslitProcessing(string CurrentFile, bool mode)
        {
            string error = string.Empty;
            CultureInfo culture = CultureInfo.CurrentCulture;
            StringComparison comparison = StringComparison.OrdinalIgnoreCase;
            //Чтение файла словаря
            StreamReader dictionaryText;
            try
            {
                dictionaryText = new("dictionary.txt");
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
            string line4 = SetLine(dictionaryText.ReadLine());
            string line3 = SetLine(dictionaryText.ReadLine());
            string line2 = SetLine(dictionaryText.ReadLine());
            string line1 = SetLine(dictionaryText.ReadLine());
            string[] lines = { line4, line3, line2, line1 };
            string lineClear = SetLine(dictionaryText.ReadLine());
            string lineStart = SetLine(dictionaryText.ReadLine());
            string lineEnd = SetLine(dictionaryText.ReadLine());

            //Заполнение словаря
            Dictionary<string, string> dictionary = new(SetDictionary(lines));
            Dictionary<string, string> dictionaryClear = new(SetDictionary(lineClear));
            Dictionary<string, string> dictionaryStart = new(SetDictionary(lineStart));
            Dictionary<string, string> dictionaryEnd = new(SetDictionary(lineEnd));

            //Открытие файлов
            StreamReader sourceText = new(CurrentFile);
            string textFileRus = CurrentFile.Replace(".txt", "_NEW.txt");
            StreamWriter translatedText = new(textFileRus);

            //Процесс транслитерации
            if (mode)
            {
                while (true)
                {
                    string? temp = sourceText.ReadLine();
                    if (temp == null) break;
                    string source = temp.Trim().ToLower(culture);

                    foreach (KeyValuePair<string, string> item in dictionaryStart)
                    {
                        if (source.StartsWith(item.Key, comparison))
                        {
                            source = source.Replace(item.Key, item.Value);
                            break;
                        }
                    }
                    foreach (KeyValuePair<string, string> item in dictionaryEnd)
                    {
                        if (source.EndsWith(item.Key, comparison))
                        {
                            source = source.Replace(item.Key, item.Value);
                            break;
                        }
                    }
                    foreach (KeyValuePair<string, string> item in dictionaryClear)
                    {
                        if (source.Contains(item.Key))
                        {
                            source = source.Replace(item.Key, item.Value);
                        }
                    }

                    foreach (KeyValuePair<string, string> item in dictionary)
                    {
                        if (source.Contains(item.Key))
                        {
                            source = source.Replace(item.Key, item.Value);
                        }
                    }

                    for (int i = 0; i < source.Length; i++)
                    {
                        if (source[i] == '=')
                        {
                            source = source.Replace(source[i].ToString(), string.Empty);
                        }
                    }
                    if (source.Length > 1)
                    {
                        source = string.Concat(source[0].ToString().ToUpper(culture), source.AsSpan(1));
                    }
                    else
                    {
                        source = source.ToUpper(culture);
                    }
                    translatedText.WriteLine(source);
                }
            }
            else
            {
                while (true)
                {
                    string? temp = sourceText.ReadLine();
                    if (temp == null) break;
                    string source = temp.Trim().ToLower(culture);

                    foreach (KeyValuePair<string, string> item in dictionaryStart)
                    {
                        if (source.StartsWith(item.Value, comparison))
                        {
                            source = source.Replace(item.Value, item.Key);
                            break;
                        }
                    }
                    foreach (KeyValuePair<string, string> item in dictionaryEnd)
                    {
                        if (source.EndsWith(item.Value, comparison))
                        {
                            source = source.Replace(item.Value, item.Key);
                            break;
                        }
                    }
                    foreach (KeyValuePair<string, string> item in dictionaryClear)
                    {
                        if (source.Contains(item.Value))
                        {
                            source = source.Replace(item.Value, item.Key);
                        }
                    }

                    foreach (KeyValuePair<string, string> item in dictionary)
                    {
                        if (source.Contains(item.Value))
                        {
                            source = source.Replace(item.Value, item.Key);
                        }
                    }

                    for (int i = 0; i < source.Length; i++)
                    {
                        if (source[i] == '=')
                        {
                            source = source.Replace(source[i].ToString(), string.Empty);
                        }
                    }
                    if (source.Length > 1)
                    {
                        source = string.Concat(source[0].ToString().ToUpper(culture), source.AsSpan(1));
                    }
                    else
                    {
                        source = source.ToUpper(culture);
                    }
                    translatedText.WriteLine(source);
                }
            }

            //Завершение работы
            sourceText.Close();
            translatedText.Close();
            return (true, error);
        }

        private static string SetLine(string? line)
        {
            return line is null ? string.Empty : line.Trim();
        }

        private static Dictionary<string, string> SetDictionary(string line)
        {
            Dictionary<string, string> dictionary = new();
            foreach (string item in line.Split(","))
            {
                string[] symbols = item.Split(":");
                dictionary.Add(symbols[0], symbols[1]);
            }
            return dictionary;
        }

        private static Dictionary<string, string> SetDictionary(string[] lines)
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
    }
}
