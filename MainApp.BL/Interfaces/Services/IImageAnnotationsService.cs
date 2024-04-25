using DTOs.MainApp.BL.DatasetDTOs;
using SD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainApp.BL.Interfaces.Services
{
    public interface IImageAnnotationsService
    {
        #region Read
        Task<List<ImageAnnotationDTO>> GetImageAnnotationsByImageId(Guid datasetImageId);
        #endregion

        #region Create/Update
        Task<ResultDTO<bool>> BulkUpdateImageAnnotations(EditImageAnnotationsDTO editImageAnnotations);
        #endregion

        #region Delete        
        #endregion
    }
}
