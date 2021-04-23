using AJobBoard.Data;
using AJobBoard.Models.Dto;
using AJobBoard.Models.Entity;
using AJobBoard.Services;
using Hangfire;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AJobBoard.Utils.HangFire
{
    public class SummaryGeneratorJob : ICustomJob
    {
        private readonly ILogger<KeyPhraseGeneratorJob> _logger;
        private readonly IJobPostingRepository _jobPostingRepository;
        private readonly INLTKService _nltkService;
        private readonly ApplicationDbContext _ctx;

        public SummaryGeneratorJob(ILogger<KeyPhraseGeneratorJob> logger,
            IJobPostingRepository jobPostingRepository,
            INLTKService nltkService,
            ApplicationDbContext ctx)
        {
            _jobPostingRepository = jobPostingRepository;
            _nltkService = nltkService;
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
            _logger.LogInformation("SummaryGeneratorJob Starts... ");
            List<JobPosting> jobpostingsWithoutSummaries = await _jobPostingRepository.GetAllWithOutSummary();

            foreach (JobPosting jobPosting in jobpostingsWithoutSummaries)
            {
                var Description = new string(jobPosting.Description.Where(c => !char.IsPunctuation(c)).ToArray());
                SummaryDTO nltkSummary = await _nltkService.ExtractSummary(Description);

                jobPosting.Summary = nltkSummary.SummaryText;

                await _jobPostingRepository.Put(jobPosting.Id, jobPosting);
            }

            _logger.LogInformation("SummaryGeneratorJob Ends... ");
        }
    }
}
