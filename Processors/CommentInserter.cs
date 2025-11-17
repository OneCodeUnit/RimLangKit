using System.Xml.Linq;
using RimLangKit.Checks;
using RimLangKit.Models;

namespace RimLangKit.Processors
{
    /// <summary>
    /// Вставляет комментарии с содержимым тегов в XML-файлы переводов.
    /// </summary>
    /// <remarks>
    /// Используется для добавления комментариев <!-- EN: ... --> перед каждым тегом перевода,
    /// чтобы переводчики могли видеть оригинальный английский текст.
    /// </remarks>
    public static class CommentInserter
    {
        /// <summary>
        /// Вставляет комментарий с содержимым тега перед каждым элементом в указанном XML-файле.
        /// </summary>
        /// <param name="currentFile">Полный путь к текущему XML-файлу.</param>
        /// <returns>
        /// Кортеж, содержащий:
        /// - bool: true при успешной вставке комментариев, false при ошибке.
        /// - string: Пустая строка при успехе, сообщение об ошибке при неудаче.
        /// </returns>
        /// <remarks>
        /// Метод создает комментарии вида "<!-- EN: [содержимое тега] -->" перед каждым элементом в LanguageData.
        /// </remarks>
        public static (bool, string) InsertComments(string currentFile)
        {
            (bool, string) result = XmlErrorChecker.XmlErrorCheck(currentFile);
            if (!result.Item1)
            {
                return result;
            }

            XDocument xDoc = XDocument.Load(currentFile, LoadOptions.PreserveWhitespace);
            XElement? root = xDoc.Element("LanguageData");

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
}