using DTOs.MainApp.BL.DatasetDTOs;
using SD;

namespace MainApp.BL.Interfaces.Services
{
    public interface IImageAnnotationsService
    {
        #region Read
        Task<ResultDTO<List<ImageAnnotationDTO>>> GetImageAnnotationsByImageId(Guid datasetImageId);
        #endregion

        #region Create/Update
        Task<ResultDTO<bool>> BulkUpdateImageAnnotations(EditImageAnnotationsDTO editImageAnnotations);
        #endregion

        #region Delete        
        #endregion
    }
}
