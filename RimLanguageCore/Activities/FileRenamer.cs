using System;
using System.IO;

namespace RimLanguageCore.Activities
{
    public static class FileRenamer
    {
        public static (bool, string) FileRenamerActivity(string currentFile)
        {
            // Разделение пути до файла на части и поиск нужной
            string[] path = currentFile.Split('\\');
            // Сохранение с добавлением имени папки. Берется имя папки на уровень выше "Languages"
            if (path[^1].EndsWith(".xml", StringComparison.OrdinalIgnoreCase) && path.Length > 7)
            {
                string newPath = $"{currentFile[..^4]}_{path[^6]}.xml";
                File.Move(currentFile, newPath);
                return (true, string.Empty);
            }
            else
            {
                return (false, "Папка модуля не найдена. Точно ли \"Languages\" находится в папке с названием модуля?");
            }
        }
    }
}