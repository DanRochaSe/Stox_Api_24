using AutoMapper;
using webapi.Models;

namespace webapi.Profiles
{
    public class CompareProfile: Profile
    {
        public CompareProfile()
        {
            CreateMap<Entities.StoxComparison, StoxComparisonForCreationDTO>();
            CreateMap<StoxComparisonForCreationDTO, Entities.StoxComparison>();
        }
    }
}
