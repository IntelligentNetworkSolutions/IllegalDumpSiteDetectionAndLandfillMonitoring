using DTOs.MainApp.BL;
using DTOs.MainApp.BL.RegisteredDumpsiteDTOs;

namespace MainApp.MVC.ViewModels.IntranetPortal.RegisteredDumpsite;

public class DumpsiteDetailsViewModel
{
    public RegisteredDumpsiteDTO Dumpsite { get; set; }
    public List<RegisteredDumpsiteFileDTO> Files { get; set; } = new List<RegisteredDumpsiteFileDTO>();
    public List<RegisteredDumpsiteInspectionDTO> Inspections { get; set; } = new List<RegisteredDumpsiteInspectionDTO>();
    public List<UserDTO> AvailableInspectors { get; set; } = new List<UserDTO>();
    public Dictionary<Guid, List<RegisteredDumpsiteInspectionFileDTO>> InspectionFiles { get; set; } = new Dictionary<Guid, List<RegisteredDumpsiteInspectionFileDTO>>();
    public string CurrentUserId { get; set; }
}
