namespace DTOs.MainApp.BL.DatasetDTOs
{
    public class DatasetClassForDatasetDTO
    {
        public required Guid DatasetClassId { get; init; }
        public required Guid DatasetDatasetClassId { get; init; }
        public required string ClassName { get; init; }
        public required int ClassValue { get; init; }
    }
}