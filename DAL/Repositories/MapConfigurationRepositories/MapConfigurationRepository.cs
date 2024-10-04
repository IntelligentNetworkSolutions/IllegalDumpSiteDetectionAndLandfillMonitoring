using DAL.ApplicationStorage;
using DAL.Interfaces.Repositories.DatasetRepositories;
using DAL.Interfaces.Repositories.MapConfigurationRepositories;
using DAL.Repositories.DatasetRepositories;
using Entities.DatasetEntities;
using Entities.MapConfigurationEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.MapConfigurationRepositories
{
    public class MapConfigurationRepository : BaseResultRepository<MapConfiguration, Guid>, IMapConfigurationRepository
    {
        private readonly ApplicationDbContext _db;
        private static ILogger<MapConfigurationRepository> _logger;
        public MapConfigurationRepository(ApplicationDbContext db, ILogger<MapConfigurationRepository> logger) : base(db)
        {
            _db = db;
            _logger = logger;
        }


        #region Read
        #region Get MapConfig/es 
        public async Task<MapConfiguration> GetMapConfigurationByName(string mapName)
        {
            try
            {
                var list = await _db.MapConfigurations.Where(x => x.MapName.Equals(mapName)).Include(x => x.MapLayerConfigurations).Include(x => x.MapLayerGroupConfigurations).ThenInclude(x => x.MapLayerConfigurations).FirstOrDefaultAsync();
                if(list == null)
                {
                    return new MapConfiguration();
                }
                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
            
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
