using AutoMapper;
using DTOs.MainApp.BL.TrainingDTOs;
using MainApp.MVC.ViewModels.IntranetPortal.Training;

namespace MainApp.MVC.Mappers
{
    public class TrainingProfile : Profile
    {
        public TrainingProfile()
        {
            CreateMap<TrainingRunDTO, TrainingRunViewModel>().ReverseMap();
            CreateMap<TrainingRunDTO, TrainingRunIndexViewModel>().ReverseMap();
            CreateMap<TrainedModelDTO, TrainedModelViewModel>().ReverseMap();
        }
    }
}
