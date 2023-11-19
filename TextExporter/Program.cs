using System.Xml.Linq;

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
            Console.WriteLine("Путь к папке мода:");
            //string? modPath = Console.ReadLine();
            string? modPath = "C:\\Users\\inqui\\Desktop\\2975771801";
            if (modPath == null)
            {
                Console.WriteLine("Путь не введён");
                return;
            }
            if (!Directory.Exists(modPath))
            {
                Console.WriteLine("Папка не существует");
                return;
            }
            string[] allDirectories = Directory.GetDirectories(modPath, "defs", SearchOption.AllDirectories);
            string? defPath = allDirectories.OrderBy(directory => directory.Length).FirstOrDefault();
            if (defPath == null)
            {
                Console.WriteLine("defs не найдена");
                return;
            }

            string[] allFiles = Directory.GetFiles(defPath, "*.xml", SearchOption.AllDirectories);
            int count = 0;
            int errCount = 0;
            bool result;
            result = FindDefs("C:\\Users\\inqui\\Desktop\\2975771801\\1.4\\Defs\\DamageDefs\\Damages_Misc.xml");
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
            // Перевод контекста в тег Defs
            XElement? root = xDoc.Element("Defs");
            if (root?.Elements() is null)
            {
                Console.WriteLine($"Тег Defs пуст {currentFile}");
                return false;
            }
            // Перевод контекста в Def класс
            foreach (var defClass in root.Elements())
            {
                Dictionary<string, string> block = new();
                string currentDefClass = defClass.Name.ToString();
                // Поиск типичных тегов
                if (TagsToDefs.ContainsKey(currentDefClass))
                {
                    // Перевод контекста в Def тег
                    foreach (var tag in defClass.Elements())
                    {
                        // Поиск нужных тегов
                        if (TagsToDefs[currentDefClass].Contains(tag.Name.ToString()))
                            block.Add(tag.Name.ToString(), tag.Value.ToString());
                    }
                }
                else
                {
                    foreach (var tag in defClass.Elements())
                    {
                        if (CommonTags.Contains(tag.Name.ToString()))
                            block.Add(tag.Name.ToString(), tag.Value.ToString());
                    }
                }
                // Запуск дополнительной логики

                // Сохранение в файл
                if (block.Count > 2)
                {

                }
            }
            return true;
        }
    }
}