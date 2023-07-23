using System.Xml;
using System.Xml.Linq;

namespace RimLangKit
{
    internal static class CommentInsert
    {
        internal static bool CommentsInsertProcessing(string currentFile, TextBox textBox)
        {
            //Позволяет избежать падения при загрузке сломанного .xml файла
            try
            {
                XDocument.Load(currentFile, LoadOptions.PreserveWhitespace);
            }
            catch
            {
                textBox.AppendText($"{Environment.NewLine}Не удалось загрузить файл {currentFile}");
                return false;
            }

            XDocument xDoc = XDocument.Load(currentFile, LoadOptions.PreserveWhitespace);
            //Позволяет избежать обработки пустого или не содержащего нужных тегов файла
            if (xDoc.Element("LanguageData") is null)
            {
                textBox.AppendText($"{Environment.NewLine}Не удалось найти LanguageData {currentFile}");
                return false;
            }
            //Перевод контекста в содержимое тега LanguageData
            XElement? root = xDoc.Element("LanguageData");
            if (root?.Elements() is null)
            {
                textBox.AppendText($"{Environment.NewLine}Тег LanguageData пуст  {currentFile}");
                return false;
            }

            foreach (XElement node in root.Elements())
            {
                //Получение содержимого текущего тега
                string content = node.Value;
                //Создание комментария с ним
                XRaw comment = new("<!-- EN: " + content + " -->\n\t");
                //Добавление этого комментария перед текущим тегом
                node.AddBeforeSelf(comment);
            }
            //Перенос строки перед закрывающим тегом LanguageData
            root.LastNode?.AddAfterSelf("\n");

            //Сохранение файла
            xDoc.Save(currentFile);
            return true;
        }
    }

    //Класс, в котором описывается не до конца понятный мне трюк, позволяющий вписывать специальные символы «как есть»
    internal sealed class XRaw : XText
    {
        public XRaw(string text) : base(text) { }
        public XRaw(XText text) : base(text) { }

        public override void WriteTo(XmlWriter writer)
        {
            writer.WriteRaw(Value);
        }
    }
}
