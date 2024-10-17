using Newtonsoft.Json;

namespace DTOs.ObjectDetection.API.CocoFormatDTOs
{
    public record CocoCategoryDTO
    {
        public int Id { get; init; }
        
        public required string Name { get; init; }

        [JsonIgnore]
        public string? Supercategory { get; init; }
    }
}