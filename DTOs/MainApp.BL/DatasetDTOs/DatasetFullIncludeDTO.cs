namespace DTOs.MainApp.BL.DatasetDTOs
{
    public record DatasetFullIncludeDTO
    {
        public DatasetDTO Dataset { get; init; }
        public List<DatasetClassForDatasetDTO> DatasetClassForDataset { get; init; }
        public List<DatasetImageDTO> DatasetImages { get; init; }
        public List<ImageAnnotationDTO> ImageAnnotations { get; init; }

        public DatasetFullIncludeDTO(DatasetDTO dataset, List<DatasetClassForDatasetDTO> datasetClassForDataset, List<DatasetImageDTO> datasetImages, List<ImageAnnotationDTO> imageAnnotations)
        {
            Dataset = dataset;
            DatasetClassForDataset = datasetClassForDataset;
            DatasetImages = datasetImages;
            ImageAnnotations = imageAnnotations;
        }

        public DatasetFullIncludeDTO(DatasetDTO datasetDTO)
        {
            if(datasetDTO is null)
                throw new ArgumentNullException(nameof(datasetDTO));

            Dataset = datasetDTO;
            DatasetClassForDataset = new List<DatasetClassForDatasetDTO>();
            DatasetImages = new List<DatasetImageDTO>();
            ImageAnnotations = new List<ImageAnnotationDTO>();
        }
    }
}