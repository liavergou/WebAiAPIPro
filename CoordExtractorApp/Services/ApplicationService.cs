using AutoMapper;
using CoordExtractorApp.Repositories;
using CoordExtractorApp.Services.GenerativeAI;
using CoordExtractorApp.Services.Geoserver;
using CoordExtractorApp.Services.Keycloak;

namespace CoordExtractorApp.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IKeycloakAdminService keycloakAdminService;
        private readonly IConfiguration configuration;
        private readonly IGenerativeAIService generativeAIService;
        private readonly IHttpClientFactory httpClientFactory;

        public ApplicationService(IUnitOfWork unitOfWork, IMapper mapper, IKeycloakAdminService keycloakAdminService, IConfiguration configuration, IGenerativeAIService generativeAIService, IHttpClientFactory httpClientFactory)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.keycloakAdminService = keycloakAdminService;
            this.configuration = configuration;
            this.generativeAIService = generativeAIService;
            this.httpClientFactory = httpClientFactory;

        }

        public IUserService UserService => new UserService(unitOfWork, mapper, keycloakAdminService);
        public IProjectService ProjectService => new ProjectService(unitOfWork, mapper);
        public IPromptService PromptService => new PromptService(unitOfWork, mapper);

        public IUserProjectsService UserProjectsService => new UserProjectsService(unitOfWork,mapper);

        public IConversionJobService ConversionJobService =>
            new ConversionJobService(
                unitOfWork,
                configuration,
                generativeAIService,
                PromptService
            );

        public IGeoserverService GeoserverService => new GeoserverService(httpClientFactory,configuration);
    }
}