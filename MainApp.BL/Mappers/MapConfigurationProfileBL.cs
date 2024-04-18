using AutoMapper;
using DTOs.MainApp.BL.DatasetDTOs;
using DTOs.MainApp.BL.MapConfigurationDTOs;
using Entities.DatasetEntities;
using Entities.MapConfigurationEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
