using System.ComponentModel.DataAnnotations;

namespace CoordExtractorApp.DTO
{
    /// <summary>
    /// Data Transfer Object for updating an existing prompt.
    /// </summary>
    public class PromptUpdateDTO
    {
        /// <summary>
        /// The updated prompt name.
        /// </summary>
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Prompt name must be between 2 and 100 characters.")]
        public string? PromptName { get; set; }

        /// <summary>
        /// The updated prompt text content.
        /// </summary>
        public string? PromptText { get; set; }
    }
}
