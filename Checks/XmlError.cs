namespace RimLangKit.Checks
{
    /// <summary>
    /// Представляет результат операции с XML-файлами или базой данных.
    /// </summary>
    /// <remarks>
    /// Используется для возврата статуса операции и сопутствующего сообщения.
    /// Применяется во многих классах проекта для унификации обработки ошибок.
    /// </remarks>
    public class XmlError
    {
        /// <summary>
        /// Результат операции: true - успешно, false - ошибка.
        /// </summary>
        public bool IsValid { get; set; }
        /// <summary>
        /// Сообщение с подробностями: описание ошибки или информация о результате операции.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Создает результат операции с указанным статусом и сообщением.
        /// </summary>
        /// <param name="isValid">true - операция успешна, false - произошла ошибка.</param>
        /// <param name="message">Сообщение с подробностями.</param>
        public XmlError(bool isValid, string message)
        {
            this.IsValid = isValid;
            this.Message = message;
        }
    }
}