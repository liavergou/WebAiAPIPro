using CoordExtractorApp.Core.Filters;
using CoordExtractorApp.DTO;
using CoordExtractorApp.Models;

namespace CoordExtractorApp.Services
{
    /// <summary>
    /// Service interface for managing Prompts.
    /// </summary>
    public interface IPromptService
    {
        /// <summary>
        /// Retrieves a Prompt by its unique id.
        /// </summary>
        /// <param name="id">The prompt ID.</param>
        /// <returns>A DTO containing the prompt details.</returns>
        /// <exception cref="EntityNotFoundException">Thrown if the prompt is not found.</exception>
        Task<PromptReadOnlyDTO?> GetPromptByIdAsync(int id);

        /// <summary>
        /// Retrieves a Prompt by its name.
        /// </summary>
        /// <param name="promptName">The name of the prompt.</param>
        /// <returns>A DTO containing the prompt details.</returns>
        /// <exception cref="EntityNotFoundException">Thrown if the prompt is not found.</exception>
        Task<PromptReadOnlyDTO?> GetPromptByPromptNameAsync(string promptName);

        /// <summary>
        /// Retrieves a paginated and filtered list of Prompts.
        /// </summary>
        /// <param name="page">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="promptFilterDTO">Filter criteria (e.g., prompt name).</param>
        /// <returns>A paginated result containing the prompts.</returns>
        Task<PaginatedResult<PromptReadOnlyDTO>> GetPaginatedPromptsAsync(int page, int pageSize, PromptFilterDTO promptFilterDTO);

        /// <summary>
        /// Retrieves all Prompts from the database.
        /// </summary>
        /// <returns>A list of all prompts ordered by ID.</returns>
        Task<List<PromptReadOnlyDTO>> GetAllPromtsAsync();

        /// <summary>
        /// Creates a new Prompt.
        /// </summary>
        /// <param name="promptCreateDTO">The data for the new prompt.</param>
        /// <returns>The created prompt DTO.</returns>
        /// <exception cref="EntityAlreadyExistsException">Thrown if a prompt with the same name already exists.</exception>
        Task<PromptReadOnlyDTO> CreatePromptAsync(PromptCreateDTO promptCreateDTO);

        /// <summary>
        /// Updates an existing Prompt.
        /// </summary>
        /// <param name="id">The prompt ID.</param>
        /// <param name="promptupdatedto">The updated prompt data.</param>
        /// <returns>True if the update was successful.</returns>
        /// <exception cref="EntityNotFoundException">Thrown if the prompt is not found.</exception>
        /// <exception cref="EntityAlreadyExistsException">Thrown if the new name conflicts with another prompt.</exception>
        Task<bool> UpdatePromptAsync(int id, PromptUpdateDTO promptupdatedto);

        /// <summary>
        /// Soft deletes a Prompt and cascades deletion to associated conversion jobs.
        /// </summary>
        /// <param name="id">The prompt ID.</param>
        /// <returns>True if deletion was successful.</returns>
        /// <exception cref="EntityNotFoundException">Thrown if the prompt is not found.</exception>
        Task<bool> DeletePromptAsync(int id);
    }
}