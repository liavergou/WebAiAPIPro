using CoordExtractorApp.DTO;

namespace CoordExtractorApp.Services
{
    /// <summary>
    /// Service interface for managing conversion jobs, including creation, updates, deletion, and retrieval.
    /// </summary>
    public interface IConversionJobService
    {
        /// <summary>
        /// Creates a new conversion job, processes the image using Generative AI to extract WKT geometry, and saves the result.
        /// </summary>
        /// <param name="dto">The DTO containing the image, project ID, and prompt ID.</param>
        /// <param name="userId">The ID of the user creating the job.</param>
        /// <returns>A read-only DTO with the job results, including the extracted coordinates.</returns>
        /// <exception cref="EntityNotFoundException">Thrown if the project, user, or prompt is not found.</exception>
        /// <exception cref="EntityNotAuthorizedException">Thrown if the user is a Member and tries to create a job in an unassigned project.</exception>
        /// <exception cref="ArgumentNullException">Thrown if a required argument is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the LLM result is not a valid polygon.</exception>
        Task<ConversionJobReadOnlyDTO> CreateAndProcessJobAsync(ConversionJobInsertDTO dto, int userId);

        /// <summary>
        /// Updates an existing conversion job, typically modifying its geometry (coordinates).
        /// </summary>
        /// <param name="id">The job ID.</param>
        /// <param name="dto">The DTO containing the updated coordinates.</param>
        /// <param name="userId">The ID of the user performing the update.</param>
        /// <returns>A read-only DTO with the updated job details.</returns>
        /// <exception cref="EntityNotFoundException">Thrown if the job or user is not found.</exception>
        /// <exception cref="EntityNotAuthorizedException">Thrown if the user is not authorized to modify the project's jobs.</exception>
        /// <exception cref="InvalidArgumentException">Thrown if the provided coordinates do not form a valid polygon.</exception>
        Task<ConversionJobReadOnlyDTO> UpdateConversionJobAsync(int id, ConversionJobUpdateDTO dto, int userId);

        /// <summary>
        /// Soft deletes a conversion job and moves its associated image file to a "deleted" directory.
        /// </summary>
        /// <param name="id">The job ID.</param>
        /// <param name="userId">The ID of the user performing the deletion.</param>
        /// <returns>True if the deletion was successful.</returns>
        /// <exception cref="EntityNotFoundException">Thrown if the job or user is not found.</exception>
        /// <exception cref="EntityNotAuthorizedException">Thrown if the user is not authorized to delete the job.</exception>
        Task<bool> DeleteConversionJobAsync(int id, int userId);

        /// <summary>
        /// Retrieves a conversion job by its id, including its geometry converted to coordinates.
        /// </summary>
        /// <param name="id">The job ID.</param>
        /// <param name="userId">The ID of the user requesting the job.</param>
        /// <returns>A read-only DTO containing the job details.</returns>
        /// <exception cref="EntityNotFoundException">Thrown if the job or user is not found.</exception>
        /// <exception cref="EntityNotAuthorizedException">Thrown if the user is not authorized to view the job.</exception>
        Task<ConversionJobReadOnlyDTO> GetConversionJobByIdAsync(int id, int userId);
    }
}