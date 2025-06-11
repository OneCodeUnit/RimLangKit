namespace RimLanguageUI.Model
{
    public class RimTag
    {
        private readonly string tagKey;
        private string tagText = string.Empty;
        private string defType = string.Empty;
        private readonly string defName;

        public RimTag(string tagKey, string tagText, string defType, string defName)
        {
            this.tagKey = tagKey;
            SetTagText(tagText, defType);
            SetParentDef(defType);
            this.defName = defName;
        }

        private void SetTagText(string tagText, string defType)
        {
            if (defType.Equals("BackstoryDef", StringComparison.OrdinalIgnoreCase) && tagText.Contains('['))
                this.tagText = tagText.Replace('[', '{').Replace(']', '}');
            else
                this.tagText = tagText;
        }

        private void SetParentDef(string defType)
        {
            if (defType.Contains(' '))
                this.defType = defType[0..defType.IndexOf(' ')];
            else
                this.defType = defType;
        }

        public string GetTagKey()
        {
            return tagKey;
        }
        public string GetTagText()
        {
            return tagText;
        }
        public string GetDefType()
        {
            return defType;
        }
        public string GetDefName()
        {
            return defName;
        }
    }
}