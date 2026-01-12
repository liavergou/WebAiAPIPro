using System.ComponentModel.DataAnnotations;

namespace CoordExtractorApp.DTO
{
    /// <summary>
    /// Data Transfer Object for updating an existing project.
    /// </summary>
    public class ProjectUpdateDTO
    {
        /// <summary>
        /// The updated project name.
        /// </summary>
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Project name must be between 2 and 200 characters.")]
        public string? ProjectName { get; set; }

        /// <summary>
        /// The updated project description.
        /// </summary>
        [StringLength(500, ErrorMessage = "Project description must be between 2 and 500 characters.")]
        public string? Description { get; set; }
    }
}
