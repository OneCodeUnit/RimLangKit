using RimLangKit.Common;
using RimLangKit.Models.Morpher;

namespace RimLangKit.Services
{
    /// <summary>
    /// Интерфейс для работы с сервисом морфологии Morpher
    /// </summary>
    public interface IMorpherService
    {
        /// <summary>
        /// Получает количество оставшихся запросов к сервису
        /// </summary>
        /// <returns>Результат с количеством запросов или ошибкой</returns>
        Task<Result<int>> GetRequestLimitAsync();

        /// <summary>
        /// Получает склонения слова на русском языке
        /// </summary>
        /// <param name="word">Слово для склонения</param>
        /// <returns>Результат с формами слова или ошибкой</returns>
        Task<Result<WordForms>> GetWordFormsAsync(string word);
    }
}
