using System;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RimLanguageCore.Misc
{
    public class MorpherHttpClient
    {
        private static readonly HttpClient Client = new();
        static MorpherHttpClient()
        {
            Client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36");
        }

        /// <summary>
        /// Получает количество оставшихся запросов к сервису Morpher.
        /// </summary>
        /// <returns>Количество оставшихся запросов или null в случае ошибки.</returns>
        public static int? GetMorpherRequestLimit()
        {
            HttpResponseMessage response;
            try
            {
                response = Client.GetAsync("https://ws3.morpher.ru/get-queries-left?format=json").Result;
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
        public static WordForms GetMorpherWords(string word)
        {
            HttpResponseMessage response;
            try
            {
                word = Uri.EscapeDataString(word);
                response = Client.GetAsync($"https://ws3.morpher.ru/russian/declension?s={word}&format=json").Result;
                response.EnsureSuccessStatusCode();
            }
            catch
            {
                return null;
            }
            string text = response.Content.ReadAsStringAsync().Result;
            WordForms json = JsonSerializer.Deserialize<WordForms>(text);
            return json;
        }
    }

    public class WordForms
    {
        [JsonPropertyName("Р")]
        public string Genitive { get; set; }

        [JsonPropertyName("Д")]
        public string Dative { get; set; }

        [JsonPropertyName("В")]
        public string Accusative { get; set; }

        [JsonPropertyName("Т")]
        public string Instrumental { get; set; }

        [JsonPropertyName("П")]
        public string Prepositional { get; set; }

        [JsonPropertyName("множественное")]
        public PluralForms Plural { get; set; }
    }

    public class PluralForms
    {
        [JsonPropertyName("И")]
        public string Nominative { get; set; }
    }
}
