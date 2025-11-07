using CoordExtractorApp.Data;
using CoordExtractorApp.Models;
using System.Linq.Expressions;

namespace CoordExtractorApp.Repositories
{
    public interface IUserRepository
    {
        Task<int?> GetUserIdByKeycloakIdAsync(string keykloakId);

        Task<User?> GetUserByUsernameAsync(string username);

        //με φίλτρα. παίρνει user δίνει bool
        Task<PaginatedResult<User>> GetUsersAsync(int pageNumber, int pageSize,
           List<Expression<Func<User, bool>>> predicates);





    }
}
