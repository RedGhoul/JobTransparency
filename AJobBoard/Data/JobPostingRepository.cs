using AJobBoard.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AJobBoard.Data
{
    public class JobPostingRepository : IJobPostingRepository
    {
        private readonly ApplicationDbContext _ctx;

        public JobPostingRepository(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<IEnumerable<JobPosting>> GetJobPostingsAsync(int amount)
        {
            return await _ctx.JobPostings.Take(amount).ToListAsync();
        }

        public async Task<JobPosting> GetJobPostingById(int id)
        {
            var jobPosting = await _ctx.JobPostings.FindAsync(id);

            if (jobPosting == null)
            {
                return null;
            }

            return jobPosting;
        }

        public async Task<bool> JobPostingExistsByURL(string url)
        {
            var jobPostingCount = await _ctx.JobPostings
                .Where(x => x.URL.Equals(url))
                .FirstOrDefaultAsync();

            if (jobPostingCount != null)
            {
                return true;
            }
            return false;
        }

        public async Task<JobPosting> PutJobPostingAsync(int id, JobPosting jobPosting)
        {

            _ctx.Entry(jobPosting).State = EntityState.Modified;

            try
            {
                await _ctx.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JobPostingExistsById(id))
                {
                    return null;
                }
                else
                {
                    return jobPosting;
                }
            }

            return jobPosting;
        }

        public async Task<JobPosting> CreateJobPostingAsync(JobPosting jobPosting)
        {
            try
            {
                await _ctx.JobPostings.AddAsync(jobPosting);
                await _ctx.SaveChangesAsync();
            }
            catch (Exception)
            {
                return null;
            }
            return jobPosting;
        }

        public async Task<JobPosting> DeleteJobPostingAsync(int id)
        {
            var jobPosting = await _ctx.JobPostings.FindAsync(id);
            if (jobPosting == null)
            {
                return null;
            }

            _ctx.JobPostings.Remove(jobPosting);
            await _ctx.SaveChangesAsync();

            return jobPosting;
        }


        private bool JobPostingExistsById(int id)
        {
            return _ctx.JobPostings.Any(e => e.Id == id);
        }

        public async Task<JobPosting> TickNumberOfViewAsync(JobPosting jobPosting)
        {
            jobPosting.NumberOfViews++;

            try
            {
                _ctx.Update(jobPosting);
                await _ctx.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JobPostingExistsById(jobPosting.Id))
                {
                }
                else
                {
                    throw;
                }
            }
            return jobPosting;
        }

        public async Task<(List<JobPosting>, TimeSpan)> ConfigureSearchAsync(HomeIndexViewModel homeIndexVM)
        {
            IQueryable<JobPosting> jobsQuery = null;
            List<JobPosting> Jobs = null;
            DateTime start = DateTime.Now;

            // find By Location
            if (homeIndexVM.FindModel.Location.ToLower().Equals("anywhere") || string.IsNullOrEmpty(homeIndexVM.FindModel.Location))
            {
                jobsQuery = _ctx.JobPostings;
            }
            else if (homeIndexVM.FindModel.Location.ToLower().Equals("ontario"))
            {
                jobsQuery = _ctx.JobPostings.Where(x => x.Location.Contains("vancouver") == false);
            }
            else
            {
                jobsQuery = _ctx.JobPostings.Where(x => x.Location.Contains(homeIndexVM.FindModel.Location) == true);
            }

            // find By Key Words
            if (!string.IsNullOrEmpty(homeIndexVM.FindModel.KeyWords))
            {
                jobsQuery = jobsQuery.Where(x => x.Title.Contains(homeIndexVM.FindModel.KeyWords) ||
                            x.Summary.Contains(homeIndexVM.FindModel.KeyWords));
            }


            // add Max Results

            Jobs = await jobsQuery.Take(homeIndexVM.FindModel.MaxResults).OrderByDescending(x => x.Title).ToListAsync();
            // Calculate time
            TimeSpan duration = DateTime.Now - start;
            return (Jobs,duration);
        }
    }
}
