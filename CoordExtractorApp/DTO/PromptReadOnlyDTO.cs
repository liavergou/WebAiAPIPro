namespace CoordExtractorApp.DTO
{
    /// <summary>
    /// Data Transfer Object for returning read-only prompt details.
    /// </summary>
    public class PromptReadOnlyDTO
    {
        /// <summary>
        /// The unique identifier of the prompt.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of the prompt.
        /// </summary>
        public string PromptName { get; set; } = null!;

        /// <summary>
        /// The text content of the prompt.
        /// </summary>
        public string PromptText { get; set; } = null!;
    }
}
