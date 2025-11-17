namespace RimLangKit.Processors
{
    /// <summary>
    /// Предоставляет функциональность для переименования XML-файлов переводов с добавлением имени модуля.
    /// </summary>
    public static class FileRenamer
    {
        /// <summary>
        /// Переименовывает XML-файл, добавляя имя папки модуля к имени файла.
        /// </summary>
        /// <param name="currentFile">Полный путь к текущему XML-файлу.</param>
        /// <returns>
        /// Кортеж, содержащий:
        /// - bool: true при успешном переименовании, false при ошибке.
        /// - string: Пустая строка при успехе, сообщение об ошибке при неудаче.
        /// </returns>
        /// <remarks>
        /// Метод извлекает имя папки модуля (на 6 уровней выше имени файла) и добавляет его к имени файла.
        /// Требуется, чтобы папка "Languages" находилась внутри папки модуля.
        /// </remarks>
        public static (bool, string) FileRenamerActivity(string currentFile)
        {
            // Разделение пути до файла на части и поиск нужной
            string[] path = currentFile.Split('\\');
            // Сохранение с добавлением имени папки. Берется имя папки на уровень выше "Languages"
            if (path[^1].EndsWith(".xml", StringComparison.OrdinalIgnoreCase) && path.Length > 7)
            {
                string newPath = $"{currentFile[..^4]}_{path[^6]}.xml";
                File.Move(currentFile, newPath);
                return (true, string.Empty);
            }
            else
            {
                return (false, "Папка модуля не найдена. Точно ли \"Languages\" находится в папке с названием модуля?");
            }
        }
    }
}