using AutoMapper;
using webapi.Models;

namespace webapi.Profiles
{
    public class PostProfile : Profile
    {

        public PostProfile()
        {
            CreateMap<Entities.Post, PostDto>();
            CreateMap<Entities.Post, PostForCreationDTO>();
            CreateMap<Entities.Post, PostForUpdateDTO>();
            CreateMap<PostForCreationDTO, Entities.Post>();
            CreateMap<PostForUpdateDTO, Entities.Post>();
            
        }
    }
}
