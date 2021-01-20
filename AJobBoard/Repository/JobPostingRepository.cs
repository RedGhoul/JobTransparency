using AJobBoard.Models.Dto;
using AJobBoard.Models.Entity;
using AJobBoard.Models.View;
using AJobBoard.Services;
using AutoMapper;
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
       
        private readonly ElasticService _es;
        private readonly IMapper _mapper;
        public JobPostingRepository(IMapper mapper, ApplicationDbContext ctx, ElasticService es)
        {
            _ctx = ctx;
           
            _es = es;
            _mapper = mapper;
        }

        public async Task<List<JobPosting>> GetAllWithKeyPhrase()
        {
            try
            {
                List<JobPosting> jobs = await _ctx.JobPostings.Include(x => x.KeyPhrases).ToListAsync();

                return jobs;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new List<JobPosting>();
            }

        }

        public string GetTotal()
        {
            return _ctx.JobPostings.Count().ToString();
        }

        public async Task<bool> Exists(TestCheckDTO tcDTO)
        {
            JobPosting jobPosting = await _ctx.JobPostings
                .Where(x => x.Description.Equals(tcDTO.description) == true &&
                    x.Title.Equals(tcDTO.title) == true)
                .FirstOrDefaultAsync();

            if (jobPosting != null)
            {
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<JobPosting>> GetWithKeyPhrase(int amount)
        {
            List<JobPosting> jobs = await _ctx.JobPostings.Take(amount)
                .Include(x => x.KeyPhrases).ToListAsync();
            return jobs;
        }


        public async Task<IEnumerable<JobPosting>> GetAllNoneKeywords()
        {
            return await _ctx.JobPostings.Include(x => x.KeyPhrases)
                .Where(x => x.KeyPhrases == null ||
                x.KeyPhrases.Count == 0 || x.KeyPhrases.Count == 1).ToListAsync();
        }

        public async Task<JobPosting> GetById(int id)
        {
            var jobPosting = await _ctx.JobPostings.FindAsync(id);

            return jobPosting;
        }

        public async Task<bool> ExistsByUrl(string url)
        {
            JobPosting jobPostingCount = await _ctx.JobPostings
                .Where(x => x.URL.Equals(url))
                .FirstOrDefaultAsync();

            if (jobPostingCount != null)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> DoesExistByDescription(string Summary)
        {
            JobPosting jobPostingCount = await _ctx.JobPostings
                .Where(x => x.Description.Equals(Summary))
                .FirstOrDefaultAsync();

            if (jobPostingCount != null)
            {
                return true;
            }
            return false;
        }

        public async Task<JobPosting> Put(int id, JobPosting jobPosting)
        {

            //_ctx.Entry(jobPosting).State = EntityState.Modified;

            try
            {
                await _ctx.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return !ExistsById(id) ? null : jobPosting;
            }

            return jobPosting;
        }

        public async Task<JobPosting> Create(JobPosting jobPosting)
        {
            try
            {
                jobPosting.Id = 0;
                await _ctx.JobPostings.AddAsync(jobPosting);
                await _ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
            }
            return jobPosting;
        }

        public async Task<JobPosting> DeleteById(int id)
        {
            JobPosting jobPosting = await _ctx.JobPostings.FindAsync(id);
            if (jobPosting == null)
            {
                return null;
            }

            _ctx.JobPostings.Remove(jobPosting);
            await _ctx.SaveChangesAsync();

            return jobPosting;
        }


        private bool ExistsById(int id)
        {
            return _ctx.JobPostings.Any(e => e.Id == id);
        }

        public async Task<JobPosting> IncrementNumberOfViews(JobPosting jobPosting)
        {
            jobPosting.NumberOfViews++;

            try
            {
                _ctx.Update(jobPosting);
                await _ctx.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ExistsById(jobPosting.Id))
                {
                    Console.WriteLine("JobPosting does not exist");
                }
                else
                {
                    Console.WriteLine(ex.InnerException);
                }
            }
            return jobPosting;
        }

        public async Task<bool> IncrementNumberOfApplies(int JobPostingId)
        {
            JobPosting job = _ctx.JobPostings.Where(x => x.Id == JobPostingId).FirstOrDefault();
            job.NumberOfApplies++;

            try
            {
                _ctx.Update(job);
                await _ctx.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine($"Error occured: ${ex.InnerException}");
                return false;
            }
        }
        public async Task<List<JobPostingDTO>> GetRandomSet()
        {
            var jobPostings = _mapper.Map<List<JobPostingDTO>>(await _ctx.JobPostings.Skip(new Random().Next(1, 13)).Take(10).ToListAsync());
            return jobPostings;
        }

        public async Task<List<JobPostingDTO>> ConfigureSearch(HomeIndexViewModel homeIndexVm)
        {
            int fromNumber = 0;
            if (homeIndexVm.FindModel.Page > 1)
            {
                fromNumber = homeIndexVm.FindModel.Page * 12;
            }
            List<JobPostingDTO> jobsCollection = await _es.QueryJobPosting(fromNumber, homeIndexVm.FindModel.KeyWords);

            return jobsCollection;
        }

        public JobPosting GetByIdWithKeyPhrases(int id)
        {
            JobPosting jobPosting = _ctx.JobPostings.Where(x => x.Id == id)
                .Include(x => x.KeyPhrases)
                .FirstOrDefault();
            return jobPosting;
        }

        public async Task<List<KeyPhraseDTO>> GetByKeyPhrases(int id)
        {
            List<KeyPhrase> keyPhrases = await _ctx.KeyPhrase.Include(x => x.JobPosting)
                .Where(x => x.JobPosting.Id == id)
                .Take(10)
                .ToListAsync();
            return _mapper.Map<List<KeyPhraseDTO>>(keyPhrases);
        }

        public async Task<List<JobPosting>> GetAll()
        {
            List<JobPosting> jobs = await _ctx.JobPostings.ToListAsync();
            return jobs;
        }

        public List<JobPostingDTO> GetAllFromElastic()
        {
            List<JobPostingDTO> jobsCollection = _es.GetAllJobPostings();
            return jobsCollection;
        }

        public async Task<List<JobPosting>> GetAllWithOutSummary()
        {
            List<JobPosting> jobs = await _ctx.JobPostings
                .Where(x => x.Summary.Length <= 4 && !x.Summary.Contains("NULL")).ToListAsync();
            return jobs;
        }

    }
}
