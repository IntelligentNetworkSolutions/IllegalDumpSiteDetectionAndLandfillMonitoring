using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.LegalLandfillsManagementEntites
{
    public class LegalLandfillPointCloudFile : BaseEntity<Guid>
    {
        public string FileName { get; set; }
        public DateTime ScanDateTime { get; set; }
        public Guid LegalLandfillId { get; set; }
        public virtual LegalLandfill? LegalLandfill { get; set; }
    }
}
