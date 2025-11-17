using RimLangKit.Models.Morpher;
using RimLangKit.Services;
using System.Xml.Linq;

namespace RimLangKit.Processors
{
    /// <summary>
    /// Создает падежные формы и информацию о роде для русских слов из файлов переводов.
    /// </summary>
    /// <remarks>
    /// Использует сервис Morpher для получения падежных форм и определяет род по окончаниям слов.
    /// </remarks>
    public static class CaseCreator
    {
        /// <summary>Относительный путь к папке с информацией о словах.</summary>
        private const string CasePath = "\\Languages\\Russian\\WordInfo";

        /// <summary>
        /// Извлекает слова из XML-файла для обработки.
        /// </summary>
        /// <param name="file">Путь к XML-файлу.</param>
        /// <returns>Список слов, извлеченных из тегов label и chargeNoun.</returns>
        private static List<string> ExtractWords(string file)
        {
            XDocument xDoc = XDocument.Load(file, LoadOptions.PreserveWhitespace);
            XElement? root = xDoc.Element("LanguageData");
            List<string> words = [];
            if (root is null) { return words; }
            foreach (XElement node in root.Elements())
            {
                string nodeName = node.Name.ToString();
                StringComparison nodeComparison = StringComparison.OrdinalIgnoreCase;
                if (nodeName.EndsWith(".label", nodeComparison) && !nodeName.Contains(".stages.") && !nodeName.Contains(".verbs.") || nodeName.EndsWith(".chargeNoun", nodeComparison))
                {
                    words.Add(node.Value);
                }
            }
            return words;
        }

        /// <summary>
        /// Обрабатывает файл и извлекает слова, если файл соответствует указанному типу определения.
        /// </summary>
        /// <param name="currentFile">Полный путь к XML-файлу.</param>
        /// <param name="defType">Тип определения для фильтрации файлов.</param>
        /// <returns>Список извлеченных слов или пустой список, если файл не соответствует defType.</returns>
        public static List<string> FindWordsProcessing(string currentFile, string defType)
        {
            List<string> words = new();
            // Если файл соответствует заданному defType, то запускатся в работу
            if (currentFile.Contains(defType, StringComparison.OrdinalIgnoreCase))
            {
                words.AddRange(ExtractWords(currentFile));
            }
            return words;
        }

        /// <summary>
        /// Создает файлы с падежными формами и формами множественного числа для указанных слов.
        /// </summary>
        /// <param name="directoryPath">Путь к корневой директории мода/игры.</param>
        /// <param name="words">Словарь слов, где ключ - слово, значение - тип определения.</param>
        /// <param name="defType">Тип определения для фильтрации слов.</param>
        /// <remarks>
        /// Создает файлы Case.txt (падежные формы) и Plural.txt (формы множественного числа)
        /// в папке Languages\Russian\WordInfo.
        /// Использует сервис Morpher для получения форм слов.
        /// </remarks>
        public static void CreateCase(string directoryPath, Dictionary<string, string> words, string defType)
        {
            string path = directoryPath + CasePath;
            Directory.CreateDirectory(path); // Созданиие директории, которая наверняка отсутствует
            StreamWriter writerCase = new(path + "\\Case.txt", true, System.Text.Encoding.UTF8);
            StreamWriter writerPlural = new(path + "\\Plural.txt", true, System.Text.Encoding.UTF8);
            writerCase.WriteLine("// " + defType);
            writerPlural.WriteLine("// " + defType);

            foreach (var word in words)
            {
                if (word.Value != defType)
                {
                    continue;
                }

                string tempWord = word.Key;
                WordForms? result = MorpherService.GetMorpherWords(tempWord);
                // Case.txt
                string tempStringCase = result is null
                    ? $"{tempWord}; {tempWord}; {tempWord}; {tempWord}; {tempWord}; {tempWord}"
                    : $"{tempWord}; {result.Genitive}; {result.Dative}; {result.Accusative}; {result.Instrumental}; {result.Prepositional}";
                writerCase.WriteLine(tempStringCase);

                // Plural.txt
                tempStringCase = result is null
                    ? $"{tempWord}; {tempWord}"
                    : result.Plural is null ? string.Empty : $"{tempWord}; {result.Plural.Nominative}";
                writerPlural.WriteLine(tempStringCase);
            }
            writerCase.WriteLine();
            writerCase.Close();
            writerPlural.WriteLine();
            writerPlural.Close();
        }

