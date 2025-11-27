using AutoMapper;
using CoordExtractorApp.Data;
using CoordExtractorApp.DTO;
using CoordExtractorApp.Exceptions;
using CoordExtractorApp.Repositories;
using CoordExtractorApp.Services.Keycloak;
using Microsoft.Identity.Client;
using Serilog;

namespace CoordExtractorApp.Services
{
    public class UserProjectsService : IUserProjectsService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<UserProjectsService> logger =
            new LoggerFactory().AddSerilog().CreateLogger<UserProjectsService>();

        public UserProjectsService(IUnitOfWork unitOfWork,ILogger<UserProjectsService> logger)
        {
            this.unitOfWork = unitOfWork;
        }

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
