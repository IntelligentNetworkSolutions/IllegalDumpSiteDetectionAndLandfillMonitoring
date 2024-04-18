using AutoMapper;
using DAL.Interfaces.Repositories.MapConfigurationRepositories;
using MainApp.BL.Interfaces.Services.MapConfigurationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainApp.BL.Services.MapConfigurationServices
{
    public class MapLayerGroupsConfigurationService : IMapLayerGroupsConfigurationService
    {
        private readonly IMapLayerGroupsConfigurationRepository _mapLayerGroupsConfigRepository;
        private readonly IMapper _mapper;
        public MapLayerGroupsConfigurationService(IMapLayerGroupsConfigurationRepository mapLayerGroupsConfigRepository, IMapper mapper)
        {
            _mapLayerGroupsConfigRepository = mapLayerGroupsConfigRepository;
            _mapper = mapper;
        }
        #region Read
        #region Get MapLayerGroupsConfig/es

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