        /// <summary>
        /// Создает файлы с классификацией слов по родам на основе их окончаний.
        /// </summary>
        /// <param name="directoryPath">Путь к корневой директории мода/игры.</param>
        /// <param name="words">Словарь слов, где ключ - слово, значение - тип определения.</param>
        /// <param name="defType">Тип определения для фильтрации слов.</param>
        /// <remarks>
        /// Создает файлы в папке Languages\Russian\WordInfo\Gender:
        /// - Female.txt (женский род: слова на -а, -я)
        /// - Male.txt (мужской род: согласные окончания)
        /// - Neuter.txt (средний род: слова на -о, -е)
        /// - Plural.txt (множественное число: слова на -ы, -и)
        /// - Undefined.txt (неопределенные слова)
        /// </remarks>
        public static void CreateGender(string directoryPath, Dictionary<string, string> words, string defType)
        {
            string pathGender = directoryPath + CasePath + "\\Gender";
            Directory.CreateDirectory(pathGender);

            string pathGenderFemale = pathGender + "\\Female.txt";
            StreamWriter writerFemale = new(pathGenderFemale, true, System.Text.Encoding.UTF8);
            writerFemale.WriteLine("// " + defType);
            foreach (var word in words)
            {
                if (word.Value != defType)
                {
                    continue;
                }
                else if (word.Key.EndsWith("а", StringComparison.OrdinalIgnoreCase) || word.Key.EndsWith("я", StringComparison.OrdinalIgnoreCase))
                {
                    writerFemale.WriteLine(word.Key);
                    words.Remove(word.Key);
                }
            }
            writerFemale.WriteLine();
            writerFemale.Close();

            string pathGenderMale = pathGender + "\\Male.txt";
            StreamWriter writerMale = new(pathGenderMale, true, System.Text.Encoding.UTF8);
            writerMale.WriteLine("// " + defType);
            foreach (var word in words)
            {
                if (word.Value != defType)
                {
                    continue;
                }
                else if (!word.Key.EndsWith("а", StringComparison.OrdinalIgnoreCase) && !word.Key.EndsWith("я", StringComparison.OrdinalIgnoreCase)
                && !word.Key.EndsWith("о", StringComparison.OrdinalIgnoreCase) && !word.Key.EndsWith("е", StringComparison.OrdinalIgnoreCase)
                && !word.Key.EndsWith("ы", StringComparison.OrdinalIgnoreCase) && !word.Key.EndsWith("и", StringComparison.OrdinalIgnoreCase))
                {
                    writerMale.WriteLine(word.Key);
                    words.Remove(word.Key);
                }
            }
            writerMale.WriteLine();
            writerMale.Close();

            string pathGenderNeuter = pathGender + "\\Neuter.txt";
            StreamWriter writerNeuter = new(pathGenderNeuter, true, System.Text.Encoding.UTF8);
            writerNeuter.WriteLine("// " + defType);
            foreach (var word in words)
            {
                if (word.Value != defType)
                {
                    continue;
                }
                else if (word.Key.EndsWith("о", StringComparison.OrdinalIgnoreCase) || word.Key.EndsWith("е", StringComparison.OrdinalIgnoreCase))
                {
                    writerNeuter.WriteLine(word.Key);
                    words.Remove(word.Key);
                }
            }
            writerNeuter.WriteLine();
            writerNeuter.Close();

            string pathGenderPlural = pathGender + "\\Plural.txt";
            StreamWriter writerPlural = new(pathGenderPlural, true, System.Text.Encoding.UTF8);
            writerPlural.WriteLine("// " + defType);
            foreach (var word in words)
            {
                if (word.Value != defType)
                {
                    continue;
                }
                else if (word.Key.EndsWith("ы", StringComparison.OrdinalIgnoreCase) || word.Key.EndsWith("и", StringComparison.OrdinalIgnoreCase))
                {
                    writerPlural.WriteLine(word.Key);
                    words.Remove(word.Key);
                }
            }
            writerPlural.WriteLine();
            writerPlural.Close();

            if (words.Count < 1)
                return;
            string pathGenderUndefined = pathGender + "\\Undefined.txt";
            StreamWriter writerUndefined = new(pathGenderUndefined, true, System.Text.Encoding.UTF8);
            writerUndefined.WriteLine("// " + defType);
            foreach (var word in words)
            {
                if (word.Value != defType)
                {
                    continue;
                }

                writerUndefined.WriteLine(word.Key);
            }
            writerUndefined.WriteLine();
            writerUndefined.Close();
        }
    }
}