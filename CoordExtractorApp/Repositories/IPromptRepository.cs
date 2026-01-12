using CoordExtractorApp.Data;
using CoordExtractorApp.Models;
using System.Linq.Expressions;

namespace CoordExtractorApp.Repositories
{
    /// <summary>
    /// Repository interface for Prompt management.
    /// </summary>
    public interface IPromptRepository
    {
        /// <summary>
        /// Retrieves a prompt by its name.
        /// </summary>
        Task<Prompt?>GetPromptByPromptNameAsync(string promptName);

        /// <summary>
        /// Retrieves a paginated list of prompts based on optional filtering predicates.
        /// </summary>
        Task<PaginatedResult<Prompt>> GetPaginatedPromptsAsync(int pageNumber, int pageSize,
            List<Expression<Func<Prompt, bool>>> predicates);
    }
}
