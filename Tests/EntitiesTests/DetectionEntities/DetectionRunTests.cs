using Entities.DetectionEntities;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.EntitiesTests.DetectionEntities
{
    public class DetectionRunTests
    {
        [Fact]
        public void DetectionRun_ShouldInitialize_WithDefaultValues()
        {
            var detectionRun = new DetectionRun();

            Assert.False(detectionRun.IsCompleted);
            Assert.Null(detectionRun.Description);
            Assert.Null(detectionRun.CreatedBy);
            Assert.Null(detectionRun.DetectedDumpSites);
        }

        [Fact]
        public void DetectionRun_ShouldSetAndGetName()
        {
            var detectionRun = new DetectionRun();
            var name = "Test Detection Run";

            detectionRun.Name = name;

            Assert.Equal(name, detectionRun.Name);
        }

        [Fact]
        public void DetectionRun_ShouldSetAndGetDescription()
        {
            var detectionRun = new DetectionRun();
            var description = "This is a test description";

            detectionRun.Description = description;

            Assert.Equal(description, detectionRun.Description);
        }

        [Fact]
        public void DetectionRun_ShouldSetAndGetIsCompleted()
        {
            var detectionRun = new DetectionRun();

            detectionRun.IsCompleted = true;

            Assert.True(detectionRun.IsCompleted);
        }

        [Fact]
        public void DetectionRun_ShouldSetAndGetImagePath()
        {
            var detectionRun = new DetectionRun();
            var imagePath = "/images/test.jpg";

            detectionRun.ImagePath = imagePath;

            Assert.Equal(imagePath, detectionRun.ImagePath);
        }

        [Fact]
        public void DetectionRun_ShouldSetAndGetImageFileName()
        {
            var detectionRun = new DetectionRun();
            var imageFileName = "test.jpg";

            detectionRun.ImageFileName = imageFileName;

            Assert.Equal(imageFileName, detectionRun.ImageFileName);
        }

        [Fact]
        public void DetectionRun_ShouldSetAndGetCreatedById()
        {
            var detectionRun = new DetectionRun();
            var createdById = "user123";

            detectionRun.CreatedById = createdById;

            Assert.Equal(createdById, detectionRun.CreatedById);
        }

        [Fact]
        public void DetectionRun_ShouldSetAndGetCreatedOn()
        {
            var detectionRun = new DetectionRun();
            var createdOn = DateTime.Now;

            detectionRun.CreatedOn = createdOn;

            Assert.Equal(createdOn, detectionRun.CreatedOn);
        }

        [Fact]
        public void DetectionRun_ShouldSetAndGetCreatedBy()
        {
            var detectionRun = new DetectionRun();
            var createdBy = new ApplicationUser { Id = "user123", UserName = "testuser" };

            detectionRun.CreatedBy = createdBy;

            Assert.Equal(createdBy, detectionRun.CreatedBy);
        }

      
    }
}
