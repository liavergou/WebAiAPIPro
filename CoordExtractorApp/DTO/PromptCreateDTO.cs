using System.ComponentModel.DataAnnotations;

namespace CoordExtractorApp.DTO
{
    public class PromptCreateDTO
    {
        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Prompt name must be between 2 and 100 characters.")]
        public string PromptName { get; set; } = null!;

        [Required(ErrorMessage = "The {0} field is required.")]       
        public string PromptText { get; set; } = null!;
    }
}
