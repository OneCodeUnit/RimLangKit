using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;

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
            }
            catch
            {
                return null;
            }
            string text = response.Content.ReadAsStringAsync().Result;
            Root json = JsonSerializer.Deserialize<Root>(text);
            return json;
        }

        public static Stream GetGithubArchive(string repo)
        {
            HttpResponseMessage response;
            try
            {
                response = Client.GetAsync($"https://github.com/{repo}/archive/refs/heads/master.zip").Result;
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
#pragma warning disable IDE1006, CA1707
        public string sha { get; set; }
        public string tag_name { get; set; }
#pragma warning restore IDE1006, CA1707
    }
}