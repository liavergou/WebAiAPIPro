using CoordExtractorApp.Core.Enums;
using CoordExtractorApp.Data;
using CoordExtractorApp.DTO;
using CoordExtractorApp.Exceptions;
using CoordExtractorApp.Helpers;
using CoordExtractorApp.Repositories;
using CoordExtractorApp.Services.GenerativeAI;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Serilog;

namespace CoordExtractorApp.Services
{

    //διαχείριση/αποθηκευση του cropped image file
    //Κλήση google llm
    //parsing response
    //save job
    public class ConversionJobService : IConversionJobService
    {

        //https://postgis.net/workshops/postgis-intro/geometries.html SELECT name, ST_AsText(geom) FROM geometries;

        private readonly IUnitOfWork unitOfWork;
        private readonly IConfiguration configuration;
        private readonly IGenerativeAIService generativeAIService;
        private readonly IPromptService promptService;
        private readonly ILogger<ConversionJobService> logger =
            new LoggerFactory().AddSerilog().CreateLogger<ConversionJobService>();
        public ConversionJobService(
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            IGenerativeAIService generativeAIService,
            IPromptService promptService)
        {
            this.unitOfWork = unitOfWork;
            this.configuration = configuration;
            this.generativeAIService = generativeAIService;
            this.promptService = promptService;
        }
        public async Task<ConversionJobReadOnlyDTO> CreateAndProcessJobAsync(ConversionJobInsertDTO dto, int userId)
        {
            logger.LogInformation("START CONVERSION: Job creation and processing for user {UserId}", userId);


            var newJob = new ConversionJob
            {
                ProjectId = dto.ProjectId,
                PromptId = dto.PromptId,
                UserId = userId,
                OriginalFileName = dto.ImageFile.FileName,
                Status = JobStatus.Processing //αρχικά Processing
            };

            try
            {
                //****μετατροπή απο το IFormFile σε byte[]
                byte[] imageBytes;
                using (var memoryStream = new MemoryStream())
                {
                    await dto.ImageFile.CopyToAsync(memoryStream);
                    imageBytes = memoryStream.ToArray();
                }


                //****καλώ το filehelper που μου εχει επιστρέψει το όνομα του αρχείου
                var uniqueFileName = await FileHelper.SaveImageFromBytesAsync(
                    imageBytes, dto.ImageFile.FileName, dto.ProjectId, configuration);

           
                newJob.CroppedFileName = uniqueFileName; // Αποθηκεύουμε το unique filename
                logger.LogInformation("Image saved in project folder {ProjectId} filename:{FileName}",uniqueFileName,dto.ProjectId);

                //**να πάρω το promptText από την επιλογή του User.
                var prompt = await promptService.GetPromptByIdAsync(dto.PromptId);
              
                logger.LogInformation("Prompt {PromptId} found.", dto.PromptId);

                if (prompt == null)
                {

                    throw new EntityNotFoundException("Prompt",$"Prompt with ID:{dto.PromptId}");

                }

                // ***************ΚΛΗΣΗ LLM
                logger.LogInformation("Generative AI call...");
                //περιμενω να επιστρέψει το WKT polygon
                var wktResult = await generativeAIService.GetWktFromImageAsync(
                    imageBytes,
                    dto.ImageFile.ContentType,
                    prompt.PromptText
                );

                logger.LogInformation("Generative AI returned successfully.");

                // ******PARSING WKT → GEOMETRY
                // NetTopologySuite WKTReader: μετατρέπει WKT string σε Geometry object
                var wktReader = new WKTReader();
                var geometry = wktReader.Read(wktResult); //https://nettopologysuite.github.io/NetTopologySuite/api/NetTopologySuite.IO.WKTReader.html

                //έλεγχος αν δεν είναι πολύγωνο
                if (geometry is not Polygon)
                {
                    throw new InvalidOperationException($"The LLM result is not a valid polygon geometry. Geometry:{geometry.GeometryType}");
                }

                newJob.Geom = geometry;

                // Job status COMPLETED
                newJob.Status = JobStatus.Completed;
                newJob.ModelUsed = configuration["Gemini:Model"];
            }
            
            catch (Exception ex) when 
            (ex is InvalidOperationException || ex is EntityNotFoundException || ex is ArgumentNullException)
            {
                // Job status FAILURE
                logger.LogError(ex, "Job processing FAILED.");
                newJob.Status = JobStatus.Failed;
                newJob.ErrorMessage = ex.Message;
                //TODO ΝΑ ΑΠΟΘΗΚΕΥΣΩ ΤΟ FAILED????? ME TON GeoServer τι γινεται? θα χτυπήσει?

            }

            //αποθήκευση στη βάση
            await unitOfWork.ConversionJobRepository.AddAsync(newJob);
            await unitOfWork.SaveAsync(); // Commit στη βάση

            logger.LogInformation("Job {JobId} saved with final status: {Status}", newJob.Id, newJob.Status);

            //mapping για επιστροφή response
            var responseDto = new ConversionJobReadOnlyDTO
            {
                Id = newJob.Id,
                OriginalFileName = newJob.OriginalFileName,
                CroppedFileName = newJob.CroppedFileName,
                ModelUsed = newJob.ModelUsed,
                Status = newJob.Status,
                ErrorMessage = newJob.ErrorMessage
            };

            // μετατροπή geometry σε σημεία
            if (newJob.Geom != null && newJob.Geom is NetTopologySuite.Geometries.Polygon polygon)
            {

                //Coordinate → CoordinateDTO
                //https://nettopologysuite.github.io/NetTopologySuite/api/NetTopologySuite.Geometries.Polygon.html
                //https://nettopologysuite.github.io/NetTopologySuite/api/NetTopologySuite.Geometries.Polygon.html#NetTopologySuite_Geometries_Polygon_Coordinate
                responseDto.Coordinates = polygon.ExteriorRing.Coordinates
                    .Take(polygon.ExteriorRing.Coordinates.Length - 1) // Αφαιρούμε το τελευταίο σημείο γιατι ειναι επαναληψη του πρώτου.
                    .Select((coord, index) => new CoordinateDTO
                    {
                        Order = index + 1,  // +1 γιατι ειναι 0 το πρώτο. να πάρει σωστό ordering
                        X = coord.X,
                        Y = coord.Y
                    })
                    .ToList();
            }

            return responseDto; // 200 OK
        }
    }
}