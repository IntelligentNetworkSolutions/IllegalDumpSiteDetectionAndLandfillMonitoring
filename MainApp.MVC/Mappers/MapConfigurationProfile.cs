using AutoMapper;
using DTOs.MainApp.BL.MapConfigurationDTOs;
using MainApp.MVC.ViewModels.IntranetPortal.MapConfiguration;

namespace MainApp.MVC.Mappers
{
    public class MapConfigurationProfile : Profile
    {
        public MapConfigurationProfile()
        {
            CreateMap<MapConfigurationDTO, MapConfigurationViewModel>().ReverseMap();
            CreateMap<MapLayerConfigurationDTO, MapLayerConfigurationViewModel>().ReverseMap();
            CreateMap<MapLayerGroupConfigurationDTO, MapLayerGroupConfigurationViewModel>().ReverseMap();

        }
    }
}
