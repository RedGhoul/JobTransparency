using AJobBoard.Data;
using AJobBoard.Models.Dto;
using AJobBoard.Models.Entity;
using AJobBoard.Services;
using AJobBoard.Utils.Config;
using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Npgsql;
using RandomUserAgent;
using RestSharp.Serialization;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
        private readonly IConfiguration _configuration;
        private const int MillisecondsTimeout = 5000;

        public IsJobExpiredJobPostingsJob(
            IHttpClientFactory clientFactory,
            ILogger<GetJobPostingsJob> logger,
            ApplicationDbContext ctx,
            IConfiguration configuration)
        {
            _clientFactory = clientFactory;
            _logger = logger;
            _ctx = ctx;
            _configuration = configuration;
        }

        public async Task Run(IJobCancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            await RunAtTimeOf(DateTime.UtcNow);
        }

        public async Task RunAtTimeOf(DateTime now)
        {
            _logger.LogInformation("IsJobExpiredJobPostingsJob Starts... ");
            string connectionString = Secrets.GetDBConnectionString(_configuration);

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                // Create the Command and Parameter objects.
                NpgsqlCommand command = new NpgsqlCommand(@"
                      select ""Id"", ""URL""
                        from public.""JobPostings"" WHERE ""Expried"" = FALSE", connection);

                try
                {
                    connection.Open();
                    var reader = await command.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        var Id = (int)reader[0];
                        var URL = (string)reader[1];
                        var request = new HttpRequestMessage
                        {
                            Method = HttpMethod.Get,
                            RequestUri = new Uri(URL),
                        };

                        HttpClient client = _clientFactory.CreateClient("NLTK");

                        var data = await client.SendAsync(request);

                        var stringData = await data.Content.ReadAsStringAsync();

                        if (stringData.Contains("This job has expired on Indeed"))
                        {
                            var currentSite = _ctx.JobPostings.FirstOrDefault(x => x.Id == Id);
                            currentSite.Expried = true;
                            await _ctx.SaveChangesAsync();
                        }

                        Thread.Sleep(MillisecondsTimeout);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
            }


            _logger.LogInformation("IsJobExpiredJobPostingsJob Ends... ");
        }
    }
}
