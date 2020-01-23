using System.Collections.Generic;
using System.Threading.Tasks;
using AJobBoard.Models;

namespace AJobBoard.Data
{
    public interface IAppliesRepository
    {
        Task<List<Apply>> GetUsersAppliesAsync(ApplicationUser User);
    }
}