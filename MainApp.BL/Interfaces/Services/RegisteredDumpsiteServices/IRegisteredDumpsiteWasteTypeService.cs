using DTOs.MainApp.BL.RegisteredDumpsiteDTOs;
using SD;

namespace MainApp.BL.Interfaces.Services.RegisteredDumpsiteServices;

public interface IRegisteredDumpsiteWasteTypeService
{
    Task<ResultDTO<List<RegisteredDumpsiteWasteTypeDTO>>> GetAllRegisteredDumpsiteWasteTypes();
    Task<ResultDTO<RegisteredDumpsiteWasteTypeDTO>> GetRegisteredDumpsiteWasteTypeById(Guid registeredDumpsiteWasteTypeId);
    Task<ResultDTO> CreateRegisteredDumpsiteWasteType(RegisteredDumpsiteWasteTypeDTO registeredDumpsiteWasteTypeDTO);
    Task<ResultDTO> EditRegisteredDumpsiteWasteType(RegisteredDumpsiteWasteTypeDTO registeredDumpsiteWasteTypeDTO);
    Task<ResultDTO> DeleteRegisteredDumpsiteWasteType(RegisteredDumpsiteWasteTypeDTO registeredDumpsiteWasteTypeDTO);
}
