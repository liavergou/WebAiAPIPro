using CoordExtractorApp.Data;
using CoordExtractorApp.Models;
using System.Linq.Expressions;

namespace CoordExtractorApp.Repositories
{
    /// <summary>
    /// Repository interface for Project management.
    /// </summary>
    public interface IProjectRepository
    {
        /// <summary>
        /// Retrieves a project by its name.
        /// </summary>
        Task<Project?> GetProjectByProjectNameAsync(string projectName);

        /// <summary>
        /// Retrieves a paginated list of projects based on optional filtering predicates.
        /// </summary>
        Task<PaginatedResult<Project>> GetPaginatedProjectsAsync(int pageNumber, int pageSize,
            List<Expression<Func<Project, bool>>> predicates);

        /// <summary>
        /// Retrieves a list of projects corresponding to the provided list of IDs.
        /// Primarily used to fetch full project details (like name and description) for a given list of IDs.
        /// Only projects that exist in the database are returned (invalid IDs are ignored).
        /// </summary>
        /// <param name="ids">The list of project IDs to search for.</param>
        /// <returns>A list of found projects.</returns>
        Task<List<Project>> GetProjectsByIdsAsync(List<int> ids); //απο τη λιστα των id των projects να παρω τα objects
    }
}
