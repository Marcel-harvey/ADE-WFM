using ADE_WFM.Models.DTOs;
using ADE_WFM.Models.DTOs.UserDtos;

namespace ADE_WFM.Services.UserService
{
    public interface IUserService
    {
        // CREATE service
        Task<ServiceResult<UserResponseDto>> RegisterNewUser(CreateUserDto dto);
        Task<ServiceResult<InviteTokenResponseDto>> CreateTenantInvite(InviteTokenDto dto);
        Task<ServiceResult<RegisterUserResponseDto>> AcceptTenantInvite(RegisterUserDto dto);

        // GET service
        Task<ServiceResult<List<UserResponseDto>>> GetAllUsers();
        Task<ServiceResult<LoginResponseDto>> LoginUser(LoginUserDto loginUser);

        // UPDATE service (To be added later)
        Task<ServiceResult<LoginResponseDto>> ChangePassword(ChangePasswordDto dto);

        // DELETE service
        Task<ServiceResult<UserResponseDto>> DeleteUser(DeleteUserDto dto);
    }
}
