namespace DTOs.ObjectDetection.API.CocoFormatDTOs
{
    public record CocoLicenseDTO
    {
        public int Id { get; set; } = 1;
        public string Name { get; set; } = "Apache 2.0";
        public string Url { get; set; } = "https://github.com/IntelligentNetworkSolutions/IllegalDumpSiteDetectionAndLandfillMonitoring?tab=Apache-2.0-1-ov-file#readme";
    }
}