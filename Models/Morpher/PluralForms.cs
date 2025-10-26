using System.Text.Json.Serialization;

namespace RimLangKit.Models.Morpher
{
    public class PluralForms
    {
        [JsonPropertyName("И")]
        public string? Nominative { get; set; }
    }
}