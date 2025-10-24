using ADE_WFM.Models.DTOs.UserDtos;

namespace ADE_WFM.Services.UserService
{
    public interface IUserService
    {
        // CREATE services
        Task <CreateUserResponseDto> AddUser(CreateUserDto dto);

        // GET Services

        // UPDATE services

        // DELETE servicves
    }
}
