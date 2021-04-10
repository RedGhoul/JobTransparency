using AJobBoard.Data;
using AJobBoard.Models.Dto;
using AJobBoard.Models.Entity;
using AJobBoard.Models.View;
using Jobtransparency.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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

            ApplicationUser currentUser = await _userRepository.getUserFromHttpContextAsync(HttpContext);

            System.Collections.Generic.List<AppliesDTO> applications = await _appliesRepository.GetUsersAppliesAsync(currentUser);

            AppliesIndexViewModel vm = new AppliesIndexViewModel();

            foreach (AppliesDTO applies in applications)
            {
                vm.Applies.Add(new AppliesIndexDTO
                {
                    Id = applies.Id,
                    JobId = applies.JobId,
                    Title = applies.Title,
                    Company = applies.Company,
                    Location = applies.Location,
                    JobSource = applies.JobSource,
                    Applicates = applies.Applicates,
                    Views = applies.Views,
                    URL = applies.URL,
                    PostDate = applies.PostDate
                });
            }

            return View(vm);
        }

        public async Task<IActionResult> Delete(int applyId)
        {
            await _appliesRepository.DeleteAppliesAsync(applyId);
            return RedirectToAction("Index");
        }
    }
}
