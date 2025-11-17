namespace RimLangKit.Common
{
    /// <summary>
    /// Типы процессоров для обработки файлов
    /// </summary>
    public enum FileProcessorType
    {
        /// <summary>
        /// Переименование файлов
        /// </summary>
        FileRenamer,

        /// <summary>
        /// Транскрипция имен
        /// </summary>
        NamesTranslator,

        /// <summary>
        /// Сбор статистики тегов
        /// </summary>
        TagCollector,

        /// <summary>
        /// Поиск сломанных файлов
        /// </summary>
        FileFixer,

        /// <summary>
        /// Исправление кодировки
        /// </summary>
        EncodingFixer,

        /// <summary>
        /// Добавление комментариев
        /// </summary>
        CommentInserter
    }
}
