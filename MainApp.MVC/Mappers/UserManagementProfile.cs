using AutoMapper;
using DTOs.MainApp.MVC;
using MainApp.MVC.Helpers;
using MainApp.MVC.ViewModels.IntranetPortal.UserManagement;
using SD;

namespace MainApp.MVC.Mappers
{
    public class UserManagementProfile : Profile
    {
        public UserManagementProfile()
        {
            CreateMap<UserManagementDTO, UserManagementCreateUserViewModel>().ReverseMap();
            CreateMap<UserManagementDTO, UserManagementEditUserViewModel>().ReverseMap();
            CreateMap<RoleManagementDTO, UserManagementCreateRoleViewModel>().ReverseMap();
            CreateMap<RoleManagementDTO, UserManagementEditRoleViewModel>().ReverseMap();
            CreateMap<UserDTO, UserManagementUserViewModel>().ReverseMap();

            CreateMap<AuthClaim, RoleClaimDTO>()
            .ForMember(dest => dest.ClaimType, opt => opt.MapFrom(src => src.Value))
            .ForMember(dest => dest.ClaimValue, opt => opt.MapFrom(src => src.Description));

            CreateMap<AuthClaim, UserClaimDTO>()
            .ForMember(dest => dest.ClaimType, opt => opt.MapFrom(src => src.Value))
            .ForMember(dest => dest.ClaimValue, opt => opt.MapFrom(src => src.Description));
        }
    }
}
