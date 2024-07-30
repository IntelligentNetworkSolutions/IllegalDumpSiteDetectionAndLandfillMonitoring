using Newtonsoft.Json;

namespace DTOs.ObjectDetection.API.CocoFormatDTOs
{
    public record CocoAnnotationDTO
    {
        public int Id { get; init; }
        [JsonProperty(PropertyName = "image_id")]
        public int ImageId { get; init; }
        [JsonProperty(PropertyName = "category_id")]
        public int CategoryId { get; init; }
        public required List<float> Bbox { get; init; }
        public List<List<float>>? Segmentation { get; init; }
        public float Area { get; init; }
        [JsonProperty(PropertyName = "iscrowd")]
        public int IsCrowd { get; init; }
    }
}