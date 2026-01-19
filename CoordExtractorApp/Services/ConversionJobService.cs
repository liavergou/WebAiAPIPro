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
    /// <summary>
    /// Service implementation for handling conversion jobs.
    /// Responsibilities include managing image files, invoking Generative AI services,
    /// parsing responses, and persisting job data.
    /// </summary>
    public class ConversionJobService : IConversionJobService
    {
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
            try
            {
                var projectExists = await unitOfWork.ProjectRepository.GetAsync(dto.ProjectId);
                if (projectExists == null)
                {
                    throw new EntityNotFoundException("Project", $"Project with id :{dto.ProjectId} not found.");
                }
                var userExists = await unitOfWork.UserRepository.GetAsync(userId);
                if (userExists == null)
                {
                    throw new EntityNotFoundException("User", $"User with id :{userId} not found.");
                }

                if (userExists.Role == "Member")
                {
                    var assignedProjectIds = await unitOfWork.UserRepository.GetProjectIdsForUserAsync(userId);
                    if (!assignedProjectIds.Contains(dto.ProjectId))
                    {
                        logger.LogWarning("User {UserId} (Member) tried to create job on unassigned Project {ProjectId}", userId, dto.ProjectId);
                        throw new EntityNotAuthorizedException("Project", "You are not authorized to create jobs on this project.");
                    }
                }

                var newJob = new ConversionJob
                {
                    ProjectId = dto.ProjectId,
                    PromptId = dto.PromptId,
                    UserId = userId,
                    OriginalFileName = dto.ImageFile.FileName,
                    Status = JobStatus.Processing
                };

                try
                {
                    byte[] imageBytes;
                    using (var memoryStream = new MemoryStream())
                    {
                        await dto.ImageFile.CopyToAsync(memoryStream);
                        imageBytes = memoryStream.ToArray();
                    }

                    var uniqueFileName = await FileHelper.SaveImageFromBytesAsync(
                        imageBytes, dto.ImageFile.FileName, dto.ProjectId, configuration);

                    newJob.CroppedFileName = uniqueFileName;
                    logger.LogInformation("Image saved in project folder {ProjectId} filename:{FileName}", uniqueFileName, dto.ProjectId);

                    var prompt = await promptService.GetPromptByIdAsync(dto.PromptId);
                  
                    logger.LogInformation("Prompt {PromptId} found.", dto.PromptId);

                    if (prompt == null)
                    {
                        throw new EntityNotFoundException("Prompt",$"Prompt with ID:{dto.PromptId}");
                    }

                    logger.LogInformation("Generative AI call...");
                    
                    var wktResult = await generativeAIService.GetWktFromImageAsync(
                        imageBytes,
                        dto.ImageFile.ContentType,
                        prompt.PromptText
                    );

                    logger.LogInformation("Generative AI returned successfully.");

                    var wktReader = new WKTReader();
                    var geometry = wktReader.Read(wktResult);

                    if (geometry is not Polygon)
                    {
                        throw new InvalidOperationException($"The LLM result is not a valid polygon geometry. Geometry:{geometry.GeometryType}");
                    }

                    newJob.Geom = geometry;
                    newJob.Status = JobStatus.Completed;
                    newJob.ModelUsed = configuration["Gemini:Model"];
                }
                catch (Exception ex) when
                (ex is InvalidOperationException || ex is EntityNotFoundException || ex is ArgumentNullException)
                {
                    logger.LogError(ex, "Job processing FAILED.");
                    newJob.Status = JobStatus.Failed;
                    newJob.ErrorMessage = ex.Message;
                }

                await unitOfWork.ConversionJobRepository.AddAsync(newJob);
                await unitOfWork.SaveAsync();

                logger.LogInformation("Job {JobId} saved with final status: {Status}", newJob.Id, newJob.Status);

                var responseDto = new ConversionJobReadOnlyDTO
                {
                    Id = newJob.Id,
                    OriginalFileName = newJob.OriginalFileName,
                    CroppedFileName = newJob.CroppedFileName,
                    ModelUsed = newJob.ModelUsed,
                    Status = newJob.Status,
                    ErrorMessage = newJob.ErrorMessage,
                    ProjectId = newJob.ProjectId,
                    PromptId = newJob.PromptId,
                    DeletedAt = newJob.DeletedAt,
                };

                if (newJob.Geom != null && newJob.Geom is NetTopologySuite.Geometries.Polygon polygon)
                {
                    responseDto.Coordinates = polygon.ExteriorRing.Coordinates
                        .Take(polygon.ExteriorRing.Coordinates.Length - 1)
                        .Select((coord, index) => new CoordinateDTO
                        {
                            Order = index + 1,
                            X = coord.X,
                            Y = coord.Y
                        })
                        .ToList();
                }

                return responseDto;
            }
            catch (EntityNotFoundException ex)
            {
                logger.LogError("Error in job creation. {Message}", ex.Message);
                throw;
            }
            catch (EntityNotAuthorizedException ex)
            {
                logger.LogError("Unauthorized job creation attempt by User {UserId}. {Message}", userId, ex.Message);
                throw;
            }
        }

        public async Task<ConversionJobReadOnlyDTO> UpdateConversionJobAsync(int id, ConversionJobUpdateDTO dto, int userId)
        {
            try
            {
                var job = await unitOfWork.ConversionJobRepository.GetAsync(id);

                if (job == null) throw new EntityNotFoundException("ConversionJob", $"Job with id {id} not found");

                var userExists = await unitOfWork.UserRepository.GetAsync(userId);
                if (userExists == null)
                {
                    throw new EntityNotFoundException("User", $"User with id :{userId} not found.");
                }

                if (userExists.Role == "Member")
                {
                    var assignedProjectIds = await unitOfWork.UserRepository.GetProjectIdsForUserAsync(userId);
                    if (!assignedProjectIds.Contains(job.ProjectId))
                    {
                        logger.LogWarning("User {UserId} (Member) tried to update job on unassigned Project {ProjectId}", userId, job.ProjectId);
                        throw new EntityNotAuthorizedException("Project", "You are not authorized to update jobs on this project.");
                    }
                }
                
                var coordinates = dto.Coordinates;
                if (coordinates == null || coordinates.Count < 3) throw new InvalidArgumentException("Coordinates", "A valid polygon geometry requires at least 3 points.");

                var newCoordinates = coordinates
                    .OrderBy(c => c.Order)
                    .Select(c => new Coordinate(c.X, c.Y))
                    .ToList();

                if (!newCoordinates.First().Equals2D(newCoordinates.Last()))
                {
                    newCoordinates.Add(newCoordinates.First());
                }

                var factory = new GeometryFactory(new PrecisionModel(), 2100);
                job.Geom = factory.CreatePolygon(newCoordinates.ToArray());

                await unitOfWork.SaveAsync();
                logger.LogInformation("Job {JobId} updated successfully from user with {UserId}", job.Id, userId);

                return new ConversionJobReadOnlyDTO
                {
                    Id = job.Id,
                    OriginalFileName = job.OriginalFileName,
                    CroppedFileName = job.CroppedFileName,
                    ModelUsed = job.ModelUsed,
                    Status = job.Status,
                    ErrorMessage = job.ErrorMessage,
                    Coordinates = coordinates,
                    ProjectId = job.ProjectId,
                    PromptId = job.PromptId,
                    DeletedAt = job.DeletedAt
                };
            }
            catch (EntityNotFoundException ex)
            {
                logger.LogError("Error updating job with jobid :{JobId}. {Message}", id, ex.Message);
                throw;
            }
            catch (EntityNotAuthorizedException ex)
            {
                logger.LogError("Unauthorized update attempt for Job {JobId} by User {UserId}.{Message}", id, userId, ex.Message);
                throw;
            }
            catch (InvalidArgumentException ex)
            {
                logger.LogError("Invalid polygon for job {JobId}. {Message}", id, ex.Message);
                throw;
            }
        }

        public async Task<bool> DeleteConversionJobAsync(int id, int userId)
        {
            try
            {
                var job = await unitOfWork.ConversionJobRepository.GetAsync(id);

                if (job == null)
                {
                    throw new EntityNotFoundException("Conversion Job", $"Job with ID: {id} not found");
                }

                var userExists = await unitOfWork.UserRepository.GetAsync(userId);
                if (userExists == null)
                {
                    throw new EntityNotFoundException("User", $"User with id :{userId} not found.");
                }

                if (userExists.Role == "Member")
                {
                    var assignedProjectIds = await unitOfWork.UserRepository.GetProjectIdsForUserAsync(userId);
                    if (!assignedProjectIds.Contains(job.ProjectId))
                    {
                        logger.LogWarning("User {UserId} (Member) tried to update job on unassigned Project {ProjectId}", userId, job.ProjectId);
                        throw new EntityNotAuthorizedException("Project", "You are not authorized to update jobs on this project.");
                    }
                }

                bool movedFile = FileHelper.MoveImageToDeleted(job.CroppedFileName, job.ProjectId, configuration);

                if (movedFile)
                {
                    logger.LogInformation("File moved to deleted for {Id} ", job.Id);
                }
                else
                {
                    logger.LogWarning("File not moved for {Id}", job.Id);
                }

                await unitOfWork.ConversionJobRepository.DeleteAsync(id);
                await unitOfWork.SaveAsync();
                logger.LogInformation("Job {JobId} deleted successfully from user with {UserId}", job.Id, userId);

                return true;
            }
            catch (EntityNotFoundException ex)
            {
                logger.LogError("Error deleting conversion job {JobId}.{Message}", id, ex.Message);
                throw;
            }
            catch (EntityNotAuthorizedException ex)
            {
                logger.LogError("Unauthorized delete attempt for Job {JobId} by User {UserId}.{Message}", id, userId, ex.Message);
                throw;
            }
        }

        public async Task<ConversionJobReadOnlyDTO> GetConversionJobByIdAsync(int id, int userId)
        {
            try
            {
                var job = await unitOfWork.ConversionJobRepository.GetAsync(id);

                if (job == null)
                {
                    throw new EntityNotFoundException("Conversion Job", $"Job with ID: {id} not found");
                }

                var userExists = await unitOfWork.UserRepository.GetAsync(userId);
                if (userExists == null)
                {
                    throw new EntityNotFoundException("User", $"User with id :{userId} not found.");
                }

                if (userExists.Role == "Member")
                {
                    var assignedProjectIds = await unitOfWork.UserRepository.GetProjectIdsForUserAsync(userId);
                    if (!assignedProjectIds.Contains(job.ProjectId))
                    {
                        logger.LogWarning("User {UserId} (Member) tried to retrieve job on unassigned Project {ProjectId}", userId, job.ProjectId);
                        throw new EntityNotAuthorizedException("Project", "You are not authorized to retrieve jobs on this project.");
                    }
                }

                logger.LogInformation("Job {JobId} retrieved successfully by User {UserId}", id, userId);

                var dto = new ConversionJobReadOnlyDTO
                {
                    Id = job.Id,
                    OriginalFileName = job.OriginalFileName,
                    CroppedFileName = job.CroppedFileName,
                    ModelUsed = job.ModelUsed,
                    Status = job.Status,
                    ErrorMessage = job.ErrorMessage,
                    ProjectId = job.ProjectId,
                    PromptId = job.PromptId,
                    DeletedAt = job.DeletedAt
                };

                if (job.Geom != null && job.Geom is NetTopologySuite.Geometries.Polygon polygon)
                {
                    dto.Coordinates = polygon.ExteriorRing.Coordinates
                        .Take(polygon.ExteriorRing.Coordinates.Length - 1)
                        .Select((coord, index) => new CoordinateDTO
                        {
                            Order = index + 1,
                            X = coord.X,
                            Y = coord.Y
                        })
                        .ToList();
                }

                return dto;
            }
            catch (EntityNotFoundException ex)
            {
                logger.LogError("Error retrieving job {JobId}. {Message}", id, ex.Message);
                throw;
            }
            catch (EntityNotAuthorizedException ex)
            {
                logger.LogError("Unauthorized attempt to view Job {JobId} by User {UserId}. {Message}", id, userId, ex.Message);
                throw;
            }
        }
    }
}