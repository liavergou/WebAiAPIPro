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
            CreateMap<User, DTO.UserCreateDTO>()
                .ReverseMap();

            CreateMap<Project, DTO.ProjectCreateDTO>();
            CreateMap<ConversionJob, DTO.ConversionJobInsertDTO>();
           
        }

    }
}
