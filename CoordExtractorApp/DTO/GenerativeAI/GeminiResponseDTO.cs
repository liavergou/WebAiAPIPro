using System.Text.Json.Serialization;

namespace CoordExtractorApp.DTO.GenerativeAI
{
    public class GeminiResponseDTO
    {
        [JsonPropertyName("wktOutput")] //ποιο πεδίο του json είναι
        public string WktPolygon { get; set; } = "";

        [JsonPropertyName("pointCount")]
        public int PointCount { get; set; }

        [JsonPropertyName("errors")]
        public List<string> Errors { get; set; } = [];
    }
}
