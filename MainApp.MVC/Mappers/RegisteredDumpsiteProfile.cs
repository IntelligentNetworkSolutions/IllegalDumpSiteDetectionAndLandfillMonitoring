using AutoMapper;
using DTOs.MainApp.BL.RegisteredDumpsiteDTOs;
using MainApp.MVC.ViewModels.IntranetPortal.RegisteredDumpsite;

namespace MainApp.MVC.Mappers;

public class RegisteredDumpsiteProfile : Profile
{
    public RegisteredDumpsiteProfile()
    {
        CreateMap<RegisteredDumpsiteDTO, RegisteredDumpsiteViewModel>().ReverseMap();
        CreateMap<RegisteredDumpsiteWasteTypeDTO, RegisteredDumpsiteWasteTypeViewModel>().ReverseMap();
        CreateMap<RegisteredDumpsiteRiskLevelDTO, RegisteredDumpsiteRiskLevelViewModel>().ReverseMap();
        CreateMap<CompleteInspectionDTO, CompleteInspectionViewModel>().ReverseMap();
    }
}
