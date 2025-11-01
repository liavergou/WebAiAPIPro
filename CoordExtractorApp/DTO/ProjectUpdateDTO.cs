using System.ComponentModel.DataAnnotations;

namespace CoordExtractorApp.DTO
{
    public class ProjectUpdateDTO
    {
     
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Project name must be between 2 and 200 characters.")]
        public string? ProjectName { get; set; } = null!;

        [StringLength(500, ErrorMessage = "Project description must be between 2 and 500 characters.")]
        public string? Description { get; set; }
    }
}
