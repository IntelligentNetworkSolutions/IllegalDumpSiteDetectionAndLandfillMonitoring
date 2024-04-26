using AutoMapper;
using DTOs.MainApp.BL.DatasetDTOs;
using DTOs.MainApp.BL;
using MainApp.MVC.ViewModels.IntranetPortal.Dataset;
using MainApp.MVC.ViewModels.IntranetPortal.Detection;
using DTOs.MainApp.BL.DetectionDTOs;

namespace MainApp.MVC.Mappers
{
    public class DetectionProfile : Profile
    {
        public DetectionProfile()
        {
            CreateMap<DetectionRunDTO, DetectionRunViewModel>().ReverseMap();
            CreateMap<DetectedDumpSiteDTO, DetectedDumpSiteViewModel>().ReverseMap();

        }
    }
}
