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
    }
}
