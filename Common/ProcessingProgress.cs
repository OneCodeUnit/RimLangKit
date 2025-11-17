namespace RimLangKit.Common
{
    /// <summary>
    /// Класс для отчета о прогрессе обработки файлов
    /// </summary>
    public class ProcessingProgress
    {
        /// <summary>
        /// Процент выполнения (0-100)
        /// </summary>
        public int Percentage { get; init; }

        /// <summary>
        /// Текущий обрабатываемый файл
        /// </summary>
        public string? CurrentFile { get; init; }

        /// <summary>
        /// Сообщение о текущей операции
        /// </summary>
        public string? Message { get; init; }

        /// <summary>
        /// Количество успешно обработанных файлов
        /// </summary>
        public int ProcessedCount { get; init; }

        /// <summary>
        /// Количество пропущенных файлов (с ошибками)
        /// </summary>
        public int SkippedCount { get; init; }

        /// <summary>
        /// Общее количество файлов
        /// </summary>
        public int TotalCount { get; init; }

        /// <summary>
        /// Операция завершена
        /// </summary>
        public bool IsCompleted { get; init; }

        /// <summary>
        /// Операция отменена пользователем
        /// </summary>
        public bool IsCancelled { get; init; }

        /// <summary>
        /// Создает отчет о прогрессе
        /// </summary>
        public static ProcessingProgress Create(
            int processedCount,
            int totalCount,
            int skippedCount = 0,
            string? currentFile = null,
            string? message = null)
        {
            var percentage = totalCount > 0 ? (int)((double)processedCount / totalCount * 100) : 0;

            return new ProcessingProgress
            {
                Percentage = percentage,
                CurrentFile = currentFile,
                Message = message,
                ProcessedCount = processedCount,
                SkippedCount = skippedCount,
                TotalCount = totalCount,
                IsCompleted = processedCount >= totalCount,
                IsCancelled = false
            };
        }

        /// <summary>
        /// Создает отчет о завершении
        /// </summary>
        public static ProcessingProgress Completed(int processedCount, int skippedCount, int totalCount, string message)
        {
            return new ProcessingProgress
            {
                Percentage = 100,
                Message = message,
                ProcessedCount = processedCount,
                SkippedCount = skippedCount,
                TotalCount = totalCount,
                IsCompleted = true,
                IsCancelled = false
            };
        }

        /// <summary>
        /// Создает отчет об отмене
        /// </summary>
        public static ProcessingProgress Cancelled(int processedCount, int totalCount, string message)
        {
            return new ProcessingProgress
            {
                Percentage = (int)((double)processedCount / totalCount * 100),
                Message = message,
                ProcessedCount = processedCount,
                TotalCount = totalCount,
                IsCompleted = false,
                IsCancelled = true
            };
        }
    }
}
