using AutoMapper;
using CoordExtractorApp.Data;

namespace CoordExtractorApp.Configuration
{
    public class MapperConfig : Profile
    {

        public MapperConfig() 
        {
            CreateMap<User, DTO.UserReadOnlyDTO>()
                .ReverseMap();

            CreateMap<DTO.UserCreateDTO, User>();
            CreateMap<DTO.UserUpdateDTO, User>();

            CreateMap<Project, DTO.ProjectReadOnlyDTO>()
                .ReverseMap();
            CreateMap<DTO.ProjectCreateDTO, Project>();
            CreateMap<DTO.ProjectUpdateDTO, Project>();

            CreateMap<Prompt, DTO.PromptReadOnlyDTO>()
                .ReverseMap();
            CreateMap<DTO.PromptCreateDTO, Prompt>();
            CreateMap<DTO.PromptUpdateDTO, Prompt>();


        }

    }
}
