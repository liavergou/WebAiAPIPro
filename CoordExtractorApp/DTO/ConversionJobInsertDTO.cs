using System.ComponentModel.DataAnnotations;

namespace CoordExtractorApp.DTO
{
    public class ConversionJobInsertDTO
    {
        [Required(ErrorMessage = "{0} is required.")]
        //TODO filesize και file ext ελεγχο
        public IFormFile ImageFile { get; set; } = null!;

        [Required(ErrorMessage = "Job must be assigned to a project")]
        [Range(1, int.MaxValue, ErrorMessage = "Job must be assigned to a valid project")]   
        public int ProjectId { get; set; }

        [Required(ErrorMessage = "Job must be assigned to a prompt")]
        [Range(1, int.MaxValue, ErrorMessage = "Job must be assigned to a valid prompt")]
        public int PromptId { get; set; }


    }
}
