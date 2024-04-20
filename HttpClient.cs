using System.Text.Json;

namespace RimLangKit
{
    public class HttpClient
    {
        private static readonly System.Net.Http.HttpClient Client = new();
        static HttpClient()
        {
            Client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:110.0) Gecko/20100101 Firefox/110.0");
        }

        public static Root? GetGithubJson()
        {
            HttpResponseMessage response;
            try
            {
                response = Client.GetAsync("https://api.github.com/repos/OneCodeUnit/RimLangKit/releases/latest").Result;
            }
            catch
            {
                return null;
            }
            string? text = response.Content.ReadAsStringAsync().Result;
            Root? json = JsonSerializer.Deserialize<Root?>(text);
            return json;
        }
    }

    public class Root
    {
#pragma warning disable CA1707, CS8618, IDE1006
        public string tag_name { get; set; }
#pragma warning restore CA1707, CS8618, IDE1006
    }
}
