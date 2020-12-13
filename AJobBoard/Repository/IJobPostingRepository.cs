using AJobBoard.Models.Dto;
using AJobBoard.Models.Entity;
using AJobBoard.Models.View;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AJobBoard.Data
{
    public interface IJobPostingRepository
    {
        Task<List<JobPosting>> GetAllWithOutSummary();
        Task<IEnumerable<JobPosting>> GetWithKeyPhrase(int amount);
        Task<List<JobPosting>> GetAllWithKeyPhrase();
        Task<List<JobPosting>> GetAll();
        Task<string> GetTotalAsync();
        Task<JobPosting> GetById(int id);
        JobPosting GetByIdWithKeyPhrases(int id);
        Task<IEnumerable<JobPosting>> GetAllNoneKeywords();
        Task<bool> ExistsByUrl(string url);
        Task<bool> DoesExistByDescription(string Summary);
        Task<JobPosting> Put(int id, JobPosting jobPosting);
        Task<JobPosting> Create(JobPosting jobPosting);
        Task<JobPosting> DeleteById(int id);
        Task<JobPosting> IncrementNumberOfViews(JobPosting jobPosting);
        Task<List<JobPostingDTO>> ConfigureSearch(HomeIndexViewModel homeIndexVm);
        Task<List<KeyPhraseDTO>> GetByKeyPhrases(int id);
        Task<bool> Exists(TestCheckDTO tcDto);
        Task<List<JobPostingDTO>> GetRandomSet();
        Task<bool> IncrementNumberOfApplies(int JobPostingId);

    }
}