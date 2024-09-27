using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.DatasetEntities;

namespace DAL.ApplicationStorage.SeedDatabase.TestSeedData
{
    public static class DatasetDatasetClassesSeedData
    {
        public static void SeedData(ApplicationDbContext dbContext)
            => dbContext.Datasets_DatasetClasses.AddRange([FirstDatasetFirstDatasetClass, FirstDatasetSecondDatasetClass]);

        public static readonly Dataset_DatasetClass FirstDatasetFirstDatasetClass = new Dataset_DatasetClass()
        {
            Id = Guid.Parse("0781163a-677f-4a92-b872-b76603ace0ce"),
            DatasetClassId = DatasetClassesSeedData.FirstDatasetClass.Id,
            DatasetId = DatasetsSeedData.FirstDataset.Id,
            DatasetClassValue = 1
        };

        public static readonly Dataset_DatasetClass FirstDatasetSecondDatasetClass = new Dataset_DatasetClass()
        {
            Id = Guid.Parse("df180578-de29-42e5-8cec-329f2a52a13d"),
            DatasetClassId = DatasetClassesSeedData.SecondDatasetClass.Id,
            DatasetId = DatasetsSeedData.FirstDataset.Id,
            DatasetClassValue = 2
        };

        public static readonly Dataset_DatasetClass SecondDatasetFirstDatasetClass = new Dataset_DatasetClass()
        {
            Id = Guid.Parse("6825c1e3-77cf-4894-b556-55926135881a"),
            DatasetClassId = DatasetClassesSeedData.FirstDatasetClass.Id,
            DatasetId = DatasetsSeedData.SecondDataset.Id,
            DatasetClassValue = 1
        };

        public static readonly Dataset_DatasetClass SecondDatasetSecondDatasetClass = new Dataset_DatasetClass()
        {
            Id = Guid.Parse("671834e7-d588-4789-afd5-b4f47338a8d2"),
            DatasetClassId = DatasetClassesSeedData.SecondDatasetClass.Id,
            DatasetId = DatasetsSeedData.SecondDataset.Id,
            DatasetClassValue = 2
        };
    }
}
