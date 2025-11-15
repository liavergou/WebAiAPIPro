using System.Text.Json.Serialization;

namespace CoordExtractorApp.DTO.Keycloak
{
    //https://www.keycloak.org/docs-api/latest/rest-api/index.html
    //CredentialRepresentation
    public class KeycloakCredentials
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; } = "password";

        [JsonPropertyName("value")]
        public string Value { get; set; } = null!;
    }
}
