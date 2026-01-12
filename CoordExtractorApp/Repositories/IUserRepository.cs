using CoordExtractorApp.Data;
using CoordExtractorApp.Models;
using System.Linq.Expressions;

namespace CoordExtractorApp.Repositories
{
    /// <summary>
    /// Repository interface for User management.
    /// Extends <see cref="IBaseRepository{User}"/> with user-specific operations.
    /// </summary>
    public interface IUserRepository : IBaseRepository<User>
    {
        /// <summary>
        /// Retrieves a user by their username.
        /// </summary>
        Task<User?> GetUserByUsernameAsync(string username);

        /// <summary>
        /// Retrieves a user by their Keycloak id (Subject ID).
        /// </summary>
        Task<User?> GetUserByKeycloakIdAsync(string keycloakId);

        /// <summary>
        /// Retrieves a paginated list of users based on optional filtering predicates.
        /// </summary>
        /// <param name="pageNumber">The page number</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <param name="predicates">A list of filter expressions.</param>
        /// <returns>A paginated result containing users.</returns>
        Task<PaginatedResult<User>> GetUsersAsync(int pageNumber, int pageSize, 
            List<Expression<Func<User, bool>>> predicates);

        /// <summary>
        /// Retrieves a user by database id.
        /// </summary>
        Task<User?> GetUserByIdAsync(int id);

        /// <summary>
        /// Retrieves the list of project IDs assigned to a user.
        /// </summary>
        Task<List<int>>GetProjectIdsForUserAsync(int id);

        // <summary>
        /// Updates the list of projects assigned to a user (full replacement).
        /// </summary>
        Task SetProjectsForUserAsync(int id, List<int> projectIds);
    }
}
