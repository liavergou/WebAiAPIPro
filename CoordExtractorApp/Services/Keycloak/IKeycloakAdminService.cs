using CoordExtractorApp.DTO.Keycloak;

namespace CoordExtractorApp.Services.Keycloak
{
    public interface IKeycloakAdminService
    {
        //με τα στοιχεία χρήστη για το Keycloak
        Task<string?> CreateUserAsync(KeycloakUserDTO keycloakUser);

        Task<bool> AssignUserRoleToUserAsync(string userId, string roleName);

        //update email, firstname, lastname
        Task<bool> UpdateUserDetailsAsync(string keycloakId, DTO.UserUpdateDTO userUpdateDto);

        Task<bool> UpdateUserRoleAsync(string keycloakId, string newRoleName);

        Task<bool> DeleteUserAsync(string keycloakId);
    }
}
