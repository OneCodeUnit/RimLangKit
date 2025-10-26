using RimLangKit.Client;
using RimLangKit.Models.Morpher;
using System.Globalization;
using System.Text.Json;

namespace RimLangKit.Services
{
    public class MorpherService
    {
        /// <summary>
        /// Получает количество оставшихся запросов к сервису Morpher.
        /// </summary>
        /// <returns>Количество оставшихся запросов или null в случае ошибки.</returns>
        public static int? GetMorpherRequestLimit()
        {
            HttpResponseMessage response;
            try
            {
                response = RimLangHttpClient.Client.GetAsync("https://ws3.morpher.ru/get-queries-left?format=json").Result;
                response.EnsureSuccessStatusCode();
            }
            catch
            {
                return null;
            }
            string limit = response.Content.ReadAsStringAsync().Result;
            int? number = Convert.ToInt32(limit, CultureInfo.InvariantCulture);
            return number;
        }

        /// <summary>
        /// Получает склонения слова от сервиса Morpher.
        /// </summary>
        /// <returns>Файл со склонениями или null в случае ошибки.</returns>
        public static WordForms? GetMorpherWords(string word)
        {
            HttpResponseMessage response;
            try
            {
                word = Uri.EscapeDataString(word);
                response = RimLangHttpClient.Client.GetAsync($"https://ws3.morpher.ru/russian/declension?s={word}&format=json").Result;
                response.EnsureSuccessStatusCode();
            }
            catch
            {
                return null;
            }
            string text = response.Content.ReadAsStringAsync().Result;
            WordForms? json = JsonSerializer.Deserialize<WordForms>(text);
            return json;
        }
    }
}