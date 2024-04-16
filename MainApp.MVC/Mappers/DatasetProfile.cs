using AutoMapper;
using DTOs.MainApp.BL;
using DTOs.MainApp.BL.DatasetDTOs;
using Entities.DatasetEntities;
using MainApp.MVC.ViewModels.IntranetPortal.Dataset;
using MainApp.MVC.ViewModels.IntranetPortal.UserManagement;
using SD;

namespace MainApp.MVC.Mappers
{
    public class DatasetProfile : Profile
    {
        public DatasetProfile()
        {
            CreateMap<DatasetDTO, DatasetViewModel>().ReverseMap();
            CreateMap<CreateDatasetDTO, CreateDatasetViewModel>().ReverseMap();
            CreateMap<EditDatasetDTO, EditDatasetViewModel>().ReverseMap();
            CreateMap<DatasetClassDTO, DatasetClassViewModel>().ReverseMap();
            CreateMap<CreateDatasetViewModel, DatasetDTO>()
                .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => DateTime.Now));
            CreateMap<Dataset_DatasetClassDTO, Dataset_DatasetClassViewModel>().ReverseMap();

        }
    }
}
