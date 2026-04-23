using System.Text.Json.Serialization;

namespace MauiBuecherei.Models
{
    public class TopBuchDto
    {
        [JsonPropertyName("buchnummer")]
        public string Buchnummer { get; set; } = string.Empty;

        [JsonPropertyName("titel")]
        public string Titel { get; set; } = string.Empty;

        [JsonPropertyName("anzahl")]
        public int Anzahl { get; set; }
    }
}