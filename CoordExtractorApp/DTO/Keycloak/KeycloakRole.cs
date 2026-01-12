using System.Text.Json.Serialization;

namespace CoordExtractorApp.DTO.Keycloak
{
    //https://www.keycloak.org/docs-api/latest/rest-api/index.html
    //RoleRepresentation

    /// <summary>
    /// Represents a role in Keycloak
    /// </summary>
    public class KeycloakRole
    {
        /// <summary>
        /// The unique identifier of the role
        /// </summary>
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        /// <summary>
        /// The name of the role
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }
}
