using System.Text.Json.Serialization;

namespace RimLangKit.Models.Morpher
{
    public class WordForms
    {
        [JsonPropertyName("Р")]
        public string? Genitive { get; set; }

        [JsonPropertyName("Д")]
        public string? Dative { get; set; }

        [JsonPropertyName("В")]
        public string? Accusative { get; set; }

        [JsonPropertyName("Т")]
        public string? Instrumental { get; set; }

        [JsonPropertyName("П")]
        public string? Prepositional { get; set; }

        [JsonPropertyName("множественное")]
        public PluralForms? Plural { get; set; }
    }
}