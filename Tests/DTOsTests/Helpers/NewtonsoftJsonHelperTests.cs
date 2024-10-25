using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs.Helpers;
using Newtonsoft.Json;

namespace Tests.DTOsTests.Helpers
{
    public class NewtonsoftJsonHelperTests
    {
        [Fact]
        public void IsValidJson_WithValidJson_ReturnsTrue()
        {
            string json = "{\"name\":\"John\",\"age\":30}";
            Assert.True(NewtonsoftJsonHelper.IsValidJson(json));
        }

        [Fact]
        public void IsValidJson_WithInvalidJson_ReturnsFalse()
        {
            string json = "{\"name\":\"John\",\"age\":30";
            Assert.False(NewtonsoftJsonHelper.IsValidJson(json));
        }

        [Fact]
        public void Serialize_WithObject_ReturnsSnakeCaseJson()
        {
            var obj = new { FirstName = "John", LastName = "Doe" };
            string json = NewtonsoftJsonHelper.Serialize(obj);
            Assert.Contains("first_name", json);
            Assert.Contains("last_name", json);
        }

        [Fact]
        public void Deserialize_WithSnakeCaseJson_ReturnsObject()
        {
            string json = "{\"first_name\":\"John\",\"last_name\":\"Doe\"}";
            var obj = NewtonsoftJsonHelper.Deserialize<dynamic>(json);
            Assert.Equal("John", (string)obj.first_name);
            Assert.Equal("Doe", (string)obj.last_name);
        }

        [Fact]
        public async Task ReadFromFileAsDeserializedJson_WithValidFile_ReturnsCorrectResponse()
        {
            string json = @"{
            ""detections"": [
                {""label"": 1, ""score"": 0.95, ""bbox"": [10.5, 20.5, 30.5, 40.5]},
                {""label"": 2, ""score"": 0.85, ""bbox"": [15.5, 25.5, 35.5, 45.5]}
            ]
        }";
            string filePath = Path.GetTempFileName();
            await File.WriteAllTextAsync(filePath, json);

            try
            {
                var result = await NewtonsoftJsonHelper.ReadFromFileAsDeserializedJson<DetectionRunFinishedResponse>(filePath);
                Assert.True(result.IsSuccess);
                Assert.NotNull(result.Data);
                Assert.Equal(2, result.Data.labels.Length);
                Assert.Equal(2, result.Data.scores.Length);
                Assert.Equal(2, result.Data.bboxes.Length);
                Assert.Equal(1, result.Data.labels[0]);
                Assert.Equal(0.95f, result.Data.scores[0]);
                Assert.Equal(new double[] { 10.5, 20.5, 30.5, 40.5 }, result.Data.bboxes[0]);
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        [Fact]
        public async Task ReadFromFileAsDeserializedJson_WithInvalidJson_ReturnsFailure()
        {
            string json = "{invalid json}";
            string filePath = Path.GetTempFileName();
            await File.WriteAllTextAsync(filePath, json);

            try
            {
                var result = await NewtonsoftJsonHelper.ReadFromFileAsDeserializedJson<DetectionRunFinishedResponse>(filePath);
                Assert.False(result.IsSuccess);
                Assert.Equal("JSON is not valid", result.ErrMsg);
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        [Fact]
        public async Task ReadFromFileAsDeserializedJson_WithMissingDetections_ReturnsFailure()
        {
            string json = "{}";
            string filePath = Path.GetTempFileName();
            await File.WriteAllTextAsync(filePath, json);

            try
            {
                var result = await NewtonsoftJsonHelper.ReadFromFileAsDeserializedJson<DetectionRunFinishedResponse>(filePath);
                Assert.False(result.IsSuccess);
                Assert.Equal("Detections array not found", result.ErrMsg);
            }
            finally
            {
                File.Delete(filePath);
            }
        }
    }

    public class NumericJsonConverterTests
    {
        private readonly NumericJsonConverter _converter = new NumericJsonConverter();

        [Fact]
        public void CanConvert_WithNumericTypes_ReturnsTrue()
        {
            Assert.True(_converter.CanConvert(typeof(double)));
            Assert.True(_converter.CanConvert(typeof(double?)));
            Assert.True(_converter.CanConvert(typeof(float)));
            Assert.True(_converter.CanConvert(typeof(float?)));
        }

        [Fact]
        public void CanConvert_WithNonNumericTypes_ReturnsFalse()
        {
            Assert.False(_converter.CanConvert(typeof(int)));
            Assert.False(_converter.CanConvert(typeof(string)));
        }

        [Fact]
        public void ReadJson_WithFloatValue_ReturnsCorrectValue()
        {
            var reader = new JsonTextReader(new StringReader("3.14"));
            reader.Read();
            var result = _converter.ReadJson(reader, typeof(float), null, new JsonSerializer());
            Assert.IsType<float>(result);
            Assert.Equal(3.14f, result);
        }

        [Fact]
        public void ReadJson_WithDoubleValue_ReturnsCorrectValue()
        {
            var reader = new JsonTextReader(new StringReader("3.14159265359"));
            reader.Read();
            var result = _converter.ReadJson(reader, typeof(double), null, new JsonSerializer());
            Assert.IsType<double>(result);
            Assert.Equal(3.14159265359, result);
        }

        [Fact]
        public void WriteJson_WithFloatValue_WritesRoundedValue()
        {
            var stringWriter = new StringWriter();
            var writer = new JsonTextWriter(stringWriter);
            _converter.WriteJson(writer, 3.14159f, new JsonSerializer());
            Assert.Equal("3.14", stringWriter.ToString());
        }

        [Fact]
        public void WriteJson_WithDoubleValue_WritesRoundedValue()
        {
            var stringWriter = new StringWriter();
            var writer = new JsonTextWriter(stringWriter);
            _converter.WriteJson(writer, 3.14159265359, new JsonSerializer());
            Assert.Equal("3.14", stringWriter.ToString());
        }

        [Fact]
        public void WriteJson_WithNullValue_WritesNull()
        {
            var stringWriter = new StringWriter();
            var writer = new JsonTextWriter(stringWriter);
            _converter.WriteJson(writer, null, new JsonSerializer());
            Assert.Equal("null", stringWriter.ToString());
        }
    }

    public class DetectionRunFinishedResponse
    {
        public int[] labels { get; set; }
        public float[] scores { get; set; }
        public double[][] bboxes { get; set; }
    }
}
