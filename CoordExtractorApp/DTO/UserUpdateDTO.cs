using CoordExtractorApp.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace CoordExtractorApp.DTO
{
    public class UserUpdateDTO
    {
     
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Username must be between 2 and 50 characters.")]
        public string? Username { get; set; }
     
        [StringLength(100, ErrorMessage = "Email must not exceed 100 characters.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string? Email { get; set; }
              
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Firstname must be between 2 and 50 characters.")]
        public string? Firstname { get; set; }

       
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Lastname must be between 2 and 50 characters.")]
        public string? Lastname { get; set; }

    }
}
