﻿using AJobBoard.Data;
using AJobBoard.Utils.HangFire;
using Hangfire;
using Jobtransparency.Models.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jobtransparency.HangFire
{
    public class GenerateKeyTopKeyWordsJobs : ICustomJob
    {
     
        private readonly ApplicationDbContext _ctx;

        public GenerateKeyTopKeyWordsJobs(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task Run(IJobCancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            await RunAtTimeOf(DateTime.UtcNow);
        }

        public async Task RunAtTimeOf(DateTime now)
        {
            if (_ctx.HangfireConfigs.Count() == 0) return;
            var config = _ctx.HangfireConfigs.FirstOrDefault();

            var allJobs = await _ctx.JobPostings
                .Include(x => x.Tags).Include(x => x.KeyPhrases)
                .Where(x => x.KeyPhrases.Count > 0 && x.Tags.Count == 0)
                .ToListAsync();
            foreach (var jobPosting in allJobs)
            {
                foreach (var keyPhrase in jobPosting.KeyPhrases)
                {
                    var keyPhraseText = keyPhrase.Text.Trim();

                    if (keyPhrase.Affinty > config.AffinityThreshold && keyPhraseText.Length <= config.MinKeyPhraseLengthThreshold)
                    {
                        if (_ctx.Tags.Any(x => x.Text.Trim() == keyPhraseText))
                        {
                            var tagFromDB = _ctx.Tags.Where(x => x.Text.Trim() == keyPhraseText).FirstOrDefault();
                            jobPosting.Tags.Add(tagFromDB);
                        }
                        else
                        {
                            jobPosting.Tags.Add(new Tag()
                            {
                                Text = keyPhraseText
                            });
                        }
                        await _ctx.SaveChangesAsync();
                    }
                }
            }
        }
    }
}
