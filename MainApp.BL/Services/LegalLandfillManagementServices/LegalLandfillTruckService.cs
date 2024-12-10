using AutoMapper;
using DAL.Interfaces.Repositories.LegalLandfillManagementRepositories;
using DTOs.MainApp.BL.LegalLandfillManagementDTOs;
using Entities.LegalLandfillsManagementEntites;
using MainApp.BL.Interfaces.Services.LegalLandfillManagmentServices;
using Microsoft.Extensions.Logging;
using SD;
using System.Linq.Expressions;

namespace MainApp.BL.Services.LegalLandfillManagementServices
{
    public class LegalLandfillTruckService : ILegalLandfillTruckService
    {
        private readonly ILegalLandfillTruckRepository _legalLandfillTruckRepository;
        private readonly ILegalLandfillWasteImportRepository _legalLandfillWasteImportRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<LegalLandfillTruckService> _logger;
        public LegalLandfillTruckService(ILegalLandfillTruckRepository legalLandfillTruckRepository, IMapper mapper, ILogger<LegalLandfillTruckService> logger, ILegalLandfillWasteImportRepository legalLandfillWasteImportRepository)
        {
            _legalLandfillTruckRepository = legalLandfillTruckRepository;
            _mapper = mapper;
            _logger = logger;
            _legalLandfillWasteImportRepository = legalLandfillWasteImportRepository;
        }

        #region Create
        public async Task<ResultDTO> CreateLegalLandfillTruck(LegalLandfillTruckDTO legalLandfillTruckDTO)
        {
            try
            {
                LegalLandfillTruck legalLandfillTruckEntity = _mapper.Map<LegalLandfillTruck>(legalLandfillTruckDTO);
                if (legalLandfillTruckEntity == null)
                    return ResultDTO.Fail("Mapping landfill truck failed");

                ResultDTO resultCreate = await _legalLandfillTruckRepository.Create(legalLandfillTruckEntity);
                if (resultCreate.IsSuccess == false && resultCreate.HandleError())                
                    return ResultDTO.Fail(resultCreate.ErrMsg!);
                
                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO.ExceptionFail(ex.Message, ex);

            }
        }
        #endregion

        #region Update
        public async Task<ResultDTO> EditLegalLandfillTruck(LegalLandfillTruckDTO legalLandfillTruckDTO)
        {
            try
            {
                LegalLandfillTruck? legalLandfillTruckEntity = _mapper.Map<LegalLandfillTruck>(legalLandfillTruckDTO);
                if (legalLandfillTruckEntity == null)
                    return ResultDTO.Fail("Mapping landfill truck failed");

                ResultDTO resultCreate = await _legalLandfillTruckRepository.Update(legalLandfillTruckEntity);
                if (resultCreate.IsSuccess == false && resultCreate.HandleError())                
                    return ResultDTO.Fail(resultCreate.ErrMsg!);
                
                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
        }
        #endregion

        #region Delete
        public async Task<ResultDTO> DeleteLegalLandfillTruck(LegalLandfillTruckDTO legalLandfillTruckDTO)
        {
            try
            {
                var isTruckUsed = await IsTruckUsedInWasteImports(legalLandfillTruckDTO.Id);
                if (isTruckUsed)
                {
                    return ResultDTO.Fail("Cannot delete this truck. It is being used in waste imports");
                }

                LegalLandfillTruck? legalLandfillTruckEntity = _mapper.Map<LegalLandfillTruck>(legalLandfillTruckDTO);
                if (legalLandfillTruckEntity == null)
                    return ResultDTO.Fail("Mapping landfill truck failed");

                ResultDTO resultCreate = await _legalLandfillTruckRepository.Delete(legalLandfillTruckEntity);
                if (resultCreate.IsSuccess == false && resultCreate.HandleError())                
                    return ResultDTO.Fail(resultCreate.ErrMsg!);
                
                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO> DisableLegalLandfillTruck(LegalLandfillTruckDTO legalLandfillTruckDTO)
        {
            try
            {
                LegalLandfillTruck legalLandfillTruckEntity = _mapper.Map<LegalLandfillTruck>(legalLandfillTruckDTO);
                if (legalLandfillTruckEntity == null)
                    return ResultDTO.Fail("Mapping landfill truck failed");

                legalLandfillTruckEntity.IsEnabled = false;

                ResultDTO resultUpdate = await _legalLandfillTruckRepository.Update(legalLandfillTruckEntity);
                if (resultUpdate.IsSuccess == false && resultUpdate.HandleError())                
                    return ResultDTO.Fail(resultUpdate.ErrMsg!);                

                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
        }


        #endregion

        #region Read
        public async Task<ResultDTO<List<LegalLandfillTruckDTO>>> GetAllLegalLandfillTrucks()
        {
            try
            {
                Expression<Func<LegalLandfillTruck, bool>> filter = truck => truck.IsEnabled;

                ResultDTO<IEnumerable<LegalLandfillTruck>> resultGetAllEntites = await _legalLandfillTruckRepository.GetAll(filter);
                if (resultGetAllEntites.IsSuccess == false && resultGetAllEntites.HandleError())                
                    return ResultDTO<List<LegalLandfillTruckDTO>>.Fail(resultGetAllEntites.ErrMsg!);
                if (resultGetAllEntites.Data == null)
                    return ResultDTO<List<LegalLandfillTruckDTO>>.Fail("Landfill truck not found");                

                List<LegalLandfillTruckDTO> dtos = _mapper.Map<List<LegalLandfillTruckDTO>>(resultGetAllEntites.Data);
                if (dtos == null)
                    return ResultDTO<List<LegalLandfillTruckDTO>>.Fail("Mapping landfill truck list failed");

                return ResultDTO<List<LegalLandfillTruckDTO>>.Ok(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO<List<LegalLandfillTruckDTO>>.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO<LegalLandfillTruckDTO>> GetLegalLandfillTruckById(Guid legalLandfillTruckId)
        {
            try
            {
                ResultDTO<LegalLandfillTruck?> resultGetEntity = await _legalLandfillTruckRepository.GetById(legalLandfillTruckId);

                if (resultGetEntity.IsSuccess == false && resultGetEntity.HandleError())                
                    return ResultDTO<LegalLandfillTruckDTO>.Fail(resultGetEntity.ErrMsg!);
                if (resultGetEntity.Data == null)
                    return ResultDTO<LegalLandfillTruckDTO>.Fail("Legal landfill truck not found");                

                LegalLandfillTruckDTO? dto = _mapper.Map<LegalLandfillTruckDTO>(resultGetEntity.Data);
                if (dto == null)
                    return ResultDTO<LegalLandfillTruckDTO>.Fail("Mapping landfill truck failed");

                return ResultDTO<LegalLandfillTruckDTO>.Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO<LegalLandfillTruckDTO>.ExceptionFail(ex.Message, ex);
            }
        }
        #endregion

        private async Task<bool> IsTruckUsedInWasteImports(Guid truckId)
        {
            var result = await _legalLandfillWasteImportRepository.GetAll(
                 filter: w => w.LegalLandfillTruckId == truckId,
                 track: false
             );

            return result.Data is not null && result.Data.Any();
        }
    }
}
