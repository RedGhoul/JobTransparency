using AJobBoard.Models.Entity;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace AJobBoard.Data
{
    public interface IUserRepository
    {
        Task<ApplicationUser> getUserFromHttpContextAsync(HttpContext context);

        Task<bool> AddApplyToUser(ApplicationUser User, JobPosting JobPosting, Apply CurrentApply);
    }
}