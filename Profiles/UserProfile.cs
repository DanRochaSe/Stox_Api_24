using AutoMapper;
using webapi.Models;

namespace webapi.Profiles
{
    public class UserProfile : Profile
    {

        public UserProfile()
        {
            CreateMap<Entities.User, UserForAuthentication>();
            CreateMap<UserForAuthentication, Entities.User>();
            CreateMap<UserForCreationDTO, Entities.User>();
            CreateMap<Entities.User, UserForCreationDTO>();
            CreateMap<Entities.User, UserForReturnDTO>();
            CreateMap<UserForReturnDTO, Entities.User>();
        }

    }
}
