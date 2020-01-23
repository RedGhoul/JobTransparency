using AJobBoard.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AJobBoard.Data
{
    public class AppliesRepository : IAppliesRepository
    {
        private readonly ApplicationDbContext _ctx;

        public AppliesRepository(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<List<Apply>> GetUsersAppliesAsync(ApplicationUser User)
        {
            var applications = await _ctx.Applies.Include(x => x.JobPosting)
                .Where(x => x.Applier.Id == User.Id).ToListAsync();

            return applications;
        }
    }
}
