using System.ComponentModel.DataAnnotations;

namespace CoordExtractorApp.DTO
{
    /// <summary>
    /// Data Transfer Object for creating a new project.
    /// </summary>
    public class ProjectCreateDTO
    {
        /// <summary>
        /// The name of the project.
        /// </summary>
        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Project name must be between 2 and 200 characters.")]
        public string ProjectName { get; set; } = null!;

        /// <summary>
        /// A description of the project.
        /// </summary>
        [StringLength(500, ErrorMessage = "Project description must be between 2 and 500 characters.")]
        public string? Description { get; set; }

    }
}
