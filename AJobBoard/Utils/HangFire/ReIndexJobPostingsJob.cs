using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AJobBoard.Data;
using AJobBoard.Models;
using AJobBoard.Models.Data;
using AJobBoard.Services;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AJobBoard.Utils.HangFire
{
    public class ReIndexJobPostingsJob : IMyJob
    {
        private readonly ILogger<ReIndexJobPostingsJob> _logger;
        private readonly IJobPostingRepository _jobPostingRepository;
        private readonly ElasticService _es;
        private readonly ApplicationDbContext _ctx;

        public ReIndexJobPostingsJob(ILogger<ReIndexJobPostingsJob> logger,
            IJobPostingRepository jobPostingRepository,
            ApplicationDbContext ctx,
            ElasticService elasticService)
        {
            _jobPostingRepository = jobPostingRepository;
            _logger = logger;
            _es = elasticService;
            _ctx = ctx;
        }

        public async Task Run(IJobCancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            await RunAtTimeOf(DateTime.Now);
        }

        public async Task RunAtTimeOf(DateTime now)
        {
            _logger.LogInformation("ReIndexJobPostingsJob Starts... ");
            //if (await _es.DeleteJobPostingIndexAsync())
            //{
                var jobs = await _jobPostingRepository.GetAllJobPostingsWithKeyPhrase();
                foreach (var item in jobs)
                {
                    await _es.CreateJobPostingAsync(item);
                }
            //}

            _logger.LogInformation("ReIndexJobPostingsJob Ends... ");
        }
    }
}
