using AutoMapper;
using DTOs.MainApp.BL.RegisteredDumpsiteDTOs;
using Entities.RegisteredDumpsiteEntities;

namespace MainApp.BL.Mappers;

public class RegisteredDumpsiteProfileBL : Profile
{
    public RegisteredDumpsiteProfileBL()
    {
        CreateMap<RegisteredDumpsite, RegisteredDumpsiteDTO>().ReverseMap();
        CreateMap<CreateRegisteredDumpsiteDTO, RegisteredDumpsite>().ReverseMap();
        CreateMap<RegisteredDumpsiteWasteType, RegisteredDumpsiteWasteTypeDTO>().ReverseMap();
        CreateMap<RegisteredDumpsiteRiskLevel, RegisteredDumpsiteRiskLevelDTO>().ReverseMap();
        CreateMap<RegisteredDumpsiteFile, RegisteredDumpsiteFileDTO>().ReverseMap();
        CreateMap<RegisteredDumpsiteInspection, RegisteredDumpsiteInspectionDTO>()
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => (SD.Enums.RegisteredDumpsiteInspectionStatus)src.RegisteredDumpsiteInspectionStatusId))
            .ReverseMap()
            .ForMember(dest => dest.RegisteredDumpsiteInspectionStatus, opt => opt.Ignore())
            .ForMember(dest => dest.RegisteredDumpsiteInspectionStatusId, opt => opt.MapFrom(src => (int)src.Status))
            .ForMember(dest => dest.Assignments, opt => opt.MapFrom(src => src.Assignments.Select(a => a).ToList()))
            .ReverseMap()
            .ForMember(dest => dest.Assignments, opt => opt.Ignore());
        CreateMap<RegisteredDumpsiteInspectionFile, RegisteredDumpsiteInspectionFileDTO>().ReverseMap();
        CreateMap<InspectionAssignment, InspectionAssignmentDTO>()
            .ForMember(dest => dest.Inspector,
                       opt => opt.MapFrom(src => src.Inspector))
            .ForMember(dest => dest.AssignedOn,
                       opt => opt.MapFrom(src => src.AssignedOn))
            .ForMember(dest => dest.RegisteredDumpsiteInspectionId,
                       opt => opt.MapFrom(src => src.RegisteredDumpsiteInspectionId));

    }
}
