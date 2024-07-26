using Entities.DatasetEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.LegalLandfillsManagementEntites
{
    public class LegalLandfill : BaseEntity<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<LegalLandfillPointCloudFile>? LegalLandfillPointCloudFiles { get; set; }
    }
}
