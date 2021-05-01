using AJobBoard.Data;
using AJobBoard.Models.Dto;
using AJobBoard.Models.Entity;
using AJobBoard.Services;
using Hangfire;
using Jobtransparency.Models.DTO.QuickType;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Jobtransparency.HangFire
{
    public class OkRemoteJob
    {
        private readonly int MinAffintyScore = 5;
        private readonly ApplicationDbContext _ctx;
        private readonly ILogger<OkRemoteJob> _logger;
        private readonly INLTKService _nltkService;

        public OkRemoteJob(INLTKService nltkService, ILogger<OkRemoteJob> logger, ApplicationDbContext ctx)
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

            var stuff = await client.GetAsync("https://remoteok.io/api");


            if (stuff.IsSuccessStatusCode)
            {
                var contentString = await stuff.Content.ReadAsStringAsync();

                try
                {
                    var okRemoteData = OkRemoteData.FromJson(contentString);

                    foreach (var okRemoteJob in okRemoteData)
                    {
                        if (string.IsNullOrEmpty(okRemoteJob.Position)) continue;
                        if(!_ctx.JobPostings.Any(x => x.Title == okRemoteJob.Position && x.Description == okRemoteJob.Description))
                        {
                            var Description = new string(okRemoteJob.Description.Where(c => !char.IsPunctuation(c)).ToArray());
                            SummaryDTO nltkSummary = await _nltkService.ExtractSummary(Description);
                            KeyPhrasesWrapperDTO wrapper = await _nltkService.ExtractKeyPhrases(Description);

                            var newJobPosting = new JobPosting()
                            {
                                Title = okRemoteJob.Position,
                                Description = okRemoteJob.Description,
                                URL = okRemoteJob.Url.OriginalString,
                                Company = okRemoteJob.Company,
                                Location = okRemoteJob.Location,
                                PostDate = okRemoteJob.Date.ToString(),
                                Salary = "",
                                JobSource = "RemoteOk",
                                CompanyLogoUrl = okRemoteJob.CompanyLogo,
                                Summary = nltkSummary.SummaryText
                            };

                            await _ctx.AddAsync(newJobPosting);
                            await _ctx.SaveChangesAsync();

                            List<KeyPhrase> ListKeyPhrase = new();
                            _logger.LogInformation("List<KeyPhrase> ListKeyPhrase");
                            foreach (KeyPhraseDTO KeyPhrase in wrapper.rank_list)
                            {
                                if (float.Parse(KeyPhrase.Affinty) > MinAffintyScore)
                                {
                                    _ctx.KeyPhrase.Add(new KeyPhrase
                                    {
                                        Affinty = float.Parse(KeyPhrase.Affinty),
                                        Text = KeyPhrase.Text,
                                        JobPostingId = newJobPosting.Id
                                    });
                                }


                                _logger.LogInformation($"item.Affinty {KeyPhrase.Affinty}");
                                _logger.LogInformation($"item.Text {KeyPhrase.Text}");

                            }
                            await _ctx.SaveChangesAsync();

                            foreach (var item in okRemoteJob.Tags)
                            {
                                if(_ctx.Tags.Any(x => x.Text.Trim() == item.Trim()))
                                {
                                    var tagFromDB = _ctx.Tags.Where(x => x.Text.Trim() == item.Trim()).FirstOrDefault();
                                    newJobPosting.Tags.Add(tagFromDB);
                                }
                                else
                                {
                                    newJobPosting.Tags.Add(new Models.Entity.Tag()
                                    {
                                        Text = item.Trim()
                                    });
                                }

                            }
                            await _ctx.SaveChangesAsync();
                        }
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to deserialize");
                }
               
            }
        }
    }
}
