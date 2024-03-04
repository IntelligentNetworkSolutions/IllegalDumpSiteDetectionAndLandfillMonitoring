using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.MainApp.BL.DatasetDTOs
{
    public record Dataset_DatasetClassDTO
    {
        public Guid? DatasetId { get; set; }
        public DatasetDTO? Dataset { get; set; }

        public Guid? DatasetClassId { get; set; }
        public DatasetClassDTO? DatasetClass { get; set; }
    }
}
