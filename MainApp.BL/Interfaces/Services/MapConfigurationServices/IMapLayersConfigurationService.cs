using DTOs.MainApp.BL.MapConfigurationDTOs;
using SD;

namespace MainApp.BL.Interfaces.Services.MapConfigurationServices
{
    public interface IMapLayersConfigurationService
    {
        #region Read
        #region Get MapLayersConfig/es
        Task<ResultDTO<List<MapLayerConfigurationDTO>>> GetAllMapLayerConfigurations();
        Task<ResultDTO<MapLayerConfigurationDTO>> GetMapLayerConfigurationById(Guid mapLayerConfigurationId);
        #endregion
        #endregion

        #region Create
        Task<ResultDTO<MapLayerConfigurationDTO>> CreateMapLayerConfiguration(MapLayerConfigurationDTO mapLayerConfigurationDTO);
        #endregion

        #region Update
        Task<ResultDTO> EditMapLayerConfiguration(MapLayerConfigurationDTO mapLayerConfigurationDTO);
        #endregion

        #region Delete
        Task<ResultDTO> DeleteMapLayerConfiguration(MapLayerConfigurationDTO mapLayerConfigurationDTO);
        #endregion
    }
}
