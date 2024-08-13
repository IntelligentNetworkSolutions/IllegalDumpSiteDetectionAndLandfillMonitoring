using AutoMapper;
using DAL.Interfaces.Helpers;
using DAL.Interfaces.Repositories.LegalLandfillManagementRepositories;
using DAL.Repositories.LegalLandfillManagementRepositories;
using DTOs.MainApp.BL.DetectionDTOs;
using DTOs.MainApp.BL.LegalLandfillManagementDTOs;
using Entities.DetectionEntities;
using Entities.LegalLandfillsManagementEntites;
using MainApp.BL.Interfaces.Services.LegalLandfillManagmentServices;
using Microsoft.Extensions.Logging;
using SD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainApp.BL.Services.LegalLandfillManagementServices
{
    public class LegalLandfillPointCloudFileService : ILegalLandfillPointCloudFileService
    {
        private readonly ILegalLandfillPointCloudFileRepository _legalLandfillPointCloudFileRepository;
        private readonly IAppSettingsAccessor _appSettingsAccessor;
        private readonly IMapper _mapper;
        private readonly ILogger<LegalLandfillPointCloudFileService> _logger;

        public LegalLandfillPointCloudFileService(ILegalLandfillPointCloudFileRepository legalLandfillPointCloudFileRepository, IAppSettingsAccessor appSettingsAccessor, IMapper mapper, ILogger<LegalLandfillPointCloudFileService> logger)
        {
            _legalLandfillPointCloudFileRepository = legalLandfillPointCloudFileRepository;
            _appSettingsAccessor = appSettingsAccessor;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<ResultDTO<List<LegalLandfillPointCloudFileDTO>>> GetAllLegalLandfillPointCloudFiles()
        {
            try
            {
                ResultDTO<IEnumerable<LegalLandfillPointCloudFile>> resultGetAllEntites = await _legalLandfillPointCloudFileRepository.GetAll(includeProperties: "LegalLandfill");

                if (resultGetAllEntites.IsSuccess == false && resultGetAllEntites.HandleError())
                {
                    return ResultDTO<List<LegalLandfillPointCloudFileDTO>>.Fail(resultGetAllEntites.ErrMsg!);
                }

                List<LegalLandfillPointCloudFileDTO> dtos = _mapper.Map<List<LegalLandfillPointCloudFileDTO>>(resultGetAllEntites.Data);
                return ResultDTO<List<LegalLandfillPointCloudFileDTO>>.Ok(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO<List<LegalLandfillPointCloudFileDTO>>.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO<List<LegalLandfillPointCloudFileDTO>>> GetLegalLandfillPointCloudFilesByLandfillId(Guid legalLandfillId)
        {
            try
            {
                ResultDTO<IEnumerable<LegalLandfillPointCloudFile>> resultGetAllEntites = await _legalLandfillPointCloudFileRepository.GetAll(filter: x => x.LegalLandfillId == legalLandfillId);

                if (resultGetAllEntites.IsSuccess == false && resultGetAllEntites.HandleError())
                {
                    return ResultDTO<List<LegalLandfillPointCloudFileDTO>>.Fail(resultGetAllEntites.ErrMsg!);
                }

                List<LegalLandfillPointCloudFileDTO> dtos = _mapper.Map<List<LegalLandfillPointCloudFileDTO>>(resultGetAllEntites.Data);
                return ResultDTO<List<LegalLandfillPointCloudFileDTO>>.Ok(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO<List<LegalLandfillPointCloudFileDTO>>.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO<LegalLandfillPointCloudFileDTO>> GetLegalLandfillPointCloudFilesById(Guid legalLandfillPointCloudFileId)
        {
            try
            {
                ResultDTO<LegalLandfillPointCloudFile?>? resultGetEntity = await _legalLandfillPointCloudFileRepository.GetById(legalLandfillPointCloudFileId,includeProperties:"LegalLandfill");

                if (resultGetEntity.IsSuccess == false && resultGetEntity.HandleError())
                {
                    return ResultDTO<LegalLandfillPointCloudFileDTO>.Fail(resultGetEntity.ErrMsg!);
                }

                LegalLandfillPointCloudFileDTO dto = _mapper.Map<LegalLandfillPointCloudFileDTO>(resultGetEntity.Data);
                return ResultDTO<LegalLandfillPointCloudFileDTO>.Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO<LegalLandfillPointCloudFileDTO>.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO<LegalLandfillPointCloudFileDTO>> CreateLegalLandfillPointCloudFile(LegalLandfillPointCloudFileDTO legalLandfillPointCloudFileDTO)
        {
            try
            {
                LegalLandfillPointCloudFile entity = _mapper.Map<LegalLandfillPointCloudFile>(legalLandfillPointCloudFileDTO);
                ResultDTO<LegalLandfillPointCloudFile> resultCreateAndReturnEntity = await _legalLandfillPointCloudFileRepository.CreateAndReturnEntity(entity);

                if (resultCreateAndReturnEntity.IsSuccess == false && resultCreateAndReturnEntity.HandleError())
                {
                    return ResultDTO<LegalLandfillPointCloudFileDTO>.Fail(resultCreateAndReturnEntity.ErrMsg!);
                }

                if (resultCreateAndReturnEntity.Data is null)
                {
                    return ResultDTO<LegalLandfillPointCloudFileDTO>.Fail("Error Creating Detection Run");
                }
                LegalLandfillPointCloudFileDTO dto = _mapper.Map<LegalLandfillPointCloudFileDTO>(resultCreateAndReturnEntity.Data);
                return ResultDTO<LegalLandfillPointCloudFileDTO>.Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO<LegalLandfillPointCloudFileDTO>.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO> DeleteLegalLandfillPointCloudFile(Guid legalLandfillPointCloudFileId)
        {
            try
            {
                ResultDTO<LegalLandfillPointCloudFile?> resultGetEntity = await _legalLandfillPointCloudFileRepository.GetFirstOrDefault(filter: x => x.Id == legalLandfillPointCloudFileId);
                ResultDTO resultDeleteEntity = await _legalLandfillPointCloudFileRepository.Delete(resultGetEntity.Data);
                if (resultGetEntity.IsSuccess == false && resultGetEntity.HandleError())
                {
                    return ResultDTO.Fail(resultGetEntity.ErrMsg!);
                }
               
                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<ResultDTO<LegalLandfillPointCloudFileDTO>> EditLegalLandfillPointCloudFile(LegalLandfillPointCloudFileDTO legalLandfillPointCloudFileDTO)
        {
            var resultGetEntity = await _legalLandfillPointCloudFileRepository.GetById(legalLandfillPointCloudFileDTO.Id, track: true);
            if (resultGetEntity.IsSuccess == false && resultGetEntity.HandleError())
            {
                return ResultDTO<LegalLandfillPointCloudFileDTO>.Fail(resultGetEntity.ErrMsg!);
            }

            if (resultGetEntity.Data == null)
            {
                return ResultDTO<LegalLandfillPointCloudFileDTO>.Fail("File not exists in database!");
            }

            if (legalLandfillPointCloudFileDTO.LegalLandfillId != resultGetEntity.Data.LegalLandfillId)
            {
                var pointCloudUploadFolder = await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("LegalLandfillPointCloudFileUploads", "Uploads\\LegalLandfillUploads\\PointCloudUploads");
                legalLandfillPointCloudFileDTO.FilePath = string.Format("{0}\\{1}\\", pointCloudUploadFolder.Data, legalLandfillPointCloudFileDTO.LegalLandfillId.ToString());
            }

            _mapper.Map(legalLandfillPointCloudFileDTO, resultGetEntity.Data);

            ResultDTO<LegalLandfillPointCloudFile> resultUpdateEntity = await _legalLandfillPointCloudFileRepository.UpdateAndReturnEntity(resultGetEntity.Data);
            if (resultUpdateEntity.IsSuccess == false && resultUpdateEntity.HandleError())
            {
                return ResultDTO<LegalLandfillPointCloudFileDTO>.Fail(resultUpdateEntity.ErrMsg!);
            }

            LegalLandfillPointCloudFileDTO returnDto = _mapper.Map<LegalLandfillPointCloudFileDTO>(resultUpdateEntity.Data);

            return ResultDTO<LegalLandfillPointCloudFileDTO>.Ok(returnDto);

        }
    }
}
