
using GenerativeAI.Core;
using Microsoft.Extensions.Caching.Memory;
using RTools_NTS.Util;
using System.Text.Json;

namespace CoordExtractorApp.Services.Keycloak
{
    //απο https://www.keycloak.org/securing-apps/oidc-layers Token endpoint /realms/{realm-name}/protocol/openid-connect/token
    //https://openid.net/specs/openid-connect-core-1_0.html#TokenEndpoint
    //Client Credentials Grant Flow.
    ////POST /realms/{realm}/protocol/openid-connect/token
    public class KeycloakAdminTokenService : IKeycloakAdminTokenService
    {
        private readonly IHttpClientFactory httpClientFactory; //διαχειριστής του pool με τις available συνδέσεις (connection pooling!)
        private readonly IConfiguration configuration; //appsettings που τα εχω στο vault
        private readonly ILogger<KeycloakAdminTokenService> logger;

        //constructor
        public KeycloakAdminTokenService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<KeycloakAdminTokenService> logger)
        {
            this.httpClientFactory = httpClientFactory;
            this.configuration = configuration;
            this.logger = logger;
        }

        public async Task<string?> GetAdminAccessTokenAsync()
        {
            //παίρνω από appsettings
            var tokenEndpoint = configuration["Keycloak:AdminApi:TokenUrl"];
            var clientId = configuration["Keycloak:AdminApi:ClientId"];
            var clientSecret = configuration["Keycloak:AdminApi:ClientSecret"];

            //αν εχω κενό στις ρυθμίσεις να το πιασει
            if (string.IsNullOrEmpty(tokenEndpoint) || string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            {
                this.logger.LogError("Keycloak configuration is null");
                return null;
            }

            //το body του request
            var requestBody = new Dictionary<string, string>
            {
                {"grant_type", "client_credentials" },
                {"client_id", clientId},
                {"client_secret", clientSecret}
            };

            var content = new FormUrlEncodedContent(requestBody);

            //post το request
            var client = this.httpClientFactory.CreateClient();
            var response = await client.PostAsync(tokenEndpoint, content);

            //έλεγχος της απάντησης
            if (!response.IsSuccessStatusCode) {
                var errorContent = await response.Content.ReadAsStringAsync();
                this.logger.LogError("Failed to take an admin token.");
                return null;
            }

            //παίρνουμε την απάντηση και κάνουμε parse του json του token. ελεγχος πρωτα οτι υπάρχει το "access_token"
            var responseContent = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(responseContent);

            if (jsonDoc.RootElement.TryGetProperty("access_token", out var accessTokenElement))
            {
                return accessTokenElement.GetString();
            }

            //αν δεν βρει
            this.logger.LogError("Access token was not found in the response");

            return null;


        }
    }

}
