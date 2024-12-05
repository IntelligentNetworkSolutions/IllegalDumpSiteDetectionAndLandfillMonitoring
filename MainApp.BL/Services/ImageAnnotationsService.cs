using AutoMapper;
using DAL.Interfaces.Repositories.DatasetRepositories;
using DTOs.MainApp.BL.DatasetDTOs;
using Entities.DatasetEntities;
using MainApp.BL.Interfaces.Services;
using SD;

namespace MainApp.BL.Services
{
    public class ImageAnnotationsService : IImageAnnotationsService
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

            IEnumerable<ImageAnnotationDTO> updateDTOList = editImageAnnotations.ImageAnnotations.Where(x => datasetImageAnnDb.Data.Any(y => y.Id == x.Id));
            IEnumerable<ImageAnnotation> deleteList = datasetImageAnnDb.Data.Where(x => !editImageAnnotations.ImageAnnotations.Select(y => y.Id).Contains(x.Id));
            IEnumerable<ImageAnnotationDTO> insertDTOList = editImageAnnotations.ImageAnnotations.Where(x => x.Id == null);

            List<ImageAnnotation> updateList = new List<ImageAnnotation>();
            List<ImageAnnotation> insertList = new List<ImageAnnotation>();

            foreach (var updateItemDto in updateDTOList)
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

        public async Task<ResultDTO<List<ImageAnnotationDTO>>> GetImageAnnotationsByImageId(Guid datasetImageId)
        {
            var datasetImageResult = await _imageAnnotationsRepository.GetAll(filter: x => x.DatasetImageId == datasetImageId);
            if (datasetImageResult.IsSuccess == false && datasetImageResult.HandleError())
                return ResultDTO<List<ImageAnnotationDTO>>.Fail(datasetImageResult.ErrMsg!);

            var datasetImagesDto = _mapper.Map<List<ImageAnnotationDTO>>(datasetImageResult.Data);

            if (datasetImagesDto == null || !datasetImagesDto.Any())
            {
                return ResultDTO<List<ImageAnnotationDTO>>.Fail("Dataset Images Annotations list not found");
            }

            return ResultDTO<List<ImageAnnotationDTO>>.Ok(datasetImagesDto);
        }
    }
}
