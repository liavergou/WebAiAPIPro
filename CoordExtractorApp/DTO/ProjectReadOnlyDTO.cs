namespace CoordExtractorApp.DTO
{
    /// <summary>
    /// Data Transfer Object for returning read-only project details.
    /// </summary>
    public class ProjectReadOnlyDTO
    {
        /// <summary>
        /// The unique identifier of the project.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of the project.
        /// </summary>
        public string ProjectName { get; set; } = null!;

        /// <summary>
        /// The description of the project.
        /// </summary>
        public string? Description { get; set; }
    }
}
