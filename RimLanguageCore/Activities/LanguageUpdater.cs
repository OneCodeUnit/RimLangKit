using System;
using System.IO;
using System.IO.Compression;
using RimLanguageCore.Misc;

namespace RimLanguageCore.Activities
{
    public static class LanguageUpdater
    {
        private static string TempSha = string.Empty;
        private static string TempDir = string.Empty;

        public static string GetSha()
        {
            return TempSha;
        }

        public static (bool, string) TranslationVersionCheckActivity(string sha, string repo)
        {
            Root json = GitHubHttpClient.GetGithubSha(repo);
            if (json is null)
            {
                return (false, "Проверка версии: Ошибка получения данных с GitHub. Что-то с интернетом?");
            }
            if (json.sha == sha)
            {
                TempSha = string.Empty;
                return (false, $"Ваша версия - {sha[..6]}, доступная версия - {json.sha[..6]}. Обновление не требуется");
            }
            else
            {
                TempSha = json.sha;
                return (true, $"Ваша версия - {sha[..6]}, доступная версия - {json.sha[..6]}. Требуется обновление");
            }
        }

        public static (bool, string) LanguageUpdateDownload(string repo)
        {
            // Получение файлов перевода
            Stream stream = GitHubHttpClient.GetGithubArchive(repo);
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
                if ((!dirEntry.EndsWith("RimWorldUniverse", comparison)) && (ok == false))
                {
                    string module = dirEntry.Replace($"{baseDir[0]}\\", string.Empty);
                    result += $"{Environment.NewLine}Не найден модуль \"{module}\"";
                }
            }
            Directory.Delete(TempDir, true);
            result += $"{Environment.NewLine}Обновление завершено!";
            return (true, result);
        }

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
