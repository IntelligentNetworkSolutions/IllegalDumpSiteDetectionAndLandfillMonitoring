using DTOs.Helpers;
using Entities.DatasetEntities;

namespace DAL.ApplicationStorage.SeedDatabase.TestSeedData
{
    public static class ImageAnnotationsSeedData
    {
        public static void SeedData(ApplicationDbContext dbContext)
            => dbContext.ImageAnnotations.AddRange([FirstImageFirstAnnotationFirstClass]);

        public static readonly ImageAnnotation FirstImageFirstAnnotationFirstClass = new ImageAnnotation() 
        { 
            Id = Guid.Parse("132614cb-e6c3-41c8-aaf6-db6d66ec0cb1"),
            IsEnabled = true,

            DatasetClassId = DatasetClassesSeedData.FirstDatasetClass.Id,
            DatasetImageId = DatasetImagesSeedData.FirstDatasetFirstImage.Id,

            Geom = GeoJsonHelpers.ConvertBoundingBoxToPolygon([150, 250, 100, 150]),
            //Geom = WKTToPolygon.GetDummyPolygon(),
            AnnotationJson = null,

            //GeoJson = null,
            UpdatedById = null,
            
            CreatedOn = DateTime.UtcNow,
            CreatedById = UserSeedData.FirstUser.Id,
        };

        public static readonly ImageAnnotation FirstImageSecondAnnotationFirstClass = new ImageAnnotation()
        {
            Id = Guid.Parse("fd7ccf3f-b971-4008-8d78-fb970053d835"),
            IsEnabled = true,

            DatasetClassId = DatasetClassesSeedData.FirstDatasetClass.Id,
            DatasetImageId = DatasetImagesSeedData.FirstDatasetFirstImage.Id,

            Geom = GeoJsonHelpers.ConvertBoundingBoxToPolygon([550, 750, 100, 150]),
            AnnotationJson = null,

            CreatedOn = DateTime.UtcNow,
            CreatedById = UserSeedData.FirstUser.Id,
        };

        public static readonly ImageAnnotation SecondImageFirstAnnotationFirstClass = new ImageAnnotation()
        {
            Id = Guid.Parse("292fb1f8-a57c-4fc1-90f0-9ebd761fdbbc"),
            IsEnabled = true,

            DatasetClassId = DatasetClassesSeedData.FirstDatasetClass.Id,
            DatasetImageId = DatasetImagesSeedData.FirstDatasetFirstImage.Id,

            Geom = GeoJsonHelpers.ConvertBoundingBoxToPolygon([150, 250, 100, 150]),
            AnnotationJson = null,

            CreatedOn = DateTime.UtcNow,
            CreatedById = UserSeedData.FirstUser.Id,
        };

        public static readonly ImageAnnotation SecondImageSecondAnnotationFirstClass = new ImageAnnotation()
        {
            Id = Guid.Parse("b9110c81-6108-4609-a09b-45fbf69b80cd"),
            IsEnabled = true,

            DatasetClassId = DatasetClassesSeedData.FirstDatasetClass.Id,
            DatasetImageId = DatasetImagesSeedData.FirstDatasetFirstImage.Id,

            Geom = GeoJsonHelpers.ConvertBoundingBoxToPolygon([550, 750, 100, 150]),
            AnnotationJson = null,

            CreatedOn = DateTime.UtcNow,
            CreatedById = UserSeedData.FirstUser.Id,
        };
    }
}
