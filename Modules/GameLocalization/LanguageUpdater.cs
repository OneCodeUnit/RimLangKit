using System.IO.Compression;
using RimLangKit.Models.GitHub;
using RimLangKit.Services;

namespace RimLangKit.Modules.GameLocalization
{
    /// <summary>
    /// Модуль обновления языковых файлов игры RimWorld из репозитория GitHub.
    /// </summary>
    /// <remarks>
    /// Проверяет версии переводов, скачивает обновления и применяет их к установленной игре.
    /// </remarks>
    public static class LanguageUpdater
    {
        /// <summary>SHA хеш последней проверенной версии перевода.</summary>
        private static string TempSha = string.Empty;
        /// <summary>Временная директория для извлечения скачанных файлов.</summary>
        private static string TempDir = string.Empty;

        /// <summary>
        /// Получает SHA хеш последней проверенной версии перевода.
        /// </summary>
        /// <returns>SHA хеш или пустая строка, если проверка не выполнялась.</returns>
        public static string GetSha()
        {
            return TempSha;
        }

        /// <summary>
        /// Проверяет наличие обновлений перевода, сравнивая локальную и удаленную версии.
        /// </summary>
        /// <param name="sha">SHA хеш текущей установленной версии перевода.</param>
        /// <param name="repo">Полное имя репозитория перевода в формате "владелец/репозиторий".</param>
        /// <returns>
        /// Кортеж, содержащий:
        /// - bool: true если доступно обновление, false если обновление не требуется или произошла ошибка.
        /// - string: Сообщение о результате проверки с информацией о версиях.
        /// </returns>
        /// <remarks>
        /// При наличии обновления сохраняет новый SHA в TempSha для последующего использования.
        /// </remarks>
        public static (bool, string) TranslationVersionCheckActivity(string sha, string repo)
        {
            Root? json = GitHubService.GetGithubSha(repo);
            if (json is null)
            {
                return (false, "Проверка версии: Ошибка получения данных с GitHub. Что-то с интернетом?");
            }
            if (json.Sha == sha)
            {
                TempSha = string.Empty;
                return (false, $"Ваша версия - {sha[..6]}, доступная версия - {json.Sha[..6]}. Обновление не требуется");
            }
            else
            {
                TempSha = json.Sha;
                return (true, $"Ваша версия - {sha[..6]}, доступная версия - {json.Sha[..6]}. Требуется обновление");
            }
        }

        /// <summary>
        /// Скачивает и извлекает архив перевода из GitHub.
        /// </summary>
        /// <param name="repo">Полное имя репозитория перевода в формате "владелец/репозиторий".</param>
        /// <returns>
        /// Кортеж, содержащий:
        /// - bool: true при успешном скачивании и извлечении, false при ошибке.
        /// - string: Сообщение о результате операции.
        /// </returns>
        /// <remarks>
        /// Скачивает ZIP-архив ветки master репозитория и извлекает его во временную папку "tempRTK".
        /// Если временная папка существует, она будет удалена перед извлечением.
        /// </remarks>
        public static (bool, string) LanguageUpdateDownload(string repo)
        {
            // Получение файлов перевода
            Stream? stream = GitHubService.GetGithubArchive(repo);
            if (stream is null)
            {
                TempDir = string.Empty;
                return (false, "Загрузка перевода: Ошибка получения данных с GitHub. Что-то с интернетом?");
            }

            // Распаковка файлов перевода
            TempDir = "tempRTK";
            if (Directory.Exists(TempDir))
            {
                Directory.Delete(TempDir, true);
            }
            ZipArchive zipArchive = new(stream);
            zipArchive.ExtractToDirectory(TempDir);
            stream.Close();
            return (true, "Новая версия перевода загружена и извлечена");
        }

        /// <summary>
        /// Устанавливает скачанный перевод в папку игры RimWorld.
        /// </summary>
        /// <param name="gamePath">Полный путь к корневой папке игры RimWorld.</param>
        /// <param name="language">Код языка (например, "Russian").</param>
        /// <returns>
        /// Кортеж, содержащий:
        /// - bool: true при успешном обновлении, false при ошибке.
        /// - string: Подробный отчет о результатах обновления каждого модуля.
        /// </returns>
        /// <remarks>
        /// Метод обнаруживает все модули в папке Data игры и обновляет языковые файлы для каждого найденного модуля.
        /// Старые языковые файлы удаляются перед установкой новых.
        /// Временная папка "tempRTK" удаляется после завершения установки.
        /// </remarks>
        public static (bool, string) LanguageUpdateActivity(string gamePath, string language)
        {
            // Получения списка дополнений
            string[] modules = Directory.GetDirectories($"{gamePath}\\Data");
            if (modules.Length == 0)
            {
                return (false, "В указанной папке не обнаружены модули. Не та папка?");
            }
            for (int i = 0; i < modules.Length; i++)
            {
                modules[i] = modules[i].Replace($"{gamePath}\\Data\\", string.Empty);
            }

            // Обновление перевода
            string[] baseDir = Directory.GetDirectories(TempDir);
            string[] dir = Directory.GetDirectories(baseDir[0]);
            StringComparison comparison = StringComparison.OrdinalIgnoreCase;
            bool ok;
            string result = "Результат:";
            foreach (string dirEntry in dir)
            {
                ok = false;
                foreach (string module in modules)
                {
                    if (dirEntry.EndsWith(module, comparison))
                    {
                        FolderUpdate(module, language, dirEntry, gamePath);
                        result += $"{Environment.NewLine}Модуль \"{module}\" обновлён";
                        ok = true;
                        break;
                    }
                }
                if (!dirEntry.EndsWith("RimWorldUniverse", comparison) && !dirEntry.EndsWith(".github", comparison) && ok == false)
                {
                    string module = dirEntry.Replace($"{baseDir[0]}\\", string.Empty);
                    result += $"{Environment.NewLine}Не найден модуль \"{module}\"";
                }
            }
            Directory.Delete(TempDir, true);
            result += $"{Environment.NewLine}Обновление завершено!";
            return (true, result);
        }

        /// <summary>
        /// Обновляет языковую папку указанного модуля.
        /// </summary>
        /// <param name="module">Имя модуля для обновления.</param>
        /// <param name="language">Код языка.</param>
        /// <param name="dirEntry">Путь к новой папке с языковыми файлами.</param>
        /// <param name="gamePath">Путь к корневой папке игры.</param>
        /// <remarks>
        /// Удаляет существующую языковую папку модуля и заменяет ее новой версией.
        /// </remarks>
        private static void FolderUpdate(string module, string language, string dirEntry, string gamePath)
        {
            string tempDir = $"{gamePath}\\Data\\{module}\\Languages\\{language}";
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
            Directory.Move(dirEntry, tempDir);
        }
    }
}