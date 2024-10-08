﻿using AutoMapper;
using DTOs.MainApp.BL.LegalLandfillManagementDTOs;
using Entities.LegalLandfillsManagementEntites;
using MainApp.MVC.ViewModels.IntranetPortal.LegalLandfillManagement;

namespace MainApp.MVC.Mappers
{
    public class LegalLandfillManagementProfile : Profile
    {
        public LegalLandfillManagementProfile()
        {
            CreateMap<LegalLandfillDTO, LegalLandfillViewModel>().ReverseMap();
            CreateMap<LegalLandfillPointCloudFileDTO, LegalLandfillPointCloudFileViewModel>().ReverseMap();
            CreateMap<LegalLandfillTruckDTO, LegalLandfillTruckViewModel>().ReverseMap();
            CreateMap<LegalLandfillWasteTypeDTO, LegalLandfillWasteTypeViewModel>().ReverseMap();
            CreateMap<LegalLandfillWasteImportDTO, LegalLandfillWasteImportViewModel>().ReverseMap();
        }
    }
}
