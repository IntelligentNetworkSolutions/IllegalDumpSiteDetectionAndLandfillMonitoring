using DTOs.MainApp.BL.LegalLandfillManagementDTOs;
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
        Task<ResultDTO<List<LegalLandfillPointCloudFileDTO>>> GetLegalLandfillPointCloudFilesByLandfillId(Guid legalLandfillId);
        Task<ResultDTO<LegalLandfillPointCloudFileDTO>> GetLegalLandfillPointCloudFilesById(Guid legalLandfillPointCloudFileId);
        Task<ResultDTO<LegalLandfillPointCloudFileDTO>> CreateLegalLandfillPointCloudFile(LegalLandfillPointCloudFileDTO legalLandfillPointCloudFileDTO);
        Task<ResultDTO> DeleteLegalLandfillPointCloudFile(Guid legalLandfillPointCloudFileId);
        Task<ResultDTO<LegalLandfillPointCloudFileDTO>> EditLegalLandfillPointCloudFile(LegalLandfillPointCloudFileDTO legalLandfillPointCloudFileDTO);
    }
}
