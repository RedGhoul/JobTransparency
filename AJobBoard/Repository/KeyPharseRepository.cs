﻿using AJobBoard.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public void CreateKeyPhrases(List<KeyPhrase> KeyPhrases)
        {
            try
            {
                foreach (KeyPhrase item in KeyPhrases)
                {
                    _ctx.KeyPhrase.Add(item);
                }
                _ctx.SaveChanges();
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.StackTrace);
            }
        }

        public List<KeyPhrase> GetKeyPhrasesAsync(int JobId)
        {
            IQueryable<KeyPhrase> things = _ctx.KeyPhrase.Include(x => x.JobPosting).Where(x => x.JobPosting.Id == JobId);
            return things.ToList();
        }
    }
}
