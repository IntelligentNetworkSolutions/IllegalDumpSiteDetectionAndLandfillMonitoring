using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.MainApp.BL.DatasetDTOs
{
    public record CreateDatasetDTO : DatasetDTO
    {
        public List<DatasetClassDTO>? AllDatasetClasses { get; set; }
        public List<Guid>? InsertedDatasetClasses { get; set; }
    }
}
