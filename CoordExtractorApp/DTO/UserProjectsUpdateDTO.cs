using System.ComponentModel.DataAnnotations;

namespace CoordExtractorApp.DTO
{
    public class UserProjectsUpdateDTO
    {
        //για PUT /api/users/{id}/projects
        [Required(ErrorMessage = "Projects list is required, filled or empty")]
        public List<int> ProjectIds { get; set; } = []; //τα checked projects
    }
}
