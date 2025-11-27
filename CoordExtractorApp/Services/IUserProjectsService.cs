using CoordExtractorApp.DTO;

namespace CoordExtractorApp.Services
{
    public interface IUserProjectsService
    {
        Task<UserProjectsDTO> GetUserProjectsAsync(int id); //να επιστρέψει τη λίστα με τα ids των project απο τη βάση

        Task <UserProjectsDTO> UpdateUserProjectsAsync(int id, UserProjectsUpdateDTO dto);
    }
}
