using AutoMapper;
using DTOs.MainApp.BL.DatasetDTOs;
using DTOs.MainApp.BL.LegalLandfillManagementDTOs;
using Entities.DatasetEntities;
using Entities.LegalLandfillsManagementEntites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainApp.BL.Mappers
{
    public class LegalLandfillManagementProfileBL : Profile
    {
        public LegalLandfillManagementProfileBL()
        {
            CreateMap<LegalLandfillDTO, LegalLandfill>().ReverseMap();
            CreateMap<LegalLandfillPointCloudFileDTO, LegalLandfillPointCloudFile>().ReverseMap();
            CreateMap<LegalLandfillTruckDTO, LegalLandfillTruck>().ReverseMap();
            CreateMap<LegalLandfillWasteTypeDTO, LegalLandfillWasteType>().ReverseMap();
            CreateMap<LegalLandfillWasteImportDTO, LegalLandfillWasteImport>().ReverseMap();
        }
    }
}
