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
    }
}
