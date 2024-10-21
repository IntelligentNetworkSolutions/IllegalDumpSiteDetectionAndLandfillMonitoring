using Newtonsoft.Json;

namespace DTOs.ObjectDetection.API.CocoFormatDTOs
{
    public record CocoAnnotationDTO
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; init; }
        
        [JsonProperty(PropertyName = "image_id")]
        public int ImageId { get; init; }
        
        [JsonProperty(PropertyName = "category_id")]
        public int CategoryId { get; init; }

        [JsonProperty(PropertyName = "bbox")]
        public required List<float> Bbox { get; init; }

        [JsonProperty(PropertyName = "segmentation")]
        [JsonConverter(typeof(SegmentationConverter))]
        public float[][] Segmentation { get; init; } = [];

        [JsonProperty(PropertyName = "area")]
        public float Area => CalculateArea();
        
        [JsonProperty(PropertyName = "iscrowd")]
        public int IsCrowd { get; init; }

        private float CalculateArea()
        {
            if (Bbox != null && Bbox.Count == 4)
                return Bbox[2] * Bbox[3]; // width * height
        
            return 0;
        }
    }

    public class SegmentationConverter : JsonConverter<float[][]>
    {
        public override void WriteJson(JsonWriter writer, float[][] value, JsonSerializer serializer)
        {
            if (value == null || value.Length == 0)
            {
                writer.WriteStartArray();
                writer.WriteEndArray();
            }
            else
            {
                serializer.Serialize(writer, value);
            }
        }

        public override float[][] ReadJson(JsonReader reader, Type objectType, float[][] existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize<float[][]>(reader);
        }
    }
}