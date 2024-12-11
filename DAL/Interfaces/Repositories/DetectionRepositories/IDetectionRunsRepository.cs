using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.DetectionEntities;
using SD;

namespace DAL.Interfaces.Repositories.DetectionRepositories
{
    public interface IDetectionRunsRepository : IBaseResultRepository<DetectionRun, Guid>
    {
        #region Read
        #region Get DetectionRun/s
        Task<ResultDTO<List<DetectionRun>>> GetDetectionRunsWithClasses();
        Task<ResultDTO<List<DetectionRun>>> GetSelectedDetectionRunsWithClasses(List<Guid> selectedDetectionRunsIds);
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
