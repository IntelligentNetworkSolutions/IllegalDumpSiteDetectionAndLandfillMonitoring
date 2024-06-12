using DocumentFormat.OpenXml.Spreadsheet;
using Entities.DatasetEntities;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.EntitiesTests.DatasetEntittes
{
    public class ImageAnnotationTests
    {
        [Fact]
        public void AnnotationJson_DefaultsToNull()
        {
            var annotation = new ImageAnnotation();
            Assert.Null(annotation.AnnotationJson);
        }

        [Fact]
        public void Geom_CanBeSetAndRetrieved()
        {
            var polygon = new Polygon(new LinearRing(new[] {
                new Coordinate(0, 0),
                new Coordinate(0, 1),
                new Coordinate(1, 1),
                new Coordinate(1, 0),
                new Coordinate(0, 0)
            }));
            var annotation = new ImageAnnotation
            {
                Geom = polygon
            };
            Assert.Equal(polygon, annotation.Geom);
        }
       
        [Fact]
        public void IsEnabled_DefaultsToFalse()
        {
            var annotation = new ImageAnnotation();
            Assert.False(annotation.IsEnabled);
        }

        [Fact]
        public void DatasetImageId_CanBeSetAndRetrieved()
        {
            var datasetImageId = Guid.NewGuid();
            var annotation = new ImageAnnotation
            {
                DatasetImageId = datasetImageId
            };
            Assert.Equal(datasetImageId, annotation.DatasetImageId);
        }

        [Fact]
        public void DatasetClassId_CanBeSetAndRetrieved()
        {
            var datasetClassId = Guid.NewGuid();
            var annotation = new ImageAnnotation
            {
                DatasetClassId = datasetClassId
            };
            Assert.Equal(datasetClassId, annotation.DatasetClassId);
        }

        [Fact]
        public void CreatedById_CanBeSetAndRetrieved()
        {
            var createdById = "user123";
            var annotation = new ImageAnnotation
            {
                CreatedById = createdById
            };
            Assert.Equal(createdById, annotation.CreatedById);
        }

        [Fact]
        public void CreatedOn_DefaultsToUtcNow()
        {
            var beforeCreation = DateTime.UtcNow;
            var annotation = new ImageAnnotation();
            var afterCreation = DateTime.UtcNow;

            Assert.InRange(annotation.CreatedOn, beforeCreation, afterCreation);
        }

        [Fact]
        public void UpdatedById_CanBeSetAndRetrieved()
        {
            var updatedById = "user456";
            var annotation = new ImageAnnotation
            {
                UpdatedById = updatedById
            };
            Assert.Equal(updatedById, annotation.UpdatedById);
        }

        [Fact]
        public void UpdatedOn_CanBeSetAndRetrieved()
        {
            var updatedOn = DateTime.UtcNow;
            var annotation = new ImageAnnotation
            {
                UpdatedOn = updatedOn
            };
            Assert.Equal(updatedOn, annotation.UpdatedOn);
        }
    }
}
