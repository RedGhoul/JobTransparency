using AJobBoard.Data;
using AJobBoard.Models.DTO;
using AJobBoard.Models.View;
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

            Models.ApplicationUser currentUser = await _userRepository.getUserFromHttpContextAsync(HttpContext);

            System.Collections.Generic.List<Jobtransparency.Models.DTO.AppliesDTO> applications = await _appliesRepository.GetUsersAppliesAsync(currentUser);

            AppliesIndexViewModel vm = new AppliesIndexViewModel();

            foreach (Jobtransparency.Models.DTO.AppliesDTO applies in applications)
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

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            //var currentApply = await _appliesRepository.GetApplyAsync(id);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTrue(int id)
        {
            bool deletedApply = await _appliesRepository.DeleteAppliesAsync(id);

            return Redirect(nameof(Index));
        }

    }
}
