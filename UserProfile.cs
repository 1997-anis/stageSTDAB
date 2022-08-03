using AutoMapper;
using StageStdab.Areas.Identity.Data;
using StageStdab.Areas.Identity.Data.DataTransferObjects;

namespace StageStdab
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<Admin, StageStdabUser>();
            CreateMap<Manager, StageStdabUser>();
            CreateMap<Partner, StageStdabUser>();
            CreateMap<UserForAuthenticationDto, StageStdabUser>().ForMember(dest =>
         dest.Email,
            opt => opt.MapFrom(src => src.Email))
        .ForMember(dest =>
            dest.PasswordHash,
            opt => opt.MapFrom(src => src.Password));
            CreateMap<UserForAuthenticationDto, Admin>();
            CreateMap<UserForAuthenticationDto, Manager>();
            CreateMap<UserForAuthenticationDto, Partner>();


        }
    }
}
