using System.ComponentModel.DataAnnotations;

namespace CoordExtractorApp.DTO
{
    /// <summary>
    /// Data Transfer Object for updating project assignments for a user.
    /// </summary>
    public class UserProjectsUpdateDTO
    {
        /// <summary>
        /// The list of project IDs to be assigned to the user (applicable to users with role member).
        /// </summary>
        
        //για PUT /api/users/{id}/projects
        [Required(ErrorMessage = "Projects list is required, filled or empty")]
        public List<int> ProjectIds { get; set; } = []; //τα checked projects
    }
}
