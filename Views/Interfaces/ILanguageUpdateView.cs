namespace RimLangKit.Views.Interfaces
{
    /// <summary>
    /// Интерфейс представления для обновления локализации игры
    /// </summary>
    public interface ILanguageUpdateView
    {
        /// <summary>
        /// Получает текущее значение языка из TextBox
        /// </summary>
        string GetLanguageValue();

        /// <summary>
        /// Получает текущее значение репозитория из TextBox
        /// </summary>
        string GetRepositoryValue();

        /// <summary>
        /// Устанавливает значение языка в TextBox
        /// </summary>
        void SetLanguageValue(string value);

        /// <summary>
        /// Устанавливает значение репозитория в TextBox
        /// </summary>
        void SetRepositoryValue(string value);
    }
}
