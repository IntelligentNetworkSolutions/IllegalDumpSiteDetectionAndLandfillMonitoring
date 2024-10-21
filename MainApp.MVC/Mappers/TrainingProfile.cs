using AutoMapper;
using DTOs.MainApp.BL.DetectionDTOs;
using DTOs.MainApp.BL.TrainingDTOs;
using MainApp.MVC.ViewModels.IntranetPortal.Detection;
using MainApp.MVC.ViewModels.IntranetPortal.Training;

namespace MainApp.MVC.Mappers
{
    public class TrainingProfile : Profile
    {
        public TrainingProfile()
        {
            CreateMap<TrainingRunDTO, TrainingRunViewModel>().ReverseMap();    
        }
    }
}
