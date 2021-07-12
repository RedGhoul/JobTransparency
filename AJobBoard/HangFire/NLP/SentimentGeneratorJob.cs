using AJobBoard.Data;
using AJobBoard.Models.Dto;
using AJobBoard.Models.Entity;
using AJobBoard.Services;
using AJobBoard.Utils.Config;
using AutoMapper;
using Hangfire;
using Jobtransparency.Models.DTO;
using Jobtransparency.Models.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
        private readonly IConfiguration _configuration;

        public SentimentGeneratorJob(IMapper mapper, ILogger<KeyPhraseGeneratorJob> logger,
            IJobPostingRepository jobPostingRepository,
            INLTKService nltkService,
            ApplicationDbContext ctx,
            IConfiguration configuration)
        {
            _jobPostingRepository = jobPostingRepository;
            _nltkService = nltkService;
            _logger = logger;
            _ctx = ctx;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task Run(IJobCancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            await RunAtTimeOf(DateTime.UtcNow);
        }

        public async Task RunAtTimeOf(DateTime now)
        {
            if (_ctx.HangfireConfigs.Count() == 0) return;
            var config = _ctx.HangfireConfigs.FirstOrDefault();

            _logger.LogInformation("SentimentGeneratorJob Starts... ");
            string connectionString = Secrets.GetDBConnectionString(_configuration);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Create the Command and Parameter objects.
                SqlCommand command = new SqlCommand(@"
                  SELECT Id,Description FROM JobPostings
                  WHERE Id NOT IN(SELECT JobPostingId FROM Sentiment)", connection);
                command.CommandTimeout = config.SQLCommandTimeOut;
                // Open the connection in a try/catch block.
                // Create and execute the DataReader, writing the result
                // set to the console window.
                try
                {
                    connection.Open();
                    var reader = await command.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        var Id = (int)reader[0];
                        var Description = (string)reader[1];
                        Description = new string(Description.Where(c => !char.IsPunctuation(c)).ToArray());
                        Sentiment sentiment = _mapper.Map<Sentiment>(await _nltkService.ExtractSentiment(Description));
                        sentiment.JobPostingId = Id;
                        _ctx.Sentiment.Add(sentiment);
                        await _ctx.SaveChangesAsync();
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    _logger.LogInformation(ex,"SentimentGeneratorJob Ends... ");
                }
            }

       
            _logger.LogInformation("SentimentGeneratorJob Ends... ");
        }
    }
}
