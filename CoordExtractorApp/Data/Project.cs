namespace CoordExtractorApp.Data
{
    public class Project : BaseEntity
    {
        public int Id {  get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }
    }
}
