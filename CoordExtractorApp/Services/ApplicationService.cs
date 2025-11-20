using AutoMapper;
using CoordExtractorApp.Repositories;
using CoordExtractorApp.Services.Keycloak;

namespace CoordExtractorApp.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IKeycloakAdminService keycloakAdminService;

        public ApplicationService(IUnitOfWork unitOfWork, IMapper mapper, IKeycloakAdminService keycloakAdminService)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.keycloakAdminService = keycloakAdminService;
        }

        public UserService UserService => new (unitOfWork,mapper, keycloakAdminService);

        public ProjectService ProjectService => new (unitOfWork, mapper);

        public PromptService PromptService => new (unitOfWork, mapper);
    }
}
