using System.Text.Json.Serialization;

namespace CoordExtractorApp.DTO.Keycloak
{
    //https://www.keycloak.org/docs-api/latest/rest-api/index.html
    //UserRepresentation
    public class KeycloakUserDTO
    {
        [JsonPropertyName("username")]
        public string? Username { get; set; } //preferred_username στο token

        [JsonPropertyName("email")]
        public string? Email { get; set; } //email στο token

        [JsonPropertyName("firstName")]
        public string? FirstName { get; set; }  //given_name

        [JsonPropertyName("lastName")]
        public string? LastName { get; set; }  //family_name

        [JsonPropertyName("credentials")]
        public List<KeycloakCredentials>? Credentials { get; set; }
    }
}
