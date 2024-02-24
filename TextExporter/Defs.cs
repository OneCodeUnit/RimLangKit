using System.Xml.Serialization;

namespace TextExporter
{
    [XmlRoot(ElementName = "Defs")]
    public class Defs
    {
        [XmlElement(ElementName = "GeneDef")]
        public List<GeneDef>? GeneDef { get; set; }

        [XmlElement(ElementName = "HediffDef")]
        public List<HediffDef>? HediffDef { get; set; }

    }

    [XmlRoot(ElementName = "GeneDef")]
    public class GeneDef
    {
        [XmlElement(ElementName = "defName")]
        public string? DefName { get; set; }

        [XmlElement(ElementName = "label")]
        public string? Label { get; set; }

        [XmlElement(ElementName = "description")]
        public string? Description { get; set; }
    }

    [XmlRoot(ElementName = "HediffDef")]
    public class HediffDef
    {
        [XmlElement(ElementName = "defName")]
        public string? DefName { get; set; }

        [XmlElement(ElementName = "label")]
        public string? Label { get; set; }

        [XmlElement(ElementName = "description")]
        public string? Description { get; set; }
    }
}
