using AJobBoard.Models.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace AJobBoard.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _ctx;

        public UserRepository(ApplicationDbContext ctx, UserManager<ApplicationUser> userManager)
        {
            _ctx = ctx;
            _userManager = userManager;
        }

        public async Task<bool> AddApplyToUser(ApplicationUser User, JobPosting JobPosting, Apply CurrentApply)
        {

            User.Applies.Add(new Apply
            {
                DateAddedToApplies = DateTime.Now,
                JobPosting = JobPosting
            });

            return await _ctx.SaveChangesAsync() > 0;
        }

        public async Task<ApplicationUser> getUserFromHttpContextAsync(HttpContext context)
        {
            ApplicationUser User = await _userManager.GetUserAsync(context.User);
            return User;
        }

    }
}
