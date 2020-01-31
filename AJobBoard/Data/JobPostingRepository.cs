﻿using AJobBoard.Models;
using AJobBoard.Models.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AJobBoard.Utils.ControllerHelpers;
using Nest;

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
            string TotalJobs = "";
            try
            {
                TotalJobs = await _cache.GetStringAsync(cacheKey);
            }
            catch (Exception e)
            {
            }
            

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
                jobs = jobs.Reverse();
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

            var settings = new ConnectionSettings(new Uri("http://ttestelk.experimentsinthedeep.com"))
                .DefaultIndex("jobposting");
            var client = new ElasticClient(settings);
            var job = await client.UpdateAsync<JobPosting>(jobPosting.Id,
                u => u.Index("jobposting").Doc(jobPosting));

            return jobPosting;
        }

        public async Task<JobPosting> CreateJobPostingAsync(JobPosting jobPosting)
        {
            try
            {
                jobPosting.Description = jobPosting.Description;
                await _ctx.JobPostings.AddAsync(jobPosting);
                await _ctx.SaveChangesAsync();

                var settings = new ConnectionSettings(new Uri("http://ttestelk.experimentsinthedeep.com"))
                               .DefaultIndex("jobposting");

                var client = new ElasticClient(settings);

                var job = await client.IndexDocumentAsync(jobPosting);
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
                RandomjobsList.Reverse();
            }

            return RandomjobsList;
        }

        public async Task<(List<JobPosting>, TimeSpan)> ConfigureSearchAsync(HomeIndexViewModel homeIndexVm)
        {
            var start = DateTime.Now;
            var settings = new ConnectionSettings(new Uri("http://ttestelk.experimentsinthedeep.com"))
                .DefaultIndex("jobposting");

            var client = new ElasticClient(settings);
            var fromNumber = 0;
            if (homeIndexVm.FindModel.Page > 1)
            {
                fromNumber = homeIndexVm.FindModel.Page * 12;
            }
            var searchResponse = await client.SearchAsync<JobPosting>(s => s
                .From(fromNumber)
                .Size(12)
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.Description)

                        .Query(homeIndexVm.FindModel.KeyWords)
                    )
                )
            );

            var jobsCollection = searchResponse.Documents.ToList();
            var duration = DateTime.Now - start;
            return (jobsCollection, duration);
        }


        

        public async Task BuildCache()
        {
            var allJobs = await _ctx.JobPostings.ToListAsync();
            int pageSize = 12;
            var count = allJobs.Count();
            count = count / 12;
            for (int i = 0; i < count; i++)
            {
                var Jobs = allJobs.Skip(i * pageSize).Take(pageSize).ToList();
                var options = new DistributedCacheEntryOptions();
                options.SetSlidingExpiration(TimeSpan.FromMinutes(50));
                await _cache.SetStringAsync("JobPosting_12_" + i, JsonConvert.SerializeObject(Jobs), options);

            }
        }
    }
}
