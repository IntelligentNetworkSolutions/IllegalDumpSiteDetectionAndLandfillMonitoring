using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs.ObjectDetection.API.CocoFormatDTOs;
using Newtonsoft.Json;

namespace Tests.DTOsTests.ObjectDetectionApi.CocoFormatDTOs
{
    public class CocoAnnotationDTOTests
    {
        [Fact]
        public void CocoAnnotationDTO_Initialization_SetsPropertiesCorrectly()
        {
            var annotation = new CocoAnnotationDTO
            {
                Id = 1,
                ImageId = 2,
                CategoryId = 3,
                Bbox = new List<float> { 10, 20, 30, 40 },
                Segmentation = new float[][] { new float[] { 1, 2, 3, 4 } },
                IsCrowd = 0
            };

            Assert.Equal(1, annotation.Id);
            Assert.Equal(2, annotation.ImageId);
            Assert.Equal(3, annotation.CategoryId);
            Assert.Equal(new List<float> { 10, 20, 30, 40 }, annotation.Bbox);
            Assert.Equal(new float[][] { new float[] { 1, 2, 3, 4 } }, annotation.Segmentation);
            Assert.Equal(0, annotation.IsCrowd);
        }

        [Fact]
        public void CocoAnnotationDTO_AreaCalculation_ReturnsCorrectValue()
        {
            var annotation = new CocoAnnotationDTO
            {
                Bbox = new List<float> { 10, 20, 30, 40 }
            };

            Assert.Equal(1200, annotation.Area);
        }

        [Fact]
        public void CocoAnnotationDTO_AreaCalculation_ReturnsZeroForNullBbox()
        {
            var annotation = new CocoAnnotationDTO
            {
                Bbox = null
            };

            Assert.Equal(0, annotation.Area);
        }

        [Fact]
        public void CocoAnnotationDTO_AreaCalculation_ReturnsZeroForIncompleteBbox()
        {
            var annotation = new CocoAnnotationDTO
            {
                Bbox = new List<float> { 10, 20, 30 }
            };

            Assert.Equal(0, annotation.Area);
        }

        [Fact]
        public void CocoAnnotationDTO_Serialization_WorksCorrectly()
        {
            var annotation = new CocoAnnotationDTO
            {
                Id = 1,
                ImageId = 2,
                CategoryId = 3,
                Bbox = new List<float> { 10, 20, 30, 40 },
                Segmentation = new float[][] { new float[] { 1, 2, 3, 4 } },
                IsCrowd = 0
            };

            var json = JsonConvert.SerializeObject(annotation);
            var expected = "{\"id\":1,\"image_id\":2,\"category_id\":3,\"bbox\":[10.0,20.0,30.0,40.0],\"segmentation\":[[1.0,2.0,3.0,4.0]],\"area\":1200.0,\"iscrowd\":0}";

            Assert.Equal(expected, json);
        }

        [Fact]
        public void CocoAnnotationDTO_Deserialization_WorksCorrectly()
        {
            var json = "{\"id\":1,\"image_id\":2,\"category_id\":3,\"bbox\":[10.0,20.0,30.0,40.0],\"segmentation\":[[1.0,2.0,3.0,4.0]],\"iscrowd\":0}";

            var annotation = JsonConvert.DeserializeObject<CocoAnnotationDTO>(json);

            Assert.NotNull(annotation);
            Assert.Equal(1, annotation.Id);
            Assert.Equal(2, annotation.ImageId);
            Assert.Equal(3, annotation.CategoryId);
            Assert.Equal(new List<float> { 10, 20, 30, 40 }, annotation.Bbox);
            Assert.Equal(new float[][] { new float[] { 1, 2, 3, 4 } }, annotation.Segmentation);
            Assert.Equal(0, annotation.IsCrowd);
            Assert.Equal(1200, annotation.Area);
        }
    }

    public class SegmentationConverterTests
    {
        private readonly SegmentationConverter _converter = new SegmentationConverter();

        [Fact]
        public void WriteJson_WithNullValue_WritesEmptyArray()
        {
            var result = WriteJsonWithConverter(null);
            Assert.Equal("[]", result);
        }

        [Fact]
        public void WriteJson_WithEmptyArray_WritesEmptyArray()
        {
            var result = WriteJsonWithConverter(Array.Empty<float[]>());
            Assert.Equal("[]", result);
        }

        [Fact]
        public void WriteJson_WithValidArray_WritesCorrectJson()
        {
            var value = new float[][] { new float[] { 1, 2 }, new float[] { 3, 4 } };
            var result = WriteJsonWithConverter(value);
            Assert.Equal("[[1.0,2.0],[3.0,4.0]]", result);
        }

        [Fact]
        public void ReadJson_WithValidJson_ReturnsCorrectArray()
        {
            var json = "[[1.0,2.0],[3.0,4.0]]";
            var reader = new JsonTextReader(new StringReader(json));
            var result = _converter.ReadJson(reader, typeof(float[][]), null, new JsonSerializer());

            Assert.NotNull(result);
            Assert.IsType<float[][]>(result);
            var typedResult = (float[][])result;
            Assert.Equal(2, typedResult.Length);
            Assert.Equal(new float[] { 1, 2 }, typedResult[0]);
            Assert.Equal(new float[] { 3, 4 }, typedResult[1]);
        }

        private string WriteJsonWithConverter(float[][] value)
        {
            using (var stringWriter = new StringWriter())
            using (var jsonWriter = new JsonTextWriter(stringWriter))
            {
                _converter.WriteJson(jsonWriter, value, new JsonSerializer());
                return stringWriter.ToString();
            }
        }
    }
}