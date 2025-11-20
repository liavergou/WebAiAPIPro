using AutoMapper;
using CoordExtractorApp.Core.Filters;
using CoordExtractorApp.Data;
using CoordExtractorApp.DTO;
using CoordExtractorApp.Exceptions;
using CoordExtractorApp.Models;
using CoordExtractorApp.Repositories;
using Serilog;
using System.Linq.Expressions;

namespace CoordExtractorApp.Services
{
    public class PromptService : IPromptService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly ILogger<PromptService> logger =
            new LoggerFactory().AddSerilog().CreateLogger<PromptService>();


        public PromptService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<PromptReadOnlyDTO?> GetPromptByIdAsync(int id)
        {
            Prompt? prompt = null;

            try
            {
                prompt = await unitOfWork.PromptRepository.GetAsync(id);

                if (prompt == null)
                {
                    throw new EntityNotFoundException("Prompt", "Prompt with id: " + id + " not found.");
                }
                logger.LogInformation("Prompt found with ID: {Id}", id);
                var dto = mapper.Map<PromptReadOnlyDTO>(prompt);

                return dto;

            }
            catch (EntityNotFoundException ex)
            {
                logger.LogError("Error retrieving prompt with ID: {Id}. {Message}", id, ex.Message);
                throw;

            }

        }

        public async Task<PromptReadOnlyDTO?> GetPromptByPromptNameAsync(string promptName)
        {
            try
            {
                Prompt? prompt = await unitOfWork.PromptRepository.GetPromptByPromptNameAsync(promptName);
                if (prompt == null)
                {
                    throw new EntityNotFoundException("Prompt", "Prompt with name: " + promptName + " not found.");
                }
                logger.LogInformation("Prompt found:{PromptName}", promptName);

                var dto = mapper.Map<PromptReadOnlyDTO>(prompt);
                return dto;

            }
            catch (EntityNotFoundException ex)
            {
                logger.LogError("Error retrieving prompt by PromptName: {PromptName}. {Message}", promptName, ex.Message);
                throw;
            }
        }

        public async Task<List<PromptReadOnlyDTO>> GetAllPromtsAsync()
        {
            var prompts = await unitOfWork.PromptRepository.GetAllAsync();
            var dto = mapper.Map<List<PromptReadOnlyDTO>>(prompts)
                .OrderBy(p => p.Id)
                .ToList();
            logger.LogInformation("Retrieved all prompts. Count:{Count}", dto.Count);
            return dto;
        }

        //χωρις φίλτρο
        public async Task<PaginatedResult<PromptReadOnlyDTO>> GetPaginatedPromptsAsync(int pageNumber, int pageSize)
        {
            try
            {   //καλώ repository χωρις λιστα πλεον
                var prompts = await unitOfWork.PromptRepository.GetPaginatedPromptsAsync(pageNumber, pageSize);

                var result = new PaginatedResult<PromptReadOnlyDTO>
                {
                    //custom για εξασκηση
                    Data = prompts.Data.Select(p => new PromptReadOnlyDTO
                    {
                        Id = p.Id,
                        PromptName = p.PromptName,
                        PromptText = p.PromptText
                    }).ToList(),

                    TotalRecords = prompts.TotalRecords,
                    PageNumber = prompts.PageNumber,
                    PageSize = prompts.PageSize
                };
                return result;
            }
            catch (EntityNotFoundException)
            {
                logger.LogError("Error retrieving paginated prompts");
                throw;
            }
        }

        public async Task<PromptReadOnlyDTO> CreatePromptAsync(PromptCreateDTO promptCreateDTO)
        {
            try
            {
                var existingPrompt = await unitOfWork.PromptRepository.GetPromptByPromptNameAsync(promptCreateDTO.PromptName);
                if (existingPrompt != null)
                {
                    throw new EntityAlreadyExistsException("Prompt", $"Prompt with name {promptCreateDTO.PromptName} already exists");
                }

                //dto -> entity
                var prompt = mapper.Map<Prompt>(promptCreateDTO);


                await unitOfWork.PromptRepository.AddAsync(prompt);

                await unitOfWork.SaveAsync(); //commit

                //entity->dto
                var dto = mapper.Map<PromptReadOnlyDTO>(prompt);
                logger.LogInformation("Prompt with ID {id} created successfully", dto.Id);
                return dto;
            }
            catch (AppException ex)
            {
                logger.LogError("Error creating prompt.Code {Code}, Message: {Message}", ex.Code, ex.Message);
                throw;
            }
        }



        public async Task<bool> UpdatePromptAsync(int id, PromptUpdateDTO promptUpdateDTO)
        {
            try
            {
                var prompt = await unitOfWork.PromptRepository.GetAsync(id);
                //αν δεν υπάρχει
                if (prompt == null)
                {
                    throw new EntityNotFoundException("Prompt", $"Prompt with id: {id} not found");
                }
                if (!string.IsNullOrEmpty(promptUpdateDTO.PromptName) && promptUpdateDTO.PromptName != prompt.PromptName)
                {
                    //promptName unique. και ελεγχος αν υπάρχει ήδη
                    var existingPrompt = await unitOfWork.PromptRepository.GetPromptByPromptNameAsync(promptUpdateDTO.PromptName!);

                if (existingPrompt != null && existingPrompt.Id != id)
                    {
                        throw new EntityAlreadyExistsException("Prompt", $"Prompt with name {promptUpdateDTO.PromptName} already exists.");
                    }
                    //update PromptName
                    prompt.PromptName = promptUpdateDTO.PromptName;
                   
                }
                //update PromptText
                if (promptUpdateDTO.PromptText != null)
                    prompt.PromptText = promptUpdateDTO.PromptText;

                await unitOfWork.SaveAsync();

                logger.LogInformation("Prompt with {id} updated successfully", id);
                return true;

            }
            catch (EntityNotFoundException ex)
            {
                logger.LogError("Error retrieving prompt with id {id}. {Message}", id, ex.Message);
                throw;
            }
            catch (EntityAlreadyExistsException ex)
            {
                logger.LogError("Error updating prompt with id {id}. {Message}", id, ex.Message);
                throw;
            }



        }

        public async Task<bool> DeletePromptAsync(int id)
        {
            try
            {
                var prompt = await unitOfWork.PromptRepository.GetAsync(id);
                if (prompt == null)
                {
                    throw new EntityNotFoundException("Prompt", $"Prompt with id {id} not found");
                }

                //αν υπάρχουν conversion jobs συνδεδεμένα -> warning για cascade soft delete
                var jobs = await unitOfWork.ConversionJobRepository.GetJobsByPromptIdAsync(id);

                if (jobs.Count>0)
                {
                    logger.LogWarning("Prompt with {id} and {PromptText} has {JobCount} jobs assigned. Cascading soft delete", id, prompt.PromptText, jobs.Count);
                }
                //iteration στα jobs
                foreach (var job in jobs)
                {
                    await unitOfWork.ConversionJobRepository.DeleteAsync(job.Id);
                }
                await unitOfWork.PromptRepository.DeleteAsync(id);

                await unitOfWork.SaveAsync();

                logger.LogInformation("Prompt with {id} deleted successfully. Deleted {jobCount} jobs", id, jobs.Count);

                return true;

            }
            catch (EntityNotFoundException ex)
            {
                logger.LogError("Error deleting prompt with {id}. {Message}", id, ex.Message);
                throw;
            }
        }
    }

    }