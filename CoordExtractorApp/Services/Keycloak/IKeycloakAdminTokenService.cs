using CoordExtractorApp.DTO.Keycloak;

namespace CoordExtractorApp.Services.Keycloak
{
    public interface IKeycloakAdminTokenService
    {
        Task<string?> GetAdminAccessTokenAsync();
    }
}
