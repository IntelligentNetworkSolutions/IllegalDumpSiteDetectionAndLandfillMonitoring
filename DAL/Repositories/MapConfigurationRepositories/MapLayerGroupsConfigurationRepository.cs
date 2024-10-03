using DAL.ApplicationStorage;
using DAL.Interfaces.Repositories.MapConfigurationRepositories;
using Entities.MapConfigurationEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.MapConfigurationRepositories
{
    public class MapLayerGroupsConfigurationRepository : BaseResultRepository<MapLayerGroupConfiguration, Guid>, IMapLayerGroupsConfigurationRepository
    {
        public MapLayerGroupsConfigurationRepository(ApplicationDbContext db) : base(db)
        {
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
