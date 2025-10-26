using LiteDB;
using RimLangKit.Checks;
using RimLangKit.Models;

namespace RimLangKit.Repositories
{
    public class TranslationRepository
    {
        private string databasePath;

        public TranslationRepository(string databasePath)
        {
            this.databasePath = databasePath;
        }

        public TranslationRepository()
        {
            this.databasePath = "RimLang.db";
        }

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