using Azure;
using CoordExtractorApp.DTO.Keycloak;
using GenerativeAI.Types;
using Microsoft.Extensions.Caching.Memory;
using Serilog.Data;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace CoordExtractorApp.Services.Keycloak
﻿{

    //Service για την επικοινωνία με το Keycloak Admin Rest API
    //https://dev.to/kayesislam/integrating-openid-connect-to-your-application-stack-25ch (java)
    //https://stackoverflow.com/questions/77084743/secure-asp-net-core-rest-api-with-keycloak

    public class KeycloakAdminService : IKeycloakAdminService
﻿    {

        
        private readonly IHttpClientFactory httpClientFactory; //διαχειριστής του pool με τις available συνδέσεις (connection pooling!)
        private readonly IConfiguration configuration; //appsettings που τα εχω στο vault
        private readonly ILogger<KeycloakAdminService> logger;
        private readonly IKeycloakAdminTokenService keycloakAdminTokenService;

        public KeycloakAdminService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<KeycloakAdminService> logger, IKeycloakAdminTokenService keycloakAdminTokenService)
        {
            this.httpClientFactory = httpClientFactory;
            this.configuration = configuration;
            this.logger = logger;
            this.keycloakAdminTokenService = keycloakAdminTokenService;
        }
        //helper για να μη το βαζω σε ολα. Για να πάρω σύνδεση με το σωστό token
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

        //CREATE KEYCLOAK USER - POST /admin/realms/{realm}/users
        public async Task<string?> CreateUserAsync(KeycloakUserDTO keycloakUser) //dto του User που θα πάει στο Keycloak
        {
            //ο client απο τη helper
            var context = await this.GetAdminHttpClientAsync();
            if (context == null)
            {
                return null; //fail
            }
            var (client, adminApiUrl) = context.Value;

            //DTO ΣΕ JSON
            var userJson = JsonSerializer.Serialize(keycloakUser);
            var content = new StringContent(userJson, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(adminApiUrl + "users", content); //χτισιμο του url 

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();

                this.logger.LogError("Failed to create user in Keycloak. Status: {StatusCode}, Response: {ErrorContent}", response.StatusCode, errorContent);
                return null;
            }

            if (response.Headers.Location != null)
            {
                var locationHeader = response.Headers.Location;
                var userId = locationHeader.Segments.LastOrDefault()?.TrimEnd('/'); //to id trim το τελευταιο / του url
                return userId;
            }

            return null;      
        }

        //ASSIGN ROLE TO USER
        public async Task<bool> AssignUserRoleToUserAsync(string userId, string roleName) //role σε string (custom mapping)
        {
            var context = await this.GetAdminHttpClientAsync();
            if (context == null)
            {
                return false; //fail
            }
            var (client, adminApiUrl) = context.Value;



            var roleResponse = await client.GetAsync($"{adminApiUrl}roles/{roleName}");
            if (!roleResponse.IsSuccessStatusCode)
            {
                var errorContent = await roleResponse.Content.ReadAsStringAsync();
                this.logger.LogError("Failed to find role '{RoleName}' in Keycloak. Status: {StatusCode}, Response: {ErrorContent}", roleName, roleResponse.StatusCode, errorContent);
                return false;
            }
            //Βρίσκουμε το role απο το token
            var role = await roleResponse.Content.ReadFromJsonAsync<KeycloakRole>();
            if (role == null) return false;

            var rolesToAssign = new[] { role };
            var content = new StringContent(JsonSerializer.Serialize(rolesToAssign), Encoding.UTF8, "application/json");
            //GET /admin/realms/{realm}/users/{user-id}/role-mappings/realm
            var assignResponse = await client.PostAsync($"{adminApiUrl}users/{userId}/role-mappings/realm", content);

            if (!assignResponse.IsSuccessStatusCode)
            {
                var errorContent = await assignResponse.Content.ReadAsStringAsync();
                this.logger.LogError("Failed to assign role to user {UserId}. Status: {StatusCode}, Response: {ErrorContent}", userId, assignResponse.StatusCode, errorContent);
            }

            return assignResponse.IsSuccessStatusCode;
        }

        //USER KEYCLOAK UPDATE
        public async Task<bool> UpdateUserDetailsAsync(string keycloakId, DTO.UserUpdateDTO userUpdateDto)
﻿        {
            var context = await this.GetAdminHttpClientAsync();
            if (context == null)
            {
                return false; //fail
            }
            var (client, adminApiUrl) = context.Value;

            // Βρίσκουμε τα στοιχεία του user στο keycloak
            var userResponse = await client.GetAsync($"{adminApiUrl}users/{keycloakId}"); //GET /admin/realms/{realm}/users/{keycloakId}
            if (!userResponse.IsSuccessStatusCode) return false;
﻿
﻿            var keycloakUser = await userResponse.Content.ReadFromJsonAsync<KeycloakUserDTO>();
﻿            if (keycloakUser == null) return false;


            // update τα στοιχεία του με τα στοιχεία που εχουμε πάρει απο το userUpdateDto
            keycloakUser.FirstName = userUpdateDto.Firstname ?? keycloakUser.FirstName;
﻿            keycloakUser.LastName = userUpdateDto.Lastname ?? keycloakUser.LastName;
﻿            keycloakUser.Email = userUpdateDto.Email ?? keycloakUser.Email;
﻿
﻿            var userJson = JsonSerializer.Serialize(keycloakUser);
﻿            var content = new StringContent(userJson, Encoding.UTF8, "application/json");
﻿
﻿            //Put προς τον keycloak με τα νέα
﻿            var updateResponse = await client.PutAsync($"{adminApiUrl}users/{keycloakId}", content); //PUT /admin/realms/{realm}/users/{keycloakId}

            return updateResponse.IsSuccessStatusCode;
﻿        }
        
        
        //UPDATE USER ROLE
        public async Task<bool> UpdateUserRoleAsync(string keycloakId, string newRoleName)
        {
            var context = await this.GetAdminHttpClientAsync();
            if (context == null) return false;
            var (client, adminApiUrl) = context.Value;



            // //Βρίσκουμε το υπάρχον role
            var currentRolesResponse = await client.GetAsync($"{adminApiUrl}users/{keycloakId}/role-mappings/realm"); //role-mappings
            if (!currentRolesResponse.IsSuccessStatusCode) return false;
﻿                    var currentRoles = await currentRolesResponse.Content.ReadFromJsonAsync<List<KeycloakRole>>();
﻿        
﻿                    //Βρίσκουμε την αναπαράσταση του νέου ρόλου.
﻿                    var newRoleResponse = await client.GetAsync($"{adminApiUrl}roles/{newRoleName}"); //GET /admin/realms/{realm}/users/{userId}/role-mappings/realm
            if (!newRoleResponse.IsSuccessStatusCode) return false;
﻿                    var newRole = await newRoleResponse.Content.ReadFromJsonAsync<KeycloakRole>(); //απο το DTO
﻿        
﻿                    if (newRole == null) return false;
﻿        
﻿                    //αφαίρεση του παλιου
﻿                    if (currentRoles != null && currentRoles.Any())
﻿                    {
﻿                        var removeContent = new StringContent(JsonSerializer.Serialize(currentRoles), Encoding.UTF8, "application/json");
﻿                        var removeRequest = new HttpRequestMessage(HttpMethod.Delete, $"{adminApiUrl}users/{keycloakId}/role-mappings/realm") //DELETE /admin/realms/{realm}/users/{userId}/role-mappings/realm
                        {
﻿                            Content = removeContent
﻿                        };
﻿                        await client.SendAsync(removeRequest);
﻿                    }
﻿        
﻿                    //Προσθήκη του νέου
﻿                    var addContent = new StringContent(JsonSerializer.Serialize(new[] { newRole }), Encoding.UTF8, "application/json");
﻿                    var addResponse = await client.PostAsync($"{adminApiUrl}users/{keycloakId}/role-mappings/realm", addContent); //POST /admin/realms/{realm}/users/{userId}/role-mappings/realm

            return addResponse.IsSuccessStatusCode;
﻿                }

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
