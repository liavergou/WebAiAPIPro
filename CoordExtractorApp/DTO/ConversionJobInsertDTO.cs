using System.ComponentModel.DataAnnotations;

namespace CoordExtractorApp.DTO
{
    public class ConversionJobInsertDTO
    {
        [Required(ErrorMessage = "{0} is required.")]
        //TODO filesize και file ext ελεγχο
        public IFormFile File { get; set; } = null!;

        [Required(ErrorMessage = "{0} is required.")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "File name must be between 2 and 50 characters.")]
        public string OriginalFileName { get; set; } = null!;

        [Required(ErrorMessage = "Job must be assigned to a project")]
        [Range(1, int.MaxValue, ErrorMessage = "Job must be assigned to a valid project")]   
        public int ProjectId { get; set; }

        [Required(ErrorMessage = "Job must be assigned to a prompt")]
        [Range(1, int.MaxValue, ErrorMessage = "Job must be assigned to a valid prompt")]
        public int PromptId { get; set; }
        //public Guid ClientRequestId { get; set; } //.net 8 αντι για uuid. (επειδή θα είναι async) αφου θα επιστρεψω id χρειάζεται???


    }
}
