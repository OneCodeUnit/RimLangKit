using RimLangKit.Checks;
using RimLangKit.Parsers;
using RimLangKit.Repositories;

namespace RimLangKit.Modules.AutoTranslation
{
    public static class AutoTranslator
    {
        public static XmlError LoadDatabase(string databasePath)
        {
            var repository = new TranslationRepository(databasePath);
            var analysisResult = repository.Analyze("translated_tags");
            return analysisResult;
        }

        public static void NewDatabase(string databasePath)
        {
            if (File.Exists(databasePath))
                File.Delete(databasePath);
            var repository = new TranslationRepository(databasePath);
        }

        public static XmlError CreateDatabase(string databasePath, string currentFile, string collectionName, bool rewrite)
        {
            var repository = new TranslationRepository(databasePath);
            XmlError result = TranslationParser.ParseAndSaveToDatabase(currentFile, repository, collectionName, rewrite);
            return result;
        }

        public static XmlError TranslateFile(string databasePath, string currentFile, string collectionName)
        {
            var repository = new TranslationRepository(databasePath);
            XmlError result = TranslationParser.ParseAndTranslateTags(currentFile, repository, collectionName);
            return result;
        }
    }
}
