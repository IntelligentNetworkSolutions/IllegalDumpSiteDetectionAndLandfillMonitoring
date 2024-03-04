using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.MainApp.BL.DatasetDTOs
{
    public record EditDatasetClassDTO
    {
        public Guid Id { get; set; }
        public string ClassName { get; set; }
        public Guid? ParentClassId { get; set; }
    }
}
