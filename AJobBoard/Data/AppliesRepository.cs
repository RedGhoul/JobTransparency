using AJobBoard.Models;
using Microsoft.EntityFrameworkCore;
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


        public async Task<Apply> DeleteAppliesAsync(int id)
        {
            var apply = await _ctx.Applies.FindAsync(id);
            if (apply == null)
            {
                return null;
            }

            _ctx.Applies.Remove(apply);
            await _ctx.SaveChangesAsync();

            return apply;
        }

        public async Task<Apply> PutApplyAsync(int id, Apply apply)
        {

            _ctx.Entry(apply).State = EntityState.Modified;

            try
            {
                await _ctx.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return !ApplyExistsById(id) ? null : apply;
            }

            return apply;
        }

        private bool ApplyExistsById(int id)
        {
            return _ctx.Applies.Any(e => e.Id == id);
        }

        public async Task<Apply> GetApplyAsync(int id)
        {
            var apply = await _ctx.Applies.Where(x => x.Id == id).Include(x => x.JobPosting).FirstOrDefaultAsync();
            return apply;
        }
    }
}
