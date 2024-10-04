using DTOs.MainApp.BL.MapConfigurationDTOs;
using SD;

namespace MainApp.BL.Interfaces.Services.MapConfigurationServices
{
    public interface IMapConfigurationService
    {
        #region Read
        #region Get Mapconfig/es
        Task<MapConfigurationDTO> GetMapConfigurationByName(string mapName);
        Task<ResultDTO<List<MapConfigurationDTO>>> GetAllMapConfigurations();
        Task<ResultDTO<MapConfigurationDTO>> GetMapConfigurationById(Guid mapConfigurationId);
        #endregion
        #endregion

        #region Create
        Task<ResultDTO> CreateMapConfiguration(MapConfigurationDTO mapConfigurationDTO);
        #endregion

        #region Update
        Task<ResultDTO> EditMapConfiguration(MapConfigurationDTO mapConfigurationDTO);

        #endregion

        #region Delete
        Task<ResultDTO> DeleteMapConfiguration(MapConfigurationDTO mapConfigurationDTO);
        #endregion
    }
}
