using Newtonsoft.Json;

namespace DTOs.ObjectDetection.API.CocoFormatDTOs
{
    public record CocoImageDTO
    {
        public int Id { get; init; }
        [JsonProperty(PropertyName = "file_name")]
        public required string FileName { get; init; }
        public required int Width { get; init; }
        public required int Height { get; init; }
        [JsonProperty(PropertyName = "date_captured")]
        public string DateCaptured { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");
        public int License { get; set; } = 1;
    }
}