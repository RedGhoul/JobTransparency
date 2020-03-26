using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AJobBoard.Data;
using AJobBoard.Models;
using AJobBoard.Models.Data;
using AJobBoard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nest;
using Newtonsoft.Json;

namespace AJobBoard.Controllers.Views
{
    [Authorize(Roles="Admin")]
    //[ApiController]
    public class DataAdminController : Controller
    {
        private readonly IJobPostingRepository _jobPostingRepository;
        private readonly INLTKService _NLTKService;
        private readonly IKeyPharseRepository _KeyPharseRepository;
        private readonly ElasticService _es;
        private readonly ApplicationDbContext _ctx;

        public DataAdminController(
            IJobPostingRepository jobPostingRepository,
            INLTKService NLTKService,
            IKeyPharseRepository KeyPharseRepository,
            ElasticService es,
            ApplicationDbContext ctx)
        {
            _jobPostingRepository = jobPostingRepository;
            _NLTKService = NLTKService;
            _KeyPharseRepository = KeyPharseRepository;
            _es = es;
            _ctx = ctx;
        }



        public async Task<IActionResult> Ingest()
        {
            var status = await _ctx.ETLStatus.OrderByDescending(x => x.Started)
                .Where(x => x.Finished == false)
                .FirstOrDefaultAsync();

            if (status != null)
            {
                IEnumerable<JobPosting> things = await _jobPostingRepository.GetJobPostingsWithKeyPhraseAsync(10000);

                foreach (var JobPosting in things)
                {
                    bool change = false;

                    if (JobPosting.KeyPhrases == null || JobPosting.KeyPhrases.Count == 0)
                    {
                        var wrapper = await _NLTKService.GetNLTKKeyPhrases(JobPosting.Description);
                        if (wrapper != null && wrapper.rank_list != null && wrapper.rank_list.Count > 0)
                        {
                            var ListKeyPhrase = new List<KeyPhrase>();

                            foreach (var item in wrapper.rank_list)
                            {
                                if (double.Parse(item.Affinty) > 20)
                                {
                                    ListKeyPhrase.Add(new KeyPhrase
                                    {
                                        Affinty = item.Affinty,
                                        Text = item.Text,
                                        JobPosting = JobPosting
                                    });
                                }

                            }

                            await _KeyPharseRepository.CreateKeyPhrasesAsync(ListKeyPhrase);

                            JobPosting.KeyPhrases = ListKeyPhrase;
                            change = true;
                        }


                    }

                    if (string.IsNullOrEmpty(JobPosting.Summary))
                    {
                        var NLTKSummary = await _NLTKService.GetNLTKSummary(JobPosting.Description);

                        JobPosting.Summary = NLTKSummary.SummaryText;
                        change = true;
                    }

                    if (change == true)
                    {
                        await _jobPostingRepository.PutJobPostingAsync(JobPosting.Id, JobPosting);
                        change = false;
                    }
                }

                status.Finished = true;
                status.Ended = DateTime.Now;
                await _ctx.SaveChangesAsync();

            }

            return RedirectToAction("Index", "ETLStatus");
        }


        public async Task<IActionResult> IndexJobPostings()
        {

            for (int i = 5349; i < 5413; i++)
            {
                var item =  _jobPostingRepository.GetJobPostingByIdWithKeyPhrase(i);
                if (item != null)
                {
                    if (item.Summary == null)
                    {
                        item.Summary = "";
                    }

                    if (item.Description == null)
                    {
                        item.Description = "";
                    }

                    if (item.KeyPhrases == null)
                    {
                        item.KeyPhrases = new List<KeyPhrase>();
                        item.KeyPhrases.Add(new KeyPhrase
                        {
                            Affinty = "Affinty",
                            Text = "item.Text",
                            JobPosting = item
                        });
                    }
                    var json = JsonConvert.SerializeObject(item, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });
                    var data = new StringContent(json, Encoding.UTF8, "application/json");

                    var client = new HttpClient();

                    var response = await client.PutAsync("http://a-main-elastic.experimentsinthedeep.com" + "/jobpostings/_doc/" + item.Id, data);
                }
              
            }


            return RedirectToAction("Index", "Home");
        }
    }
}