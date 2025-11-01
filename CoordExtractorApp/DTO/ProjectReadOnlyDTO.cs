namespace CoordExtractorApp.DTO
{
    public class ProjectReadOnlyDTO
    {
        public int Id { get; set; }

        public string ProjectName { get; set; } = null!;

        public string? Description { get; set; }
    }
}
