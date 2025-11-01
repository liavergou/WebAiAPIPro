using CoordExtractorApp.Core.Filters;
using CoordExtractorApp.Data;
using CoordExtractorApp.DTO;
using CoordExtractorApp.Models;

namespace CoordExtractorApp.Services
{
    public interface IUserService
    {

        Task<User?> VerifyAndGetUserAsync(UserLoginDTO credentials);

        Task<UserReadOnlyDTO?> GetUserByUsernameAsync(string username);

        Task<PaginatedResult<UserReadOnlyDTO>> GetPaginatedUsersFilteredAsync(int pageNumber, int pageSize,
            UserFiltersDTO userFiltersDTO);

        Task<User?> GetUserByIdAsync(int id);

        Task CreateUserAsync (UserCreateDTO request);

        //Task UpdateUserAsync(UserUpdateDTO request);



    }
}
