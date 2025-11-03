using AutoMapper;
using CoordExtractorApp.Data;

namespace CoordExtractorApp.Configuration
{
    public class MapperConfig : Profile //κληρονομεί απο την Profile του AutoMapper. για την αντιστοίχιση όταν ξεκινά η εφαρμογή
    {

        //constructor. δηλώνω τις αντιστοιχίες
        public MapperConfig() 
        {
            //αντιστοιχία απο το data model User προς το UserReadOnlyDTO
            CreateMap<User, DTO.UserReadOnlyDTO>()
                .ReverseMap(); //αντίστροφη αντιστοίχιση

            CreateMap<DTO.UserCreateDTO, User>(); //απο controller προς service
            CreateMap<DTO.UserUpdateDTO, User>(); //κάνω partial update. να μη το βαλω? ομοιως για τα αλλα update

            CreateMap<Project, DTO.ProjectReadOnlyDTO>()
                .ReverseMap(); //αντίστροφη αντιστοίχιση
            CreateMap<DTO.ProjectCreateDTO, Project>();
            CreateMap<DTO.ProjectUpdateDTO, Project>();

            CreateMap<Prompt, DTO.PromptReadOnlyDTO>()
                .ReverseMap(); //αντίστροφη αντιστοίχιση
            CreateMap<DTO.PromptCreateDTO, Prompt>();
            CreateMap<DTO.PromptUpdateDTO, Prompt>();


        }

    }
}
