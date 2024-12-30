using System;
using System.Linq;
using System.Xml.Linq;
/// <summary>
/// Проверяет XML-файл на наличие ошибок и соответствие ожидаемой структуре.
/// </summary>
/// <param name="currentFile">Путь к проверяемому XML-файлу.</param>
/// <returns>
/// Кортеж, где первый элемент указывает на успешность проверки,
/// а второй содержит сообщение об ошибке в случае неудачи.
/// </returns>
namespace RimLanguageCore.Misc
{
    public static class XmlErrorChecker
    {
        public static (bool, string) XmlErrorCheck(string currentFile)
        {
            XDocument xDoc;
            // Позволяет избежать падения при загрузке сломанного .xml файла
            try
            {
                xDoc = XDocument.Load(currentFile, LoadOptions.PreserveWhitespace);
            }
            catch (Exception ex)
            {
                return (false, $"Не удалось загрузить файл: {ex.Message}");
            }

            // Позволяет избежать обработки пустого или не содержащего нужных тегов файла
            XElement root = xDoc.Element("LanguageData");
            if (root is null)
            {
                return (false, "Не удалось найти корневой элемент 'LanguageData'");
            }
            // Перевод контекста в содержимое тега LanguageData
            if (root.Elements().Any())
            {
                return (true, string.Empty);
            }
            else
            {
                return (false, "Корневой элемент 'LanguageData' не содержит дочерних элементов.");
            }
        }
    }
}
