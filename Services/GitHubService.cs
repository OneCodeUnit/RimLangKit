using Microsoft.Extensions.Logging;
using RimLangKit.Client;
using RimLangKit.Common;
using RimLangKit.Models.GitHub;
using System.Text.Json;

namespace RimLangKit.Services
{
    /// <summary>
    /// Сервис для работы с GitHub API
    /// </summary>
    public class GitHubService : IGitHubService
    {
        private readonly ILogger<GitHubService> _logger;
        private readonly HttpClient _httpClient;

        public GitHubService(ILogger<GitHubService> logger)
        {
            _logger = logger;
            _httpClient = RimLangHttpClient.Client;
        }

        /// <summary>
        /// Получает информацию о последнем релизе приложения
        /// </summary>
        public async Task<Result<Root>> GetLatestReleaseAsync()
        {
            try
            {
                _logger.LogInformation("Запрос последнего релиза RimLangKit");

                var response = await _httpClient.GetAsync(
                    "https://api.github.com/repos/OneCodeUnit/RimLangKit/releases/latest");

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var release = JsonSerializer.Deserialize<Root>(content);

                if (release == null)
                {
                    _logger.LogWarning("Не удалось десериализовать ответ от GitHub API");
                    return Result<Root>.Failure("Не удалось обработать ответ от GitHub");
                }

                _logger.LogInformation("Успешно получен релиз {TagName}", release.TagName);
                return Result<Root>.Success(release);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Ошибка HTTP при запросе к GitHub API");
                return Result<Root>.Failure("Не удалось подключиться к GitHub. Проверьте подключение к интернету.", ex);
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Превышено время ожидания при запросе к GitHub API");
                return Result<Root>.Failure("Превышено время ожидания ответа от GitHub", ex);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Ошибка десериализации JSON от GitHub API");
                return Result<Root>.Failure("Ошибка обработки данных от GitHub", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Неожиданная ошибка при получении релиза");
                return Result<Root>.Failure("Произошла неожиданная ошибка", ex);
            }
        }

        /// <summary>
        /// Получает SHA последнего коммита из репозитория
        /// </summary>
        public async Task<Result<Root>> GetLatestCommitShaAsync(string repository)
        {
            try
            {
                _logger.LogInformation("Запрос последнего коммита из репозитория {Repository}", repository);

                var response = await _httpClient.GetAsync(
                    $"https://api.github.com/repos/{repository}/commits/master");

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var commit = JsonSerializer.Deserialize<Root>(content);

                if (commit == null)
                {
                    _logger.LogWarning("Не удалось десериализовать ответ от GitHub API");
                    return Result<Root>.Failure("Не удалось обработать ответ от GitHub");
                }

                _logger.LogInformation("Успешно получен SHA коммита из {Repository}", repository);
                return Result<Root>.Success(commit);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Ошибка HTTP при запросе коммита из {Repository}", repository);
                return Result<Root>.Failure($"Не удалось получить данные из репозитория {repository}", ex);
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Превышено время ожидания при запросе к {Repository}", repository);
                return Result<Root>.Failure("Превышено время ожидания ответа от GitHub", ex);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Ошибка десериализации JSON от GitHub API");
                return Result<Root>.Failure("Ошибка обработки данных от GitHub", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Неожиданная ошибка при получении коммита из {Repository}", repository);
                return Result<Root>.Failure("Произошла неожиданная ошибка", ex);
            }
        }

        /// <summary>
        /// Загружает архив репозитория
        /// </summary>
        public async Task<Result<Stream>> DownloadRepositoryArchiveAsync(string repository, string branch = "master")
        {
            try
            {
                _logger.LogInformation("Загрузка архива репозитория {Repository} (ветка: {Branch})", repository, branch);

                var response = await _httpClient.GetAsync(
                    $"https://github.com/{repository}/archive/refs/heads/{branch}.zip");

                response.EnsureSuccessStatusCode();

                var stream = await response.Content.ReadAsStreamAsync();

                _logger.LogInformation("Успешно загружен архив {Repository}/{Branch}", repository, branch);
                return Result<Stream>.Success(stream);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Ошибка HTTP при загрузке архива {Repository}/{Branch}", repository, branch);
                return Result<Stream>.Failure($"Не удалось загрузить архив репозитория {repository}", ex);
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Превышено время ожидания при загрузке архива {Repository}", repository);
                return Result<Stream>.Failure("Превышено время ожидания загрузки", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Неожиданная ошибка при загрузке архива {Repository}", repository);
                return Result<Stream>.Failure("Произошла неожиданная ошибка при загрузке", ex);
            }
        }

        #region Старые методы для обратной совместимости (устарели, будут удалены)

        [Obsolete("Используйте GetLatestReleaseAsync() вместо этого метода")]
        public static Root? GetGithubJson()
        {
            try
            {
                var response = RimLangHttpClient.Client.GetAsync("https://api.github.com/repos/OneCodeUnit/RimLangKit/releases/latest").Result;
                response.EnsureSuccessStatusCode();
                string text = response.Content.ReadAsStringAsync().Result;
                Root? json = JsonSerializer.Deserialize<Root>(text);
                return json;
            }
            catch
            {
                return null;
            }
        }

        [Obsolete("Используйте GetLatestCommitShaAsync() вместо этого метода")]
        public static Root? GetGithubSha(string repo)
        {
            try
            {
                var response = RimLangHttpClient.Client.GetAsync($"https://api.github.com/repos/{repo}/commits/master").Result;
                response.EnsureSuccessStatusCode();
                string text = response.Content.ReadAsStringAsync().Result;
                Root? json = JsonSerializer.Deserialize<Root>(text);
                return json;
            }
            catch
            {
                return null;
            }
        }

        [Obsolete("Используйте DownloadRepositoryArchiveAsync() вместо этого метода")]
        public static Stream? GetGithubArchive(string repo, string branch = "master")
        {
            try
            {
                var response = RimLangHttpClient.Client.GetAsync($"https://github.com/{repo}/archive/refs/heads/{branch}.zip").Result;
                response.EnsureSuccessStatusCode();
                Stream stream = response.Content.ReadAsStreamAsync().Result;
                return stream;
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
