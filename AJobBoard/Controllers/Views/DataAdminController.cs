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
using Nest;
using Newtonsoft.Json;

namespace AJobBoard.Controllers.Views
{
    [Authorize(Roles="Admin")]
    public class DataAdminController : Controller
    {
        private readonly IJobPostingRepository _jobPostingRepository;
        private readonly INLTKService _NLTKService;
        private readonly IKeyPharseRepository _KeyPharseRepository;
        private readonly ElasticService _es;

        public DataAdminController(
            IJobPostingRepository jobPostingRepository,
            INLTKService NLTKService,
            IKeyPharseRepository KeyPharseRepository,
            ElasticService es)
        {
            _jobPostingRepository = jobPostingRepository;
            _NLTKService = NLTKService;
            _KeyPharseRepository = KeyPharseRepository;
            _es = es;
        }

       
        public async Task<IActionResult> Ingest()
        {
            IEnumerable<JobPosting> things = await _jobPostingRepository.GetJobPostingsWithKeyPhraseAsync(5000);

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
                            ListKeyPhrase.Add(new KeyPhrase
                            {
                                Affinty = item.Affinty,
                                Text = item.Text,
                                JobPosting = JobPosting
                            });
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

            return RedirectToAction("Index","Home");
        }


        public async Task<IActionResult> IndexJobPostings()
        {

            for (int i = 388; i < 2664; i++)
            {
                var item =  _jobPostingRepository.GetJobPostingByIdWithKeyPhrase(i);
                if (item != null)
                {
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