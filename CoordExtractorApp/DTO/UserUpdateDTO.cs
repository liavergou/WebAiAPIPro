using CoordExtractorApp.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace CoordExtractorApp.DTO
{
    /// <summary>
    /// Data Transfer Object for updating an existing user.
    /// </summary>
    public class UserUpdateDTO
    {
        /// <summary>
        /// The updated email address.
        /// </summary>
        [StringLength(100, ErrorMessage = "Email must not exceed 100 characters.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string? Email { get; set; }

        /// <summary>
        /// The updated first name.
        /// </summary>
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Firstname must be between 2 and 50 characters.")]
        public string? Firstname { get; set; }

        /// <summary>
        /// The updated last name.
        /// </summary>
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Lastname must be between 2 and 50 characters.")]
        public string? Lastname { get; set; }

        /// <summary>
        /// The updated role
        /// </summary>
        public string? Role { get; set; }
    }
}
