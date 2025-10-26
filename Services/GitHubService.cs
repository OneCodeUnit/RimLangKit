using RimLangKit.Client;
using RimLangKit.Models.GitHub;
using System.Text.Json;

namespace RimLangKit.Services
{
    public class GitHubService
    {
        public static Root? GetGithubJson()
        {
            HttpResponseMessage response;
            try
            {
                response = RimLangHttpClient.Client.GetAsync("https://api.github.com/repos/OneCodeUnit/RimLangKit/releases/latest").Result;
                response.EnsureSuccessStatusCode();
            }
            catch
            {
                return null;
            }
            string text = response.Content.ReadAsStringAsync().Result;
            Root? json = JsonSerializer.Deserialize<Root>(text);
            return json;
        }

        public static Root? GetGithubSha(string repo)
        {
            HttpResponseMessage response;
            try
            {
                response = RimLangHttpClient.Client.GetAsync($"https://api.github.com/repos/{repo}/commits/master").Result;
                response.EnsureSuccessStatusCode();
            }
            catch
            {
                return null;
            }
            string text = response.Content.ReadAsStringAsync().Result;
            Root? json = JsonSerializer.Deserialize<Root>(text);
            return json;
        }

        public static Stream? GetGithubArchive(string repo, string branch = "master")
        {
            HttpResponseMessage response;
            try
            {
                response = RimLangHttpClient.Client.GetAsync($"https://github.com/{repo}/archive/refs/heads/{branch}.zip").Result;
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
}