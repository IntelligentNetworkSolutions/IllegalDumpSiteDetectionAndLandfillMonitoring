using AutoMapper;
using DTOs.MainApp.BL.DetectionDTOs;
using MainApp.MVC.ViewModels.IntranetPortal.Detection;

namespace MainApp.MVC.Mappers
{
    public class DetectionProfile : Profile
    {
        public DetectionProfile()
        {
            CreateMap<DetectionRunDTO, DetectionRunViewModel>().ReverseMap();
            CreateMap<DetectedDumpSiteDTO, DetectedDumpSiteViewModel>().ReverseMap();
            CreateMap<DetectionInputImageDTO, DetectionInputImageViewModel>().ReverseMap();

        }
    }
}
