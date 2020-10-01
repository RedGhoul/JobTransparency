using AJobBoard.Models.Dto;
using System.Threading.Tasks;

namespace AJobBoard.Services
{
    public interface INLTKService
    {
        Task<KeyPhrasesWrapperDTO> GetNLTKKeyPhrases(string Description);
        Task<SummaryDTO> GetNLTKSummary(string Description);
    }
}