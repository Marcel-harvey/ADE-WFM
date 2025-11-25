using ADE_WFM.Models.DTOs;
using ADE_WFM.Models.DTOs.WorkFlowDtos;
using ADE_WFM.Models.DTOs.WorkFlowViewModels;

namespace ADE_WFM.Services.WorkFlowService {
    public interface IProgramService {
        // ADD services
        Task<ServiceResult<ProgramResponseDto>> AddProgram(CreateProgramDto dto);
        Task<ServiceResult<ProgramResponseDto>> AddUserToProgram(AddUserProgramDto dto);

        // GET services
        Task<ServiceResult<List<ProgramResponseDto>>> GetAllPrograms();
        Task<ServiceResult<ProgramResponseDto>> GetProgramById(GetProgramInfoDto dto);

        // UPDATE services
        Task<ServiceResult<ProgramResponseDto>> UpdateProgram(UpdateProgramNameDto dto);

        // DELETE services
        Task<ServiceResult<ProgramResponseDto>> DeleteProgram(GetProgramInfoDto dto);
        Task<ServiceResult<ProgramResponseDto>> RemoveUserFromProgram(RemoveUserFromProgramDto dto);
    }
}
