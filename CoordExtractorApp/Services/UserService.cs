using AutoMapper;
﻿using CoordExtractorApp.Core.Filters;
﻿using CoordExtractorApp.Data;
﻿using CoordExtractorApp.DTO;
﻿using CoordExtractorApp.Exceptions;
﻿using CoordExtractorApp.Models;
﻿using CoordExtractorApp.Repositories;
using CoordExtractorApp.Services.Keycloak;
using Serilog;
﻿using System.Linq.Expressions;
﻿using System.Security.Claims;
﻿
﻿namespace CoordExtractorApp.Services
﻿{
﻿    public class UserService : IUserService
﻿    {
﻿        private readonly IUnitOfWork unitOfWork;
﻿        private readonly IMapper mapper;
﻿        private readonly IKeycloakAdminService keycloakAdminService;
        private readonly ILogger<UserService> logger =
            new LoggerFactory().AddSerilog().CreateLogger<UserService>();

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, IKeycloakAdminService keycloakAdminService)
﻿        {
﻿            this.unitOfWork = unitOfWork;
﻿            this.mapper = mapper;
﻿            this.keycloakAdminService = keycloakAdminService;
﻿        }
﻿
﻿        public async Task<User?> GetUserByIdAsync(int id)
﻿        {
﻿            User? user = null;
﻿            try
﻿            {
﻿                user = await unitOfWork.UserRepository.GetAsync(id);
                if (user == null)
                {
                    throw new EntityNotFoundException("User", "User with id: {id} not found");
                }
                logger.LogInformation("User found with: {id}", id);
                return user;

            }catch (EntityNotFoundException ex)
﻿            {
﻿                logger.LogError("Error retrieving user by ID: {Id}. {Message}", id, ex.Message);
                throw;
﻿            }
﻿        }
﻿
﻿        public async Task<UserReadOnlyDTO?> GetUserByUsernameAsync(string username)
﻿        {
﻿            try
﻿            {
﻿                User? user = await unitOfWork.UserRepository.GetUserByUsernameAsync(username);
﻿                if (user == null)
﻿                {
﻿                    throw new EntityNotFoundException("User", "User with username: " + " not found");
﻿                }
﻿                logger.LogInformation("User found: {Username}", username);
﻿                return mapper.Map<UserReadOnlyDTO>(user);
﻿            }
﻿            catch (EntityNotFoundException ex)
﻿            {
﻿                logger.LogError("Error retrieving user by username: {Username}. {Message}", username, ex.Message);
﻿                throw;
﻿            }
﻿        }
﻿
﻿        public async Task<PaginatedResult<UserReadOnlyDTO>> GetPaginatedUsersFilteredAsync(int pageNumber, int pageSize, 
             UserFiltersDTO userFiltersDTO)
﻿        {
﻿            List<Expression<Func<User, bool>>> predicates = [];
﻿            if (!string.IsNullOrEmpty(userFiltersDTO.Username))
﻿            {
﻿                predicates.Add(u => u.Username == userFiltersDTO.Username);
﻿            }

            if (!string.IsNullOrEmpty(userFiltersDTO.Role))
            {
                predicates.Add(u => u.Role == userFiltersDTO.Role);
            }


            var result = await unitOfWork.UserRepository.GetUsersAsync(pageNumber, pageSize, predicates);
﻿            var dtoResult = new PaginatedResult<UserReadOnlyDTO>()
﻿            {
﻿                Data = mapper.Map<List<UserReadOnlyDTO>>(result.Data),
﻿                TotalRecords = result.TotalRecords,
﻿                PageNumber = result.PageNumber,
﻿                PageSize = result.PageSize
﻿            };
﻿            logger.LogInformation("Retrieved {Count} users", dtoResult.Data.Count);
﻿            return dtoResult;
﻿        }
﻿
﻿        public async Task<User> CreateUserAsync(User user)
﻿        {
﻿            try
﻿            {
﻿                User? existingUser = await unitOfWork.UserRepository.GetUserByUsernameAsync(user.Username);
﻿                if (existingUser != null)
﻿                {
﻿                    throw new EntityAlreadyExistsException("User", "User with username " + existingUser.Username + " already exists");
﻿                }
﻿                await unitOfWork.UserRepository.AddAsync(user);
﻿                await unitOfWork.SaveAsync();
﻿                logger.LogInformation("User {Username} signed up successfully.", user.Username);
﻿                return user;
﻿            }
﻿            catch (EntityAlreadyExistsException ex)
﻿            {
﻿                logger.LogError("Error creating user {Username}. {Message}", user.Username, ex.Message);
﻿                throw;
﻿            }
﻿        }
﻿
﻿        public async Task<bool> UpdateUserAsync(int id, UserUpdateDTO userUpdateDto)
﻿        {
﻿            try
﻿            {
﻿                var user = await unitOfWork.UserRepository.GetAsync(id);
﻿                if (user == null)
﻿                {
﻿                    throw new EntityNotFoundException("User", $"User with id: {id} not found");
﻿                }
﻿
﻿                await keycloakAdminService.UpdateUserDetailsAsync(user.KeycloakId, userUpdateDto);
﻿
﻿                if (!string.IsNullOrEmpty(userUpdateDto.Role))
﻿                {
﻿                    await keycloakAdminService.UpdateUserRoleAsync(user.KeycloakId, userUpdateDto.Role);
﻿                }

﻿                user.Email = userUpdateDto.Email ?? user.Email;
﻿                user.Firstname = userUpdateDto.Firstname ?? user.Firstname;
﻿                user.Lastname = userUpdateDto.Lastname ?? user.Lastname;
﻿                user.Role = userUpdateDto.Role ?? user.Role;
﻿
﻿                await unitOfWork.SaveAsync();
﻿                logger.LogInformation("User {Id} updated successfully.", id);
﻿                return true;
﻿            }
﻿            catch (EntityNotFoundException ex)
﻿            {
﻿                logger.LogError("Error updating user {Id}. {Message}", id, ex.Message);
﻿                throw;
﻿            }
﻿        }
﻿        
﻿        public async Task<bool> DeleteUserAsync(int id)
﻿        {
﻿            try
﻿            {
﻿                var user = await unitOfWork.UserRepository.GetAsync(id);
﻿                if (user == null)
﻿                {
﻿                    throw new EntityNotFoundException("User", $"User with id: {id} not found");
﻿                }
﻿
﻿                bool keycloakDeleteSuccess = await keycloakAdminService.DeleteUserAsync(user.KeycloakId);
﻿
﻿                if (!keycloakDeleteSuccess)
﻿                {
﻿                    logger.LogError("Failed to delete user {KeycloakId} from Keycloak. Aborting local delete.", user.KeycloakId);
﻿                    return false;
﻿                }
﻿
﻿                await unitOfWork.UserRepository.DeleteAsync(id);
﻿                await unitOfWork.SaveAsync();
﻿                
﻿                logger.LogInformation("User with local id {id} and Keycloak Id {keycloakId} deleted successfully.", id, user.KeycloakId);
﻿                return true;
﻿            }
﻿            catch (EntityNotFoundException ex)
﻿            {
﻿                logger.LogError("Error deleting user {id}. {Message}", id, ex.Message);
﻿                throw;
﻿            }
﻿        }
﻿        
﻿        public async Task<ApplicationUser> GetUserInfoAsync(ClaimsPrincipal user)
﻿        {
﻿            if (user == null || user.Identity == null || !user.Identity.IsAuthenticated)
﻿            {
﻿                logger.LogError("User is not authenticated or identity is missing.");
﻿                throw new EntityNotAuthorizedException("User", "User is not authenticated");
﻿            }
﻿
﻿            var keycloakId = user.FindFirst("sub")?.Value;
﻿            if (string.IsNullOrEmpty(keycloakId))
﻿            {
﻿                logger.LogError("KeycloakId not found in claims");
﻿                throw new EntityNotAuthorizedException("User", "Missing identifier key");
﻿            }
﻿
﻿            var userFromDb = await unitOfWork.UserRepository.GetUserByKeycloakIdAsync(keycloakId);
﻿            if (userFromDb == null)
﻿            {
﻿                logger.LogError("User with KeycloakId {KeycloakId} not found in local database.", keycloakId);
﻿                throw new EntityNotAuthorizedException("User", "User is not provisioned in the local system.");
﻿            }
﻿
﻿            var applicationUser = new ApplicationUser
﻿            {
﻿                Id = userFromDb.Id,
﻿                KeycloakId = keycloakId,
﻿                Username = user.FindFirst("preferred_username")?.Value,
﻿                Email = user.FindFirst("email")?.Value,
﻿                Lastname = user.FindFirst("family_name")?.Value,
﻿                Firstname = user.FindFirst("given_name")?.Value,
﻿                Role = userFromDb.Role
﻿            };
﻿            
﻿            return applicationUser;
﻿        }
﻿    }
﻿}
﻿
﻿