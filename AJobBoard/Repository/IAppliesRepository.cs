using AJobBoard.Models.Entity;
using Jobtransparency.Models.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AJobBoard.Data
{
    public interface IAppliesRepository
    {
        Task<List<AppliesDTO>> GetUsersAppliesAsync(ApplicationUser User);
        Task<Apply> GetApplyByIdAsync(int id);
        Task<bool> DeleteAppliesAsync(int id);
        Task<Apply> PutApplyAsync(int id, Apply apply);
    }
}