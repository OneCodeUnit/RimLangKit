using Cyriller;
using Cyriller.Model;
using System.Xml.Linq;

namespace RimLangKit
{
    internal static class CaseCreate
    {
        private static readonly CyrNounCollection cyrNounCollection = new();
        private static readonly CyrAdjectiveCollection cyrAdjectiveCollection = new();
        private static readonly CyrPhrase cyrPhrase = new(cyrNounCollection, cyrAdjectiveCollection);

        private static string[] Declension(string word)
        {
            CyrNoun nouns = cyrNounCollection.Get(word, out string foundWord, out CasesEnum @case, out NumbersEnum number);
            string[] result = nouns.Decline().ToArray();
            result[0] = word;

            string[] resultP = nouns.DeclinePlural().ToArray();
            string[] attributes = { resultP[0], nouns.Gender.ToString() };

            return result;
        }
        private static string[] DeclensionComposite(string word)
        {
            CyrResult nouns = cyrPhrase.Decline(word, GetConditionsEnum.Similar);
            string[] result = nouns.ToArray();
            result[0] = word;
            return result;
        }

        private static string[] Attributes(string word)
        {
            CyrNoun nouns = cyrNounCollection.Get(word, out string foundWord, out CasesEnum @case, out NumbersEnum number);
            string[] result = nouns.DeclinePlural().ToArray();
            string[] attributes = { result[0], nouns.Gender.ToString() };
            return attributes;
        }
        private static string[] AttributesComposite(string word)
        {
            CyrResult nouns = cyrPhrase.DeclinePlural(word, GetConditionsEnum.Similar);
            string[] result = nouns.ToArray();
            string[] attributes = { result[0], "Composite" };
            return attributes;
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
        
        internal static List<string> FindWordsProcessing(string CurrentFile, string DefType)
        {
            List<string> words = new();
            if (CurrentFile.Contains(DefType, StringComparison.OrdinalIgnoreCase))
            {
                words.AddRange(WordsExtract(CurrentFile));
            }
            return words;
        }

        internal static void CreateCase(string DirectoryPath, List<string> words, string DefType)
        {
            string pathCase = DirectoryPath + "\\Languages\\Russian\\WordInfo\\Case.txt";
            StreamWriter writerCase = new(pathCase, true, System.Text.Encoding.UTF8);
            writerCase.WriteLine(Environment.NewLine + "// " + DefType);

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
            writerCase.Close();
        }

        internal static void CreateGender(string DirectoryPath, List<string> words, string DefType)
        {
            string pathPlural = DirectoryPath + "\\Languages\\Russian\\WordInfo\\Plural.txt";
            StreamWriter writerPlural = new(pathPlural, true, System.Text.Encoding.UTF8);
            writerPlural.WriteLine(Environment.NewLine + "// " + DefType);

            string pathGenderFemale = DirectoryPath + "\\Languages\\Russian\\WordInfo\\Gender\\Female.txt";
            StreamWriter writerFemale = new(pathGenderFemale, true, System.Text.Encoding.UTF8);
            writerFemale.WriteLine(Environment.NewLine + "// " + DefType);
            string pathGenderMale = DirectoryPath + "\\Languages\\Russian\\WordInfo\\Gender\\Male.txt";
            StreamWriter writerMale = new(pathGenderMale, true, System.Text.Encoding.UTF8);
            writerMale.WriteLine(Environment.NewLine + "// " + DefType);
            string pathGenderNeuter = DirectoryPath + "\\Languages\\Russian\\WordInfo\\Gender\\Neuter.txt";
            StreamWriter writerNeuter = new(pathGenderNeuter, true, System.Text.Encoding.UTF8);
            writerNeuter.WriteLine(Environment.NewLine + "// " + DefType);
            string pathGenderUndefined = DirectoryPath + "\\Languages\\Russian\\WordInfo\\Gender\\Undefined.txt";
            StreamWriter writerUndefined = new(pathGenderUndefined, true, System.Text.Encoding.UTF8);
            writerUndefined.WriteLine(Environment.NewLine + "// " + DefType);

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
            writerPlural.Close();
            writerFemale.Close();
            writerMale.Close();
            writerNeuter.Close();
            writerUndefined.Close();
        }
    }
}
