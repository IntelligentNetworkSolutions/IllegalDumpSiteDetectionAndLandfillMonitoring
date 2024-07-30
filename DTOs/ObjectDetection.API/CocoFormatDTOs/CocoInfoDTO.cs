using System.Text.Json.Serialization;

namespace DTOs.ObjectDetection.API.CocoFormatDTOs
{
    public record CocoInfoDTO
    {
        public int Year { get; set; } = DateTime.Now.Year;
        public string Version { get; set; } = "1.0";
        public string? Description { get; set; }
        public string? Contributor { get; set; }
        [JsonPropertyName("date_created")]
        public string DateCreated { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");
    }
}