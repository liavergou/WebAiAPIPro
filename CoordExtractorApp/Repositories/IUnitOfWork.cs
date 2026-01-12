namespace CoordExtractorApp.Repositories
{
    /// <summary>
    /// Interface for the Unit of Work pattern.
    /// Coordinates the work of multiple repositories and ensures data integrity by handling transaction commits.
    /// </summary>
    public interface IUnitOfWork
    { 
        UserRepository UserRepository { get; } // Προσθήκη
        PromptRepository PromptRepository { get; }
        ProjectRepository ProjectRepository { get; }
        ConversionJobRepository ConversionJobRepository { get; }

        /// <summary>
        /// Commits all changes made in the context to the database.
        /// </summary>
        /// <returns>True if the save was successful, otherwise false.</returns>
        Task<bool> SaveAsync();
    }
}
