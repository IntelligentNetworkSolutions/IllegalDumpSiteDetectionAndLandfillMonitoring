using AutoMapper;
using DAL.Interfaces.Repositories.MapConfigurationRepositories;
using DTOs.MainApp.BL.MapConfigurationDTOs;
using MainApp.BL.Interfaces.Services.MapConfigurationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainApp.BL.Services.MapConfigurationServices
{
    public class MapLayersConfigurationService : IMapLayersConfigurationService
    {
        private readonly IMapLayersConfigurationRepository _mapLayersConfigRepository;
        private readonly IMapper _mapper;
        public MapLayersConfigurationService(IMapLayersConfigurationRepository mapLayersConfigRepository, IMapper mapper)
        {
            _mapLayersConfigRepository = mapLayersConfigRepository;
            _mapper = mapper;
        }
        #region Read
        #region Get MapLayersConfig/es
       
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
