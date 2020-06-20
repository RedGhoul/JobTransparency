using System.Collections.Generic;
using System.Threading.Tasks;
using AJobBoard.Models.Data;

namespace AJobBoard.Data
{
    public interface IKeyPharseRepository
    {
        Task CreateKeyPhrasesAsync(List<KeyPhrase> KeyPhrases);
        List<KeyPhrase> GetKeyPhrasesAsync(int JobId);
    }
}