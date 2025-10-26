namespace RimLangKit.Models
{
    public class RimTag
    {
        // Поля public, потому что иначе LiteDB не сможет их обрабатывать
        public string TagDef { get; set; }
        public string TagText { get; set; }
        public string TagComment { get; set; }
        public string TagType { get; set; }

        // Полный конструктор используется для сбора данных из существующего перевода
        public RimTag(string tagDef, string tagText, string tagComment, string tagType)
        {
            TagDef = tagDef;
            TagText = tagText;
            TagComment = tagComment;
            TagType = tagType;
        }

        // Конструктор без комментария используется при сборе данных переводимого текста. TagComment все равно будет идентичен TagText
        public RimTag(string tagDef, string tagText, string tagType)
        {
            TagDef = tagDef;
            TagText = tagText;
            TagComment = string.Empty;
            TagType = tagType;
        }

        // Пустой конструктор необходим для LiteDB
        public RimTag()
        {
            TagDef = string.Empty;
            TagText = string.Empty;
            TagComment = string.Empty;
            TagType = string.Empty;
        }
    }
}