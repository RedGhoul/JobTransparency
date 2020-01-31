using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AJobBoard.Data;
using AJobBoard.Models;
using AJobBoard.Models.Data;
using AJobBoard.Services;
using AJobBoard.Utils.ControllerHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nest;

namespace AJobBoard.Controllers.Views
{
    [AllowAnonymous]
    public class JobPostingsController : Controller
    {
        private readonly IJobPostingRepository _jobPostingRepository;
        private readonly INLTKService _NLTKService;
        private readonly IKeyPharseRepository _KeyPharseRepository;

        public JobPostingsController(
            IJobPostingRepository jobPostingRepository,
            INLTKService NLTKService,
            IKeyPharseRepository KeyPharseRepository)
        {
            _jobPostingRepository = jobPostingRepository;
            _NLTKService = NLTKService;
            _KeyPharseRepository = KeyPharseRepository;
        }

        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> IndexDoc()
        //{
        //    var allJobs = await _jobPostingRepository.GetJobPostingsAsync(1978);

        //    var settings = new ConnectionSettings(new Uri("http://ttestelk.experimentsinthedeep.com"))
        //        .DefaultIndex("jobposting");

        //    var client = new ElasticClient(settings);

        //    foreach (var jobPosting in allJobs)
        //    {
        //        var job = await client.IndexDocumentAsync(jobPosting);
        //    }

        //    return RedirectToAction(nameof(Index));
        //}


        // Need to put this in its own controller
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Ingest()
        {
            IEnumerable<JobPosting> things = await _jobPostingRepository.GetJobPostingsWithKeyPhraseAsync(1000);

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

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Index(HomeIndexViewModel homeIndexVm)
        {
            homeIndexVm = JobPostingHelper.SetDefaultFindModel(homeIndexVm);
            
            JobPostingHelper.SetupViewBag(homeIndexVm,ViewBag);


            var result = await _jobPostingRepository.ConfigureSearchAsync(homeIndexVm);
            var jobsCollection = result.Item1;
            var duration = result.Item2;

            var count = await _jobPostingRepository.GetTotalJobs();
            
            ViewBag.MaxPage = int.Parse(count)/ homeIndexVm.FindModel.Page;

            ViewBag.SecsToQuery = duration.TotalSeconds
                .ToString(CultureInfo.CurrentCulture)
                .Replace("-", "");

            ViewBag.Page = homeIndexVm.FindModel.Page;
            homeIndexVm.jobPostings = jobsCollection;
            return View(homeIndexVm);
        }


        // GET: JobPostings/Details/5
        public async Task<IActionResult> Details(int id)
        {
            JobPosting jobPosting = await _jobPostingRepository.GetJobPostingById(id);
            if (jobPosting == null)
            {
                return NotFound();
            }

            jobPosting = await _jobPostingRepository.TickNumberOfViewAsync(jobPosting);

            return View(jobPosting);
        }


        // GET: JobPostings/Create
        [Authorize(Policy = "CanCreatePosting")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: JobPostings/Create
        [HttpPost]
        [Authorize(Policy = "CanCreatePosting")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,URL,Company,Location,PostDate,Salary,Posters")] JobPosting jobPosting)
        {
            if (ModelState.IsValid)
            {
                JobPosting newPosting = await _jobPostingRepository.CreateJobPostingAsync(jobPosting);

                var wrapper = await _NLTKService.GetNLTKKeyPhrases(jobPosting.Description);

                if (newPosting.KeyPhrases == null)
                {
                    newPosting.KeyPhrases = new List<KeyPhrase>();
                }

                foreach (var item in wrapper.rank_list)
                {
                    newPosting.KeyPhrases.Add(new KeyPhrase
                    {
                        Affinty = item.Affinty,
                        Text = item.Text
                    });
                }

                var NLTKSummary = await _NLTKService.GetNLTKSummary(jobPosting.Description);

                newPosting.Summary = NLTKSummary.SummaryText;

                await _jobPostingRepository.PutJobPostingAsync(newPosting.Id, newPosting);

                return RedirectToAction(nameof(Index));
            }
            return View(jobPosting);
        }

        // GET: JobPostings/Edit/5
        [Authorize(Policy = "CanEditPosting")]
        public async Task<IActionResult> Edit(int id)
        {
            JobPosting jobPosting = await _jobPostingRepository.GetJobPostingById(id);
            if (jobPosting == null)
            {
                return NotFound();
            }
            return View(jobPosting);
        }

        // POST: JobPostings/Edit/5
        [HttpPost]
        [Authorize(Policy = "CanEditPosting")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Summary,URL,Company,Location,PostDate,Salary,Posters")] JobPosting jobPosting)
        {
            if (id != jobPosting.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _jobPostingRepository.PutJobPostingAsync(id, jobPosting);
            }
            return View(jobPosting);
        }


        // GET: JobPostings/Delete/5
        [Authorize(Policy = "CanDeletePosting")]
        public async Task<IActionResult> Delete(int id)
        {
            JobPosting jobPosting = await _jobPostingRepository.GetJobPostingById(id);
            if (jobPosting == null)
            {
                return NotFound();
            }

            return View(jobPosting);
        }

        // POST: JobPostings/Delete/5
        [Authorize(Policy = "CanDeletePosting")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _jobPostingRepository.DeleteJobPostingAsync(id);
            return RedirectToAction(nameof(Index));
        }

    }
}
