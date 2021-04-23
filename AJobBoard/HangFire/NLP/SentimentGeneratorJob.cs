using AJobBoard.Data;
using AJobBoard.Models.Dto;
using AJobBoard.Models.Entity;
using AJobBoard.Services;
using AutoMapper;
using Hangfire;
using Jobtransparency.Models.DTO;
using Jobtransparency.Models.Entity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AJobBoard.Utils.HangFire
{
    public class SentimentGeneratorJob : ICustomJob
    {
        private readonly ILogger<KeyPhraseGeneratorJob> _logger;
        private readonly IJobPostingRepository _jobPostingRepository;
        private readonly INLTKService _nltkService;
        private readonly ApplicationDbContext _ctx;
        private readonly IMapper _mapper;

        public SentimentGeneratorJob(IMapper mapper, ILogger<KeyPhraseGeneratorJob> logger,
            IJobPostingRepository jobPostingRepository,
            INLTKService nltkService,
            ApplicationDbContext ctx)
        {
            _jobPostingRepository = jobPostingRepository;
            _nltkService = nltkService;
            _logger = logger;
            _ctx = ctx;
            _mapper = mapper;
        }

        public async Task Run(IJobCancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            await RunAtTimeOf(DateTime.Now);
        }

        public async Task RunAtTimeOf(DateTime now)
        {
            _logger.LogInformation("SentimentGeneratorJob Starts... ");
            var JobPostingsWithSentiment = _ctx.Sentiment.ToList().Select(x => x.JobPostingId);
            List<JobPosting> jobpostingsWithoutSentiment = 
                _ctx.JobPostings.Where(x => !JobPostingsWithSentiment.Contains(x.Id)).ToList();

            foreach (JobPosting jobPosting in jobpostingsWithoutSentiment)
            {
                var Description = new string(jobPosting.Description.Where(c => !char.IsPunctuation(c)).ToArray());
                Sentiment sentiment = _mapper.Map<Sentiment>(await _nltkService.ExtractSentiment(Description));
                sentiment.JobPostingId = jobPosting.Id;
                _ctx.Sentiment.Add(sentiment);
                await _ctx.SaveChangesAsync();
            }

            _logger.LogInformation("SentimentGeneratorJob Ends... ");
        }
    }
}
