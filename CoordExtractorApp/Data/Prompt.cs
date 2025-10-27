namespace CoordExtractorApp.Data
{
    public class Prompt : BaseEntity
    {
        public int Id { get; set; }

        public string PromptName { get; set; } = null!;

        public string PromptText { get; set; } = null!;

    }
}
