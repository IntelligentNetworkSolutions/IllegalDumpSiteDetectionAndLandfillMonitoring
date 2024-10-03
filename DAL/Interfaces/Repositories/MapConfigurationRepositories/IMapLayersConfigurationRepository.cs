using Entities.MapConfigurationEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces.Repositories.MapConfigurationRepositories
{
    public interface IMapLayersConfigurationRepository : IBaseResultRepository<MapLayerConfiguration, Guid>
    {
        #region Read
        #region Get MapLayerConfig/es
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
