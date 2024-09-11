using DTOs.MainApp.BL.LegalLandfillManagementDTOs;
using SD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainApp.BL.Interfaces.Services.LegalLandfillManagmentServices
{
	public interface ILegalLandfillWasteTypeService
	{
		Task<ResultDTO<List<LegalLandfillWasteTypeDTO>>> GetAllLegalLandfillWasteTypes();
		Task<ResultDTO<LegalLandfillWasteTypeDTO>> GetLegalLandfillWasteTypeById(Guid legalLandfillWasteTypeId);
		Task<ResultDTO> CreateLegalLandfillWasteType(LegalLandfillWasteTypeDTO legalLandfillWasteTypeDTO);
		Task<ResultDTO> EditLegalLandfillWasteType(LegalLandfillWasteTypeDTO legalLandfillWasteTypeDTO);
		Task<ResultDTO> DeleteLegalLandfillWasteType(LegalLandfillWasteTypeDTO legalLandfillWasteTypeDTO);
	}
}
