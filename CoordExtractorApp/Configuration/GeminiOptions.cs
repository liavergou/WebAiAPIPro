namespace CoordExtractorApp.Configuration
{
    public class GeminiOptions
    {
        public const string Gemini = "Gemini";

        public GeminiCredentials Credentials { get; set; } = new();
        public string Model { get; set; } = string.Empty;
    }

    public class GeminiCredentials
    {
        public string ApiKey { get; set; } = string.Empty;
    }
}
