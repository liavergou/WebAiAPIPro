using CoordExtractorApp.DTO;
using CoordExtractorApp.Models;

namespace CoordExtractorApp.Services
{
    public interface IPromptService
    {

        Task<PromptReadOnlyDTO?> GetPromptByIdAsync(int id);
        
        Task<PromptReadOnlyDTO?> GetPromptByPromptNameAsync(string promptName);

        Task<PaginatedResult<PromptReadOnlyDTO>> GetPaginatedPromptsAsync(int page, int pageSize);

        Task CreatePromptAsync(PromptCreateDTO request);

        Task<bool> UpdatePromptAsync(int id, PromptUpdateDTO promptupdatedto);

        Task<bool> DeletePromptAsync(int id);
    }
}
