using AutoMapper;
using DTOs.MainApp.BL;
using DTOs.MainApp.BL.DatasetDTOs;
using Entities;
using Entities.DatasetEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainApp.BL.Mappers
{
    public class DatasetProfileBL : Profile
    {
        public DatasetProfileBL()
        {
            CreateMap<DatasetDTO, Dataset>().ReverseMap();
            CreateMap<DatasetClassDTO, DatasetClass>().ReverseMap();
            CreateMap<CreateDatasetClassDTO, DatasetClass>()
                .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => DateTime.UtcNow));
            CreateMap<EditDatasetClassDTO, DatasetClass>().ReverseMap();
            CreateMap<DatasetImageDTO, DatasetImage>().ReverseMap();
            CreateMap<EditDatasetImageDTO, DatasetImage>()
                .ForMember(dest => dest.UpdatedOn, opt => opt.MapFrom(src => DateTime.UtcNow));
            CreateMap<ImageAnnotationDTO, ImageAnnotation>().ReverseMap();
            CreateMap<Dataset_DatasetClassDTO, Dataset_DatasetClass>().ReverseMap();
        }
    }
}
