namespace CoordExtractorApp.DTO
{
    /// <summary>
    /// Data Transfer Object for returning user details.
    /// </summary>
    public class UserReadOnlyDTO
    {
        /// <summary>
        /// The unique user ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The username.
        /// </summary>
        public string Username { get; set; } = null!;

        /// <summary>
        /// The email address.
        /// </summary>
        public string Email { get; set; } = null!;

        /// <summary>
        /// The first name.
        /// </summary>
        public string Firstname { get; set; } = null!;

        /// <summary>
        /// The last name.
        /// </summary>
        public string Lastname { get; set; } = null!;

        /// <summary>
        /// The assigned role.
        /// </summary>
        public string Role { get; set; } = null!;
    }
}
