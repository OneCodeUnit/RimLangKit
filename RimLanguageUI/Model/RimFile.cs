using System.Xml.Linq;

namespace RimLanguageUI.Model
{
    public class RimFile
    {
        private readonly string path;
        private readonly List<RimTag> tags = [];

        private static readonly HashSet<string> tagNames = ["label", "description", "title", "titleShort"];

        public RimFile(string path)
        {
            this.path = path;
        }

        public int GetCount()
        {
            return tags.Count;
        }

        public string Open()
        {
            XDocument xDoc;
            // Позволяет избежать падения при загрузке сломанного .xml файла
            try
            {
                xDoc = XDocument.Load(path);
            }
            catch (Exception ex)
            {
                return $"Не удалось загрузить файл: {ex.Message}";
            }
            // Позволяет избежать обработки пустого или не содержащего нужных тегов файла
            XElement? root = xDoc.Element("Defs");
            if (root is null)
                return "Не удалось найти корневой элемент 'Defs'";
            // Перевод контекста в содержимое тега Defs
            if (!root.Elements().Any())
                return "Корневой элемент 'Defs' не содержит дочерних элементов.";

            // Сбор тегов файла
            foreach (XElement node in root.Elements())
            {
                string tempDefType = node.Name.LocalName;
                string tempDefName = string.Empty;
                foreach (XElement currentNode in node.Elements())
                {
                    if (currentNode.Name.LocalName == "defName")
                        tempDefName = currentNode.Value;
                    if (tagNames.Contains(currentNode.Name.LocalName))
                    {
                        RimTag tag = new(currentNode.Name.LocalName, currentNode.Value, tempDefType, tempDefName);
                        tags.Add(tag);
                    }
                }
            }
            return "OK";
        }
    }
}
