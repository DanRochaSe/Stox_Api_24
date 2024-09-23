using AutoMapper;
using webapi.Models;

namespace webapi.Profiles
    
{
    public class GoalProfile : Profile
    {
        public GoalProfile()
        {
            CreateMap<Entities.Goals, GoalForCreationDTO>();
            CreateMap<GoalForCreationDTO, Entities.Goals>();
            CreateMap<GoalForReturnDTO, Entities.Goals>();
            CreateMap<Entities.Goals, GoalForReturnDTO>();
        }
    }
}
