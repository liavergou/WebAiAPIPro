using CoordExtractorApp.DTO.Keycloak;

namespace CoordExtractorApp.Services.Keycloak
{
    /// <summary>
    /// Service interface for Keycloak user administration via the Admin REST API.
    /// </summary>
    public interface IKeycloakAdminService
    {
        /// <summary>
        /// Creates a new user in Keycloak.
        /// </summary>
        /// <param name="keycloakUser">The DTO containing user details.</param>
        /// <returns>The ID of the created user, or null if creation failed.</returns>
        Task<string?> CreateUserAsync(KeycloakUserDTO keycloakUser);

        /// <summary>
        /// Assigns a role to a user in Keycloak.
        /// </summary>
        /// <param name="userId">The Keycloak user ID.</param>
        /// <param name="roleName">The name of the role to assign.</param>
        /// <returns>True if the role was assigned successfully.</returns>
        Task<bool> AssignUserRoleToUserAsync(string userId, string roleName);

        /// <summary>
        /// Updates a user's details (e.g., name, email) in Keycloak.
        /// </summary>
        /// <param name="keycloakId">The Keycloak user ID.</param>
        /// <param name="userUpdateDto">The DTO containing updated details.</param>
        /// <returns>True if the update was successful.</returns>
        Task<bool> UpdateUserDetailsAsync(string keycloakId, DTO.UserUpdateDTO userUpdateDto);

        /// <summary>
        /// Updates a user's role in Keycloak by removing existing roles and assigning the new one.
        /// </summary>
        /// <param name="keycloakId">The Keycloak user ID.</param>
        /// <param name="newRoleName">The new role name.</param>
        /// <returns>True if the update was successful.</returns>
        Task<bool> UpdateUserRoleAsync(string keycloakId, string newRoleName);

        /// <summary>
        /// Deletes a user from Keycloak.
        /// </summary>
        /// <param name="keycloakId">The Keycloak user ID.</param>
        /// <returns>True if deletion was successful.</returns>
        Task<bool> DeleteUserAsync(string keycloakId);
    }
}
