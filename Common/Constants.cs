namespace RimLangKit.Common
{
    /// <summary>
    /// Константы приложения
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Константы для работы с файлами
        /// </summary>
        public static class Files
        {
            public const string XmlMask = "*.xml";
            public const string TxtMask = "*.txt";
            public const string DatabaseExtension = ".db";
            public const string DefaultDatabaseName = "RimLang.db";
        }

        /// <summary>
        /// Константы для путей
        /// </summary>
        public static class Paths
        {
            /// <summary>
            /// Steam App ID для RimWorld (используется для проверки некорректных путей)
            /// </summary>
            public const string SteamRimWorldAppId = "294100";

            public const string CommonFolderName = "Common";
            public const string DataFolderName = "Data";
            public const string LanguagesFolderName = "Languages";
            public const string WordInfoSubPath = "Languages\\Russian\\WordInfo";
        }

        /// <summary>
        /// Константы для репозиториев
        /// </summary>
        public static class Repository
        {
            public const string DefaultLanguage = "Russian (GitHub)";
            public const string DefaultRepo = "Ludeon/RimWorld-ru";
            public const string DefaultSha = "00000000";
        }

        /// <summary>
        /// Константы для парсинга
        /// </summary>
        public static class Parsing
        {
            /// <summary>
            /// Минимальная длина XML комментария вида <!-- EN: ... -->
            /// </summary>
            public const int MinCommentLength = 13;

            public const string LanguageDataRootElement = "LanguageData";
        }

        /// <summary>
        /// Константы для Morpher API
        /// </summary>
        public static class MorpherApi
        {
            public const string BaseUrl = "https://ws3.morpher.ru";
            public const string QueriesLeftEndpoint = "/get-queries-left?format=json";
            public const string DeclensionEndpoint = "/russian/declension?s={0}&format=json";
        }

        /// <summary>
        /// Константы для GitHub API
        /// </summary>
        public static class GitHubApi
        {
            public const string BaseUrl = "https://api.github.com";
            public const string LatestReleaseEndpoint = "/repos/OneCodeUnit/RimLangKit/releases/latest";
            public const string CommitsEndpointFormat = "/repos/{0}/commits/master";
            public const string ArchiveUrlFormat = "https://github.com/{0}/archive/refs/heads/{1}.zip";
        }

        /// <summary>
        /// Типы Def для создания вспомогательных файлов
        /// </summary>
        public static class DefTypes
        {
            public static readonly string[] SupportedTypes =
            [
                "AbilityDef", "BodyDef", "BodyPartDef", "BodyPartGroupDef",
                "ChemicalDef", "FactionDef", "HediffDef", "MemeDef",
                "MentalBreakDef", "MentalFitDef", "MentalStateDef",
                "OrderedTakeGroupDef", "PawnCapacityDef", "PawnKindDef",
                "ScenarioDef", "SitePartDef", "SkillDef", "StyleCategoryDef",
                "ThingDef", "ToolCapacityDef", "WorldObjectDef", "XenotypeDef"
            ];
        }

        /// <summary>
        /// Константы для UI
        /// </summary>
        public static class UI
        {
            public const int MaxErrorsToDisplay = 5;
        }
    }
}
