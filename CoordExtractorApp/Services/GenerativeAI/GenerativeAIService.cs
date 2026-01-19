using CoordExtractorApp.DTO.GenerativeAI;
using GenerativeAI;
using GenerativeAI.Types;

namespace CoordExtractorApp.Services.GenerativeAI
{
    /// <summary>
    /// Service implementation for extracting WKT geometry from images using a third-party .NET SDK (Google GenerativeAI) based on Google Gemini REST APIs.
    /// </summary>
    public class GenerativeAIService(IConfiguration configuration, ILogger<GenerativeAIService> logger) : IGenerativeAIService
    {

        private readonly IConfiguration configuration = configuration;
        private readonly ILogger <GenerativeAIService> logger = logger;

        
        public async Task<string> GetWktFromImageAsync(byte[] imageBytes, string mimeType, string promptText)
        {
            try
            {
                var apiKey = configuration["Gemini:Credentials:ApiKey"];
                if (string.IsNullOrEmpty(apiKey))
                {
                    throw new InvalidOperationException("Gemini Api Key is not configured in appsettings");
                }

                var modelName = configuration["Gemini:Model"];
                if (string.IsNullOrEmpty(modelName))
                {
                    throw new InvalidOperationException("Gemini Model is not configured in appsettings");
                }
                
                if (!modelName.StartsWith("models/"))
                {
                    modelName = $"models/{modelName}";
                }

                if (string.IsNullOrEmpty(promptText))
                {
                    throw new ArgumentNullException(nameof(promptText), "Prompt text cannot be null or empty");
                }


                logger.LogInformation("Calling Gemini API with model: {Model}", modelName);

                
                var googleAI = new GoogleAi(apiKey);

                var model = googleAI.CreateGenerativeModel(modelName);

                var textPart = new Part { Text = promptText };

                var imagePart = new Part                
                {
                    InlineData = new Blob
                    {
                        MimeType = mimeType,
                        Data = Convert.ToBase64String(imageBytes)
                    }
                };

                var parts = new List<Part> { textPart, imagePart };
                var result = await model.GenerateObjectAsync<GeminiResponseDTO>(parts);
                

                if (result == null)
                {
                    throw new InvalidOperationException("Gemini API response cannot be null");
                }

                if (result.Errors != null && result.Errors.Count>0)
                {
                    string errorMessages = string.Join(";", result.Errors);
                    throw new InvalidOperationException($"Gemini returned errors:{errorMessages}");
                }

                if (string.IsNullOrEmpty(result.WktPolygon))
                {
                    throw new InvalidOperationException("Gemini response did not contain a WKT polygon");
                }

                return result.WktPolygon;

            } catch (Exception ex) {
                logger.LogError(ex, "An error occured during Gemini API Call");
                throw;
            }            
        }
    }
}
