using AutoMapper;
using webapi.Models;

namespace webapi.Profiles
{
    public class StoxProfile : Profile
    {
        public StoxProfile()
        {
            CreateMap<Entities.Stox, StoxForCreationDTO>();
            CreateMap<StoxForCreationDTO, Entities.Stox>();
        }
    }
}
