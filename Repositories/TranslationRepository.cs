using LiteDB;
using RimLangKit.Checks;
using RimLangKit.Models;

namespace RimLangKit.Repositories
{
    /// <summary>
    /// Репозиторий для работы с базой данных переводов на основе LiteDB.
    /// </summary>
    /// <remarks>
    /// Предоставляет методы для сохранения, извлечения и управления тегами переводов.
    /// Использует LiteDB для хранения данных локально.
    /// </remarks>
    public class TranslationRepository
    {
        /// <summary>Путь к файлу базы данных LiteDB.</summary>
        private string databasePath;

        /// <summary>
        /// Создает экземпляр репозитория с указанным путем к базе данных.
        /// </summary>
        /// <param name="databasePath">Полный путь к файлу базы данных.</param>
        public TranslationRepository(string databasePath)
        {
            this.databasePath = databasePath;
        }

        /// <summary>
        /// Создает экземпляр репозитория с путем к базе данных по умолчанию.
        /// </summary>
        /// <remarks>
        /// Использует путь по умолчанию "RimLang.db" в текущей директории.
        /// </remarks>
        public TranslationRepository()
        {
            this.databasePath = "RimLang.db";
        }

        /// <summary>
        /// Сохраняет список тегов в указанную коллекцию базы данных.
        /// </summary>
        /// <param name="tags">Список тегов для сохранения.</param>
        /// <param name="collectionName">Имя коллекции в базе данных.</param>
        /// <param name="rewrite">
        /// true - перезаписывать существующие теги (Upsert).
        /// false - добавлять только новые теги, пропуская существующие.
        /// </param>
        /// <remarks>
        /// При rewrite=false проверяет существование тегов по полям TagDef и TagType.
        /// </remarks>
        public void SaveTags(List<RimTag> tags, string collectionName, bool rewrite)
        {
            using (var db = new LiteDatabase(databasePath))
            {
                var collection = db.GetCollection<RimTag>(collectionName);

                foreach (var tag in tags)
                {
                    if (rewrite)
                    {
                        collection.Upsert(tag);
                    }
                    else
                    {
                        if (!collection.Exists(x => x.TagDef == tag.TagDef && x.TagType == tag.TagType))
                        {
                            collection.Insert(tag);
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Ищет перевод для указанного тега в базе данных.
        /// </summary>
        /// <param name="tag">Тег с заполненными полями TagType и TagComment (английский текст).</param>
        /// <param name="collectionName">Имя коллекции для поиска.</param>
        /// <returns>
        /// Текст перевода (TagText) найденного тега или пустая строка, если тег не найден.
        /// </returns>
        /// <remarks>
        /// Поиск выполняется по совпадению TagType и TagComment (английский текст из комментария).
        /// </remarks>
        public string GetTag(RimTag tag, string collectionName)
        {
            using (var db = new LiteDatabase(databasePath))
            {
                var collection = db.GetCollection<RimTag>(collectionName);
                var result = collection.FindOne(x =>x.TagType == tag.TagType && x.TagComment == tag.TagText);
                if (result is null)
                    return string.Empty;
                else
                    return result.TagText;
            }
        }

        /// <summary>
        /// Анализирует коллекцию и возвращает количество тегов в ней.
        /// </summary>
        /// <param name="collectionName">Имя коллекции для анализа.</param>
        /// <returns>
        /// Объект XmlError с результатом:
        /// - IsValid=true и сообщение с количеством тегов при наличии записей.
        /// - IsValid=false и сообщение об ошибке при отсутствии записей.
        /// </returns>
        public XmlError Analyze(string collectionName)
        {
            using (var db = new LiteDatabase(databasePath))
            {
                var collection = db.GetCollection<RimTag>(collectionName);
                var count = collection.Count();
                var tags = collection.FindAll().ToList();
                if (count > 0)
                {
                    return new XmlError(true, $"{count} тегов.");
                }
                else
                {
                    return new XmlError(false, $"ничего нет или произошла ошибка");
                }
            }
        }

        /// <summary>
        /// Удаляет все записи из указанной коллекции.
        /// </summary>
        /// <param name="collectionName">Имя коллекции для очистки.</param>
        /// <returns>
        /// Объект XmlError с результатом операции и количеством удаленных записей.
        /// </returns>
        public XmlError Clear(string collectionName)
        {
            using (var db = new LiteDatabase(databasePath))
            {
                var collection = db.GetCollection<RimTag>(collectionName);
                var result = collection.DeleteAll();
                return new XmlError(true, $"Удалено {result} записей.");
            }
        }
    }
}