using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DTOs.MainApp.BL.TrainingDTOs;
using Entities.TrainingEntities;

namespace MainApp.BL.Mappers
{
    public class TrainingProfileBL : Profile
    {
        public TrainingProfileBL()
        {
            CreateMap<TrainingRunDTO, TrainingRun>().ReverseMap();
            CreateMap<TrainedModelDTO, TrainedModel>().ReverseMap();
            CreateMap<TrainedModelStatisticsDTO, TrainedModelStatistics>().ReverseMap();
            CreateMap<TrainingRunTrainParamsDTO, TrainingRunTrainParams>().ReverseMap();
        }
    }
}
