using AJobBoard.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace AJobBoard.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ApplicationUser> getUserFromHttpContextAsync(HttpContext context)
        {
            var User = await _userManager.GetUserAsync(context.User);
            return User;
        }

    }
}
