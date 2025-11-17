using Microsoft.Extensions.Logging;
using RimLangKit.Client;
using RimLangKit.Common;
using RimLangKit.Models.Morpher;
using System.Globalization;
using System.Text.Json;

namespace RimLangKit.Services
{
    /// <summary>
    /// Сервис для работы с API морфологического анализа Morpher
    /// </summary>
    public class MorpherService : IMorpherService
    {
        private readonly ILogger<MorpherService> _logger;
        private readonly HttpClient _httpClient;

        public MorpherService(ILogger<MorpherService> logger)
        {
            _logger = logger;
            _httpClient = RimLangHttpClient.Client;
        }

        /// <summary>
        /// Получает количество оставшихся запросов к сервису
        /// </summary>
        public async Task<Result<int>> GetRequestLimitAsync()
        {
            try
            {
                _logger.LogInformation("Запрос лимита запросов к Morpher API");

                var response = await _httpClient.GetAsync(
                    "https://ws3.morpher.ru/get-queries-left?format=json");

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                if (!int.TryParse(content, NumberStyles.Integer, CultureInfo.InvariantCulture, out int limit))
                {
                    _logger.LogWarning("Не удалось распарсить лимит запросов: {Content}", content);
                    return Result<int>.Failure("Не удалось обработать ответ от Morpher API");
                }

                _logger.LogInformation("Получен лимит запросов: {Limit}", limit);
                return Result<int>.Success(limit);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Ошибка HTTP при запросе к Morpher API");
                return Result<int>.Failure("Не удалось подключиться к Morpher API. Проверьте подключение к интернету.", ex);
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Превышено время ожидания при запросе к Morpher API");
                return Result<int>.Failure("Превышено время ожидания ответа от Morpher API", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Неожиданная ошибка при получении лимита запросов");
                return Result<int>.Failure("Произошла неожиданная ошибка", ex);
            }
        }

        /// <summary>
        /// Получает склонения слова на русском языке
        /// </summary>
        public async Task<Result<WordForms>> GetWordFormsAsync(string word)
        {
            if (string.IsNullOrWhiteSpace(word))
            {
                return Result<WordForms>.Failure("Слово не может быть пустым");
            }

            try
            {
                _logger.LogInformation("Запрос склонений для слова: {Word}", word);

                var encodedWord = Uri.EscapeDataString(word);
                var response = await _httpClient.GetAsync(
                    $"https://ws3.morpher.ru/russian/declension?s={encodedWord}&format=json");

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var wordForms = JsonSerializer.Deserialize<WordForms>(content);

                if (wordForms == null)
                {
                    _logger.LogWarning("Не удалось десериализовать ответ от Morpher API для слова {Word}", word);
                    return Result<WordForms>.Failure("Не удалось обработать ответ от Morpher API");
                }

                _logger.LogInformation("Успешно получены склонения для слова: {Word}", word);
                return Result<WordForms>.Success(wordForms);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Ошибка HTTP при запросе склонений для {Word}", word);
                return Result<WordForms>.Failure($"Не удалось получить склонения для слова '{word}'", ex);
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Превышено время ожидания при запросе склонений для {Word}", word);
                return Result<WordForms>.Failure("Превышено время ожидания ответа от Morpher API", ex);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Ошибка десериализации JSON от Morpher API для {Word}", word);
                return Result<WordForms>.Failure("Ошибка обработки данных от Morpher API", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Неожиданная ошибка при получении склонений для {Word}", word);
                return Result<WordForms>.Failure("Произошла неожиданная ошибка", ex);
            }
        }

        #region Старые методы для обратной совместимости (устарели, будут удалены)

        [Obsolete("Используйте GetRequestLimitAsync() вместо этого метода")]
        public static int? GetMorpherRequestLimit()
        {
            try
            {
                var response = RimLangHttpClient.Client.GetAsync("https://ws3.morpher.ru/get-queries-left?format=json").Result;
                response.EnsureSuccessStatusCode();
                string limit = response.Content.ReadAsStringAsync().Result;
                int? number = Convert.ToInt32(limit, CultureInfo.InvariantCulture);
                return number;
            }
            catch
            {
                return null;
            }
        }

        [Obsolete("Используйте GetWordFormsAsync() вместо этого метода")]
        public static WordForms? GetMorpherWords(string word)
        {
            try
            {
                word = Uri.EscapeDataString(word);
                var response = RimLangHttpClient.Client.GetAsync($"https://ws3.morpher.ru/russian/declension?s={word}&format=json").Result;
                response.EnsureSuccessStatusCode();
                string text = response.Content.ReadAsStringAsync().Result;
                WordForms? json = JsonSerializer.Deserialize<WordForms>(text);
                return json;
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
