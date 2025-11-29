namespace CoordExtractorApp.Services
{
    public interface IApplicationService
    {
        IUserService UserService { get; }
        IProjectService ProjectService { get; }
        IPromptService PromptService { get; }

        IUserProjectsService UserProjectsService { get; }

        IConversionJobService ConversionJobService { get; }

        
        


    }
}
