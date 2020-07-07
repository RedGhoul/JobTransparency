using AJobBoard.Data;
using AJobBoard.Models;
using AJobBoard.Models.Data;
using AJobBoard.Services;
using Hangfire;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            _logger.LogInformation("KeyPhraseGeneratorJob Job Starts... ");
            IEnumerable<JobPosting> things = await _jobPostingRepository.GetAllNoneKeywordsJobPostings();

            foreach (var JobPosting in things)
            {
                
                    var wrapper = await _NLTKService.GetNLTKKeyPhrases(JobPosting.Description);
                    if (wrapper != null && wrapper.rank_list != null && wrapper.rank_list.Count > 0)
                    {
                        var ListKeyPhrase = new List<KeyPhrase>();

                        foreach (var item in wrapper.rank_list)
                        {
                           
                                ListKeyPhrase.Add(new KeyPhrase
                                {
                                    Affinty = item.Affinty,
                                    Text = item.Text,
                                    JobPosting = JobPosting
                                });

                        }

                        await _KeyPharseRepository.CreateKeyPhrasesAsync(ListKeyPhrase);

                        JobPosting.KeyPhrases = ListKeyPhrase;
                    }
                
                    await _jobPostingRepository.PutJobPostingAsync(JobPosting.Id, JobPosting);
            }

            _logger.LogInformation("My Job Ends... ");
        }
    }
}
