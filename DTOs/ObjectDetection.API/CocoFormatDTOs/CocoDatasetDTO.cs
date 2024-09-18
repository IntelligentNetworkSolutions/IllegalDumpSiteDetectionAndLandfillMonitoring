using Newtonsoft.Json;

namespace DTOs.ObjectDetection.API.CocoFormatDTOs
{
    public record CocoDatasetDTO
    {
        public CocoInfoDTO? Info { get; init; }
        public List<CocoLicenseDTO>? Licenses { get; init; }
        public required List<CocoImageDTO> Images { get; set; }
        public required List<CocoAnnotationDTO> Annotations { get; set; }
        public required List<CocoCategoryDTO> Categories { get; set; }
    }
}