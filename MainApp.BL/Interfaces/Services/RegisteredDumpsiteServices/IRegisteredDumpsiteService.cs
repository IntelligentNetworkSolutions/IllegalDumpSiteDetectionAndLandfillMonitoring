using DTOs.MainApp.BL.RegisteredDumpsiteDTOs;
using SD;

namespace MainApp.BL.Interfaces.Services.RegisteredDumpsiteServices;

public interface IRegisteredDumpsiteService
{
    Task<ResultDTO<List<RegisteredDumpsiteDTO>>> GetAllRegisteredDumpsitesDTOs();
    Task<ResultDTO> CreateRegisteredDumpsiteFromDTO(RegisteredDumpsiteDTO dto);
    Task<ResultDTO<RegisteredDumpsiteDTO?>> GetRegisteredDumpsiteById(Guid id);
    Task<ResultDTO> DeleteRegisteredDumpsiteFromDTO(RegisteredDumpsiteDTO dto);
    Task<ResultDTO> UpdateRegisteredDumpsiteFromDTO(RegisteredDumpsiteDTO dto);
    Task<ResultDTO> MergeRegisteredDumpsites(CreateRegisteredDumpsiteDTO newDumpsiteData, List<Guid> existingIds);
}
