using System.Xml.Serialization;

namespace TextExporter
{
    [XmlRoot(ElementName = "Defs")]
    public class Defs
    {
        [XmlElement(ElementName = "AbilityDef")]
        public List<AbilityDef>? AbilityDef { get; set; }
        [XmlElement(ElementName = "BackstoryDef")]
        public List<BackstoryDef>? BackstoryDef { get; set; }
        [XmlElement(ElementName = "CultureDef")]
        public List<CultureDef>? CultureDef { get; set; }
        [XmlElement(ElementName = "DamageDef")]
        public List<DamageDef>? DamageDef { get; set; }
        [XmlElement(ElementName = "FactionDef")]
        public List<FactionDef>? FactionDef { get; set; }
        [XmlElement(ElementName = "GameConditionDef")]
        public List<GameConditionDef>? GameConditionDef { get; set; }
        [XmlElement(ElementName = "GeneDef")]
        public List<GeneDef>? GeneDef { get; set; }
        [XmlElement(ElementName = "HairDef")]
        public List<HairDef>? HairDef { get; set; }
        [XmlElement(ElementName = "HediffDef")]
        public List<HediffDef>? HediffDef { get; set; }
        [XmlElement(ElementName = "IncidentDef")]
        public List<IncidentDef>? IncidentDef { get; set; }
        [XmlElement(ElementName = "JobDef")]
        public List<JobDef>? JobDef { get; set; }
        [XmlElement(ElementName = "PawnKindDef")]
        public List<PawnKindDef>? PawnKindDef { get; set; }
        [XmlElement(ElementName = "PawnsArrivalModeDef")]
        public List<PawnsArrivalModeDef>? PawnsArrivalModeDef { get; set; }
        [XmlElement(ElementName = "RaidStrategyDef")]
        public List<RaidStrategyDef>? RaidStrategyDef { get; set; }
        [XmlElement(ElementName = "StyleItemCategoryDef")]
        public List<StyleItemCategoryDef>? StyleItemCategoryDef { get; set; }
        [XmlElement(ElementName = "ThingDef")]
        public List<ThingDef>? ThingDef { get; set; }
        [XmlElement(ElementName = "ToolCapacityDef")]
        public List<ToolCapacityDef>? ToolCapacityDef { get; set; }
        [XmlElement(ElementName = "WeatherDef")]
        public List<WeatherDef>? WeatherDef { get; set; }
        [XmlElement(ElementName = "XenotypeDef")]
        public List<XenotypeDef>? XenotypeDef { get; set; }
    }

    [XmlRoot(ElementName = "AbilityDef")]
    public class AbilityDef
    {
        [XmlElement(ElementName = "defName")]
        public string? DefName { get; set; }

        [XmlElement(ElementName = "label")]
        public string? Label { get; set; }

        [XmlElement(ElementName = "description")]
        public string? Description { get; set; }

    }

    [XmlRoot(ElementName = "BackstoryDef")]
    public class BackstoryDef
    {
        [XmlElement(ElementName = "defName")]
        public string? DefName { get; set; }

        [XmlElement(ElementName = "title")]
        public string? Title { get; set; }

        [XmlElement(ElementName = "titleShort")]
        public string? TitleShort { get; set; }

        [XmlElement(ElementName = "description")]
        public string? Description { get; set; }

    }

    [XmlRoot(ElementName = "CultureDef")]
    public class CultureDef
    {
        [XmlElement(ElementName = "defName")]
        public string? DefName { get; set; }

        [XmlElement(ElementName = "label")]
        public string? Label { get; set; }

        [XmlElement(ElementName = "description")]
        public string? Description { get; set; }

    }

    [XmlRoot(ElementName = "DamageDef")]
    public class DamageDef
    {
        [XmlElement(ElementName = "defName")]
        public string? DefName { get; set; }

        [XmlElement(ElementName = "label")]
        public string? Label { get; set; }

        [XmlElement(ElementName = "deathMessage")]
        public string? DeathMessage { get; set; }

    }

    [XmlRoot(ElementName = "FactionDef")]
    public class FactionDef
    {
        [XmlElement(ElementName = "defName")]
        public string? DefName { get; set; }

        [XmlElement(ElementName = "label")]
        public string? Label { get; set; }

        [XmlElement(ElementName = "fixedName")]
        public string? FixedName { get; set; }

        [XmlElement(ElementName = "pawnsPlural")]
        public string? PawnsPlural { get; set; }

        [XmlElement(ElementName = "leaderTitle")]
        public string? LeaderTitle { get; set; }

        [XmlElement(ElementName = "pawnSingular")]
        public string? PawnSingular { get; set; }

    }

    [XmlRoot(ElementName = "GameConditionDef")]
    public class GameConditionDef
    {
        [XmlElement(ElementName = "defName")]
        public string? DefName { get; set; }

        [XmlElement(ElementName = "label")]
        public string? Label { get; set; }

