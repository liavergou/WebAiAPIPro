namespace CoordExtractorApp.Data
{
    public class Project : BaseEntity
    {
        public int Id {  get; set; }

        public string ProjectName { get; set; } = null!;

        public string? Description { get; set; }

        public virtual ICollection<User> Users { get; set; } = new List<User>(); //navigation property

        public virtual ICollection<ConversionJob> ConversionJobs { get; set; } = new List<ConversionJob>();

    }
}
