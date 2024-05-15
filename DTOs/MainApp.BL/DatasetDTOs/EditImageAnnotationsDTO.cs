using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.MainApp.BL.DatasetDTOs
{
    public class EditImageAnnotationsDTO
    {
        public Guid? DatasetImageId { get; init; }

        public List<ImageAnnotationDTO>? ImageAnnotations { get; init; }

    }
}
