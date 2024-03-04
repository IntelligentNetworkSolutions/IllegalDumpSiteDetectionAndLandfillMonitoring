using DTOs.MainApp.BL.DatasetDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainApp.BL.Interfaces.Services.DatasetServices
{
    public interface IDataset_DatasetClassService
    {
        #region Read
        #region Get obejct/s
        Task<List<Dataset_DatasetClassDTO>> GetDataset_DatasetClassByClassId(Guid classId);
        #endregion
        #endregion

        #region Create
        
        #endregion

        #region Update
        
        #endregion

        #region Delete
      

        #endregion
    }
}
