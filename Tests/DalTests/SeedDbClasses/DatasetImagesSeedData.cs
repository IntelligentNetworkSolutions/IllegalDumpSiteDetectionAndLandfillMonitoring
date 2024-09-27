using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.DatasetEntities;

namespace DAL.ApplicationStorage.SeedDatabase.TestSeedData
{
    public static class DatasetImagesSeedData
    {
        public static void SeedData(ApplicationDbContext dbContext)
            => dbContext.DatasetImages.AddRange([FirstDatasetFirstImage, FirstDatasetSecondImage, SecondDatasetFirstImage, SecondDatasetSecondImage]);

        public static readonly DatasetImage FirstDatasetFirstImage =
            new()
            {
                Id = Guid.Parse("741eb9d4-76ad-4604-9f50-0517a5e1c808"),
                Name = "First Dataset First Image",
                DatasetId = DatasetsSeedData.FirstDataset.Id,
                FileName = "firsttestdatasetimage",
                ImagePath = "firsttestdatasetimage.png",
                ThumbnailPath = "firsttestdatasetimage.png",
                IsEnabled = true,
                CreatedById = UserSeedData.FirstUser.Id,
                UpdatedById = null,
            };

        public static readonly DatasetImage FirstDatasetSecondImage =
            new()
            {
                Id = Guid.Parse("491c3d10-f9b6-4445-ab6f-77db732c69f6"),
                Name = "First Dataset Second Image",
                DatasetId = DatasetsSeedData.FirstDataset.Id,
                FileName = "secondtestdatasetimage",
                ImagePath = "secondtestdatasetimage.png",
                ThumbnailPath = "secondtestdatasetimage.png",
                IsEnabled = true,
                CreatedById = UserSeedData.FirstUser.Id,
                UpdatedById = null,
            };

        public static readonly DatasetImage SecondDatasetFirstImage =
            new()
            {
                Id = Guid.Parse("9f48542a-127f-41f0-871e-58f4973069b7"),
                Name = "Second Dataset First Dataset Image",
                DatasetId = DatasetsSeedData.SecondDataset.Id,
                FileName = "firsttestdatasetimage",
                ImagePath = "firsttestdatasetimage.png",
                ThumbnailPath = "firsttestdatasetimage.png",
                IsEnabled = true,
                CreatedById = UserSeedData.FirstUser.Id,
                UpdatedById = null,
            };

        public static readonly DatasetImage SecondDatasetSecondImage =
            new()
            {
                Id = Guid.Parse("a1206c64-9c06-4121-8a2d-956a1c7aa6d4"),
                Name = "Second Dataset Second Image",
                DatasetId = DatasetsSeedData.SecondDataset.Id,
                FileName = "secondtestdatasetimage",
                ImagePath = "secondtestdatasetimage.png",
                ThumbnailPath = "secondtestdatasetimage.png",
                IsEnabled = true,
                CreatedById = UserSeedData.FirstUser.Id,
                UpdatedById = null,
            };
    }
}
