using CoordExtractorApp.Data;
using CoordExtractorApp.Models;
using System.Linq.Expressions;

namespace CoordExtractorApp.Repositories
{
    public interface IProjectRepository
    {
        Task<Project?> GetProjectByProjectNameAsync(string projectName);
        Task<PaginatedResult<Project>> GetProjectsAsync(int pageNumber, int pageSize,
            List<Expression<Func<Project, bool>>> predicates);
    }
}
