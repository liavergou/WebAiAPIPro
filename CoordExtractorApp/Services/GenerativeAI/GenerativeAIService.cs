using CoordExtractorApp.DTO.GenerativeAI;
using GenerativeAI;
using GenerativeAI.Types;

namespace CoordExtractorApp.Services.GenerativeAI
{
    public class GenerativeAIService(IConfiguration configuration, ILogger<GenerativeAIService> logger) : IGenerativeAIService
    {

        private readonly IConfiguration configuration = configuration;
        private readonly ILogger <GenerativeAIService> logger = logger;

        public async Task<string> GetWktFromImageAsync(byte[] imageBytes, string mimeType, string promptText)
        {
            

            try
            {
                
                var apiKey = configuration["Gemini:Credentials:ApiKey"]; //έλεγχος ύπαρξης apiKey από settings
                if (string.IsNullOrEmpty(apiKey))
                {
                    throw new InvalidOperationException("Gemini Api Key is not configured in appsettings");
                }


                //έλεγχος ύπαρξης model
                var modelName = configuration["Gemini:Model"]; //απο settings
                if (string.IsNullOrEmpty(modelName))
                {
                    throw new InvalidOperationException("Gemini Model is not configured in appsettings");
                }


                //ελεγχος αν υπάρχει το πρόθεμα models/ που πρέπει να υπάρχει για το gemini api
                if (!modelName.StartsWith("models/"))
                {
                    modelName = $"models/{modelName}";
                }

                if (string.IsNullOrEmpty(promptText))
                {

                    throw new ArgumentNullException(nameof(promptText), "Prompt text cannot be null or empty");

                }


                logger.LogInformation("Calling Gemini API with model: {Model}", modelName);

                //δημιουργία Instance για το GenerativeAi
                var googleAI = new GoogleAi(apiKey);

            //https://gunpal5.github.io/generative-ai/html/aafe0c76-3855-8b3c-d0ed-76af0fe53276.htm
            //https://github.com/gunpal5/Google_GenerativeAI?tab=readme-ov-file#1-using-google-ai
                // Δημιουργία του GenerativeModel με το καθορισμένο μοντέλο. Δινω μονο την πρώτη παράμετρο. todo να χρησιμοποιήσω το systemInstruction 
                var model = googleAI.CreateGenerativeModel(modelName);




            //***** PROMPT TEXT KAI EIKONA GIA TO REQUEST. ΔΗΜΙΟΥΡΓΙΑ ΤΩΝ PARTS
            //https://gunpal5.github.io/generative-ai/html/c26d5035-f439-24ad-0804-09da2c89e445.htm
            //https://gunpal5.github.io/generative-ai/html/d31a5a25-a65b-d8ae-dee8-bc6e3bd84400.htm

                var textPart = new Part { Text = promptText };


                var imagePart = new Part                
                {
                    //https://gunpal5.github.io/generative-ai/html/f2f8b5d4-408b-4854-57a4-afd3506944be.htm
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

                return result.WktPolygon; //αν ολα οκ επιστροφή WktPolygon

            } catch (Exception ex) {
                logger.LogError(ex, "An error occured during Gemini API Call");
                throw;
            }            
        }
    }
}
