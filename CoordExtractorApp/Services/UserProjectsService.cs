using AutoMapper;
using CoordExtractorApp.Data;
using CoordExtractorApp.DTO;
using CoordExtractorApp.Exceptions;
using CoordExtractorApp.Repositories;
using Serilog;

namespace CoordExtractorApp.Services
{
    public class UserProjectsService : IUserProjectsService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly ILogger<UserProjectsService> logger =
            new LoggerFactory().AddSerilog().CreateLogger<UserProjectsService>();

        public UserProjectsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        /// <summary>
        /// Retrieves the list of project IDs assigned to a specific user.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <returns>A DTO containing the list of assigned project IDs.</returns>
        /// <exception cref="EntityNotFoundException">Thrown if the user is not found.</exception>
        public async Task<UserProjectsDTO>GetUserProjectsAsync(int id)
        {
            User? user = null;
            try
            {
                user = await unitOfWork.UserRepository.GetAsync(id);
                if (user == null)
                {
                    throw new EntityNotFoundException("User", $"User with id: {id} not found");
                }

                var assignedProjectIds = await unitOfWork.UserRepository.GetProjectIdsForUserAsync(id);
                var dto = new UserProjectsDTO
                {
                    ProjectIds = assignedProjectIds
                };

                logger.LogInformation("Retrieved {Count} projects for user {id}", id,assignedProjectIds.Count);

                return  dto;
            }
            catch (EntityNotFoundException ex)
            {
                logger.LogError("Error retrieving user by ID: {Id}. {Message}", id, ex.Message);
                throw;
            }        
        }

        /// <summary>
        /// Retrieves the list of Project objects assigned to a user (used for Project Cards).
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <returns>A list of read-only project DTOs.</returns>
        public async Task<List<ProjectReadOnlyDTO>> GetUserProjectsByUserIdAsync(int id)
        {
            var assignedProjectIds = await unitOfWork.UserRepository.GetProjectIdsForUserAsync(id);
            if (assignedProjectIds == null)
            {
                return [];
            }

            var projects = await unitOfWork.ProjectRepository.GetProjectsByIdsAsync(assignedProjectIds);

            var dto = mapper.Map<List<ProjectReadOnlyDTO>>(projects);

            logger.LogInformation("Retrieved {Count} projects for user {id}", id, assignedProjectIds.Count);

            return dto;
        }

        /// <summary>
        /// Updates the project assignments for a user.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <param name="dto">The DTO containing the new list of project IDs.</param>
        /// <returns>The updated UserProjectsDTO.</returns>
        /// <exception cref="EntityNotFoundException">Thrown if the user is not found.</exception>
        public async Task<UserProjectsDTO> UpdateUserProjectsAsync(int id, UserProjectsUpdateDTO dto)
        {
            try
            {
                var user = await unitOfWork.UserRepository.GetAsync(id);

                if (user == null)
                {
                    throw new EntityNotFoundException("User", $"User with id: {id} not found");
                }
                await unitOfWork.UserRepository.SetProjectsForUserAsync(id, dto.ProjectIds);

                await unitOfWork.SaveAsync();

                logger.LogInformation($"Update {dto.ProjectIds.Count} projects for user {id}.", id, dto.ProjectIds.Count);
                return await GetUserProjectsAsync(id); //επιστροφή των id των project. θα μπορουσα να κανω και list με project....
            }

            catch (EntityNotFoundException ex)
            {
                logger.LogError(
                    "Error updating projects for user {id}.{Message}", id, ex.Message);
                throw;

            }
        }
    }
}
