using AutoMapper;
using CoordExtractorApp.Configuration;
using CoordExtractorApp.Repositories;
using CoordExtractorApp.Services.GenerativeAI;
using CoordExtractorApp.Services.Geoserver;
using CoordExtractorApp.Services.Keycloak;
using Microsoft.Extensions.Options;

namespace CoordExtractorApp.Services
{
    /// <summary>
    /// Central coordinator for all application services
    /// </summary>
    public class ApplicationService : IApplicationService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IKeycloakAdminService keycloakAdminService;
        private readonly IConfiguration configuration;
        private readonly IGenerativeAIService generativeAIService;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger<ConversionJobService> conversionJobLogger;
        private readonly IOptions<GeminiOptions> geminiOptions;

        public ApplicationService(IUnitOfWork unitOfWork, IMapper mapper, IKeycloakAdminService keycloakAdminService, IConfiguration configuration, IGenerativeAIService generativeAIService, IHttpClientFactory httpClientFactory, ILogger<ConversionJobService> conversionJobLogger, IOptions<GeminiOptions> geminiOptions)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.keycloakAdminService = keycloakAdminService;
            this.configuration = configuration;
            this.generativeAIService = generativeAIService;
            this.httpClientFactory = httpClientFactory;
            this.conversionJobLogger = conversionJobLogger;
            this.geminiOptions = geminiOptions;
        }

        public IUserService UserService => new UserService(unitOfWork, mapper, keycloakAdminService);
        public IProjectService ProjectService => new ProjectService(unitOfWork, mapper, configuration);
        public IPromptService PromptService => new PromptService(unitOfWork, mapper);

        public IUserProjectsService UserProjectsService => new UserProjectsService(unitOfWork, mapper);

        public IConversionJobService ConversionJobService =>
            new ConversionJobService(
                unitOfWork,
                configuration,
                generativeAIService,
                PromptService,
                conversionJobLogger,
                geminiOptions
            );

        public IGeoserverService GeoserverService => new GeoserverService(httpClientFactory, configuration);
    }
}