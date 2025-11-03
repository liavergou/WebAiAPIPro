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
                return mapper.Map<PromptReadOnlyDTO>(prompt);

            }
            catch (EntityNotFoundException ex)
            {
                logger.LogError("Error retrieving prompt by ID: {Id}. {Message}", id, ex.Message);
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
                
                return mapper.Map<PromptReadOnlyDTO>(prompt);

            }
            catch (EntityNotFoundException ex)
            {
                logger.LogError("Error retrieving prompt by PromptName: {PromptName}. {Message}", promptName, ex.Message);
                throw;
            }
        }

        //χωρις φίλτρο
        public async Task<PaginatedResult<PromptReadOnlyDTO>> GetPaginatedPromptsAsync(int pageNumber, int pageSize)
        {
            try
            {   //καλώ repository χωρις λιστα πλεον
                var prompts = await unitOfWork.PromptRepository.GetPaginatedPromptsAsync(pageNumber, pageSize);
                //Data = mapper.Map<List<PromptReadOnlyDTO>>(prompts.Data),
                var result = new PaginatedResult<PromptReadOnlyDTO>
                {

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

        public Task CreatePromptAsync(PromptCreateDTO request)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdatePromptAsync(int id, PromptUpdateDTO promptupdatedto)
        {
            try
            {
                var prompt = await unitOfWork.PromptRepository.GetAsync(id);
                if (prompt == null)
                {
                    throw new EntityNotFoundException("Prompt", $"Prompt with id: {id} not found");
                }
                //partial update
                if (promptupdatedto.PromptName != null) prompt.PromptName = promptupdatedto.PromptName;
                if (promptupdatedto.PromptText != null) prompt.PromptText = promptupdatedto.PromptText;

                await unitOfWork.SaveAsync();
                logger.LogInformation("Prompt with {id} updated succesfully", id);
                return true;
            } catch (EntityNotFoundException ex) {
                logger.LogError("Error updating prompt {id}. {Message}", id, ex.Message);
                throw;
            }
        }
        public async Task<bool> DeletePromptAsync(int id)
        {
            try
            {
                bool deletedPrompt = await unitOfWork.PromptRepository.DeleteAsync(id);
                if (!deletedPrompt)
                {
                    throw new EntityNotFoundException("Prompt", $"Prompt with id: {id} not found");
                }
                await unitOfWork.SaveAsync();
                logger.LogInformation("Prompt with {id} deleted successfully", id);
                return true;
            
            }catch (EntityNotFoundException ex)
            {
                logger.LogError("Error deleting prompt with {id}. {Message}", id, ex.Message);
                throw;
            }
        }        
             
       
    }
   

}
