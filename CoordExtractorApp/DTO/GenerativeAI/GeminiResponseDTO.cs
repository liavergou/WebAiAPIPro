using System.Text.Json.Serialization;

namespace CoordExtractorApp.DTO.GenerativeAI
{
    /// <summary>
    /// Data Transfer Object representing the response from the Gemini AI service.
    /// </summary>
    public class GeminiResponseDTO
    {
        /// <summary>
        /// The WKT (Well Known Text) of the extracted polygon
        /// </summary>
        [JsonPropertyName("wktOutput")] //ποιο πεδίο του json είναι
        public string WktPolygon { get; set; } = "";

        /// <summary>
        /// The number of points of the extracted polygon.
        /// </summary>
        [JsonPropertyName("pointCount")]
        public int PointCount { get; set; }

        /// <summary>
        /// A list of error messages returned by the AI service, if any.
        /// </summary>
        [JsonPropertyName("errors")]
        public List<string> Errors { get; set; } = [];
    }
}
