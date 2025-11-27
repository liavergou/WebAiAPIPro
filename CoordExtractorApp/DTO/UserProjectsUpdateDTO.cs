namespace CoordExtractorApp.DTO
{
    public class UserProjectsUpdateDTO
    {
        //για PUT /api/users/{id}/projects
        public int Id { get; set; }
        public List<int> ProjectIds { get; set; } = []; //τα checked projects
    }
}
