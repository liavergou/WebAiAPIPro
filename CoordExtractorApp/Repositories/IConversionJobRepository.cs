using CoordExtractorApp.Data;
using CoordExtractorApp.Models;
using System.Linq.Expressions;

namespace CoordExtractorApp.Repositories
{
    public interface IConversionJobRepository
    {
        
        Task<List<ConversionJob>> GetJobsByUserIdAsync(int userId);

        Task<List<ConversionJob>> GetJobsByProjectIdAsync(int projectId);

        Task<List<ConversionJob>> GetJobsByPromptIdAsync(int projectId);

        Task<PaginatedResult<ConversionJob>>GetPaginatedJobsAsync(int pageNumber, int pageSize,
            List<Expression<Func<ConversionJob, bool>>> predicates);
    }
}
