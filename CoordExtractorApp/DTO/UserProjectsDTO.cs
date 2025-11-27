namespace CoordExtractorApp.DTO
{
    public class UserProjectsDTO
    {
        //για GET /api/users/{id}/projects
        public int Id {  get; set; }
        public List<int> ProjectIds { get; set; } = []; //τα ήδη assigned project ids
    }
}
