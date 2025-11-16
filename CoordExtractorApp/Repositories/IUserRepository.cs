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
    }
}
