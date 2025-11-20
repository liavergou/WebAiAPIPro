using CoordExtractorApp.DTO;
using CoordExtractorApp.Models;

namespace CoordExtractorApp.Services
{
    public interface IPromptService
    {
        //read
        Task<PromptReadOnlyDTO?> GetPromptByIdAsync(int id);
        
        Task<PromptReadOnlyDTO?> GetPromptByPromptNameAsync(string promptName);

        Task<PaginatedResult<PromptReadOnlyDTO>> GetPaginatedPromptsAsync(int page, int pageSize);

        Task<List<PromptReadOnlyDTO>> GetAllPromtsAsync();

        //create
        Task<PromptReadOnlyDTO>CreatePromptAsync(PromptCreateDTO promptCreateDTO);

        //update
        Task<bool> UpdatePromptAsync(int id, PromptUpdateDTO promptupdatedto);

        //delete
        Task<bool> DeletePromptAsync(int id);
    }
}
