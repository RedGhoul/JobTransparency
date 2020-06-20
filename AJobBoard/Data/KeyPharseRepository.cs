using AJobBoard.Models.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AJobBoard.Data
{
    public class KeyPharseRepository : IKeyPharseRepository
    {
        private readonly ApplicationDbContext _ctx;
        private readonly IDistributedCache _cache;

        public KeyPharseRepository(ApplicationDbContext ctx, IDistributedCache cache)
        {
            _ctx = ctx;
            _cache = cache;
        }

        public async Task CreateKeyPhrasesAsync(List<KeyPhrase> KeyPhrases)
        {
            try
            {
                foreach (var item in KeyPhrases)
                {
                    await _ctx.KeyPhrase.AddAsync(item);
                }
                await _ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        public List<KeyPhrase> GetKeyPhrasesAsync(int JobId)
        {
            var things = _ctx.KeyPhrase.Include(x => x.JobPosting).Where(x => x.JobPosting.Id == JobId);
            return things.ToList();
        }
    }
}
