namespace CoordExtractorApp.Data
{
    /// <summary>
    /// Project entity containing conversion jobs.
    /// </summary>
    public class Project : BaseEntity
    {
        public string ProjectName { get; set; } = null!;
        public string? Description { get; set; }

        public virtual ICollection<User> Users { get; set; } = new List<User>();
        public virtual ICollection<ConversionJob> ConversionJobs { get; set; } = new List<ConversionJob>();
    }
}
