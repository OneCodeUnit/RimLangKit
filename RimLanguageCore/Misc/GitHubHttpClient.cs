using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RimLanguageCore.Misc
{
    public class GitHubHttpClient
    {
        private static readonly HttpClient Client = new();
        static GitHubHttpClient()
        {
            Client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36");
        }

        public static Root GetGithubJson()
        {
            HttpResponseMessage response;
            try
            {
                response = Client.GetAsync("https://api.github.com/repos/OneCodeUnit/RimLangKit/releases/latest").Result;
                response.EnsureSuccessStatusCode();
            }
            catch
            {
                return null;
            }
            string text = response.Content.ReadAsStringAsync().Result;
            Root json = JsonSerializer.Deserialize<Root>(text);
            return json;
        }

        public static Root GetGithubSha(string repo)
        {
            HttpResponseMessage response;
            try
            {
                response = Client.GetAsync($"https://api.github.com/repos/{repo}/commits/master").Result;
                response.EnsureSuccessStatusCode();
            }
            catch
            {
                return null;
            }
            string text = response.Content.ReadAsStringAsync().Result;
            Root json = JsonSerializer.Deserialize<Root>(text);
            return json;
        }

        public static Stream GetGithubArchive(string repo, string branch = "master")
        {
            HttpResponseMessage response;
            try
            {
                response = Client.GetAsync($"https://github.com/{repo}/archive/refs/heads/{branch}.zip").Result;
                response.EnsureSuccessStatusCode();
                Stream stream = response.Content.ReadAsStreamAsync().Result;
                return stream;
            }
            catch
            {
                return null;
            }
        }
    }

    public class Root
    {
        [JsonPropertyName("sha")]
        public string Sha { get; set; }

        [JsonPropertyName("tag_name")]
        public string TagName { get; set; }

        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("body")]
        public string Body { get; set; }
    }
}