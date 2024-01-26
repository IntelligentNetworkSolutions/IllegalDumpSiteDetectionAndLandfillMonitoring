using AutoMapper;
using DTOs.MainApp.BL;
using Entities;

namespace MainApp.BL.Mappers
{
    public class ApplicationSettingsProfileBL : Profile
    {
        public ApplicationSettingsProfileBL()
        {
            CreateMap<AppSettingDTO, ApplicationSettings>()
                .ForMember(dto => dto.Key, mapOpts => mapOpts.MapFrom(entity => entity.Key))
                .ForMember(dto => dto.Value, mapOpts => mapOpts.MapFrom(entity => entity.Value))
                .ForMember(dto => dto.Description, mapOpts => mapOpts.MapFrom(entity => entity.Description))
                .ForMember(dto => dto.Module, mapOpts => mapOpts.MapFrom(entity => entity.Module))
                .ForMember(dto => dto.DataType, mapOpts => mapOpts.MapFrom(entity => entity.DataType))
                .ReverseMap(); // Other way around
        }
    }
}
