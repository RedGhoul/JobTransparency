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
    public class KeyPhraseGeneratorJob : ICustomJob
    {
        private readonly int MinAffintyScore = 5;
        private readonly ILogger<KeyPhraseGeneratorJob> _logger;
        private readonly IJobPostingRepository _jobPostingRepository;
        private readonly INLTKService _NLTKService;
        private readonly IKeyPharseRepository _KeyPharseRepository;
        private readonly ApplicationDbContext _ctx;
        private readonly IConfiguration _configuration;

        public KeyPhraseGeneratorJob(ILogger<KeyPhraseGeneratorJob> logger,
            IJobPostingRepository jobPostingRepository,
            INLTKService NLTKService,
            IKeyPharseRepository KeyPharseRepository,
            ApplicationDbContext ctx,
            IConfiguration configuration)
        {
            _jobPostingRepository = jobPostingRepository;
            _NLTKService = NLTKService;
            _KeyPharseRepository = KeyPharseRepository;
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
            _logger.LogInformation("KeyPhraseGeneratorJob Job Starts... ");

            string connectionString = Secrets.GetDBConnectionString(_configuration);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Create the Command and Parameter objects.
                SqlCommand command = new SqlCommand(@"
                    SELECT [Id]
                          ,[Description]
                      FROM [JobTransparency].[dbo].[JobPostings] 
                      WHERE [Id] NOT IN (SELECT [JobPostingId] FROM [dbo].[KeyPhrase])

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
                        var Description = (string)reader[1];
                        var Id = (int)reader[0];
                        if (Description.Length <= 5)
                        {
                            continue;
                        }

                        Description = new string(Description.Where(c => !char.IsPunctuation(c)).ToArray());

                        KeyPhrasesWrapperDTO wrapper = await _NLTKService.ExtractKeyPhrases(Description);

                        if (wrapper != null && wrapper.rank_list != null && wrapper.rank_list.Count > 0)
                        {
                            List<KeyPhrase> ListKeyPhrase = new List<KeyPhrase>();
                            _logger.LogInformation("List<KeyPhrase> ListKeyPhrase");
                            foreach (KeyPhraseDTO item in wrapper.rank_list)
                            {
                                if (float.Parse(item.Affinty) > MinAffintyScore)
                                {
                                    ListKeyPhrase.Add(new KeyPhrase
                                    {
                                        Affinty = float.Parse(item.Affinty),
                                        Text = item.Text,
                                        JobPostingId = Id
                                    });
                                }


                                _logger.LogInformation($"item.Affinty {item.Affinty}");
                                _logger.LogInformation($"item.Text {item.Text}");

                            }

                            _KeyPharseRepository.CreateKeyPhrases(ListKeyPhrase);
                            _logger.LogInformation("_KeyPharseRepository.CreateKeyPhrases(ListKeyPhrase);");
                        }

                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,"KeyPhraseGeneratorJob Ends... ");
                }
            }

            _logger.LogInformation("KeyPhraseGeneratorJob Ends... ");
        }
    }
}
