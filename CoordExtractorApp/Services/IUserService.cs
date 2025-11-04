using CoordExtractorApp.Core.Filters;
using CoordExtractorApp.Data;
using CoordExtractorApp.DTO;
using CoordExtractorApp.Models;

namespace CoordExtractorApp.Services
{
    public interface IUserService
    {

        //Task<User?> VerifyAndGetUserAsync(UserLoginDTO credentials);

        Task<User?> GetUserByIdAsync(int id);
        Task<UserReadOnlyDTO?> GetUserByUsernameAsync(string username);

        Task<PaginatedResult<UserReadOnlyDTO>> GetPaginatedUsersFilteredAsync(int pageNumber, int pageSize,
            UserFiltersDTO userFiltersDTO);        

        Task CreateUserAsync (UserCreateDTO request);

        Task<bool> UpdateUserAsync(int id, UserUpdateDTO userupdatedto);

        Task<bool> DeleteUserAsync(int id);





    }
}
