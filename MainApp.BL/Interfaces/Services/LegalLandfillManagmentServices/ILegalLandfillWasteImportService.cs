using DTOs.MainApp.BL.LegalLandfillManagementDTOs;
using SD;

namespace MainApp.BL.Interfaces.Services.LegalLandfillManagmentServices
{
	public interface ILegalLandfillWasteImportService
	{
		Task<ResultDTO<List<LegalLandfillWasteImportDTO>>> GetAllLegalLandfillWasteImports();
		Task<ResultDTO<LegalLandfillWasteImportDTO>> GetLegalLandfillWasteImportById(Guid legalLandfillWasteImportId);
		Task<ResultDTO> CreateLegalLandfillWasteImport(LegalLandfillWasteImportDTO legalLandfillWasteImportDTO);
		Task<ResultDTO> EditLegalLandfillWasteImport(LegalLandfillWasteImportDTO legalLandfillWasteImportDTO);
		Task<ResultDTO> DeleteLegalLandfillWasteImport(LegalLandfillWasteImportDTO legalLandfillWasteImportDTO);
	}
}
