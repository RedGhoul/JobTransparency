using AJobBoard.Models;
using Jobtransparency.Models.DTO;
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