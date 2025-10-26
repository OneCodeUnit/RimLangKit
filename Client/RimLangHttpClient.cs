using System.Reflection;

namespace RimLangKit.Client
{
    /// <summary>
    /// HTTP-клиент для всех сервисов приложения.
    /// </summary>
    public class RimLangHttpClient
    {
        private static readonly HttpClient _client = new();

        static RimLangHttpClient()
        {
            var appVersion = Assembly.GetEntryAssembly()?.GetName().Version?.ToString()[..^2];
            _client.DefaultRequestHeaders.UserAgent.ParseAdd($"RimLangKit/{appVersion} (Windows NT 10.0; Win64; x64)");

            // Можно добавить другие общие настройки
            _client.Timeout = TimeSpan.FromSeconds(60);
        }

        /// <summary>
        /// Получение экземпляра HTTP-клиента.
        /// </summary>
        public static HttpClient Client => _client;
    }
}