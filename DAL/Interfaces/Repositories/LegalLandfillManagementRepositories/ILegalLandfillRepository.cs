using Entities.DetectionEntities;
using Entities.LegalLandfillsManagementEntites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces.Repositories.LegalLandfillManagementRepositories
{
    public interface ILegalLandfillRepository : IBaseResultRepository<LegalLandfill, Guid>
    {
    }
}
