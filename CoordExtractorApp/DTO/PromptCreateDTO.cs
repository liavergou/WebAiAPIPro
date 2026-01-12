using System.ComponentModel.DataAnnotations;

namespace CoordExtractorApp.DTO
{
    /// <summary>
    /// Data Transfer Object for creating a new prompt.
    /// </summary>
    public class PromptCreateDTO
    {
        /// <summary>
        /// The name of the prompt.
        /// </summary>
        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Prompt name must be between 2 and 100 characters.")]
        public string PromptName { get; set; } = null!;

        /// <summary>
        /// The text content of the prompt.
        /// </summary>
        [Required(ErrorMessage = "The {0} field is required.")]       
        public string PromptText { get; set; } = null!;
    }
}
