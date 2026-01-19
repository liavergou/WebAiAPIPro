namespace CoordExtractorApp.Services.GenerativeAI
{
    /// <summary>
    /// Service interface for extracting WKT geometry from images using using a third-party .NET SDK (Google GenerativeAI) based on Google Gemini REST APIs.
    /// </summary>
    public interface IGenerativeAIService
    {
        /// <summary>
        /// Sends an image and a text prompt to the Gemini Generative AI API to extract geometry in WKT format.
        /// </summary>
        /// <param name="imageBytes">The image file data as a byte array.</param>
        /// <param name="mimeType">The MIME type of the image (e.g. "image/png").</param>
        /// <param name="promptText">The text prompt instructing the AI on what to extract.</param>
        /// <returns>A string containing the extracted geometry in Well-Known Text format.</returns>
        /// <exception cref="InvalidOperationException">Thrown if API keys or models are missing, or if the API response is invalid.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the prompt text is null or empty.</exception>
        Task<string> GetWktFromImageAsync(byte[] imageBytes, string mimeType, string promptText);
    }
}
