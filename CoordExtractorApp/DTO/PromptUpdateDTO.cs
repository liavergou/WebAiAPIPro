using System.ComponentModel.DataAnnotations;

namespace CoordExtractorApp.DTO
{
    public class PromptUpdateDTO
    {
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Prompt name must be between 2 and 100 characters.")]
        public string? PromptName { get; set; }
   
        public string? PromptText { get; set; }
    }
}
