using CoordExtractorApp.Core.Filters;
using CoordExtractorApp.Data;
using CoordExtractorApp.DTO;
using CoordExtractorApp.Models;

namespace CoordExtractorApp.Services
{
    public interface IProjectService
    {
        //read
        Task<ProjectDTO?> GetProjectByIdAsync(int id);
        Task<ProjectReadOnlyDTO?> GetProjectByProjectNameAsync(string projectName);
        Task<List<ProjectReadOnlyDTO>> GetAllProjectsAsync();
        Task<PaginatedResult<ProjectDTO>> GetPaginatedProjectsAsync(int pageNumber, int pageSize, ProjectFilterDTO projectFilterDTO);        

        //create
        Task<ProjectDTO> CreateProjectAsync(ProjectCreateDTO projectCreateDTO);

        //update
        Task<bool> UpdateProjectAsync(int id, ProjectUpdateDTO projectUpdateDTO);

        //delete
        Task<bool> DeleteProjectAsync(int id);


    }
}
