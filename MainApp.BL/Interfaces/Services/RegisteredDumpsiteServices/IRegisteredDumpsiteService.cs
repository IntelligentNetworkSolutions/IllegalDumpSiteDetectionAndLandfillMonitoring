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
    Task<ResultDTO> MergeRegisteredDumpsites(CreateRegisteredDumpsiteDTO newDumpsiteData, List<Guid> existingIds, Guid? targetDumpsiteId = null);
    Task<ResultDTO<List<RegisteredDumpsiteFileDTO>>> GetFilesByDumpsiteId(Guid dumpsiteId);
    Task<ResultDTO<RegisteredDumpsiteFileDTO>> UploadFile(RegisteredDumpsiteFileDTO dumpsiteFileDto);
    Task<ResultDTO> DeleteFile(Guid fileId);
    Task<ResultDTO<RegisteredDumpsiteFileDTO>> GetSingleFileById(Guid fileId);
    Task<ResultDTO> CompleteInspection(Guid inspectionId, string findings, string? recommendations);


    Task<ResultDTO<RegisteredDumpsiteDTO?>> ConvertDetectedDumpsiteAsync(ConvertDetectedDumpsiteRequest request);
    Task<ResultDTO> AssignInspector(Guid inspectionId, string inspectorId);
    Task<ResultDTO> DeleteInspection(Guid id);
    Task<ResultDTO> UpdateInspection(RegisteredDumpsiteInspectionDTO dto);
    Task<ResultDTO> CreateInspection(RegisteredDumpsiteInspectionDTO dto);
    Task<ResultDTO<RegisteredDumpsiteInspectionDTO?>> GetInspectionById(Guid id);
    Task<ResultDTO<List<RegisteredDumpsiteInspectionDTO>>> GetInspectionsByDumpsiteId(Guid dumpsiteId);

    //

    Task<ResultDTO<RegisteredDumpsiteInspectionFileDTO>> UploadInspectionFile(RegisteredDumpsiteInspectionFileDTO inspectionFileDto);
    Task<ResultDTO> DeleteInspectionFile(Guid fileId);
    Task<ResultDTO<RegisteredDumpsiteInspectionFileDTO>> GetSingleInspectionFileById(Guid fileId);
    Task<ResultDTO<List<RegisteredDumpsiteInspectionFileDTO>>> GetInspectionFilesByInspectionId(Guid inspectionId);
    Task<ResultDTO<bool>> IsUserAssignedToInspection(Guid inspectionId, string userId);

}
