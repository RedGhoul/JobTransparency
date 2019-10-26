using AJobBoard.Data;
using AJobBoard.Models;
using AJobBoard.Models.View;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AJobBoard.Controllers.Views
{
    public class AppliesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AppliesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Applies
        public async Task<IActionResult> Index()
        {
            var User = await _userManager.GetUserAsync(HttpContext.User);

            var applications = await _context.Applies.Include(x => x.JobPosting)
                .Where(x => x.Applier.Id == User.Id).ToListAsync();

            List<AppliesIndexViewModel> vm = new List<AppliesIndexViewModel>();

            foreach (var applies in applications)
            {
                vm.Add(new AppliesIndexViewModel
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

            ViewBag.dataSource = vm;
            return View();
        }
    }
}
