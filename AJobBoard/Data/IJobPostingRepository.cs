using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AJobBoard.Models;
using AJobBoard.Models.DTO;

namespace AJobBoard.Data
{
    public interface IJobPostingRepository
    {
        Task<IEnumerable<JobPosting>> GetJobPostingsWithKeyPhraseAsync(int amount);
        Task<string> GetTotalJobs();
        Task<JobPosting> GetJobPostingById(int id);
        Task<IEnumerable<JobPosting>> GetJobPostingsAsync(int amount);
        Task<IEnumerable<JobPosting>> GetAllNoneKeywordsJobPostings();
        Task<bool> JobPostingExistsByUrl(string url);
        Task<bool> JobPostingExistsByDescription(string Summary);
        Task<JobPosting> PutJobPostingAsync(int id, JobPosting jobPosting);
        Task<JobPosting> CreateJobPostingAsync(JobPosting jobPosting);
        Task<JobPosting> DeleteJobPostingAsync(int id);
        Task<JobPosting> TickNumberOfViewAsync(JobPosting jobPosting);
        Task<(List<JobPosting>, TimeSpan)> ConfigureSearchAsync(HomeIndexViewModel homeIndexVM);
        Task<bool> JobPostingExists(TestCheckDTO tcDTO);
        Task<List<JobPosting>> GetRandomSetOfJobPostings();
    }
}