using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            CreateMap<DetectionRunDTO, DetectionRun>().ReverseMap();
            CreateMap<DetectionIgnoreZoneDTO, DetectionIgnoreZone>().ReverseMap();
        }
    }
}
