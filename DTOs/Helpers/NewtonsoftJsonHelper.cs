using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs.ObjectDetection.API.Responses.DetectionRun;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using SD;

namespace DTOs.Helpers
{
    public static class NewtonsoftJsonHelper
    {
        private static readonly JsonSerializerSettings _settings;

        public static bool IsValidJson(string jsonString)
        {
            try
            {
                JToken.Parse(jsonString);
                // If it gets here, the JSON is well-formed
                return true;
            }
            catch (JsonReaderException)
            {
                // JSON was not in a valid format
                return false;
            }
        }

        static NewtonsoftJsonHelper()
        {
            _settings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                },
                Formatting = Formatting.Indented,
                Converters = { new NumericJsonConverter() }
            };
        }

        public static string Serialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, _settings);
        }

        public static T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, _settings);
        }

        public static async Task<ResultDTO<DetectionRunFinishedResponse>> ReadFromFileAsDeserializedJson<TJson>(string filePath) where TJson : class
        {
            try
            {
                string jsonContent = await File.ReadAllTextAsync(filePath);
                if (NewtonsoftJsonHelper.IsValidJson(jsonContent) == false)
                    return ResultDTO<DetectionRunFinishedResponse>.Fail("JSON is not valid");


                var jsonObject = JObject.Parse(jsonContent);
                if (jsonObject == null)
                    return ResultDTO<DetectionRunFinishedResponse>.Fail("Parsing failed");

                var detectionsArray = jsonObject["detections"] as JArray;
                if (detectionsArray == null)
                    return ResultDTO<DetectionRunFinishedResponse>.Fail("Detections array not found");

                var labels = new List<int>();
                var scores = new List<float>();
                var bboxes = new List<double[]>();

                foreach (JToken detection in detectionsArray)
                {
                    int label = (int)detection["label"]!;
                    float score = (float)detection["score"]!;
                    double[]? bbox = detection["bbox"]!.ToObject<double[]>();

                    labels.Add(label);
                    scores.Add(score);
                    bboxes.Add(bbox!);
                }

                DetectionRunFinishedResponse response = new DetectionRunFinishedResponse
                {
                    labels = labels.ToArray(),
                    scores = scores.ToArray(),
                    bboxes = bboxes.ToArray()
                };

                if (response == null)
                    return ResultDTO<DetectionRunFinishedResponse>.Fail("Failed Deserialization");

                return ResultDTO<DetectionRunFinishedResponse>.Ok(response);
            }
            catch (Exception ex)
            {
                return ResultDTO<DetectionRunFinishedResponse>.ExceptionFail(ex.Message, ex);
            }
        }
    }

    public class NumericJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(double) || objectType == typeof(double?) ||
                   objectType == typeof(float) || objectType == typeof(float?);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            if (reader.TokenType == JsonToken.Float || reader.TokenType == JsonToken.Integer)
            {
                if (objectType == typeof(float) || objectType == typeof(float?))
                    return Convert.ToSingle(reader.Value);
                return Convert.ToDouble(reader.Value);
            }

            throw new JsonSerializationException($"Unexpected token {reader.TokenType} when parsing numeric value.");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                double doubleValue = Convert.ToDouble(value);
                writer.WriteValue(Math.Round(doubleValue, 2));
            }
        }
    }
}
