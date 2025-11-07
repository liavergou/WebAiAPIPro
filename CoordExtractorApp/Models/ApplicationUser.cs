namespace CoordExtractorApp.Models
{
    public class ApplicationUser
    {
        public int? Id { get; set; } //της βάσης

        public string? KeycloakId { get; set; } = string.Empty; //του token
        public string? Username { get; set; } = string.Empty; //του token

        public string? Email {  get; set; } = string.Empty; //από το token

        public string? Lastname {  get; set; } = string.Empty; //απο το token

        public string? Firstname {  get; set; } = string.Empty; //απο το token

        public string? Role { get; set; } = string.Empty; //απο το token αφου γινει mapping. ClaimTypes.Role από το keycloak
    }
}
