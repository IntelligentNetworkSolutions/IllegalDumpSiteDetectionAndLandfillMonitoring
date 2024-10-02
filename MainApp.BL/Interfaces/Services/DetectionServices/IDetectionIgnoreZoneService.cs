using DTOs.MainApp.BL.DetectionDTOs;
using SD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainApp.BL.Interfaces.Services.DetectionServices
{
    public interface IDetectionIgnoreZoneService
    {
        Task<ResultDTO<List<DetectionIgnoreZoneDTO>>> GetAllIgnoreZonesDTOs();
        Task<ResultDTO> CreateDetectionIgnoreZoneFromDTO(DetectionIgnoreZoneDTO dto);
        Task<ResultDTO<DetectionIgnoreZoneDTO?>> GetIgnoreZoneById(Guid id);
        Task<ResultDTO> DeleteDetectionIgnoreZoneFromDTO(DetectionIgnoreZoneDTO dto);
        Task<ResultDTO> UpdateDetectionIgnoreZoneFromDTO(DetectionIgnoreZoneDTO dto);
    }
}
