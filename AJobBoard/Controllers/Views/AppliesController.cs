using AJobBoard.Data;
using AJobBoard.Models;
using AJobBoard.Models.DTO;
using AJobBoard.Models.View;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace AJobBoard.Controllers.Views
{
    [Authorize]
    public class AppliesController : Controller
    {
        private readonly IAppliesRepository _appliesRepository;

        private readonly IUserRepository _userRepository;

        public AppliesController(IAppliesRepository appliesRepository, IUserRepository userRepository)
        {
            _appliesRepository = appliesRepository;
            _userRepository = userRepository;
        }

        // GET: Applies
        public async Task<IActionResult> Index()
        {

            var currentUser = await _userRepository.getUserFromHttpContextAsync(HttpContext);

            var applications = await _appliesRepository.GetUsersAppliesAsync(currentUser);

            AppliesIndexViewModel vm = new AppliesIndexViewModel();

            foreach (var applies in applications)
            {
                vm.Applies.Add(new AppliesIndexDTO
                {
                    Id = applies.Id,
                    JobId = applies.JobPosting.Id,
                    Title = applies.JobPosting.Title,
                    Company = applies.JobPosting.Company,
                    Location = applies.JobPosting.Location,
                    JobSource = applies.JobPosting.JobSource,
                    Applicates = applies.JobPosting.NumberOfApplies,
                    Views = applies.JobPosting.NumberOfViews,
                    URL = applies.JobPosting.URL,
                    PostDate = applies.JobPosting.PostDate
                });
            }
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var currentApply = await _appliesRepository.GetApplyAsync(id);
            return View(currentApply);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTrue(int id)
        { 
            var deletedApply = await _appliesRepository.DeleteAppliesAsync(id);

            return Redirect(nameof(Index));
        }

    }
}
