using Newtonsoft.Json;

namespace RTK
{
    internal class RimHttpClient
    {
        private static readonly HttpClient Client = new();
        static RimHttpClient()
        {
            Client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:110.0) Gecko/20100101 Firefox/110.0");
        }

        public static List<Root>? GetGithubJson(string repo)
        {
            HttpResponseMessage response;
            try
            {
                response = Client.GetAsync($"https://api.github.com/repos/Ludeon/{repo}/commits").Result;
            }
            catch
            {
                return new List<Root> { };
            }
            string text = response.Content.ReadAsStringAsync().Result;
            List<Root>? json = JsonConvert.DeserializeObject<List<Root>?>(text);
            return json;
        }

        public static Stream GetGithubArchive(string repo)
        {
            // HttpResponseMessage response = Client.GetAsync("https://api.github.com/repos/Ludeon/" + repo + "/zipball/master").Result;
            HttpResponseMessage response = Client.GetAsync($"https://github.com/Ludeon/{repo}/archive/refs/heads/master.zip").Result;
            Stream stream = response.Content.ReadAsStreamAsync().Result;
            return stream;
        }
    }
}
