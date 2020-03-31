﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AJobBoard.Data;
using AJobBoard.Models;
using AJobBoard.Models.Data;
using AJobBoard.Models.View;
using AJobBoard.Services;
using AJobBoard.Utils.ControllerHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nest;
using Newtonsoft.Json;

namespace AJobBoard.Controllers.Views
{
    [AllowAnonymous]
    public class JobPostingsController : Controller
    {
        private readonly IJobPostingRepository _jobPostingRepository;
        private readonly INLTKService _nltkService;
        private readonly IKeyPharseRepository _keyPharseRepository;
        private readonly string _searchVmCacheKey = "SearchVMCacheKey";
        public JobPostingsController(
            IJobPostingRepository jobPostingRepository,
            INLTKService nltkService,
            IKeyPharseRepository keyPharseRepository)
        {
            _jobPostingRepository = jobPostingRepository;
            _nltkService = nltkService;
            _keyPharseRepository = keyPharseRepository;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var value = HttpContext.Session.GetString(_searchVmCacheKey);

            HomeIndexViewModel homeIndexVm = string.IsNullOrEmpty(value) ? 
                JobPostingHelper.SetDefaultFindModel(new HomeIndexViewModel()) : 
                JsonConvert.DeserializeObject<HomeIndexViewModel>(value);

            JobPostingHelper.SetupViewBag(homeIndexVm,ViewBag);

            var result = await _jobPostingRepository.ConfigureSearchAsync(homeIndexVm);
            foreach (var job in result)
            {
                job.KeyPhrases = await _jobPostingRepository.GetJobPostingKeyPhrases(job.Id);
            }
            var count = await _jobPostingRepository.GetTotalJobs();
            
            ViewBag.MaxPage = int.Parse(count)/ homeIndexVm.FindModel.Page;

            ViewBag.Page = homeIndexVm.FindModel.Page;
            homeIndexVm.JobPostings = result;
            return View(homeIndexVm);
        }

        [HttpPost]
        public IActionResult IndexPost(HomeIndexViewModel homeIndexVm)
        {
            homeIndexVm = JobPostingHelper.SetDefaultFindModel(homeIndexVm);

            JobPostingHelper.SetupViewBag(homeIndexVm, ViewBag);

            var vmData = JsonConvert.SerializeObject(homeIndexVm);
                HttpContext.Session.SetString(_searchVmCacheKey, vmData);

            return RedirectToAction("Index");
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

                var wrapper = await _nltkService.GetNLTKKeyPhrases(jobPosting.Description);

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

                var nltkSummary = await _nltkService.GetNLTKSummary(jobPosting.Description);

                newPosting.Summary = nltkSummary.SummaryText;

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
