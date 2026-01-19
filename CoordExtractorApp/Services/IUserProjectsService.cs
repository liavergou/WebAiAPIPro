using CoordExtractorApp.DTO;

namespace CoordExtractorApp.Services
{
    /// <summary>
    /// Service interface for managing user-project assignments.
    /// Currently used to restrict Member role access to specific projects.
    /// Admin and Manager roles access all projects via Project endpoints.
    /// The service remains open for all roles for potential future extensibility.
    /// </summary>
    public interface IUserProjectsService
    {
        /// <summary>
        /// Retrieves the list of project IDs assigned to a specific user.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <returns>A DTO containing the list of assigned project IDs.</returns>
        /// <exception cref="EntityNotFoundException">Thrown if the user is not found.</exception>
        Task<UserProjectsDTO> GetUserProjectsAsync(int id);

        /// <summary>
        /// Updates the project assignments for a user.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <param name="dto">The DTO containing the new list of project IDs.</param>
        /// <returns>The updated UserProjectsDTO.</returns>
        /// <exception cref="EntityNotFoundException">Thrown if the user is not found.</exception>
        Task<UserProjectsDTO> UpdateUserProjectsAsync(int id, UserProjectsUpdateDTO dto);

        /// <summary>
        /// Retrieves the list of Project objects assigned to a user.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <returns>A list of read-only project DTOs.</returns>
        Task<List<ProjectReadOnlyDTO>> GetUserProjectsByUserIdAsync(int id);
    }
}