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
using System.Threading.Tasks;

namespace AJobBoard.Utils.HangFire
{
    public class GetJobPostingsJob : ICustomJob
    {
        private readonly ILogger<GetJobPostingsJob> _logger;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ApplicationDbContext _ctx;

        public GetJobPostingsJob(
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
            _logger.LogInformation("GetJobPostingsJob Starts... ");
            var mainConfig = _ctx.JobGettingConfig.FirstOrDefault(x => x.Id == 1);
            
            if(mainConfig != null)
            {
                var azureFunctionLink = mainConfig.LinkAzureFunction;

                var cities = _ctx.PositionCities.Where(x => x.JobGettingConfigId == mainConfig.Id).ToList();
                foreach (var city in cities)
                {
                    var positions = _ctx.PositionName.Where(x => x.JobGettingConfigId == mainConfig.Id).ToList();
                    foreach (var position in positions)
                    {
                        for (int i = 0; i < int.Parse(mainConfig.MaxNumber); i++)
                        {
                            int currInt = i * 10;
                            string json = JsonConvert.SerializeObject(new
                            {
                                City = city.Name,
                                Pos = position.Name,
                                Start = currInt,
                                UA = RandomUa.RandomUserAgent,
                                Job_Type = "fulltime",
                                Max_Age = mainConfig.MaxAge,
                                TechTransLink = mainConfig.LinkJobPostingCreation,
                                TechTransCheckLink = mainConfig.LinkCheckIfJobExists,
                                Host = mainConfig.Host
                            });

                            var request = new HttpRequestMessage
                            {
                                Method = HttpMethod.Get,
                                RequestUri = new Uri(azureFunctionLink),
                                Content = new StringContent(json, Encoding.UTF8, ContentType.Json),
                            };

                            HttpClient client = _clientFactory.CreateClient("NLTK");

                            var stuff = client.Send(request);
                        }
                        
                    }
                }
            }

            _logger.LogInformation("GetJobPostingsJob Ends... ");
        }
    }
}
