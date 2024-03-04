using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DatasetEntities
{
    public class Dataset_DatasetClass : BaseEntity<Guid>
    {
        public Guid DatasetId { get; set; }
        public Dataset Dataset { get; set; }

        public Guid DatasetClassId { get; set; }
        public DatasetClass DatasetClass { get; set; }
    }
}
