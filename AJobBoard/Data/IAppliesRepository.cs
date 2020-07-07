using AJobBoard.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AJobBoard.Data
{
    public interface IAppliesRepository
    {
        Task<List<Apply>> GetUsersAppliesAsync(ApplicationUser User);
        Task<Apply> GetApplyAsync(int id);
        Task<Apply> DeleteAppliesAsync(int id);
        Task<Apply> PutApplyAsync(int id, Apply apply);
    }
}