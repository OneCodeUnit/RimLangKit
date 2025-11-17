using RimLangKit.Common;
using RimLangKit.Models.GitHub;

namespace RimLangKit.Services
{
    /// <summary>
    /// Интерфейс для работы с GitHub API
    /// </summary>
    public interface IGitHubService
    {
        /// <summary>
        /// Получает информацию о последнем релизе приложения
        /// </summary>
        /// <returns>Результат с данными релиза или ошибкой</returns>
        Task<Result<Root>> GetLatestReleaseAsync();

        /// <summary>
        /// Получает SHA последнего коммита из репозитория
        /// </summary>
        /// <param name="repository">Имя репозитория (формат: owner/repo)</param>
        /// <returns>Результат с данными коммита или ошибкой</returns>
        Task<Result<Root>> GetLatestCommitShaAsync(string repository);

        /// <summary>
        /// Загружает архив репозитория
        /// </summary>
        /// <param name="repository">Имя репозитория (формат: owner/repo)</param>
        /// <param name="branch">Имя ветки (по умолчанию master)</param>
        /// <returns>Результат с потоком данных или ошибкой</returns>
        Task<Result<Stream>> DownloadRepositoryArchiveAsync(string repository, string branch = "master");
    }
}
