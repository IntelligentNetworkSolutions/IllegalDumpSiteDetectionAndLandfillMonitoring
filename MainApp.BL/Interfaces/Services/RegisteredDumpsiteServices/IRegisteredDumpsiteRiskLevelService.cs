using DTOs.MainApp.BL.RegisteredDumpsiteDTOs;
using SD;

namespace MainApp.BL.Interfaces.Services.RegisteredDumpsiteServices;

public interface IRegisteredDumpsiteRiskLevelService
{
    Task<ResultDTO<List<RegisteredDumpsiteRiskLevelDTO>>> GetAllRegisteredDumpsiteRiskLevels();
    Task<ResultDTO<RegisteredDumpsiteRiskLevelDTO>> GetRegisteredDumpsiteRiskLevelById(Guid registeredDumpsiteRiskLevelId);
    Task<ResultDTO> CreateRegisteredDumpsiteRiskLevel(RegisteredDumpsiteRiskLevelDTO registeredDumpsiteRiskLevelDTO);
    Task<ResultDTO> EditRegisteredDumpsiteRiskLevel(RegisteredDumpsiteRiskLevelDTO registeredDumpsiteRiskLevelDTO);
    Task<ResultDTO> DeleteRegisteredDumpsiteRiskLevel(RegisteredDumpsiteRiskLevelDTO registeredDumpsiteRiskLevelDTO);
}
