using AJobBoard.Data;
using AJobBoard.Models.Dto;
using AJobBoard.Models.Entity;
using AJobBoard.Services;
using Hangfire;
using Jobtransparency.Models.DTO.QuickType;
using Jobtransparency.Models.DTO.QuickType.RemotiveIoData;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Jobtransparency.HangFire
{
    public class RemotiveIoDataJob
    {
        private readonly int MinAffintyScore = 5;
        private readonly ApplicationDbContext _ctx;
        private readonly ILogger<OkRemoteJob> _logger;
        private readonly INLTKService _nltkService;

        public RemotiveIoDataJob(INLTKService nltkService, ILogger<OkRemoteJob> logger, ApplicationDbContext ctx)
        {
            _ctx = ctx;
            _logger = logger;
            _nltkService = nltkService;
        }

        public async Task Run(IJobCancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            await RunAtTimeOfAsync(DateTime.Now);
        }

        public async Task RunAtTimeOfAsync(DateTime now)
        {
            HttpClient client = new HttpClient();

            var stuff = await client.GetAsync("https://remotive.io/api/remote-jobs");


            if (stuff.IsSuccessStatusCode)
            {
                var contentString = await stuff.Content.ReadAsStringAsync();

                try
                {
                    var remotiveIO = RemotiveIoData.FromJson(contentString);

                    foreach (var okRemoteJob in remotiveIO.Jobs)
                    {
                        if (string.IsNullOrEmpty(okRemoteJob.Title)) continue;
                        if(!_ctx.JobPostings.Any(x => x.Title == okRemoteJob.Title && x.Description == okRemoteJob.Description))
                        {
                            var Description = new string(okRemoteJob.Description.Where(c => !char.IsPunctuation(c)).ToArray());
                            SummaryDTO nltkSummary = await _nltkService.ExtractSummary(Description);
                            KeyPhrasesWrapperDTO wrapper = await _nltkService.ExtractKeyPhrases(Description);

                            var newJobPosting = new JobPosting()
                            {
                                Title = okRemoteJob.Title,
                                Description = okRemoteJob.Description,
                                URL = okRemoteJob.Url.OriginalString,
                                Company = okRemoteJob.CompanyName,
                                Location = okRemoteJob.CandidateRequiredLocation,
                                PostDate = okRemoteJob.PublicationDate.ToString(),
                                Salary = okRemoteJob.Salary,
                                JobSource = "RemotiveIo",
                                CompanyLogoUrl = okRemoteJob.CompanyLogoUrl == null ? "" : okRemoteJob.CompanyLogoUrl.OriginalString,
                                Summary = nltkSummary.SummaryText
                            };

                            await _ctx.AddAsync(newJobPosting);
                            await _ctx.SaveChangesAsync();

                            List<KeyPhrase> ListKeyPhrase = new();
                            _logger.LogInformation("List<KeyPhrase> ListKeyPhrase");
                            foreach (KeyPhraseDTO KeyPhrase in wrapper.rank_list)
                            {
                                if (KeyPhrase.Affinty > MinAffintyScore)
                                {
                                    _ctx.KeyPhrase.Add(new KeyPhrase
                                    {
                                        Affinty = KeyPhrase.Affinty,
                                        Text = KeyPhrase.Text,
                                        JobPostingId = newJobPosting.Id
                                    });
                                }


                                _logger.LogInformation($"item.Affinty {KeyPhrase.Affinty}");
                                _logger.LogInformation($"item.Text {KeyPhrase.Text}");

                            }
                            await _ctx.SaveChangesAsync();

                            if (_ctx.Tags.Any(x => x.Text.Trim() == okRemoteJob.Category))
                            {
                                var tagFromDB = _ctx.Tags.Where(x => x.Text.Trim() == okRemoteJob.Category.Trim()).FirstOrDefault();
                                newJobPosting.Tags.Add(tagFromDB);
                            }
                            else
                            {
                                newJobPosting.Tags.Add(new Models.Entity.Tag()
                                {
                                    Text = okRemoteJob.Category.Trim()
                                });
                            }
                            await _ctx.SaveChangesAsync();
                        }
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                    
                }

            }
            else
            {
                throw new Exception("Is IsSuccessStatusCode was False");
            }
        }
    }
}
