namespace RimLangKit.Models
{
    /// <summary>
    /// Представляет тег перевода из XML-файла RimWorld.
    /// </summary>
    /// <remarks>
    /// Содержит информацию о теге: его имя (defName), текст перевода, английский комментарий и тип.
    /// Поля public для совместимости с LiteDB.
    /// </remarks>
    public class RimTag
    {
        /// <summary>Имя определения тега (defName), например "ThingDef.label".</summary>
        public string TagDef { get; set; }
        /// <summary>Текст перевода (русский).</summary>
        public string TagText { get; set; }
        /// <summary>Английский текст из комментария "<!-- EN: ... -->".</summary>
        public string TagComment { get; set; }
        /// <summary>Тип тега (обычно имя родительской папки, например "Defs").</summary>
        public string TagType { get; set; }

        /// <summary>
        /// Создает тег с полной информацией (используется при сборе данных из существующего перевода).
        /// </summary>
        /// <param name="tagDef">Имя определения тега.</param>
        /// <param name="tagText">Текст перевода.</param>
        /// <param name="tagComment">Английский текст из комментария.</param>
        /// <param name="tagType">Тип тега.</param>
        public RimTag(string tagDef, string tagText, string tagComment, string tagType)
        {
            TagDef = tagDef;
            TagText = tagText;
            TagComment = tagComment;
            TagType = tagType;
        }

        /// <summary>
        /// Создает тег без английского комментария (используется при сборе данных переводимого текста).
        /// </summary>
        /// <param name="tagDef">Имя определения тега.</param>
        /// <param name="tagText">Текст для перевода (английский).</param>
        /// <param name="tagType">Тип тега.</param>
        /// <remarks>
        /// TagComment устанавливается в пустую строку.
        /// </remarks>
        public RimTag(string tagDef, string tagText, string tagType)
        {
            TagDef = tagDef;
            TagText = tagText;
            TagComment = string.Empty;
            TagType = tagType;
        }

        /// <summary>
        /// Создает пустой тег (необходим для десериализации LiteDB).
        /// </summary>
        public RimTag()
        {
            TagDef = string.Empty;
            TagText = string.Empty;
            TagComment = string.Empty;
            TagType = string.Empty;
        }
    }
}