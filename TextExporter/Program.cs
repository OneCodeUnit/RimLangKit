using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.Serialization;
using RimLanguageCore.TextExporter;

namespace TextExporter
{
    internal class Program
    {
        static string testSourceFile = "C:\\Users\\inqui\\Desktop\\Новая папка\\MedievalCommon_Adult.xml";
        static string testFile = "C:\\Users\\inqui\\Desktop\\test\\test.xml";
        static string testDirectory = "C:\\Users\\inqui\\Desktop\\test";
        static string DirectoryPath = "C:\\Users\\inqui\\Desktop\\Новая папка";

        static void Main()
        {
            string[] allFiles = Directory.GetFiles(DirectoryPath, "*.xml", SearchOption.AllDirectories);
            foreach (string tempFile in allFiles)
            {
                RimFile file = new(tempFile);
                file.Open();
                file.Save();

            }
            //Console.ReadKey();
        }

        static void FindDefs(string currentFile)
        {
            // Выгрузка тегов
            FileStream open = new(currentFile, FileMode.OpenOrCreate);
            XmlSerializer serializer = new (typeof(Defs));
            Defs? defs = (Defs?)serializer.Deserialize(open);
            open.Close();

            FileStream save = new(testFile, FileMode.Create);
            serializer.Serialize(save, defs);
            save.Close();
            Console.WriteLine("Object has been serialized");

            XDocument xDoc = XDocument.Load(testFile, LoadOptions.PreserveWhitespace);
            XElement languageData = new("LanguageData");
            
            XElement oldDefs = xDoc.Element("Defs");
            foreach (XElement currentDef in oldDefs.Elements())
            {
                string defName = string.Empty;
                foreach (XElement currentTag in currentDef.Elements())
                {              
                    if (currentTag.Name == "defName")
                    {
                        defName = currentTag.Value;
                    }
                    else
                    {
                        XElement extractedTag = new($"{defName}.{currentTag.Name}", currentTag.Value);
                        languageData.Add(extractedTag);
                    }
                }
                XComment space = new("comment");
                languageData.Add(space);
            }
            xDoc.Element("Defs").Remove();
            xDoc.Add(languageData);

            string[] path = currentFile.Split('\\');
            string pathExt = path[^1];
            pathExt = testDirectory + "\\\\1" + pathExt;

            if (languageData.Elements().Count() > 0)
            {
                xDoc.Save(pathExt);
                string text = File.ReadAllText(pathExt);
                text = Regex.Replace(text, "<!--comment-->", "");
                File.WriteAllText(pathExt, text);
            }
            File.Delete(testFile);


        }
    }
}