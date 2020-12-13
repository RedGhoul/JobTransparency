using AJobBoard.Data;
using AJobBoard.Models.Entity;
using Jobtransparency.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AJobBoard.Controllers.API
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AppliesAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAppliesRepository _appliesRepository;
        private readonly IJobPostingRepository _jobPostingRepository;
        private readonly IUserRepository _userRepository;
        public AppliesAPIController(IUserRepository userRepository,
            IJobPostingRepository jobPostingRepository,
            IAppliesRepository appliesRepository, ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _appliesRepository = appliesRepository;
            _jobPostingRepository = jobPostingRepository;
            _userRepository = userRepository;
        }

        // GET: api/AppliesAPI
        [HttpGet]
        public async Task<IActionResult> GetApplies()
        {
            ApplicationUser User = await _userManager.GetUserAsync(HttpContext.User);
            List<AppliesDTO> Applies = await _appliesRepository.GetUsersAppliesAsync(User);
            if (Applies != null)
            {
                return Ok(new { data = Applies });
            }
            return Ok(new { data = "" });
        }

        // GET: api/AppliesAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Apply>> GetApply(int id)
        {
            Apply apply = await _appliesRepository.GetApplyByIdAsync(id);

            if (apply == null)
            {
                return NotFound();
            }

            return apply;
        }

        // PUT: api/AppliesAPI/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutApply(int id, Apply apply)
        {
            if (id != apply.Id)
            {
                return BadRequest();
            }
            Apply result = await _appliesRepository.PutApplyAsync(id, apply);

            return result != null ? Ok(result) : (IActionResult)BadRequest();
        }

        // POST: api/AppliesAPI
        [HttpPost("addToUserApplies/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddJobPostingToCurrentUser(int id)
        {
            ApplicationUser currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (currentUser == null)
            {
                return BadRequest("Please Sign in to Add to Applies");
            }

            await _jobPostingRepository.IncrementNumberOfApplies(id);

            var result = await _userRepository.AddApplyToUser(currentUser.Id, id);

            return result == true ? Ok() : (IActionResult)BadRequest();
        }

        // DELETE: api/AppliesAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApply(int id)
        {
            return await _appliesRepository.DeleteAppliesAsync(id) == true ?
                Ok() : (IActionResult)BadRequest();
        }
    }
}
