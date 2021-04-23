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
    public class GetJobPostingsJob
    {
        private const int MillisecondsTimeout = 30000;
        private readonly ILogger<GetJobPostingsJob> _logger;
        private readonly ApplicationDbContext _ctx;

        public GetJobPostingsJob(
            ILogger<GetJobPostingsJob> logger,
            ApplicationDbContext ctx)
        {
            _logger = logger;
            _ctx = ctx;
        }

        public void Run(IJobCancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            RunAtTimeOf(DateTime.Now);
        }

        public void RunAtTimeOf(DateTime now)
        {
            _logger.LogInformation("GetJobPostingsJob Starts... ");
            var mainConfig = _ctx.JobGettingConfig.FirstOrDefault(x => x.Id == 1);
            
            if(mainConfig != null)
            {
                var cities = _ctx.PositionCities.Where(x => x.JobGettingConfigId == mainConfig.Id).ToList();
                foreach (var city in cities)
                {
                    var positions = _ctx.PositionName.Where(x => x.JobGettingConfigId == mainConfig.Id).ToList();
                    foreach (var position in positions)
                    {
                        List<int> lissss = new();
                        for (int i = 0; i < int.Parse(mainConfig.MaxNumber); i++)
                        {
                            var startPoint = i * 10;
                            string azureFuncLink = getRandomAzureFunctionLink(mainConfig);
                            string json = JsonConvert.SerializeObject(new
                            {
                                City = city.Name,
                                Pos = position.Name,
                                Start = startPoint,
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
                                RequestUri = new Uri(azureFuncLink),
                                Content = new StringContent(json, Encoding.UTF8, ContentType.Json),
                            };

                            HttpClient client = new HttpClient();

                            var stuff = client.Send(request);
                        }

                        Thread.Sleep(MillisecondsTimeout);

                    }
                }
            }

            _logger.LogInformation("GetJobPostingsJob Ends... ");
        }

        private static string getRandomAzureFunctionLink(Jobtransparency.Models.Entity.JobGettingConfig mainConfig)
        {
            Random rnd = new();
            int value = rnd.Next(1, 11);
            var azureFuncLink = "";
            if (value >= 5)
            {
                azureFuncLink = mainConfig.LinkAzureFunction;
            }
            else
            {
                azureFuncLink = mainConfig.LinkAzureFunction2;
            }

            return azureFuncLink;
        }
    }
}
