using DAL.ApplicationStorage;
using DAL.Interfaces.Repositories.MapConfigurationRepositories;
using Entities.MapConfigurationEntities;
using System;

namespace DAL.Repositories.MapConfigurationRepositories
{
    public class MapLayersConfigurationRepository : BaseResultRepository<MapLayerConfiguration, Guid>, IMapLayersConfigurationRepository
    {
        public MapLayersConfigurationRepository(ApplicationDbContext db) : base(db) {}
    }
}
