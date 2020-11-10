using AJobBoard.Models.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AJobBoard.Data
{
    public interface IKeyPharseRepository
    {
        void CreateKeyPhrases(List<KeyPhrase> KeyPhrases);
        List<KeyPhrase> GetKeyPhrasesAsync(int JobId);
    }
}