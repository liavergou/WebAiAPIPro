using AutoMapper;
using CoordExtractorApp.Repositories;

namespace CoordExtractorApp.Services
{
    public class ApplicationService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
    
    public ApplicationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        public UserService UserService => new(unitOfWork, mapper);
    }
}
