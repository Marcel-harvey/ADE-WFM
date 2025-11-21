using ADE_WFM.Models.DTOs;
using ADE_WFM.Models.DTOs.UserDtos;

namespace ADE_WFM.Services.UserService {
    public interface IUserService {
        // CREATE service
        Task<ServiceResult<UserResponseDto>> RegisterNewUser(CreateUserDto dto);

        // GET service
        // TODO: To remove after development
        Task<ServiceResult<List<UserResponseDto>>> GetAllUsers();
        Task<ServiceResult<List<UserResponseDto>>> GetTenantUsers();
        Task<ServiceResult<LoginResponseDto>> LoginUser(LoginUserDto loginUser);

        // UPDATE service (To be added later)
        Task<ServiceResult<LoginResponseDto>> ChangePassword(ChangePasswordDto dto);

        // DELETE service
        Task<ServiceResult<UserResponseDto>> DeleteUser(DeleteUserDto dto);
    }
}
