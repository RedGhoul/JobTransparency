using AJobBoard.Models.Dto;
using Jobtransparency.Models.DTO;
using System.Threading.Tasks;

namespace AJobBoard.Services
{
    public interface INLTKService
    {
        Task<KeyPhrasesWrapperDTO> ExtractKeyPhrases(string Description);
        Task<SummaryDTO> ExtractSummary(string Description);
        Task<SentimentDTO> ExtractSentiment(string Description);
    }
}