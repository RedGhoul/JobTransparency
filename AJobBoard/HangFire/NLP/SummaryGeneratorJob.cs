using AJobBoard.Data;
using AJobBoard.Models.Dto;
using AJobBoard.Models.Entity;
using AJobBoard.Services;
using AJobBoard.Utils.Config;
using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
        private readonly IConfiguration _configuration;

        public SummaryGeneratorJob(ILogger<KeyPhraseGeneratorJob> logger,
            IJobPostingRepository jobPostingRepository,
            INLTKService nltkService,
            ApplicationDbContext ctx,
            IConfiguration configuration)
        {
            _jobPostingRepository = jobPostingRepository;
            _nltkService = nltkService;
            _logger = logger;
            _ctx = ctx;
            _configuration = configuration;

        }

        public async Task Run(IJobCancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            await RunAtTimeOf(DateTime.Now);
        }

        public async Task RunAtTimeOf(DateTime now)
        {
            _logger.LogInformation("SummaryGeneratorJob Starts... ");
            string connectionString = Secrets.GetDBConnectionString(_configuration);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Create the Command and Parameter objects.
                SqlCommand command = new SqlCommand(@"
                      SELECT [Id]
                          ,[Description]
                      FROM [JobTransparency].[dbo].[JobPostings] 
                      WHERE [Description] = ''
                ", connection);


                // Open the connection in a try/catch block.
                // Create and execute the DataReader, writing the result
                // set to the console window.
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var Id = (int)reader[0];
                        var Description = (string)reader[1];

                        Description = new string(Description.Where(c => !char.IsPunctuation(c)).ToArray());
                        SummaryDTO nltkSummary = await _nltkService.ExtractSummary(Description);

                        var Job = await _jobPostingRepository.GetById(Id);
                        Job.Summary = nltkSummary.SummaryText;

                        await _jobPostingRepository.Put(Job.Id, Job);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    _logger.LogInformation(ex,"SummaryGeneratorJob Ends... ");
                }
            }

            _logger.LogInformation("SummaryGeneratorJob Ends... ");
        }
    }
}
