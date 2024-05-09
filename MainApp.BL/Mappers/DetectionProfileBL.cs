using AutoMapper;
using DTOs.MainApp.BL.DetectionDTOs;
using Entities.DetectionEntities;

namespace MainApp.BL.Mappers
{
    public class DetectionProfileBL : Profile
    {
        public DetectionProfileBL()
        {
            CreateMap<DetectedDumpSiteDTO, DetectedDumpSite>().ReverseMap();
            CreateMap<DetectionRunDTO, DetectionRun>().ForMember(dest => dest.CreatedBy, opt => opt.Ignore()).ReverseMap();
            CreateMap<DetectionIgnoreZoneDTO, DetectionIgnoreZone>().ReverseMap();
        }
    }
}
