namespace CoordExtractorApp.DTO
{
    /// <summary>
    /// Data Transfer Object representing a project, including a count of its conversion jobs.
    /// </summary>
    
    //περιλαμβάνει το count των jobs
    public class ProjectDTO
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

        /// <summary>
        /// The total number of conversion jobs associated with this project.
        /// </summary>
        public int JobsCount { get; set; }
    }
}
