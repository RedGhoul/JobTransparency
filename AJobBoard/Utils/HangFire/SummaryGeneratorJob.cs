using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AJobBoard.Data;
using AJobBoard.Models;
using AJobBoard.Models.Data;
using AJobBoard.Services;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace AJobBoard.Utils.HangFire
{
    public class SummaryGeneratorJob : IMyJob
    {
        private readonly ILogger<KeyPhraseGeneratorJob> _logger;
        private readonly IJobPostingRepository _jobPostingRepository;
        private readonly INLTKService _nltkService;

        public SummaryGeneratorJob(ILogger<KeyPhraseGeneratorJob> logger,
            IJobPostingRepository jobPostingRepository,
            INLTKService nltkService)
        {
            _jobPostingRepository = jobPostingRepository;
            _nltkService = nltkService;
            _logger = logger;
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

            foreach (var jobPosting in things)
            {
                if (string.IsNullOrEmpty(jobPosting.Summary))
                {
                    var nltkSummary = await _nltkService.GetNLTKSummary(jobPosting.Description);

                    jobPosting.Summary = nltkSummary.SummaryText;

                    await _jobPostingRepository.PutJobPostingAsync(jobPosting.Id, jobPosting);
                }
            }

            _logger.LogInformation("My Job Ends... ");
        }
    }
}
