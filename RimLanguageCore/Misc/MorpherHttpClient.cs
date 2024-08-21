using System;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;

namespace RimLanguageCore.Misc
{
    public class MorpherHttpClient
    {
        private static readonly HttpClient Client = new();
        static MorpherHttpClient()
        {
            Client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36");
        }

        public static int? GetMorpherRequestLimit()
        {
            HttpResponseMessage response;
            try
            {
                response = Client.GetAsync("https://ws3.morpher.ru/get-queries-left?format=json").Result;
            }
            catch
            {
                return null;
            }
            string limit = response.Content.ReadAsStringAsync().Result;
            int? number = Convert.ToInt32(limit, CultureInfo.InvariantCulture);
            return number;
        }

        public static Words GetMorpherWords(string word)
        {
            HttpResponseMessage response;
            try
            {
                response = Client.GetAsync($"https://ws3.morpher.ru/russian/declension?s={word}&format=json").Result;
            }
            catch
            {
                return null;
            }
            string text = response.Content.ReadAsStringAsync().Result;
            Words json = JsonSerializer.Deserialize<Words>(text);
            return json;
        }
    }

#pragma warning disable IDE1006, CA1707
    public class Words
    {
        public string Р { get; set; }
        public string Д { get; set; }
        public string В { get; set; }
        public string Т { get; set; }
        public string П { get; set; }
        public множественное множественное { get; set; }
    }

    public class множественное
    {
        public string И { get; set; }
    }
#pragma warning restore IDE1006, CA1707
}
