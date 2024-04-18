using Entities.MapConfigurationEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces.Repositories.MapConfigurationRepositories
{
    public interface IMapConfigurationRepository : IBaseResultRepository<MapConfiguration, Guid>
    {
        #region Read
        #region Get Datasetclass/es
        Task<MapConfiguration> GetMapConfigurationByName(string mapName);
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
