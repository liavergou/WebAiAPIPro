namespace CoordExtractorApp.Data
{
    /// <summary>
    /// AI prompt template for coordinate extraction.
    /// </summary>
    public class Prompt : BaseEntity
    {
        public string PromptName { get; set; } = null!;
        public string PromptText { get; set; } = null!;

        public virtual ICollection<ConversionJob> ConversionJobs { get; set; } = new List<ConversionJob>();
    }
}
