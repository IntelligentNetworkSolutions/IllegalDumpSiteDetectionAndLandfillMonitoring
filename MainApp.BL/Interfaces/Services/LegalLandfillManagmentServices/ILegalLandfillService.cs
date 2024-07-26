using DTOs.MainApp.BL.DetectionDTOs;
using DTOs.MainApp.BL.LegalLandfillManagementDTOs;
using SD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainApp.BL.Interfaces.Services.LegalLandfillManagmentServices
{
    public interface ILegalLandfillService
    {
        Task<ResultDTO<List<LegalLandfillDTO>>> GetAllLegalLandfills();
        Task<ResultDTO<LegalLandfillDTO>> GetLegalLandfillById(Guid legalLandfillId);
        Task<ResultDTO> CreateLegalLandfill(LegalLandfillDTO legalLandfillDTO);
        Task<ResultDTO> EditLegalLandfill(LegalLandfillDTO legalLandfillDTO);
        Task<ResultDTO> DeleteLegalLandfill(LegalLandfillDTO legalLandfillDTO);
    }
}
