using DTOs.MainApp.BL.DatasetDTOs;
using X.PagedList;

namespace MainApp.MVC.ViewModels.IntranetPortal.Dataset
{
    public class EditDatasetViewModel
    {
        public List<DatasetClassDTO> UninsertedDatasetRootClasses { get; set; }
        public List<DatasetClassDTO> UninsertedDatasetSubclasses { get; set; }
        public List<DatasetClassDTO> ClassesByDatasetId { get; set; }
        public List<DatasetClassDTO>? ParentDatasetClasses { get; set; }
        public List<DatasetImageDTO> ListOfDatasetImages { get; set; }
        public List<DatasetImageDTO> ListOfAllDatasetImagesUnFiltered { get; set; }
        public int NumberOfDatasetClasses { get; set; }
        public DatasetDTO CurrentDataset { get; set; }
        public int NumberOfChildrenDatasets { get; set; }
        public Guid InsertedDatasetClass { get; set; }
        public int NumberOfDatasetImages { get; set; }
        public List<ImageAnnotationDTO> AllImageAnnotations { get; set; }
        public int NumberOfEnabledImages { get; set; }
        public int NumberOfAnnotatedImages { get; set; }
        public bool AllEnabledImagesHaveAnnotations { get; set; }
        public int NumberOfClassesNeededToPublishDataset { get; set; }
        public int NumberOfImagesNeededToPublishDataset { get; set; }
        public IPagedList<DatasetImageDTO>? PagedImagesList { get; set; }
        public string? SearchByImageName { get; set; }
        public bool? SearchByIsAnnotatedImage { get; set; }
        public bool? SearchByIsEnabledImage { get; set; }
        public int? SearchByShowNumberOfImages { get; set; }
        public string? OrderByImages { get; set; }
    }
}
