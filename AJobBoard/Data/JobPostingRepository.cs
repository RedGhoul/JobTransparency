using AJobBoard.Models;
using AJobBoard.Models.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AJobBoard.Data
{
    public class JobPostingRepository : IJobPostingRepository
    {
        private readonly ApplicationDbContext _ctx;
        private readonly IDistributedCache _cache;

        public JobPostingRepository(ApplicationDbContext ctx, IDistributedCache cache)
        {
            _ctx = ctx;
            _cache = cache;
        }

        public async Task<string> GetTotalJobs()
        {
            string cacheKey = "TotalJobs";
            string TotalJobs = await _cache.GetStringAsync(cacheKey);

            if (string.IsNullOrEmpty(TotalJobs))
            {
                int TotalJobsI = await _ctx.JobPostings.CountAsync();
                TotalJobs = TotalJobsI.ToString();
                var options = new DistributedCacheEntryOptions();
                options.SetSlidingExpiration(TimeSpan.FromMinutes(40));
                await _cache.SetStringAsync(cacheKey, TotalJobs, options);
            }

            return TotalJobs;
        }

        public async Task<bool> JobPostingExists(TestCheckDTO tcDTO)
        {
            var jobPostingCount = await _ctx.JobPostings
                .Where(x => x.URL.Equals(tcDTO.url) == true ||
                x.Description.Equals(tcDTO.description) == true ||
                x.Title.Equals(tcDTO.title) == true)
                .FirstOrDefaultAsync();

            if (jobPostingCount != null)
            {
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<JobPosting>> GetJobPostingsAsync(int amount)
        {
            string cacheKey = "Jobs" + amount;
            string JobsString = await _cache.GetStringAsync(cacheKey);
            IEnumerable<JobPosting> jobs = null;
            if (string.IsNullOrEmpty(JobsString))
            {
                jobs = await _ctx.JobPostings.Take(amount).ToListAsync();
                var options = new DistributedCacheEntryOptions();
                options.SetSlidingExpiration(TimeSpan.FromMinutes(40));
                await _cache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(jobs), options);
            }
            else
            {
                jobs = JsonConvert.DeserializeObject<IEnumerable<JobPosting>>(JobsString);
            }

            return jobs;
        }

        public async Task<IEnumerable<JobPosting>> GetJobPostingsWithKeyPhraseAsync(int amount)
        {
            string cacheKey = "JobsKeyPhrase" + amount;
            string JobsString = null;//await _cache.GetStringAsync(cacheKey);
            IEnumerable<JobPosting> jobs = null;
            if (string.IsNullOrEmpty(JobsString))
            {
                jobs = await _ctx.JobPostings.Take(amount).Include(x => x.KeyPhrases).ToListAsync();
                var options = new DistributedCacheEntryOptions();
                options.SetSlidingExpiration(TimeSpan.FromMinutes(40));

                //await _cache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(jobs, Formatting.None,
                //        new JsonSerializerSettings()
                //        {
                //            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                //        }), options);
            }
            else
            {
                jobs = JsonConvert.DeserializeObject<IEnumerable<JobPosting>>(JobsString);
            }

            return jobs;
        }

        public async Task<IEnumerable<JobPosting>> GetAllNoneKeywordsJobPostings()
        {
            return await _ctx.JobPostings.Where(x => x.KeyPhrases != null).ToListAsync();
        }

        public async Task<JobPosting> GetJobPostingById(int id)
        {
            string cacheKey = "JobSingle" + id;
            string jobPostingString = await _cache.GetStringAsync(cacheKey);

            JobPosting jobPosting = null;
            if (string.IsNullOrEmpty(jobPostingString))
            {
                jobPosting = await _ctx.JobPostings.FindAsync(id);
                var options = new DistributedCacheEntryOptions();
                options.SetSlidingExpiration(TimeSpan.FromMinutes(40));
                await _cache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(jobPosting), options);
            }
            else
            {
                jobPosting = JsonConvert.DeserializeObject<JobPosting>(jobPostingString);
            }


            return jobPosting;
        }

        public async Task<bool> JobPostingExistsByUrl(string url)
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

        public async Task<bool> JobPostingExistsByDescription(string Summary)
        {
            var jobPostingCount = await _ctx.JobPostings
                .Where(x => x.Description.Equals(Summary))
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
                return !JobPostingExistsById(id) ? null : jobPosting;
            }

            return jobPosting;
        }

        public async Task<JobPosting> CreateJobPostingAsync(JobPosting jobPosting)
        {
            try
            {
                jobPosting.Description = jobPosting.Description;
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
        public async Task<List<JobPosting>> GetRandomSetOfJobPostings()
        {
            string cacheKey = "RandomJobsFrontPage";
            string Randomjobs = await _cache.GetStringAsync(cacheKey);
            List<JobPosting> RandomjobsList = null;
            if (string.IsNullOrEmpty(Randomjobs))
            {
                Random random = new Random();
                int NumberOfResults = random.Next(10, 20);
                var Jobs = await _ctx.JobPostings.Take(NumberOfResults * 2).ToListAsync();
                int SkipNumberOfResults = random.Next(10, 20);
                RandomjobsList = Jobs.Skip(SkipNumberOfResults).Reverse().ToList();
                var options = new DistributedCacheEntryOptions();
                options.SetSlidingExpiration(TimeSpan.FromMinutes(40));
                await _cache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(RandomjobsList), options);
            }
            else
            {
                RandomjobsList = JsonConvert.DeserializeObject<List<JobPosting>>(Randomjobs);
            }

            return RandomjobsList;
        }

        public async Task<(List<JobPosting>, TimeSpan)> ConfigureSearchAsync(HomeIndexViewModel homeIndexVm)
        {
            IQueryable<JobPosting> jobsQuery = null;
            List<JobPosting> jobs = null;
            var start = DateTime.Now;

            string cacheKey = homeIndexVm.FindModel.Location.ToLower() +
                              homeIndexVm.FindModel.KeyWords.ToLower() +
                              homeIndexVm.FindModel.MaxResults;

            string SearchJobs = await _cache.GetStringAsync(cacheKey);
            if (string.IsNullOrEmpty(SearchJobs))
            {
                // find By Location
                if (homeIndexVm.FindModel.Location.ToLower().Equals("anywhere") || string.IsNullOrEmpty(homeIndexVm.FindModel.Location))
                {
                    jobsQuery = _ctx.JobPostings;
                }
                else if (homeIndexVm.FindModel.Location.ToLower().Equals("ontario"))
                {
                    jobsQuery = _ctx.JobPostings.Where(x => x.Location.ToLower().Contains("ontario"));
                }
                else if (homeIndexVm.FindModel.Location.ToLower().Equals("vancouver"))
                {
                    jobsQuery = _ctx.JobPostings.Where(x => x.Location.ToLower().Contains("vancouver"));
                }
                else
                {
                    jobsQuery = _ctx.JobPostings.Where(x => x.Location.Contains(homeIndexVm.FindModel.Location) == true);
                }

                // find By Key Words
                if (!string.IsNullOrEmpty(homeIndexVm.FindModel.KeyWords))
                {
                    jobsQuery = jobsQuery.Where(x => x.Title.Contains(homeIndexVm.FindModel.KeyWords) ||
                                x.Description.Contains(homeIndexVm.FindModel.KeyWords));
                }

                // find By Date (days)
                //if (!string.IsNullOrEmpty(homeIndexVm.FindModel.Date))
                //{
                //    DateTime dayHolder = DateTime.Now;
                //    if (homeIndexVm.FindModel.Date.Equals("Last 20 Days"))
                //    {
                //        dayHolder = dayHolder.AddDays(-20);

                //    }
                //    else if (homeIndexVm.FindModel.Date.Equals("Last 30 Days"))
                //    {
                //        dayHolder = dayHolder.AddDays(-30);
                //    }
                //    jobsQuery = jobsQuery.Where(x => x.DateAdded > dayHolder);
                //}

                // add Max Results
                jobs = await jobsQuery.Take(homeIndexVm.FindModel.MaxResults).OrderByDescending(x => x.Title).ToListAsync();
                var options = new DistributedCacheEntryOptions();
                options.SetSlidingExpiration(TimeSpan.FromMinutes(40));
                await _cache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(jobs), options);
            }
            else
            {
                jobs = JsonConvert.DeserializeObject<List<JobPosting>>(SearchJobs);

            }

            var duration = DateTime.Now - start;
            return (jobs, duration);
        }
    }
}
