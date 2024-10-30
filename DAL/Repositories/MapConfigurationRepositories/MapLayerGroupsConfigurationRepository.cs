using DAL.ApplicationStorage;
using DAL.Interfaces.Repositories.MapConfigurationRepositories;
using Entities.MapConfigurationEntities;
using System;

namespace DAL.Repositories.MapConfigurationRepositories
{
    public class MapLayerGroupsConfigurationRepository : BaseResultRepository<MapLayerGroupConfiguration, Guid>, IMapLayerGroupsConfigurationRepository
    {
        public MapLayerGroupsConfigurationRepository(ApplicationDbContext db) : base(db) {}
    }
}
