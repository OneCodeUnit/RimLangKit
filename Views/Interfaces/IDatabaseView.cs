namespace RimLangKit.Views.Interfaces
{
    /// <summary>
    /// Интерфейс представления для работы с базой данных переводов
    /// </summary>
    public interface IDatabaseView
    {
        /// <summary>
        /// Показывает диалог выбора файла базы данных
        /// </summary>
        /// <param name="selectedPath">Выбранный путь к файлу</param>
        /// <returns>True если пользователь выбрал файл</returns>
        bool ShowDatabaseFileDialog(out string selectedPath);

        /// <summary>
        /// Получает значение режима перезаписи из радио-кнопок
        /// </summary>
        /// <returns>True если включена перезапись</returns>
        bool GetRewriteMode();
    }
}
