namespace CoordExtractorApp.Services
{
    public interface IApplicationService
    {
        UserService UserService { get; }
        ProjectService ProjectService { get; }
        PromptService PromptService { get; }
        // Other services can be added here as needed

    }
}
