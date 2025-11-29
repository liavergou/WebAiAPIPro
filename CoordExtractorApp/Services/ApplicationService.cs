using AutoMapper;
using CoordExtractorApp.Repositories;
using CoordExtractorApp.Services.GenerativeAI;
using CoordExtractorApp.Services.Keycloak;
using Microsoft.Extensions.Configuration;

namespace CoordExtractorApp.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IKeycloakAdminService keycloakAdminService;
        private readonly IConfiguration configuration;
        private readonly IGenerativeAIService generativeAIService;

        public ApplicationService(IUnitOfWork unitOfWork, IMapper mapper, IKeycloakAdminService keycloakAdminService, IConfiguration configuration, IGenerativeAIService generativeAIService)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.keycloakAdminService = keycloakAdminService;
            this.configuration = configuration;
            this.generativeAIService = generativeAIService;
        }

        public IUserService UserService => new UserService(unitOfWork, mapper, keycloakAdminService);
        public IProjectService ProjectService => new ProjectService(unitOfWork, mapper);
        public IPromptService PromptService => new PromptService(unitOfWork, mapper);

        public IUserProjectsService UserProjectsService => new UserProjectsService(unitOfWork);

        public IConversionJobService ConversionJobService =>
            new ConversionJobService(
                unitOfWork,
                configuration,
                generativeAIService,
                PromptService
            );
    }
}