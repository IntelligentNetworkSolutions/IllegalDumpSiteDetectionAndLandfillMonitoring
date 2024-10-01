using DTOs.MainApp.BL.LegalLandfillManagementDTOs;
using SD;

namespace MainApp.BL.Interfaces.Services.LegalLandfillManagmentServices
{
    public interface ILegalLandfillTruckService
    {
        Task<ResultDTO<List<LegalLandfillTruckDTO>>> GetAllLegalLandfillTrucks();
        Task<ResultDTO<LegalLandfillTruckDTO>> GetLegalLandfillTruckById(Guid legalLandfillTruckId);
        Task<ResultDTO> CreateLegalLandfillTruck(LegalLandfillTruckDTO legalLandfillTruckDTO);
        Task<ResultDTO> EditLegalLandfillTruck(LegalLandfillTruckDTO legalLandfillTruckDTO);
        Task<ResultDTO> DeleteLegalLandfillTruck(LegalLandfillTruckDTO legalLandfillTruckDTO);
        Task<ResultDTO> DisableLegalLandfillTruck(LegalLandfillTruckDTO legalLandfillTruckDTO);


    }
}
