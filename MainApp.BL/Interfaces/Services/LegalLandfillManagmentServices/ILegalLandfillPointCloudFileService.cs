using DTOs.MainApp.BL.LegalLandfillManagementDTOs;
using Microsoft.AspNetCore.Http;
using SD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainApp.BL.Interfaces.Services.LegalLandfillManagmentServices
{
    public interface ILegalLandfillPointCloudFileService
    {
        Task<ResultDTO<List<LegalLandfillPointCloudFileDTO>>> GetAllLegalLandfillPointCloudFiles();
        Task<ResultDTO<List<LegalLandfillPointCloudFileDTO>>> GetFilteredLegalLandfillPointCloudFiles(List<Guid> selectedIds);
        Task<ResultDTO<List<LegalLandfillPointCloudFileDTO>>> GetLegalLandfillPointCloudFilesByLandfillId(Guid legalLandfillId);
        Task<ResultDTO<LegalLandfillPointCloudFileDTO>> GetLegalLandfillPointCloudFilesById(Guid legalLandfillPointCloudFileId);
        Task<ResultDTO<LegalLandfillPointCloudFileDTO>> CreateLegalLandfillPointCloudFile(LegalLandfillPointCloudFileDTO legalLandfillPointCloudFileDTO);
        Task<ResultDTO> DeleteLegalLandfillPointCloudFile(Guid legalLandfillPointCloudFileId);
        Task<ResultDTO<LegalLandfillPointCloudFileDTO>> EditLegalLandfillPointCloudFile(LegalLandfillPointCloudFileDTO legalLandfillPointCloudFileDTO);
        Task<ResultDTO<string>> CreateDiffWasteVolumeComparisonFile(List<LegalLandfillPointCloudFileDTO> orderedList, string webRootPath);
        Task<ResultDTO<double?>> ReadAndDeleteDiffWasteVolumeComparisonFile(string outputDiffFilePath);
        Task<ResultDTO<string>> UploadFile(IFormFile file, string uploadFolder, string fileName);
        Task<ResultDTO> ConvertToPointCloud(string potreeConverterFilePath, string uploadResultData, string convertsFolder, string filePath);
        Task<ResultDTO> CreateTifFile(string pipelineJsonTemplate, string pdalAbsPath, string filePath, string tifFilePath);
        Task<ResultDTO> CheckSupportingFiles(string fileUploadExtension);
        Task<ResultDTO> DeleteFilesFromUploads(LegalLandfillPointCloudFileDTO dto, string webRootPath);
        Task<ResultDTO> DeleteFilesFromConverts(LegalLandfillPointCloudFileDTO dto, string webRootPath);
        Task<ResultDTO> EditFileInUploads(string webRootPath, string filePath, LegalLandfillPointCloudFileDTO dto);
        Task<ResultDTO> EditFileConverts(string webRootPath, Guid oldLegalLandfillId, LegalLandfillPointCloudFileDTO dto);
    }
}
