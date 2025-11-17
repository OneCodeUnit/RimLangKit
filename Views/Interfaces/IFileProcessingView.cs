using RimLangKit.Common;

namespace RimLangKit.Views.Interfaces
{
    /// <summary>
    /// Интерфейс представления для обработки файлов
    /// </summary>
    public interface IFileProcessingView
    {
        /// <summary>
        /// Показывает диалог выбора папки
        /// </summary>
        /// <param name="selectedPath">Выбранный путь (out параметр)</param>
        /// <returns>True если пользователь выбрал папку</returns>
        bool ShowFolderBrowserDialog(out string selectedPath);

        /// <summary>
        /// Показывает прогресс обработки
        /// </summary>
        /// <param name="progress">Информация о прогрессе</param>
        void UpdateProgress(ProcessingProgress progress);

        /// <summary>
        /// Получает объект для отчета о прогрессе
        /// </summary>
        IProgress<ProcessingProgress>? GetProgressReporter();
    }
}
