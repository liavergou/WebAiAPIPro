using CoordExtractorApp.Data;
using CoordExtractorApp.Models;
using System.Linq.Expressions;

namespace CoordExtractorApp.Repositories
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User?> GetUserByKeycloakIdAsync(string keycloakId);

        Task<PaginatedResult<User>> GetUsersAsync(int pageNumber, int pageSize, 
            List<Expression<Func<User, bool>>> predicates);

        Task<User?> GetUserByIdAsync(int id);
        Task <List<int>>GetProjectIdsForUserAsync(int id);
        Task SetProjectsForUserAsync(int id, List<int> projectIds);
    }
}
