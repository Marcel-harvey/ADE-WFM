using ADE_WFM.Models.DTOs;
using ADE_WFM.Models.DTOs.UserDtos;

namespace ADE_WFM.Services.UserService
{
    public interface IUserService
    {
        // CREATE: Add a new user
        Task<ServiceResult<CreateUserResponseDto>> AddUser(CreateUserDto dto);

        // GET: Retrieve all users
        Task<ServiceResult<List<GetAllUsersResponseDto>>> GetAllUsers();

        // UPDATE: (To be added later)

        // DELETE: (To be added later)
        Task<ServiceResult<DeleteUserResponseDto>> DeleteUser(DeleteUserDto dto);
    }
}
