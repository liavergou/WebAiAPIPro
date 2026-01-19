using CoordExtractorApp.DTO.Keycloak;

namespace CoordExtractorApp.Services.Keycloak
{
    /// <summary>
    /// Service interface for obtaining Keycloak admin access tokens.
    /// </summary>
    public interface IKeycloakAdminTokenService
    {
        /// <summary>
        /// Retrieves an access token for the Keycloak Admin API using Client Credentials Flow.
        /// </summary>
        /// <returns>The access token string, or null if retrieval fails.</returns>
        Task<string?> GetAdminAccessTokenAsync();
    }
}
