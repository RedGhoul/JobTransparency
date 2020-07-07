using AJobBoard.Models;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace AJobBoard.Data
{
    public interface IUserRepository
    {
        Task<ApplicationUser> getUserFromHttpContextAsync(HttpContext context);
    }
}