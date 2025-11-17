using RimLangKit.Client;
using RimLangKit.Models.GitHub;
using System.Text.Json;

namespace RimLangKit.Services
{
    /// <summary>
    /// Предоставляет методы для взаимодействия с GitHub API.
    /// </summary>
    /// <remarks>
    /// Использует RimLangHttpClient для выполнения HTTP-запросов к GitHub API.
    /// Поддерживает получение информации о релизах, коммитах и скачивание архивов репозиториев.
    /// </remarks>
    public class GitHubService
    {
        /// <summary>
        /// Получает информацию о последнем релизе проекта RimLangKit.
        /// </summary>
        /// <returns>
        /// Объект Root с информацией о релизе или null при ошибке запроса.
        /// </returns>
        /// <remarks>
        /// Выполняет запрос к API: https://api.github.com/repos/OneCodeUnit/RimLangKit/releases/latest
        /// </remarks>
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

        /// <summary>
        /// Получает информацию о последнем коммите в ветке master указанного репозитория.
        /// </summary>
        /// <param name="repo">Полное имя репозитория в формате "владелец/репозиторий".</param>
        /// <returns>
        /// Объект Root с информацией о коммите (включая SHA) или null при ошибке запроса.
        /// </returns>
        /// <remarks>
        /// Выполняет запрос к API: https://api.github.com/repos/{repo}/commits/master
        /// </remarks>
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

        /// <summary>
        /// Скачивает ZIP-архив указанной ветки репозитория.
        /// </summary>
        /// <param name="repo">Полное имя репозитория в формате "владелец/репозиторий".</param>
        /// <param name="branch">Имя ветки для скачивания. По умолчанию "master".</param>
        /// <returns>
        /// Поток Stream с содержимым ZIP-архива или null при ошибке запроса.
        /// </returns>
        /// <remarks>
        /// Выполняет запрос к: https://github.com/{repo}/archive/refs/heads/{branch}.zip
        /// </remarks>
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