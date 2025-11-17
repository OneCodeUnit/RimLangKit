using System.Xml.Linq;

namespace RimLangKit.Checks
{
    /// <summary>
    /// Проверяет корректность XML-файлов переводов RimWorld.
    /// </summary>
    /// <remarks>
    /// Проверяет возможность загрузки XML, наличие корневого элемента LanguageData и дочерних элементов.
    /// Предоставляет два метода: устаревший с кортежами и новый с XmlError.
    /// </remarks>
    public static class XmlErrorChecker
    {
        /// <summary>
        /// Проверяет корректность XML-файла перевода (устаревший метод, использует кортежи).
        /// </summary>
        /// <param name="currentFile">Полный путь к XML-файлу для проверки.</param>
        /// <returns>
        /// Кортеж, содержащий:
        /// - bool: true если файл корректный, false при ошибке.
        /// - string: Пустая строка при успехе, описание ошибки при неудаче.
        /// </returns>
        /// <remarks>
        /// Устаревший метод. Рекомендуется использовать CheckXml(string).
        /// </remarks>
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
            XElement? root = xDoc.Element("LanguageData");
            if (root is null)
            {
                return (false, "Не удалось найти корневой элемент 'LanguageData'");
            }

            // Позволяет избежать обработки пустого тега LanguageData
            if (root.Elements().Any())
            {
                return (true, string.Empty);
            }
            else
            {
                return (false, "Корневой элемент 'LanguageData' не содержит дочерних элементов.");
            }
        }

        /// <summary>
        /// Проверяет корректность XML-файла перевода (новый метод, использует XmlError).
        /// </summary>
        /// <param name="currentFile">Полный путь к XML-файлу для проверки.</param>
        /// <returns>
        /// Объект XmlError с результатом проверки:
        /// - IsValid=true и пустое сообщение при успехе.
        /// - IsValid=false и описание ошибки при неудаче.
        /// </returns>
        /// <remarks>
        /// Проверяет:
        /// 1. Возможность загрузки XML-файла
        /// 2. Наличие корневого элемента "LanguageData"
        /// 3. Наличие дочерних элементов в "LanguageData"
        /// </remarks>
        public static XmlError CheckXml(string currentFile)
        {
            XDocument xDoc;
            // Позволяет избежать падения при загрузке сломанного .xml файла
            try
            {
                xDoc = XDocument.Load(currentFile, LoadOptions.PreserveWhitespace);
            }
            catch (Exception ex)
            {
                return new XmlError (false, $"Не удалось загрузить файл: {ex.Message}");
            }

            // Позволяет избежать обработки пустого или не содержащего нужных тегов файла
            XElement? root = xDoc.Element("LanguageData");
            if (root is null)
            {
                return new XmlError(false, "Не удалось найти корневой элемент 'LanguageData'");
            }

            // Позволяет избежать обработки пустого тега LanguageData
            if (root.Elements().Any())
            {
                return new XmlError (true, string.Empty);
            }
            else
            {
                return new XmlError (false, "Корневой элемент 'LanguageData' не содержит дочерних элементов.");
            }
        }
    }
}