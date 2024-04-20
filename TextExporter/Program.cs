using System;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace TextExporter
{
    internal class Program
    {
        static readonly List<string> CommonTags = new() { "defName", "label", "description" };
        static readonly List<string> DamageDefTags = new() { "defName", "label", "deathMessage" };
        static readonly List<string> HediffDefTags = new() { "defName", "label", "description", "labelNoun", "description" };
        static readonly Dictionary<string, List<string>> TagsToDefs = new()
        {
            {"DamageDef", DamageDefTags},
            {"HediffDef", HediffDefTags}
        };

        static void Main()
        {
            //Console.WriteLine("Путь к папке мода:");
            //string? modPath = Console.ReadLine();
            //if (modPath == null)
            //{
            //    Console.WriteLine("Путь не введён");
            //    return;
            //}
            //if (!Directory.Exists(modPath))
            //{
            //    Console.WriteLine("Папка не существует");
            //    return;
            //}
            //string[] allDirectories = Directory.GetDirectories(modPath, "defs", SearchOption.AllDirectories);
            //string? defPath = allDirectories.OrderBy(directory => directory.Length).FirstOrDefault();
            //if (defPath == null)
            //{
            //    Console.WriteLine("defs не найдена");
            //    return;
            //}

            //string[] allFiles = Directory.GetFiles(defPath, "*.xml", SearchOption.AllDirectories);
            int count = 0;
            int errCount = 0;
            bool result;
            result = FindDefs("C:\\Users\\inqui\\Desktop\\Новая папка\\GeneDefs_Aging.xml");
            //foreach (string tempFile in allFiles)
            //{
            //    result = FindDefs(tempFile);
            //    if (result) count++;
            //    else errCount++;
            //}
            Console.WriteLine($"Успешно завершено. Обработано файлов - {count}, пропущено - {errCount}");
            Console.ReadKey();
        }

        static bool FindDefs(string currentFile)
        {
            // Позволяет избежать падения при загрузке сломанного .xml файла
            try
            {
                XDocument.Load(currentFile, LoadOptions.PreserveWhitespace);
            }
            catch
            {
                Console.WriteLine($"Не удалось загрузить файл {currentFile}");
                return false;
            }
            XDocument xDoc = XDocument.Load(currentFile, LoadOptions.PreserveWhitespace);
            // Позволяет избежать падения при загрузке пустого .xml файла
            if (xDoc.Element("Defs") is null)
            {
                Console.WriteLine($"Не удалось найти Defs {currentFile}");
                return false;
            }
            // Проверка Defs
            XElement? root = xDoc.Element("Defs");
            if (root?.Elements() is null)
            {
                Console.WriteLine($"Тег Defs пуст {currentFile}");
                return false;
            }
            // Выгрузка тегов
            FileStream fs = new (currentFile, FileMode.OpenOrCreate);
            XmlSerializer serializer = new (typeof(Defs));
            Defs? tags = (Defs?)serializer.Deserialize(fs);

            // Сложно. Как это сделать?
            //foreach (var def in tags.GeneDef) 
            //{

            //}



            Console.WriteLine(tags);








            return true;
        }
    }
}