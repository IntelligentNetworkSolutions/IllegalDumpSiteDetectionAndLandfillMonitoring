using DTOs.MainApp.BL.DatasetDTOs;
using MainApp.BL.Interfaces.Services;
using SD;
using DAL.Interfaces.Repositories.DatasetRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.DatasetEntities;
using AutoMapper;

namespace MainApp.BL.Services
{
    public class ImageAnnotationsService:IImageAnnotationsService
    {
        private readonly IImageAnnotationsRepository _imageAnnotationsRepository;
        private readonly IDatasetImagesRepository _datasetImagesRepository;
        private readonly IMapper _mapper;

        public ImageAnnotationsService(IImageAnnotationsRepository imageAnnotationsRepository,
            IDatasetImagesRepository datasetImagesRepository,
            IMapper mapper
            ) 
        {
            _imageAnnotationsRepository = imageAnnotationsRepository;
            _datasetImagesRepository = datasetImagesRepository;
            _mapper = mapper;
        }

        public async Task<ResultDTO<bool>> BulkUpdateImageAnnotations(EditImageAnnotationsDTO editImageAnnotations)
        {
            var datasetImageAnnDb = await _imageAnnotationsRepository.GetAll(filter: x => x.DatasetImageId == editImageAnnotations.DatasetImageId);                

            var updateDTOList = editImageAnnotations.ImageAnnotations.Where(x => datasetImageAnnDb.Data.Any(y => y.Id == x.Id));
            var deleteList = datasetImageAnnDb.Data.Where(x=> !editImageAnnotations.ImageAnnotations.Select(y => y.Id).Contains(x.Id));
            var insertDTOList = editImageAnnotations.ImageAnnotations.Where(x => x.Id == null);

            List<ImageAnnotation> updateList = new List<ImageAnnotation>();
            List<ImageAnnotation> insertList = new List<ImageAnnotation>();

            foreach ( var updateItemDto in updateDTOList )
            {                
                var itemDb = datasetImageAnnDb.Data.First(x => x.Id == updateItemDto.Id);
                var item = _mapper.Map<ImageAnnotation>(updateItemDto);
                item.CreatedById = itemDb.CreatedById;
                item.CreatedOn = itemDb.CreatedOn;
                item.UpdatedOn = DateTime.UtcNow;
                updateList.Add(item);
            }

            foreach (var insertItemDto in insertDTOList)
            {
                var item = _mapper.Map<ImageAnnotation>(insertItemDto);
                item.CreatedOn = DateTime.UtcNow;
                insertList.Add(item);
            }

            var res = await _imageAnnotationsRepository.BulkUpdateImageAnnotations(insertList, updateList, deleteList.ToList());

            return new ResultDTO<bool>(res, res, "", null);
        }

        public async Task<List<ImageAnnotationDTO>> GetImageAnnotationsByImageId(Guid datasetImageId)
        {
            var datasetImageDb = await _imageAnnotationsRepository.GetAll(filter: x => x.DatasetImageId == datasetImageId);
            var datasetImages = datasetImageDb.Data ?? new List<ImageAnnotation>();
            var datasetImagesDto = _mapper.Map<List<ImageAnnotationDTO>>(datasetImages) ?? throw new Exception("Dataset Images Annotatons list not found");
            return datasetImagesDto;
        }
    }
}
