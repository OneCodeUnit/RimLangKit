namespace RimLangKit.Common
{
    /// <summary>
    /// Представляет результат операции с возможностью успеха или ошибки.
    /// Используется для явной обработки ошибок вместо исключений.
    /// </summary>
    /// <typeparam name="T">Тип возвращаемого значения при успехе</typeparam>
    public class Result<T>
    {
        /// <summary>
        /// Указывает, была ли операция успешной
        /// </summary>
        public bool IsSuccess { get; init; }

        /// <summary>
        /// Значение результата (доступно только при IsSuccess = true)
        /// </summary>
        public T? Value { get; init; }

        /// <summary>
        /// Сообщение об ошибке (доступно при IsSuccess = false)
        /// </summary>
        public string? ErrorMessage { get; init; }

        /// <summary>
        /// Детали исключения (если произошло)
        /// </summary>
        public Exception? Exception { get; init; }

        /// <summary>
        /// Создает успешный результат
        /// </summary>
        public static Result<T> Success(T value) => new()
        {
            IsSuccess = true,
            Value = value
        };

        /// <summary>
        /// Создает результат с ошибкой
        /// </summary>
        public static Result<T> Failure(string errorMessage, Exception? exception = null) => new()
        {
            IsSuccess = false,
            ErrorMessage = errorMessage,
            Exception = exception
        };
    }

    /// <summary>
    /// Результат операции без возвращаемого значения
    /// </summary>
    public class Result
    {
        public bool IsSuccess { get; init; }
        public string? ErrorMessage { get; init; }
        public Exception? Exception { get; init; }

        public static Result Success() => new() { IsSuccess = true };

        public static Result Failure(string errorMessage, Exception? exception = null) => new()
        {
            IsSuccess = false,
            ErrorMessage = errorMessage,
            Exception = exception
        };
    }
}
