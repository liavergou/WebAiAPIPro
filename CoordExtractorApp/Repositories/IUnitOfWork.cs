namespace CoordExtractorApp.Repositories
{
    public interface IUnitOfWork
    { 
        UserRepository UserRepository { get; } // Προσθήκη
        PromptRepository PromptRepository { get; }
        ProjectRepository ProjectRepository { get; }
        ConversionJobRepository ConversionJobRepository { get; }

        Task<bool> SaveAsync();
    }
}
