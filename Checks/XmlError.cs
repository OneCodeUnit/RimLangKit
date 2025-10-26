namespace RimLangKit.Checks
{
    public class XmlError
    {
        // IsValid отображает результат операции: true - успешно, false - ошибка
        public bool IsValid { get; set; }
        // Message содержит подробности ошибки или иную информацию в зависимости от ситуации
        public string Message { get; set; }

        public XmlError(bool isValid, string message)
        {
            this.IsValid = isValid;
            this.Message = message;
        }
    }
}