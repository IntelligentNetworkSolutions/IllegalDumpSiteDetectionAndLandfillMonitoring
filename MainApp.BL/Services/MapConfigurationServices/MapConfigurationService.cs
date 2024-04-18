using AutoMapper;
using DAL.Interfaces.Repositories.DatasetRepositories;
using DAL.Interfaces.Repositories.MapConfigurationRepositories;
using DAL.Repositories.DatasetRepositories;
using DTOs.MainApp.BL.DatasetDTOs;
using DTOs.MainApp.BL.MapConfigurationDTOs;
using MainApp.BL.Interfaces.Services.MapConfigurationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainApp.BL.Services.MapConfigurationServices
{
    public class MapConfigurationService : IMapConfigurationService
    {
        private readonly IMapConfigurationRepository _mapConfigRepository;
        private readonly IMapper _mapper;
        public MapConfigurationService(IMapConfigurationRepository mapConfigRepository, IMapper mapper)
        {
            _mapConfigRepository = mapConfigRepository;
            _mapper = mapper;
        }
        #region Read
        #region Get Mapconfig/es
        public async Task<MapConfigurationDTO> GetMapConfigurationByName(string mapName)
        {
            var mapConfing = await _mapConfigRepository.GetMapConfigurationByName(mapName) ?? throw new Exception("Object not found");
            var mapConfigDTOs = _mapper.Map<MapConfigurationDTO>(mapConfing) ?? throw new Exception("Object not found");
            return mapConfigDTOs;
        }
        #endregion
        #endregion

        #region Create

        #endregion

        #region Update

        #endregion

        #region Delete

        #endregion

    }
}
