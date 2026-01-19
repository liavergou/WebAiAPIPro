using CoordExtractorApp.Core.Filters;
using CoordExtractorApp.DTO;
using CoordExtractorApp.Models;
using System.Security.Claims;

namespace CoordExtractorApp.Services
{
    /// <summary>
    /// Service interface for managing users.
    /// User management operations are performed via Keycloak Admin API and mirrored to the local database.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Retrieves a user by their unique identifier from the local database.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <returns>A DTO containing user details.</returns>
        /// <exception cref="EntityNotFoundException">Thrown if the user is not found.</exception>
        Task<UserReadOnlyDTO?> GetUserByIdAsync(int id);

        /// <summary>
        /// Retrieves a user by their username.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>A DTO containing user details.</returns>
        /// <exception cref="EntityNotFoundException">Thrown if the user is not found.</exception>
        Task<UserReadOnlyDTO?> GetUserByUsernameAsync(string username);

        /// <summary>
        /// Retrieves all users from the system.
        /// </summary>
        /// <returns>A list of all users ordered by username.</returns>
        Task<List<UserReadOnlyDTO>> GetAllUsersAsync();

        /// <summary>
        /// Retrieves a paginated and filtered list of users.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="userFiltersDTO">Filters for username and role.</param>
        /// <returns>A paginated result containing users.</returns>
        Task<PaginatedResult<UserReadOnlyDTO>> GetPaginatedUsersFilteredAsync(int pageNumber, int pageSize, UserFiltersDTO userFiltersDTO);

        /// <summary>
        /// Creates a new user in Keycloak, assigns a role, and saves to the local database.
        /// </summary>
        /// <param name="userCreateDTO">The new user data.</param>
        /// <returns>A read-only DTO of the created user.</returns>
        /// <exception cref="InvalidArgumentException">Thrown if username or role is missing.</exception>
        /// <exception cref="EntityAlreadyExistsException">Thrown if the username already exists.</exception>
        /// <exception cref="KeycloakException">Thrown if Keycloak operations fail.</exception>
        Task<UserReadOnlyDTO> CreateUserWithKeycloakAsync(UserCreateDTO userCreateDTO);

        /// <summary>
        /// Updates an existing user's details and role in both Keycloak and the local database.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <param name="userupdatedto">The updated user data.</param>
        /// <returns>True if the update was successful.</returns>
        /// <exception cref="EntityNotFoundException">Thrown if the user is not found.</exception>
        Task<bool> UpdateUserAsync(int id, UserUpdateDTO userupdatedto);

        /// <summary>
        /// Deletes a user from Keycloak and the local database.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <returns>True if deletion was successful.</returns>
        /// <exception cref="EntityNotFoundException">Thrown if the user is not found.</exception>
        /// <exception cref="DeletionForbiddenException">Thrown if the user has associated conversion jobs.</exception>
        Task<bool> DeleteUserAsync(int id);

        /// <summary>
        /// Constructs the application user model by combining Keycloak claims with local database info.
        /// </summary>
        /// <param name="user">The claims principal of the authenticated user.</param>
        /// <returns>The application user model containing merged data.</returns>
        /// <exception cref="EntityNotAuthorizedException">Thrown if authentication fails or user is not found.</exception>
        Task<ApplicationUser> GetUserInfoAsync(ClaimsPrincipal user);
    }
}