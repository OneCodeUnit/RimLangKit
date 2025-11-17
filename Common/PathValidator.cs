using System;
using System.IO;
using System.Linq;

namespace RimLangKit.Common
{
    /// <summary>
    /// Валидация путей для обеспечения безопасности при работе с файловой системой
    /// </summary>
    public static class PathValidator
    {
        private static readonly char[] InvalidPathChars = Path.GetInvalidPathChars();
        private static readonly string[] DangerousPatterns = ["..\\", "../", "~"];

        /// <summary>
        /// Список критических системных папок Windows, с которыми не следует работать
        /// </summary>
        private static readonly string[] CriticalSystemFolders =
        [
            "Windows",
            "System32",
            "SysWOW64",
            "Program Files",
            "Program Files (x86)"
        ];

        /// <summary>
        /// Максимальная допустимая длина пути (для Windows MAX_PATH = 260)
        /// </summary>
        private const int MaxPathLength = 260;

        /// <summary>
        /// Проверяет, является ли путь безопасным для использования
        /// </summary>
        /// <param name="path">Путь для проверки</param>
        /// <param name="errorMessage">Сообщение об ошибке, если путь небезопасен</param>
        /// <returns>True, если путь безопасен</returns>
        public static bool IsPathSafe(string path, out string? errorMessage)
        {
            errorMessage = null;

            // Проверка на null или пустую строку
            if (string.IsNullOrWhiteSpace(path))
            {
                errorMessage = "Путь не может быть пустым";
                return false;
            }

            // Проверка длины пути
            if (path.Length > MaxPathLength)
            {
                errorMessage = $"Путь слишком длинный (максимум {MaxPathLength} символов)";
                return false;
            }

            // Проверка на недопустимые символы
            if (path.IndexOfAny(InvalidPathChars) >= 0)
            {
                errorMessage = "Путь содержит недопустимые символы";
                return false;
            }

            // Проверка на path traversal атаки
            if (DangerousPatterns.Any(pattern => path.Contains(pattern, StringComparison.OrdinalIgnoreCase)))
            {
                errorMessage = "Путь содержит потенциально опасные паттерны (path traversal)";
                return false;
            }

            // Проверка на критические системные папки
            try
            {
                string fullPath = Path.GetFullPath(path);
                foreach (var criticalFolder in CriticalSystemFolders)
                {
                    string systemPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows).Substring(0, 3), criticalFolder);
                    if (fullPath.StartsWith(systemPath, StringComparison.OrdinalIgnoreCase))
                    {
                        errorMessage = $"Запрещено работать с системными папками: {criticalFolder}";
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"Ошибка при проверке пути: {ex.Message}";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Проверяет безопасность пути и выбрасывает исключение при ошибке
        /// </summary>
        /// <param name="path">Путь для проверки</param>
        /// <param name="parameterName">Имя параметра для исключения</param>
        /// <exception cref="ArgumentException">Если путь небезопасен</exception>
        public static void ValidatePathOrThrow(string path, string parameterName = "path")
        {
            if (!IsPathSafe(path, out string? errorMessage))
            {
                throw new ArgumentException(errorMessage, parameterName);
            }
        }

        /// <summary>
        /// Проверяет, находится ли путь внутри указанной базовой директории
        /// </summary>
        /// <param name="basePath">Базовая директория</param>
        /// <param name="targetPath">Проверяемый путь</param>
        /// <returns>True, если targetPath находится внутри basePath</returns>
        public static bool IsPathWithinDirectory(string basePath, string targetPath)
        {
            try
            {
                string fullBasePath = Path.GetFullPath(basePath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                string fullTargetPath = Path.GetFullPath(targetPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

                return fullTargetPath.StartsWith(fullBasePath, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Нормализует путь, удаляя опасные элементы
        /// </summary>
        /// <param name="path">Исходный путь</param>
        /// <returns>Нормализованный путь</returns>
        public static string NormalizePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return string.Empty;

            try
            {
                // Получаем абсолютный путь
                return Path.GetFullPath(path);
            }
            catch
            {
                // Если не удалось нормализовать, возвращаем исходную строку
                return path;
            }
        }
    }
}
