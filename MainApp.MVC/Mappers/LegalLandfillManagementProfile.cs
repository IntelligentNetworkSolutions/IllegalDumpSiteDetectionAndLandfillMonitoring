using AutoMapper;
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
            CreateMap<LegalLandfillPointCloudFileDTO, LegalLandfillPointCloudFileViewModel>()
                .ForMember(dest => dest.ScanDateTime, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ReverseMap();
        }
    }
}
