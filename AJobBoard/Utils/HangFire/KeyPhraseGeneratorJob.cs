using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AJobBoard.Data;
using AJobBoard.Models;
using AJobBoard.Models.Data;
using AJobBoard.Services;
using Amazon.Runtime.Internal.Util;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ILogger = Amazon.Runtime.Internal.Util.ILogger;

namespace AJobBoard.Utils.HangFire
{
    public class KeyPhraseGeneratorJob : IMyJob
    {
        private readonly ILogger<KeyPhraseGeneratorJob> _logger;
        private readonly IJobPostingRepository _jobPostingRepository;
        private readonly INLTKService _NLTKService;
        private readonly IKeyPharseRepository _KeyPharseRepository;
        private readonly ApplicationDbContext _ctx;

        public KeyPhraseGeneratorJob(ILogger<KeyPhraseGeneratorJob> logger, 
            IJobPostingRepository jobPostingRepository,
            INLTKService NLTKService,
            IKeyPharseRepository KeyPharseRepository,
            ElasticService es,
            ApplicationDbContext ctx)
        {
            _jobPostingRepository = jobPostingRepository;
            _NLTKService = NLTKService;
            _KeyPharseRepository = KeyPharseRepository;
            _logger = logger;
            _ctx = ctx;
        }

        public async Task Run(IJobCancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            await RunAtTimeOf(DateTime.Now);
        }

        public async Task RunAtTimeOf(DateTime now)
        {
            _logger.LogInformation("My Job Starts... ");
            IEnumerable<JobPosting> things = await _jobPostingRepository.GetJobPostingsWithKeyPhraseAsync(10000);

                foreach (var JobPosting in things)
                {
                    bool change = false;

                    if (JobPosting.KeyPhrases == null || JobPosting.KeyPhrases.Count == 0)
                    {
                        var wrapper = await _NLTKService.GetNLTKKeyPhrases(JobPosting.Description);
                        if (wrapper != null && wrapper.rank_list != null && wrapper.rank_list.Count > 0)
                        {
                            var ListKeyPhrase = new List<KeyPhrase>();

                            foreach (var item in wrapper.rank_list)
                            {
                                if (double.Parse(item.Affinty) > 20)
                                {
                                    ListKeyPhrase.Add(new KeyPhrase
                                    {
                                        Affinty = item.Affinty,
                                        Text = item.Text,
                                        JobPosting = JobPosting
                                    });
                                }

                            }

                            await _KeyPharseRepository.CreateKeyPhrasesAsync(ListKeyPhrase);

                            JobPosting.KeyPhrases = ListKeyPhrase;
                            change = true;
                        }


                    }


                    if (change)
                    {
                        await _jobPostingRepository.PutJobPostingAsync(JobPosting.Id, JobPosting);
                    }
                }

                await _ctx.SaveChangesAsync();
                _logger.LogInformation("My Job Ends... ");
        }
    }
}
