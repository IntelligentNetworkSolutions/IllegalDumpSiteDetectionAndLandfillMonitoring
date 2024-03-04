using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.MainApp.BL.DatasetDTOs
{
    public record CreateDatasetClassDTO
    {
        public string ClassName { get; set; }
        public Guid? ParentClassId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? CreatedById { get; set; }

    }
    
}
