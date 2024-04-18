using DTOs.MainApp.BL.DatasetDTOs;
using DTOs.MainApp.BL;
using SD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs.MainApp.BL.MapConfigurationDTOs;

namespace MainApp.BL.Interfaces.Services.MapConfigurationServices
{
    public interface IMapConfigurationService
    {
        #region Read
        #region Get Mapconfig/es
        Task<MapConfigurationDTO> GetMapConfigurationByName(string mapName);      
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
