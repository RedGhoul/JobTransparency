using System.Threading.Tasks;
using AJobBoard.Models.DTO;

namespace AJobBoard.Services
{
    public interface INLTKService
    {
        Task<KeyPhrasesWrapperDTO> GetNLTKKeyPhrases(string Description);
        Task<SummaryDTO> GetNLTKSummary(string Description);
    }
}