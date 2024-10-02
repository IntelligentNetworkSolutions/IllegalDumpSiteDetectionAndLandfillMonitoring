using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.DatasetEntities;

namespace DAL.ApplicationStorage.SeedDatabase.TestSeedData
{
    public static class DatasetsSeedData
    {
        public static void SeedData(ApplicationDbContext dbContext) => dbContext.Datasets.AddRange([FirstDataset, SecondDataset]);

        public static readonly Dataset FirstDataset =
            new()
            {
                Id = Guid.Parse("30c33267-51aa-4485-9a42-b813485cb3f6"),
                Name = "First Test Dataset",
                Description = "First Test Dataset for Testing DAL.",
                CreatedById = UserSeedData.FirstUser.Id
            };

        public static readonly Dataset SecondDataset =
            new() 
            {
                Id = Guid.Parse("6ab6c7db-da58-4d66-90fa-c0c2581f0373"),
                Name = "Second Test Dataset",
                Description = "Second Test Dataset for Testing DAL",
                CreatedById = UserSeedData.FirstUser.Id
            };
    }
}
