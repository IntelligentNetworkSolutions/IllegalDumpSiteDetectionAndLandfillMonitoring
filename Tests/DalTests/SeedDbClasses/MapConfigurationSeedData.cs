using DAL.ApplicationStorage;
using DAL.ApplicationStorage.SeedDatabase.TestSeedData;
using Entities.MapConfigurationEntities;

namespace Tests.DalTests.SeedDbClasses
{
    public class MapConfigurationSeedData
    {
        public static void SeedData(ApplicationDbContext dbContext)
    => dbContext.MapConfigurations.AddRange([FirstMapConfiguration]);

        public static readonly MapConfiguration FirstMapConfiguration =
            new MapConfiguration()
            {
                Id = Guid.Parse("7c70e48e-e056-404f-a343-439b193ab6bd"),
                CenterX = 0,
                CenterY = 0,
                MaxX = 0,
                MaxY = 0,
                MinX = 0,
                MinY = 0,
                Projection = "test projection",
                Resolutions = "test resolutions",
                TileGridJs = "test tileGridJs",
                MapName = "test map name",
                CreatedById = UserSeedData.FirstUser.Id,
                MapLayerConfigurations = new List<MapLayerConfiguration>
                {
                    new MapLayerConfiguration() {
                    Id = Guid.Parse("05158846-8d0c-426a-8df5-f1b16dc42ab5"),
                    CreatedById = UserSeedData.FirstUser.Id,
                    MapConfigurationId = Guid.Parse("7c70e48e-e056-404f-a343-439b193ab6bd"),
                    LayerName = "test layer name",
                    LayerTitleJson = "test layer Tile Json",
                    LayerDescriptionJson = "test layer description",
                    LayerJs = "test layer js",
                    Order = 1,
                    }
                }

            };
    }
}
