using System.Text.Json;

namespace CoordExtractorApp.Services.Keycloak
{
    /// <summary>
    /// Implementation of Keycloak admin token service.
    /// </summary>
    public class KeycloakAdminTokenService : IKeycloakAdminTokenService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IConfiguration configuration;
        private readonly ILogger<KeycloakAdminTokenService> logger;

        public KeycloakAdminTokenService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<KeycloakAdminTokenService> logger)
        {
            this.httpClientFactory = httpClientFactory;
            this.configuration = configuration;
            this.logger = logger;
        }

        public async Task<string?> GetAdminAccessTokenAsync()
        {
            var tokenEndpoint = configuration["Keycloak:AdminApi:TokenUrl"];
            var clientId = configuration["Keycloak:AdminApi:ClientId"];
            var clientSecret = configuration["Keycloak:AdminApi:ClientSecret"];

            if (string.IsNullOrEmpty(tokenEndpoint) || string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            {
                this.logger.LogError("Keycloak configuration is null");
                return null;
            }

            var requestBody = new Dictionary<string, string>
            {
                {"grant_type", "client_credentials" },
                {"client_id", clientId},
                {"client_secret", clientSecret}
            };

            var content = new FormUrlEncodedContent(requestBody);

            var client = this.httpClientFactory.CreateClient();
            var response = await client.PostAsync(tokenEndpoint, content);

            if (!response.IsSuccessStatusCode) {
                var errorContent = await response.Content.ReadAsStringAsync();
                this.logger.LogError("Failed to take an admin token.");
                return null;
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(responseContent);

            if (jsonDoc.RootElement.TryGetProperty("access_token", out var accessTokenElement))
            {
                return accessTokenElement.GetString();
            }

            this.logger.LogError("Access token was not found in the response");

            return null;
        }
    }
}