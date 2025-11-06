using ADE_WFM.Models.DTOs;
using ADE_WFM.Models.DTOs.UserDtos;

namespace ADE_WFM.Services.UserService
{
    public interface IUserService
    {
        // CREATE service
        Task<ServiceResult<UserResponseDto>> RegisterNewUser(CreateUserDto dto);

        // GET service
        Task<ServiceResult<List<UserResponseDto>>> GetAllUsers();

        // UPDATE service (To be added later)

        // DELETE service
        Task<ServiceResult<UserResponseDto>> DeleteUser(DeleteUserDto dto);
    }
}
