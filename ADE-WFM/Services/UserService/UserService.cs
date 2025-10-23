using ADE_WFM.Data;
using ADE_WFM.Models;
using ADE_WFM.Models.DTOs;

namespace ADE_WFM.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        // CREATE

        // GET

        // UPDATE

        // DELTETE
    }
}
