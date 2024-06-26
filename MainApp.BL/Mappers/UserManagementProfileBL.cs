﻿using AutoMapper;
using DTOs.MainApp.BL;
using Entities;
using Microsoft.AspNetCore.Identity;

namespace MainApp.BL.Mappers
{
    public class UserManagementProfileBL : Profile
    {
        public UserManagementProfileBL()
        {
            CreateMap<UserDTO, ApplicationUser>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.NormalizedUserName, opt => opt.Ignore())
                .ForMember(dest => dest.NormalizedEmail, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive ?? false));

            CreateMap<UserManagementDTO, ApplicationUser>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<ApplicationUser, UserManagementDTO>();

            CreateMap<string, IdentityUserRole<string>>()
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src));

            CreateMap<string, IdentityUserClaim<string>>()
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.ClaimValue, opt => opt.MapFrom(src => src));

            CreateMap<IdentityRole, RoleManagementDTO>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.NormalizedName, opt => opt.MapFrom(src => src.NormalizedName));

            CreateMap<string, IdentityRoleClaim<string>>()
            .ForMember(dest => dest.RoleId, opt => opt.Ignore())
            .ForMember(dest => dest.ClaimType, opt => opt.MapFrom(src => "AuthorizationClaim"))
            .ForMember(dest => dest.ClaimValue, opt => opt.MapFrom(src => src));

            CreateMap<RoleManagementDTO, IdentityRole>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<IdentityRole, RoleDTO>();
            CreateMap<ApplicationUser, UserDTO>();

            CreateMap<IdentityRoleClaim<string>, RoleClaimDTO>();
            CreateMap<IdentityUserClaim<string>, UserClaimDTO>();
            CreateMap<IdentityUserRole<string>, UserRoleDTO>();
        }
    }
}
