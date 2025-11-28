namespace CoordExtractorApp.Services.GenerativeAI
{
    public interface IGenerativeAIService
    {
        Task<string> GetWktFromImageAsync(byte[] imageBytes, string mimeType, string promptText);
    }
}
