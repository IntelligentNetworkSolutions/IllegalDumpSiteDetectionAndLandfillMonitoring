using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.DetectionEntities;

namespace DAL.Interfaces.Repositories.DetectionRepositories
{
    public interface IDetectionRunsRepository : IBaseResultRepository<DetectionRun, Guid>
    {
        #region Read
        #region Get DetectionRun/s
        Task<List<DetectionRun>> GetDetectionRunsWithClasses();
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
