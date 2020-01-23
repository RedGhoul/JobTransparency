using System.Threading.Tasks;
using AJobBoard.Models;
using Microsoft.AspNetCore.Http;

namespace AJobBoard.Data
{
    public interface IUserRepository
    {
        Task<ApplicationUser> getUserFromHttpContextAsync(HttpContext context);
    }
}