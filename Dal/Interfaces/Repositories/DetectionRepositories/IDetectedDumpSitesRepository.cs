﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.DetectionEntities;

namespace DAL.Interfaces.Repositories.DetectionRepositories
{
    public interface IDetectedDumpSitesRepository : IBaseResultRepository<DetectedDumpSite, Guid>
    {
    }
}
