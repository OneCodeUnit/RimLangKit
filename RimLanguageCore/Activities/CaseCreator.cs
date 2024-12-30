using RimLanguageCore.Misc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace RimLanguageCore.Activities
{
    public static class CaseCreator
    {
        private const string CasePath = "\\Languages\\Russian\\WordInfo";

        private static List<string> ExtractWords(string file)
        {
            XDocument xDoc = XDocument.Load(file, LoadOptions.PreserveWhitespace);
            XElement root = xDoc.Element("LanguageData");
            List<string> words = new();
            if (root is null) { return words; }
            foreach (XElement node in root.Elements())
            {
                string nodeName = node.Name.ToString();
                StringComparison nodeComparison = StringComparison.OrdinalIgnoreCase;
                if ((nodeName.EndsWith(".label", nodeComparison) && !nodeName.Contains(".stages.") && !nodeName.Contains(".verbs.")) || nodeName.EndsWith(".chargeNoun", nodeComparison))
                {
                    words.Add(node.Value);
                }
            }
            return words;
        }

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
                WordForms result = MorpherHttpClient.GetMorpherWords(tempWord);
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
                else if ((!word.Key.EndsWith("а", StringComparison.OrdinalIgnoreCase)) && (!word.Key.EndsWith("я", StringComparison.OrdinalIgnoreCase))
                && (!word.Key.EndsWith("о", StringComparison.OrdinalIgnoreCase)) && (!word.Key.EndsWith("е", StringComparison.OrdinalIgnoreCase))
                && (!word.Key.EndsWith("ы", StringComparison.OrdinalIgnoreCase)) && (!word.Key.EndsWith("и", StringComparison.OrdinalIgnoreCase)))
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