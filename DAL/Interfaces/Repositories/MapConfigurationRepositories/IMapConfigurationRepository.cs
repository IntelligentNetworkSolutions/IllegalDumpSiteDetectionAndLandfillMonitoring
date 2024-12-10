using Entities.MapConfigurationEntities;
using SD;
using System;
using System.Threading.Tasks;

namespace DAL.Interfaces.Repositories.MapConfigurationRepositories
{
    public interface IMapConfigurationRepository : IBaseResultRepository<MapConfiguration, Guid>
    {
        #region Read
        #region Get Datasetclass/es
        Task<ResultDTO<MapConfiguration>> GetMapConfigurationByName(string mapName);
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
