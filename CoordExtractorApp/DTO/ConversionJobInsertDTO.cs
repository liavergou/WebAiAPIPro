using System.ComponentModel.DataAnnotations;

namespace CoordExtractorApp.DTO
{
    /// <summary>
    /// Data Transfer Object for creating a new conversion job.
    /// </summary>
    public class ConversionJobInsertDTO
    {
        /// <summary>
        /// The image file to be processed.
        /// </summary>
        [Required(ErrorMessage = "{0} is required.")]
        public IFormFile ImageFile { get; set; } = null!;

        /// <summary>
        /// The ID of the project to which this job belongs.
        /// </summary>
        [Required(ErrorMessage = "Job must be assigned to a project")]
        [Range(1, int.MaxValue, ErrorMessage = "Job must be assigned to a valid project")]   
        public int ProjectId { get; set; }

        /// <summary>
        /// The ID of the prompt used for AI processing.
        /// </summary>
        [Required(ErrorMessage = "Job must be assigned to a prompt")]
        [Range(1, int.MaxValue, ErrorMessage = "Job must be assigned to a valid prompt")]
        public int PromptId { get; set; }


    }
}
