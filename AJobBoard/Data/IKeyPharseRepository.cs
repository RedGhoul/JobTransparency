using AJobBoard.Models.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AJobBoard.Data
{
    public interface IKeyPharseRepository
    {
        Task CreateKeyPhrasesAsync(List<KeyPhrase> KeyPhrases);
        List<KeyPhrase> GetKeyPhrasesAsync(int JobId);
    }
}