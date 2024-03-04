using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.MainApp.BL.DatasetDTOs
{
    public record EditDatasetImageDTO
    {
        public Guid Id { get; set; }
        public Guid DatasetId { get; set; }
        public bool IsEnabled { get; set; }
        public string Name { get; set; }
        public string? FileName { get; set; }
    }
}
