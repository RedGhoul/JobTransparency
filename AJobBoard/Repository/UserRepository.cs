using AJobBoard.Models.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
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

        public async Task<bool> AddApplyToUser(string ApplierId, int JobPostingId)
        {
            if(_ctx.JobPostings.Any(x => x.Id.Equals(JobPostingId)) 
                && _ctx.Users.Any(x => x.Id.Equals(ApplierId))
                && !_ctx.Applies.Any(x => x.JobPostingId == JobPostingId 
                && x.ApplierId.Equals(ApplierId)))
            {
                var newApply = new Apply
                {
                    JobPostingId = JobPostingId,
                    DateAddedToApplies = DateTime.Now,
                    ApplierId = ApplierId
                };

                _ctx.Applies.Add(newApply);
                return await _ctx.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<ApplicationUser> getUserFromHttpContextAsync(HttpContext context)
        {
            ApplicationUser User = await _userManager.GetUserAsync(context.User);
            return User;
        }

    }
}
