namespace CoordExtractorApp.DTO
{
    //περιλαμβάνει το count των jobs
    public class ProjectDTO
    {
        public int Id { get; set; }

        public string ProjectName { get; set; } = null!;

        public string? Description { get; set; }

        public int JobsCount { get; set; }
    }
}
