using System.Xml.Serialization;

namespace TextExporter
{
    [XmlRoot(ElementName = "Defs")]
    public class Defs
    {
        [XmlElement(ElementName = "BackstoryDef")]
        public List<BackstoryDef>? BackstoryDef { get; set; }

        [XmlElement(ElementName = "GeneDef")]
        public List<GeneDef>? GeneDef { get; set; }

        [XmlElement(ElementName = "HediffDef")]
        public List<HediffDef>? HediffDef { get; set; }

        [XmlElement(ElementName = "ThingDef")]
        public List<ThingDef>? ThingDef { get; set; }

        [XmlElement(ElementName = "XenotypeDef")]
        public List<XenotypeDef>? XenotypeDef { get; set; }
    }

    public class BaseDef
    {
        [XmlElement(ElementName = "defName")]
        public string? DefName { get; set; }

        [XmlElement(ElementName = "label")]
        public string? Label { get; set; }
    }

    [XmlRoot(ElementName = "BackstoryDef")]
    public class BackstoryDef : BaseDef
    {

        [XmlElement(ElementName = "title")]
        public string? Title { get; set; }

        [XmlElement(ElementName = "titleShort")]
        public string? TitleShort { get; set; }

        [XmlElement(ElementName = "description")]
        public string? Description { get; set; }

    }

    [XmlRoot(ElementName = "GeneDef")]
    public class GeneDef : BaseDef
    {
        [XmlElement(ElementName = "description")]
        public string? Description { get; set; }

        [XmlElement(ElementName = "labelShortAdj")]
        public string? LabelShortAdj { get; set; }

    }

    [XmlRoot(ElementName = "HediffDef")]
    public class HediffDef : BaseDef
    {
        [XmlElement(ElementName = "description")]
        public string? Description { get; set; }

        [XmlElement(ElementName = "labelNoun")]
        public string? LabelNoun { get; set; }

        [XmlElement(ElementName = "labelNounPretty")]
        public string? LabelNounPretty { get; set; }

    }

    [XmlRoot(ElementName = "ThingDef")]
    public class ThingDef : BaseDef
    {
        [XmlElement(ElementName = "description")]
        public string? Description { get; set; }

    }

    [XmlRoot(ElementName = "XenotypeDef")]
    public class XenotypeDef : BaseDef
    {
        [XmlElement(ElementName = "description")]
        public string? Description { get; set; }

        [XmlElement(ElementName = "descriptionShort")]
        public string? DescriptionShort { get; set; }
    }
}