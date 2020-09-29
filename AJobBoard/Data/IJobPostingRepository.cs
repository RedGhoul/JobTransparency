using AJobBoard.Models;
using AJobBoard.Models.DTO;
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
        string GetTotal();
        Task<JobPosting> GetById(int id);
        JobPosting GetByIdWithKeyPhrases(int id);
        Task<IEnumerable<JobPosting>> GetAllNoneKeywords();
        Task<bool> ExistsByUrl(string url);
        Task<bool> DoesExistByDescription(string Summary);
        Task<JobPosting> Put(int id, JobPosting jobPosting);
        Task<JobPosting> Create(JobPosting jobPosting);
        Task<JobPosting> DeleteById(int id);
        Task<JobPosting> AddView(JobPosting jobPosting);
        Task<List<JobPostingDTO>> ConfigureSearch(HomeIndexViewModel homeIndexVm);
        Task<List<KeyPhraseDTO>> GetByKeyPhrases(int id);
        Task<bool> Exists(TestCheckDTO tcDto);
        Task<List<JobPostingDTO>> GetRandomSet();
        Task<bool> AddApply(int JobPostingId, ApplicationUser User);

    }
}