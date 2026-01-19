using AutoMapper;
using CoordExtractorApp.Data;
using CoordExtractorApp.DTO;
using CoordExtractorApp.Exceptions;
using CoordExtractorApp.Repositories;
using Serilog;

namespace CoordExtractorApp.Services
{
    /// <summary>
    /// Service implementation for managing user-project assignments.
    /// Currently used to restrict Member role access to specific projects.
    /// Admin and Manager roles access all projects via Project endpoints.
    /// The service remains open for all roles for potential future extensibility.
    /// </summary>
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

        public async Task<UserProjectsDTO> GetUserProjectsAsync(int id)
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

                logger.LogInformation("Retrieved {Count} projects for user {id}", assignedProjectIds.Count, id);

                return dto;
            }
            catch (EntityNotFoundException ex)
            {
                logger.LogError("Error retrieving user by ID: {Id}. {Message}", id, ex.Message);
                throw;
            }        
        }

        public async Task<List<ProjectReadOnlyDTO>> GetUserProjectsByUserIdAsync(int id)
        {
            var assignedProjectIds = await unitOfWork.UserRepository.GetProjectIdsForUserAsync(id);
            if (assignedProjectIds == null)
            {
                return [];
            }

            var projects = await unitOfWork.ProjectRepository.GetProjectsByIdsAsync(assignedProjectIds);

            var dto = mapper.Map<List<ProjectReadOnlyDTO>>(projects);

            logger.LogInformation("Retrieved {Count} projects for user {id}", assignedProjectIds.Count, id);

            return dto;
        }

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

                logger.LogInformation("Update {Count} projects for user {id}.", dto.ProjectIds.Count, id);
                return await GetUserProjectsAsync(id);
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