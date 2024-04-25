using DTOs.MainApp.BL.DetectionDTOs;
using SD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainApp.BL.Interfaces.Services.DetectionServices
{
    public interface IDetectionRunService
    {
        #region Read
        #region Get DetectionRun/s
        Task<List<HistoricDataLayerDTO>> GetDetectionRunsWithClasses();
        Task<ResultDTO<DetectionRunDTO>> GetDetectionRunById(Guid id, bool track = false);
        Task<ResultDTO<List<DetectionRunDTO>>> GetAllDetectionRunsIncludingDetectedDumpSites();
        Task<ResultDTO<List<DetectionRunDTO>>> GetAllDetectionRuns();
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
