namespace RimLangKit.Views.Interfaces
{
    /// <summary>
    /// Интерфейс главного представления (MainForm)
    /// </summary>
    public interface IMainView
    {
        /// <summary>
        /// Отображает сообщение в текстовом поле информации
        /// </summary>
        /// <param name="message">Сообщение для отображения</param>
        /// <param name="tabId">ID вкладки (0 или 1). Если null, используется текущая выбранная вкладка</param>
        void ShowMessage(string message, int? tabId = null);

        /// <summary>
        /// Показывает MessageBox с сообщением
        /// </summary>
        /// <param name="message">Текст сообщения</param>
        /// <param name="title">Заголовок окна</param>
        /// <param name="icon">Иконка сообщения</param>
        void ShowMessageBox(string message, string title, MessageBoxIcon icon = MessageBoxIcon.Information);

        /// <summary>
        /// Показывает MessageBox с вопросом и возвращает результат
        /// </summary>
        /// <param name="message">Текст вопроса</param>
        /// <param name="title">Заголовок окна</param>
        /// <returns>True если пользователь выбрал Yes, иначе False</returns>
        bool ShowConfirmation(string message, string title);

        /// <summary>
        /// Устанавливает состояние кнопок обработки файлов
        /// </summary>
        /// <param name="enabled">Доступность кнопок</param>
        void SetProcessingButtonsEnabled(bool enabled);

        /// <summary>
        /// Устанавливает текст и цвет статуса проверки директории
        /// </summary>
        /// <param name="text">Текст статуса</param>
        /// <param name="isValid">Валидна ли директория</param>
        void SetDirectoryCheckStatus(string text, bool isValid);

        /// <summary>
        /// Открывает URL в браузере
        /// </summary>
        /// <param name="url">URL для открытия</param>
        void OpenUrl(string url);

        /// <summary>
        /// Устанавливает цвет кнопки дополнительной папки
        /// </summary>
        /// <param name="isValid">Валидна ли папка</param>
        void SetAdditionalFolderButtonColor(bool isValid);

        /// <summary>
        /// Устанавливает цвет кнопки выбора папки игры
        /// </summary>
        /// <param name="color">Цвет кнопки</param>
        void SetGameFolderButtonColor(Color color);

        /// <summary>
        /// Обновляет текст метки проверки базы данных
        /// </summary>
        /// <param name="text">Текст для отображения</param>
        void SetDatabaseCheckLabelText(string text);

        /// <summary>
        /// Устанавливает доступность кнопок обновления языка
        /// </summary>
        /// <param name="enabled">Доступность кнопок</param>
        void SetLanguageUpdateButtonsEnabled(bool enabled);

        /// <summary>
        /// Получает цвет для валидного состояния
        /// </summary>
        Color GoodColor { get; }

        /// <summary>
        /// Получает цвет для невалидного состояния
        /// </summary>
        Color BadColor { get; }
    }
}
