using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.DatasetEntities;

namespace DAL.ApplicationStorage.SeedDatabase.TestSeedData
{
    public static class DatasetClassesSeedData
    {
        public static void SeedData(ApplicationDbContext dbContext)
            => dbContext.DatasetClasses.AddRange([FirstDatasetClass, SecondDatasetClass]);

        public static readonly DatasetClass FirstDatasetClass = new DatasetClass()
        {
            Id = Guid.Parse("e0fbd0ab-b18c-4a93-8428-8cf0f9e26a76"),
            ClassName = "First Test Dataset Class",
            CreatedById = UserSeedData.FirstUser.Id,
            CreatedOn = DateTime.UtcNow,
            ParentClassId = null,
        };

        public static readonly DatasetClass SecondDatasetClass = new DatasetClass()
        {
            Id = Guid.Parse("d013d1db-4a87-4492-a3c2-60c73d9257c3"),
            ClassName = "Second Test Dataset Class",
            CreatedById = UserSeedData.FirstUser.Id,
            CreatedOn = DateTime.UtcNow,
            ParentClassId = null,
        };
    }
}