        [XmlElement(ElementName = "description")]
        public string? Description { get; set; }

        [XmlElement(ElementName = "endMessage")]
        public string? EndMessage { get; set; }

        [XmlElement(ElementName = "letterText")]
        public string? LetterText { get; set; }

        [XmlElement(ElementName = "descriptionFuture")]
        public string? DescriptionFuture { get; set; }

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

        [XmlElement(ElementName = "labelShortAdj")]
        public string? LabelShortAdj { get; set; }

    }

    [XmlRoot(ElementName = "HairDef")]
    public class HairDef
    {
        [XmlElement(ElementName = "defName")]
        public string? DefName { get; set; }

        [XmlElement(ElementName = "label")]
        public string? Label { get; set; }

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

        [XmlElement(ElementName = "labelNoun")]
        public string? LabelNoun { get; set; }

        [XmlElement(ElementName = "labelNounPretty")]
        public string? LabelNounPretty { get; set; }

    }

    [XmlRoot(ElementName = "IncidentDef")]
    public class IncidentDef
    {
        [XmlElement(ElementName = "defName")]
        public string? DefName { get; set; }

        [XmlElement(ElementName = "label")]
        public string? Label { get; set; }

        [XmlElement(ElementName = "letterLabel")]
        public string? LetterLabel { get; set; }

        [XmlElement(ElementName = "letterText")]
        public string? LetterText { get; set; }

    }

    [XmlRoot(ElementName = "JobDef")]
    public class JobDef
    {
        [XmlElement(ElementName = "defName")]
        public string? DefName { get; set; }

        [XmlElement(ElementName = "reportString")]
        public string? ReportString { get; set; }

    }

    [XmlRoot(ElementName = "PawnKindDef")]
    public class PawnKindDef
    {
        [XmlElement(ElementName = "defName")]
        public string? DefName { get; set; }

        [XmlElement(ElementName = "label")]
        public string? Label { get; set; }

        [XmlElement(ElementName = "labelPlural")]
        public string? LabelPlural { get; set; }

    }

    [XmlRoot(ElementName = "PawnsArrivalModeDef")]
    public class PawnsArrivalModeDef
    {
        [XmlElement(ElementName = "defName")]
        public string? DefName { get; set; }

        [XmlElement(ElementName = "textEnemy")]
        public string? TextEnemy { get; set; }

        [XmlElement(ElementName = "textFriendly")]
        public string? TextFriendly { get; set; }

        [XmlElement(ElementName = "textWillArrive")]
        public string? TextWillArrive { get; set; }

    }

    [XmlRoot(ElementName = "RaidStrategyDef")]
    public class RaidStrategyDef
    {
        [XmlElement(ElementName = "defName")]
        public string? DefName { get; set; }

        [XmlElement(ElementName = "arrivalTextFriendly")]
        public string? ArrivalTextFriendly { get; set; }

        [XmlElement(ElementName = "arrivalTextEnemy")]
        public string? ArrivalTextEnemy { get; set; }

        [XmlElement(ElementName = "letterLabelEnemy")]
        public string? LetterLabelEnemy { get; set; }

        [XmlElement(ElementName = "letterLabelFriendly")]
        public string? LetterLabelFriendly { get; set; }

    }

    [XmlRoot(ElementName = "StyleItemCategoryDef")]
    public class StyleItemCategoryDef
    {
        [XmlElement(ElementName = "defName")]
        public string? DefName { get; set; }

        [XmlElement(ElementName = "label")]
        public string? Label { get; set; }

    }

    [XmlRoot(ElementName = "ThingDef")]
    public class ThingDef
    {
        [XmlElement(ElementName = "defName")]
        public string? DefName { get; set; }

        [XmlElement(ElementName = "label")]
        public string? Label { get; set; }

        [XmlElement(ElementName = "description")]
        public string? Description { get; set; }

    }

    [XmlRoot(ElementName = "ToolCapacityDef")]
    public class ToolCapacityDef
    {
        [XmlElement(ElementName = "defName")]
        public string? DefName { get; set; }

        [XmlElement(ElementName = "label")]
        public string? Label { get; set; }

    }

    [XmlRoot(ElementName = "WeatherDef")]
    public class WeatherDef
    {
        [XmlElement(ElementName = "defName")]
        public string? DefName { get; set; }

        [XmlElement(ElementName = "label")]
        public string? Label { get; set; }

        [XmlElement(ElementName = "description")]
        public string? Description { get; set; }

    }

    [XmlRoot(ElementName = "XenotypeDef")]
    public class XenotypeDef
    {
        [XmlElement(ElementName = "defName")]
        public string? DefName { get; set; }

        [XmlElement(ElementName = "label")]
        public string? Label { get; set; }

        [XmlElement(ElementName = "description")]
        public string? Description { get; set; }

        [XmlElement(ElementName = "descriptionShort")]
        public string? DescriptionShort { get; set; }

    }

}
