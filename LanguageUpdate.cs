using ICSharpCode.SharpZipLib.Zip;

namespace RTK
{
    internal static class LanguageUpdate
    {
        internal static (bool, string) LanguageUpdateProcessing(string CurrentFile)
        {
            string error = string.Empty;
            string sha = Properties.Settings.Default.sha;
            string repo = Properties.Settings.Default.repo;
            string language = Properties.Settings.Default.language;
            string directory = Properties.Settings.Default.directory;

            List<Root>? json = RimHttpClient.GetGithubJson(repo);
            string currentSha = json[0].sha;


            if (currentSha == sha)
            {
                error = "Обновление не требуется";
                return (false, error);
            }
            else
            {
                Properties.Settings.Default["sha"] = currentSha;
                Properties.Settings.Default["directory"] = CurrentFile;
                Properties.Settings.Default.Save();
            }

            Stream stream = RimHttpClient.GetGithubArchive(repo);
            FastZip archive = new();
            string tempDir = "temp";
            archive.ExtractZip(stream, tempDir, FastZip.Overwrite.Always, null, null, null, true, true, true);
            stream.Close();

            string[] baseDir = Directory.GetDirectories(tempDir);
            string[] dir = Directory.GetDirectories(baseDir[0]);
            StringComparison comparison = StringComparison.OrdinalIgnoreCase;
            foreach (string dirEntry in dir)
            {
                if (dirEntry.EndsWith("Biotech", comparison))
                {
                    FolderUpdate("Biotech", dirEntry, directory, language);
                }
                else if (dirEntry.EndsWith("Core", comparison))
                {
                    FolderUpdate("Core", dirEntry, directory, language);
                }
                else if (dirEntry.EndsWith("Ideology", comparison))
                {
                    FolderUpdate("Ideology", dirEntry, directory, language);
                }
                else if (dirEntry.EndsWith("Royalty", comparison))
                {
                    FolderUpdate("Royalty", dirEntry, directory, language);
                }
            }
            Directory.Delete(tempDir, true);
            return (true, error);
        }

        private static void FolderUpdate(string type, string tempPath, string path, string? language)
        {
            string tempDir = $"{path}\\Data\\{type}\\Languages\\{language}";
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
            Directory.Move(tempPath, tempDir);
        }
    }
}
