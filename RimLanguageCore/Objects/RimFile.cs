using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace RimLanguageCore.TextExporter
{
    public class RimFile
    {
        private readonly string path;
        private readonly List<RimTag> tags = new();

        private static readonly HashSet<string> tagNames = new() { "label", "description", "title", "titleShort" };
        public RimFile(string path)
        {
            this.path = path;
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
            XElement root = xDoc.Element("Defs");
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

        public void Save()
        {
            var uniqueDefs = tags.Select(tag => tag.GetDefType()).Distinct();
            foreach (var def in uniqueDefs)
            {
                XDocument xDoc = new();
                XElement languageData = new("LanguageData");

                string tempDefName = string.Empty;
                foreach (var tag in tags)
                {
                    if (tag.GetDefType().Equals(def, StringComparison.OrdinalIgnoreCase))
                    {
                        if (tempDefName != tag.GetDefName())
                        {
                            XComment tempComment = new("");
                            languageData.Add(tempComment);
                        }
                        tempDefName = tag.GetDefName();

                        XComment comment = new($" EN: {tag.GetTagText()} ");
                        languageData.Add(comment);
                        XElement extractedTag = new($"{tag.GetDefName()}.{tag.GetTagKey()}", tag.GetTagText());
                        languageData.Add(extractedTag);
                    }
                }
                xDoc.Add(languageData);

                string newPath = $"{path[0..path.LastIndexOf('\\')]}\\{def}{path[path.LastIndexOf('\\')..^4]}_New.xml";
                if (Directory.Exists(newPath[0..newPath.LastIndexOf('\\')]))
                {
                    if (File.Exists(newPath))
                    {
                        string oldText = File.ReadAllText(newPath);
                        string newText = xDoc.ToString();
                        string combinedText = oldText + newText;
                        combinedText = combinedText.Replace("</LanguageData><LanguageData>", "");
                        File.WriteAllText(newPath, combinedText);
                    }
                    else
                        xDoc.Save(newPath, SaveOptions.None);
                }
                else
                {
                    Directory.CreateDirectory(newPath[0..newPath.LastIndexOf('\\')]);
                    xDoc.Save(newPath, SaveOptions.None);
                }
                string text = File.ReadAllText(newPath);
                text = text.Replace("<!---->", "");
                File.WriteAllText(newPath, text);
            }
        }
    }
}
