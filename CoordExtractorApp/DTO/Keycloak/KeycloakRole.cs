using System.Text.Json.Serialization;

namespace CoordExtractorApp.DTO.Keycloak
{
    //https://www.keycloak.org/docs-api/latest/rest-api/index.html
    //RoleRepresentation
    public class KeycloakRole
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }
}
