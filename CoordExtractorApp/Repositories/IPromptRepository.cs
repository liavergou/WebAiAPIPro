using CoordExtractorApp.Data;
using CoordExtractorApp.Models;
using System.Linq.Expressions;

namespace CoordExtractorApp.Repositories
{
    public interface IPromptRepository
    {
        Task<Prompt?>GetPromptByPromptNameAsync(string promptName);
        Task<PaginatedResult<Prompt>> GetPromptsAsync(int pageNumber, int pageSize,
            List<Expression<Func<Prompt, bool>>> predicates);
    }
}
