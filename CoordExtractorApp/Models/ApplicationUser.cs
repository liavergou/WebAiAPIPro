namespace CoordExtractorApp.Models
{

    /// <summary>
    /// Represents the currently authenticated user within the application context.
    /// This model aggregates information from both the Identity Provider (Keycloak) and the local database.
    /// </summary>
    public class ApplicationUser
    {      

        /// <summary>
        /// The unique identifier of the user in the local PostgreSQL database.
        /// Source: Local Database.
        /// </summary>
        public int? Id { get; set; } //της βάσης

        /// <summary>
        /// The unique identifier (Subject ID) of the user in Keycloak.
        /// Source: Keycloak JWT Token (sub).
        /// </summary>
        public string? KeycloakId { get; set; } = string.Empty; //του token

        /// <summary>
        /// The username of the user.
        /// Source: Keycloak JWT Token (preferred_username).
        /// </summary>
        public string? Username { get; set; } = string.Empty; //του token

        /// <summary>
        /// The email address of the user.
        /// Source: Keycloak JWT Token (email).
        /// </summary>
        public string? Email {  get; set; } = string.Empty; //από το token

        /// <summary>
        /// The last name (surname) of the user.
        /// Source: Keycloak JWT Token (family_name).
        /// </summary>
        public string? Lastname {  get; set; } = string.Empty; //απο το token

        /// <summary>
        /// The first name of the user.
        /// Source: Keycloak JWT Token (given_name).
        /// </summary>
        public string? Firstname {  get; set; } = string.Empty; //απο το token

        /// <summary>
        /// The role assigned to the user within the application.
        /// Source: Local Database
        /// </summary>
        public string? Role { get; set; } = string.Empty; //αφου το token έχει γινει mapping. ClaimTypes.Role από το keycloak
    }
}
