using AutoMapper;
using DTOs.MainApp.BL.MapConfigurationDTOs;
using Entities.MapConfigurationEntities;

namespace MainApp.BL.Mappers
{
    public class MapConfigurationProfileBL : Profile
    {
        public MapConfigurationProfileBL()
        {
            CreateMap<MapConfigurationDTO, MapConfiguration>().ReverseMap();
            CreateMap<MapLayerConfigurationDTO, MapLayerConfiguration>().ReverseMap();
            CreateMap<MapLayerGroupConfigurationDTO, MapLayerGroupConfiguration>().ReverseMap();

        }
    }
}
