using AJobBoard.Models;
using Jobtransparency.Models.DTO;
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

        public async Task<List<AppliesDTO>> GetUsersAppliesAsync(ApplicationUser User)
        {
            var applications = await _ctx.Applies.Include(x => x.JobPosting)
                .Where(x => x.Applier.Id == User.Id).ToListAsync();
            var apps = applications.Select(x => new AppliesDTO()
            {
                Id = x.Id,
                JobId = x.JobPosting.Id,
                Title = x.JobPosting.Title,
                Company = x.JobPosting.Company,
                Location = x.JobPosting.Location,
                JobSource = x.JobPosting.JobSource,
                Applicates = x.JobPosting.NumberOfApplies,
                Views = x.JobPosting.NumberOfViews,
                URL = x.JobPosting.URL,
                PostDate = x.JobPosting.PostDate
            }).ToList();

            return apps;
        }


        public async Task<bool> DeleteAppliesAsync(int id)
        {
            var apply = await _ctx.Applies.FindAsync(id);
            if (apply == null)
            {
                return false;
            }

            _ctx.Applies.Remove(apply);
            return await _ctx.SaveChangesAsync() > 0;
        }

        public async Task<Apply> PutApplyAsync(int id, Apply apply)
        {

            _ctx.Entry(apply).State = EntityState.Modified;

            try
            {
                await _ctx.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine($"Error occured: ${ex.InnerException}");
                return !ApplyExistsById(id) ? null : apply;
            }

            return apply;
        }

        private bool ApplyExistsById(int id)
        {
            return _ctx.Applies.Any(e => e.Id == id);
        }

        public async Task<Apply> GetApplyByIdAsync(int id)
        {
            var apply = await _ctx.Applies.Where(x => x.Id == id).Include(x => x.JobPosting).FirstOrDefaultAsync();
            return apply;
        }

    }
}
