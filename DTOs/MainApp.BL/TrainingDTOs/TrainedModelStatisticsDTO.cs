using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.MainApp.BL.TrainingDTOs
{
    public class TrainedModelStatisticsDTO
    {
        public Guid? Id { get; set; }

        public Guid? TrainedModelId { get; set; }
        public virtual TrainedModelDTO? TrainedModel { get; set; }

        public TimeSpan? TrainingDuration { get; set; } = null;

        public int? TotalParameters { get; set; }

        public double? NumEpochs { get; set; }
        public double? LearningRate { get; set; }

        public double? AvgBoxLoss { get; set; }
        public double? mApp { get; set; }
    }
}
