using Cyriller;
using Cyriller.Model;
using System.IO;
using System.Xml.Linq;

namespace RimLangKit
{
    internal static class CaseCreate
    {
        private static readonly CyrNounCollection cyrNounCollection = new();
        private static readonly CyrAdjectiveCollection cyrAdjectiveCollection = new();
        private static readonly CyrPhrase cyrPhrase = new(cyrNounCollection, cyrAdjectiveCollection);
        private const string CasePath = "\\Languages\\Russian\\WordInfo";

        private static string[] Declension(string word)
        {
            // try-catch - неэффективно, но слово может совсем не найтись. Вот бы просто возвращало что-то вроде -1
            try
            {
                CyrNoun nouns = cyrNounCollection.Get(word, out string foundWord, out CasesEnum @case, out NumbersEnum number);
                string[] result = nouns.Decline().ToArray();
                result[0] = word;
                return result;
            }
            catch 
            {
                string[] result = { word, word, word, word, word, word };
                return result;
            }
        }
        private static string[] DeclensionComposite(string word)
        {
            try
            {
                CyrResult nouns = cyrPhrase.Decline(word, GetConditionsEnum.Similar);
                string[] result = nouns.ToArray();
                result[0] = word;
                return result;
            }
            catch
            {
                string[] result = { word, word, word, word, word, word };
                return result;
            }
        }

        // Возвращает форму множественного числа и пол под позициями 0 и 1 соотвественно 
        private static string[] Attributes(string word)
        {
            try
            {
                CyrNoun nouns = cyrNounCollection.Get(word, out string foundWord, out CasesEnum @case, out NumbersEnum number);
                string[] result = nouns.DeclinePlural().ToArray();
                string[] attributes = { result[0], nouns.Gender.ToString() };
                return attributes;
            }
            catch
            {
                string[] attributes = { word, "Undefined" };
                return attributes;
            }
        }
        private static string[] AttributesComposite(string word)
        {
            try
            {
                CyrResult nouns = cyrPhrase.DeclinePlural(word, GetConditionsEnum.Similar);
                string[] result = nouns.ToArray();
                string[] attributes = { result[0], "Composite" };
                return attributes;
            }
            catch
            {
                string[] attributes = { word, "Undefined" };
                return attributes;
            }
        }

        private static List<string> WordsExtract(string file)
        {
            XDocument xDoc = XDocument.Load(file, LoadOptions.PreserveWhitespace);
            XElement? root = xDoc.Element("LanguageData");
            List<string> words = new();
            foreach (XElement node in root.Elements())
            {
                if (node.Name.ToString().EndsWith("label", StringComparison.OrdinalIgnoreCase) || node.Name.ToString().EndsWith("chargeNoun", StringComparison.OrdinalIgnoreCase))
                {
                    words.Add(node.Value);
                }
            }
            return words;
        }
        
        internal static List<string> FindWordsProcessing(string currentFile, string defType)
        {
            List<string> words = new();
            // Если файл соответствует заданному defType, то запускатся в работу
            if (currentFile.Contains(defType, StringComparison.OrdinalIgnoreCase))
            {
                words.AddRange(WordsExtract(currentFile));
            }
            return words;
        }

        internal static void CreateCase(string directoryPath, List<string> words, string defType)
        {
            string path = directoryPath + CasePath;
            Directory.CreateDirectory(path); // Созданиие директории, которая наверняка отсутствует
            path += "\\Case.txt";
            StreamWriter writerCase = new(path, true, System.Text.Encoding.UTF8);
            writerCase.WriteLine("// " + defType);

            foreach (string word in words)
            {
                string[] declinations;
                if (word.Contains('-') || word.Contains(' '))
                {
                    declinations = DeclensionComposite(word);
                }
                else
                {
                    declinations = Declension(word);
                }

                // Case.txt
                string tempStringCase = string.Empty;
                foreach (string declination in declinations)
                {
                    tempStringCase += $"{declination}; ";
                }
                writerCase.WriteLine(tempStringCase[..^2]);
            }
            writerCase.WriteLine();
            writerCase.Close();
        }

        internal static void CreateGender(string directoryPath, List<string> words, string defType)
        {
            string pathPlural = directoryPath + CasePath + "\\Plural.txt";
            StreamWriter writerPlural = new(pathPlural, true, System.Text.Encoding.UTF8);
            writerPlural.WriteLine("// " + defType);

            string pathGender = directoryPath + CasePath + "\\Gender";
            Directory.CreateDirectory(pathGender);

            string pathGenderFemale = pathGender + "\\Female.txt";
            StreamWriter writerFemale = new(pathGenderFemale, true, System.Text.Encoding.UTF8);
            writerFemale.WriteLine("// " + defType);

            string pathGenderMale = pathGender + "\\Male.txt";
            StreamWriter writerMale = new(pathGenderMale, true, System.Text.Encoding.UTF8);
            writerMale.WriteLine("// " + defType);

            string pathGenderNeuter = pathGender + "\\Neuter.txt";
            StreamWriter writerNeuter = new(pathGenderNeuter, true, System.Text.Encoding.UTF8);
            writerNeuter.WriteLine("// " + defType);

            string pathGenderUndefined = pathGender + "\\Undefined.txt";
            StreamWriter writerUndefined = new(pathGenderUndefined, true, System.Text.Encoding.UTF8);
            writerUndefined.WriteLine("// " + defType);

            foreach (string word in words)
            {
                string[] attributes;
                if (word.Contains('-') || word.Contains(' '))
                {
                    attributes = AttributesComposite(word);
                }
                else
                {
                    attributes = Attributes(word);
                }

                // Plural.txt
                string tempStringPlural = $"{word}; {attributes[0]}";
                writerPlural.WriteLine(tempStringPlural);
                // Gender.txt
                switch (attributes[1])
                {
                    case "Feminine":
                        writerFemale.WriteLine(word);
                        break;
                    case "Masculine":
                        writerMale.WriteLine(word);
                        break;
                    case "Neuter":
                        writerNeuter.WriteLine(word);
                        break;
                    case "Undefined":
                        writerUndefined.WriteLine(word);
                        break;
                    case "Composite":
                        writerUndefined.WriteLine(word);
                        break;
                }
            }
            writerPlural.WriteLine();
            writerPlural.Close();
            writerFemale.WriteLine();
            writerFemale.Close();
            writerMale.WriteLine();
            writerMale.Close();
            writerNeuter.WriteLine();
            writerNeuter.Close();
            writerUndefined.WriteLine();
            writerUndefined.Close();
        }
    }
}
