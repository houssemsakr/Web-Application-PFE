using Web_Application_PFE.Models;
using Web_Application_PFE.ViewModels;
using AutoMapper;

namespace Web_Application_PFE.ConfigurationMapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserViewModel>()
                .ReverseMap();

        }
    }
}
