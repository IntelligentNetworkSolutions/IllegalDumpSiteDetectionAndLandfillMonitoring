using DTOs.MainApp.BL.MapConfigurationDTOs;
using SD;

namespace MainApp.BL.Interfaces.Services.MapConfigurationServices
{
    public interface IMapLayerGroupsConfigurationService
    {
        #region Read
        #region Get MapLayersConfig/es
        Task<ResultDTO<List<MapLayerGroupConfigurationDTO>>> GetAllMapLayerGroupConfigurations();
        Task<ResultDTO<MapLayerGroupConfigurationDTO>> GetMapLayerGroupConfigurationById(Guid mapLayerConfigurationId);
        Task<ResultDTO<MapLayerGroupConfigurationDTO>> GetAllGroupLayersById(Guid mapLayerGroupConfigurationId);

        #endregion
        #endregion

        #region Create
        Task<ResultDTO<MapLayerGroupConfigurationDTO>> CreateMapLayerGroupConfiguration(MapLayerGroupConfigurationDTO mapLayerGroupConfigurationDTO);
        #endregion

        #region Update
        Task<ResultDTO> EditMapLayerGroupConfiguration(MapLayerGroupConfigurationDTO mapLayerGroupConfigurationDTO);
        #endregion

        #region Delete
        Task<ResultDTO> DeleteMapLayerGroupConfiguration(MapLayerGroupConfigurationDTO mapLayerGroupConfigurationDTO);
        #endregion
    }
}
