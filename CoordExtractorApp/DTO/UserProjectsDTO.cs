namespace CoordExtractorApp.DTO
{
    /// <summary>
    /// Data Transfer Object for retrieving project IDs assigned to a user. (applicable to users with role Member)
    /// </summary>
    public class UserProjectsDTO
    {
        /// <summary>
        /// A list of project IDs associated with the user.
        /// </summary>
        
        //για GET /api/users/{id}/projects
        public List<int> ProjectIds { get; set; } = []; //τα assigned project ids
    }
}
