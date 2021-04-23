using AJobBoard.Data;
using AJobBoard.Models.Dto;
using AJobBoard.Models.Entity;
using AJobBoard.Services;
using Hangfire;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RandomUserAgent;
using RestSharp.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace AJobBoard.Utils.HangFire
{
    public class IsJobExpiredJobPostingsJob : ICustomJob
    {
        private readonly ILogger<GetJobPostingsJob> _logger;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ApplicationDbContext _ctx;
        private const int MillisecondsTimeout = 30000;
        public IsJobExpiredJobPostingsJob(
            IHttpClientFactory clientFactory,
            ILogger<GetJobPostingsJob> logger,
            ApplicationDbContext ctx)
        {
            _clientFactory = clientFactory;
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
            _logger.LogInformation("IsJobExpiredJobPostingsJob Starts... ");

            var sites = _ctx.JobPostings.ToList();

            foreach (var curSite in sites)
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(curSite.URL),
                };

                HttpClient client = _clientFactory.CreateClient("NLTK");

                var data = await client.SendAsync(request);

                var stringData = await data.Content.ReadAsStringAsync();

                if(stringData.Contains("This job has expired on Indeed"))
                {
                    curSite.Expried = true;
                    await _ctx.SaveChangesAsync();
                }

                Thread.Sleep(MillisecondsTimeout);
            }

            _logger.LogInformation("IsJobExpiredJobPostingsJob Ends... ");
        }
    }
}
