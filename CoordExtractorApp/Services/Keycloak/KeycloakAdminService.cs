using CoordExtractorApp.DTO.Keycloak;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace CoordExtractorApp.Services.Keycloak
{
    /// <summary>
    /// Implementation of Keycloak admin service using Keycloak Admin REST API.
    /// </summary>
    public class KeycloakAdminService : IKeycloakAdminService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IConfiguration configuration;
        private readonly ILogger<KeycloakAdminService> logger;
        private readonly IKeycloakAdminTokenService keycloakAdminTokenService;

        public KeycloakAdminService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<KeycloakAdminService> logger, IKeycloakAdminTokenService keycloakAdminTokenService)
        {
            this.httpClientFactory = httpClientFactory;
            this.configuration = configuration;
            this.logger = logger;
            this.keycloakAdminTokenService = keycloakAdminTokenService;
        }

        private async Task<(HttpClient client, string adminApiUrl)?> GetAdminHttpClientAsync()
        {
            var adminToken = await this.keycloakAdminTokenService.GetAdminAccessTokenAsync();
            if (string.IsNullOrEmpty(adminToken))
            {
                return null;
            }
            var client = this.httpClientFactory.CreateClient("KeycloakAdminClient");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
            var adminApiUrl = this.configuration["Keycloak:AdminApi:AdminApiUrl"];

            return (client, adminApiUrl!);
        }

        public async Task<string?> CreateUserAsync(KeycloakUserDTO keycloakUser)
        {
            var context = await this.GetAdminHttpClientAsync();
            if (context == null)
            {
                return null;
            }
            var (client, adminApiUrl) = context.Value;

            var userJson = JsonSerializer.Serialize(keycloakUser);
            var content = new StringContent(userJson, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(adminApiUrl + "users", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();

                this.logger.LogError("Failed to create user in Keycloak. Status: {StatusCode}, Response: {ErrorContent}", response.StatusCode, errorContent);
                return null;
            }

            if (response.Headers.Location != null)
            {
                var locationHeader = response.Headers.Location;
                var userId = locationHeader.Segments.LastOrDefault()?.TrimEnd('/');
                return userId;
            }

            return null;      
        }

        public async Task<bool> AssignUserRoleToUserAsync(string userId, string roleName)
        {
            var context = await this.GetAdminHttpClientAsync();
            if (context == null)
            {
                return false;
            }
            var (client, adminApiUrl) = context.Value;

            var roleResponse = await client.GetAsync($"{adminApiUrl}roles/{roleName}");
            if (!roleResponse.IsSuccessStatusCode)
            {
                var errorContent = await roleResponse.Content.ReadAsStringAsync();
                this.logger.LogError("Failed to find role '{RoleName}' in Keycloak. Status: {StatusCode}, Response: {ErrorContent}", roleName, roleResponse.StatusCode, errorContent);
                return false;
            }

            var role = await roleResponse.Content.ReadFromJsonAsync<KeycloakRole>();
            if (role == null) return false;

            var rolesToAssign = new[] { role };
            var content = new StringContent(JsonSerializer.Serialize(rolesToAssign), Encoding.UTF8, "application/json");
            
            var assignResponse = await client.PostAsync($"{adminApiUrl}users/{userId}/role-mappings/realm", content);

            if (!assignResponse.IsSuccessStatusCode)
            {
                var errorContent = await assignResponse.Content.ReadAsStringAsync();
                this.logger.LogError("Failed to assign role to user {UserId}. Status: {StatusCode}, Response: {ErrorContent}", userId, assignResponse.StatusCode, errorContent);
            }

            return assignResponse.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateUserDetailsAsync(string keycloakId, DTO.UserUpdateDTO userUpdateDto)
        {
            var context = await this.GetAdminHttpClientAsync();
            if (context == null)
            {
                return false;
            }
            var (client, adminApiUrl) = context.Value;

            var userResponse = await client.GetAsync($"{adminApiUrl}users/{keycloakId}");
            if (!userResponse.IsSuccessStatusCode) return false;

            var keycloakUser = await userResponse.Content.ReadFromJsonAsync<KeycloakUserDTO>();
            if (keycloakUser == null) return false;

            keycloakUser.FirstName = userUpdateDto.Firstname ?? keycloakUser.FirstName;
            keycloakUser.LastName = userUpdateDto.Lastname ?? keycloakUser.LastName;
            keycloakUser.Email = userUpdateDto.Email ?? keycloakUser.Email;

            var userJson = JsonSerializer.Serialize(keycloakUser);
            var content = new StringContent(userJson, Encoding.UTF8, "application/json");

            var updateResponse = await client.PutAsync($"{adminApiUrl}users/{keycloakId}", content);

            return updateResponse.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateUserRoleAsync(string keycloakId, string newRoleName)
        {
            var context = await this.GetAdminHttpClientAsync();
            if (context == null) return false;
            var (client, adminApiUrl) = context.Value;

            var currentRolesResponse = await client.GetAsync($"{adminApiUrl}users/{keycloakId}/role-mappings/realm");
            if (!currentRolesResponse.IsSuccessStatusCode) return false;
            
            var currentRoles = await currentRolesResponse.Content.ReadFromJsonAsync<List<KeycloakRole>>();
        
            var newRoleResponse = await client.GetAsync($"{adminApiUrl}roles/{newRoleName}");
            if (!newRoleResponse.IsSuccessStatusCode) return false;
            
            var newRole = await newRoleResponse.Content.ReadFromJsonAsync<KeycloakRole>();
        
            if (newRole == null) return false;
        
            if (currentRoles != null && currentRoles.Any())
            {
                var removeContent = new StringContent(JsonSerializer.Serialize(currentRoles), Encoding.UTF8, "application/json");
                var removeRequest = new HttpRequestMessage(HttpMethod.Delete, $"{adminApiUrl}users/{keycloakId}/role-mappings/realm")
                {
                    Content = removeContent
                };
                await client.SendAsync(removeRequest);
            }
        
            var addContent = new StringContent(JsonSerializer.Serialize(new[] { newRole }), Encoding.UTF8, "application/json");
            var addResponse = await client.PostAsync($"{adminApiUrl}users/{keycloakId}/role-mappings/realm", addContent);

            return addResponse.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteUserAsync(string keycloakId)
        {
            var context = await this.GetAdminHttpClientAsync();
            if (context == null) return false;
            var (client, adminApiUrl) = context.Value;

            var deleteResponse = await client.DeleteAsync($"{adminApiUrl}users/{keycloakId}");

            if (!deleteResponse.IsSuccessStatusCode)
            {
                var errorContent = await deleteResponse.Content.ReadAsStringAsync();
                this.logger.LogError("Failed to delete user {KeycloakId} from Keycloak. Status: {StatusCode}, Response: {ErrorContent}", keycloakId, deleteResponse.StatusCode, errorContent);
            }

            return deleteResponse.IsSuccessStatusCode;
        }
    }
}