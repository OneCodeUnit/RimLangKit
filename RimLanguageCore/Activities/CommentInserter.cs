using System.Xml.Linq;
using System.Xml;
using RimLanguageCore.Misc;

namespace RimLanguageCore.Activities
{
    /// <summary>
    /// Вставляет комментарий с содержимым тега перед каждым элементом в указанном XML-файле.
    /// </summary>
    /// <param name="currentFile">Путь к текущему XML-файлу.</param>
    /// <returns>Кортеж, содержащий статус операции (true - успех, false - ошибка) и сообщение об ошибке.</returns>
    public static class CommentInserter
    {
        public static (bool, string) InsertComments(string currentFile)
        {
            (bool, string) result = XmlErrorChecker.XmlErrorCheck(currentFile);
            if (!result.Item1)
            {
                return result;
            }

            XDocument xDoc = XDocument.Load(currentFile, LoadOptions.PreserveWhitespace);
            XElement root = xDoc.Element("LanguageData");

            foreach (XElement node in root.Elements())
            {
                // Получение содержимого текущего тега
                string content = node.Value;
                // Создание комментария с ним
                XRaw comment = new("<!-- EN: " + content + " -->\n\t");
                // Добавление этого комментария перед текущим тегом
                node.AddBeforeSelf(comment);
            }
            // Перенос строки перед закрывающим тегом LanguageData
            root.LastNode?.AddAfterSelf("\n");

            // Сохранение файла
            xDoc.Save(currentFile);
            return (true, string.Empty);
        }
    }

    // Класс, в котором описывается не до конца понятный мне трюк, позволяющий вписывать специальные символы «как есть»
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