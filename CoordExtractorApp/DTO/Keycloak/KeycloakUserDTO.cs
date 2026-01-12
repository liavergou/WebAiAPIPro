using System.Text.Json.Serialization;

namespace CoordExtractorApp.DTO.Keycloak
{
    /// <summary>
    /// Data Transfer Object representing a user in Keycloak.
    /// </summary>
    

    //https://www.keycloak.org/docs-api/latest/rest-api/index.html
    //UserRepresentation
    public class KeycloakUserDTO
    {
        /// <summary>
        /// The username. Corresponds to 'preferred_username' in tokens.
        /// </summary>
        [JsonPropertyName("username")]
        public string? Username { get; set; } //preferred_username στο token

        /// <summary>
        /// The email address.
        /// </summary>
        [JsonPropertyName("email")]
        public string? Email { get; set; } //email στο token

        /// <summary>
        /// The first name.
        /// </summary>
        [JsonPropertyName("firstName")]
        public string? FirstName { get; set; }  //given_name

        /// <summary>
        /// The last name.
        /// </summary>
        [JsonPropertyName("lastName")]
        public string? LastName { get; set; }  //family_name

        /// <summary>
        /// A list of credentials (e.g., password) associated with the user.
        /// </summary>
        [JsonPropertyName("credentials")]
        public List<KeycloakCredentials>? Credentials { get; set; }

        /// <summary>
        /// Indicates if the email address has been verified.
        /// </summary>
        [JsonPropertyName("emailVerified")]
        public bool EmailVerified { get; set; } = true;

        /// <summary>
        /// Indicates if the user account is enabled.
        /// </summary>
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; } = true;



    }
}
