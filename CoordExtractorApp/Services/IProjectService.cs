using CoordExtractorApp.Core.Filters;
using CoordExtractorApp.DTO;
using CoordExtractorApp.Models;

namespace CoordExtractorApp.Services
{
    /// <summary>
    /// Service interface for managing Projects.
    /// </summary>
    public interface IProjectService
    {
        /// <summary>
        /// Retrieves a project by its unique id
        /// </summary>
        /// <param name="id">The project ID.</param>
        /// <returns>A DTO containing project details.</returns>
        /// <exception cref="EntityNotFoundException">Thrown when the project does not exist.</exception>
        Task<ProjectDTO?> GetProjectByIdAsync(int id);

        /// <summary>
        /// Retrieves a project by its name.
        /// </summary>
        /// <param name="projectName">The name of the project.</param>
        /// <returns>A read-only DTO of the project.</returns>
        /// <exception cref="EntityNotFoundException">Thrown when the project does not exist.</exception>
        Task<ProjectReadOnlyDTO?> GetProjectByProjectNameAsync(string projectName);

        /// <summary>
        /// Retrieves all projects from the database.
        /// </summary>
        /// <returns>A list of all projects ordered by name.</returns>
        Task<List<ProjectReadOnlyDTO>> GetAllProjectsAsync();

        /// <summary>
        /// Retrieves a paginated and filtered list of projects.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <param name="projectFilterDTO">Filter criteria (e.g., project name).</param>
        /// <returns>A paginated result containing the projects.</returns>
        Task<PaginatedResult<ProjectDTO>> GetPaginatedProjectsAsync(int pageNumber, int pageSize, ProjectFilterDTO projectFilterDTO);

        /// <summary>
        /// Creates a new project in the database and initializes its storage folders.
        /// </summary>
        /// <param name="projectCreateDTO">Data for the new project.</param>
        /// <returns>The created project DTO.</returns>
        /// <exception cref="EntityAlreadyExistsException">Thrown if a project with the same name already exists.</exception>
        Task<ProjectDTO> CreateProjectAsync(ProjectCreateDTO projectCreateDTO);

        /// <summary>
        /// Updates an existing project.
        /// </summary>
        /// <param name="id">The project ID</param>
        /// <param name="projectUpdateDTO">The updated project data.</param>
        /// <returns>True if the update was successful.</returns>
        /// <exception cref="EntityNotFoundException">Thrown if the project is not found.</exception>
        /// <exception cref="EntityAlreadyExistsException">Thrown if the new name conflicts with another project.</exception>
        Task<bool> UpdateProjectAsync(int id, ProjectUpdateDTO projectUpdateDTO);

        /// <summary>
        /// Soft deletes a project and cascades the deletion to its conversion jobs.
        /// </summary>
        /// <param name="id">The project ID.</param>
        /// <returns>True if deletion was successful.</returns>
        /// <exception cref="EntityNotFoundException">Thrown if the project is not found.</exception>
        Task<bool> DeleteProjectAsync(int id);
    }
}
