using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AJobBoard.Models;
using AJobBoard.Models.Data;
using AJobBoard.Models.DTO;
using AJobBoard.Models.View;

namespace AJobBoard.Data
{
    public interface IJobPostingRepository
    {
        Task<IEnumerable<JobPosting>> GetJobPostingsWithKeyPhraseAsync(int amount);
        Task<List<JobPosting>> GetAllJobPostingsWithKeyPhrase();
        Task<string> GetTotalJobs();
        Task<JobPosting> GetJobPostingById(int id);
        JobPosting GetJobPostingByIdWithKeyPhrase(int id);
        Task<IEnumerable<JobPosting>> GetAllNoneKeywordsJobPostings();
        Task<bool> JobPostingExistsByUrl(string url);
        Task<bool> JobPostingExistsByDescription(string Summary);
        Task<JobPosting> PutJobPostingAsync(int id, JobPosting jobPosting);
        Task<JobPosting> CreateJobPostingAsync(JobPosting jobPosting);
        Task<JobPosting> DeleteJobPostingAsync(int id);
        Task<JobPosting> TickNumberOfViewAsync(JobPosting jobPosting);
        Task<List<JobPostingDTO>> ConfigureSearchAsync(HomeIndexViewModel homeIndexVm);
        Task<List<KeyPhraseDTO>> GetJobPostingKeyPhrases(int id);
        Task<bool> JobPostingExists(TestCheckDTO tcDto);
        Task<List<JobPostingDTO>> GetRandomSetOfJobPostings();

    }
}