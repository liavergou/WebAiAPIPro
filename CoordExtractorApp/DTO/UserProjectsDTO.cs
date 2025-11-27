namespace CoordExtractorApp.DTO
{
    public class UserProjectsDTO
    {
        //για GET /api/users/{id}/projects
        public List<int> ProjectIds { get; set; } = []; //τα assigned project ids
    }
}
