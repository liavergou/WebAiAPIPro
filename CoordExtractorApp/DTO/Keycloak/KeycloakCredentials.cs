using System.Text.Json.Serialization;

namespace CoordExtractorApp.DTO.Keycloak
{
    //https://www.keycloak.org/docs-api/latest/rest-api/index.html
    //CredentialRepresentation

    /// <summary>
    /// Represents user credentials for Keycloak API operations.
    /// </summary>
    public class KeycloakCredentials
    {
        /// <summary>
        /// The type of credential
        /// </summary>
        [JsonPropertyName("type")]
        public string? Type { get; set; } = "password";

        /// <summary>
        /// The password
        /// </summary>
        [JsonPropertyName("value")]
        public string Value { get; set; } = null!;
    }
}
