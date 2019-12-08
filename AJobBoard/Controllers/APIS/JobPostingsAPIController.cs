using AJobBoard.Data;
using AJobBoard.Models;
using AJobBoard.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AJobBoard.Controllers
{
    [Authorize(Policy = "AuthKey")]
    [Route("api/[controller]")]
    [ApiController]
    public class JobPostingsAPIController : ControllerBase
    {
        private readonly IJobPostingRepository _JobPostingRepository;
        public JobPostingsAPIController(IJobPostingRepository JobPostingRepository)
        {
            _JobPostingRepository = JobPostingRepository;
        }


        // GET: api/JobPostingsAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobPosting>>> GetJobPostings()
        {
            var jobPostings = await _JobPostingRepository.GetJobPostingsAsync(60);
            return jobPostings.ToList();
        }


        // GET: api/JobPostingsAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<JobPosting>> GetJobPosting(int id)
        {
            JobPosting jobposting = await _JobPostingRepository.GetJobPostingById(id);
            return jobposting;
        }

        // GET: api/JobPostingsAPI/Check
        [HttpPost("Check")]
        public async Task<ActionResult<JobPosting>> CheckJobPostingAsync(TestCheckDTO tcDTO)
        {
            if(tcDTO.url != null)
            {
                bool jobPostingCount = await _JobPostingRepository.JobPostingExistsByDescription(tcDTO.description);
                return Ok(jobPostingCount);
            }else
            {
                return BadRequest(false);
            }

        }

        // PUT: api/JobPostingsAPI/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutJobPosting(int id, JobPosting jobPosting)
        {
            if (id != jobPosting.Id)
            {
                return BadRequest();
            }

            JobPosting returnedJobPosting = await _JobPostingRepository.PutJobPostingAsync(id, jobPosting);

            if(returnedJobPosting != null)
            {
                return Ok();
            }

            return BadRequest();
        }

        // POST: api/JobPostingsAPI
        [HttpPost]
        public async Task<ActionResult<JobPosting>> PostJobPosting(JobPosting jobPosting)
        {
            await _JobPostingRepository.CreateJobPostingAsync(jobPosting);
           
            return CreatedAtAction("GetJobPosting", new { id = jobPosting.Id }, jobPosting);
        }

        // DELETE: api/JobPostingsAPI/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<JobPosting>> DeleteJobPosting(int id)
        {
            JobPosting jobPosting = await _JobPostingRepository.DeleteJobPostingAsync(id);
  
            if (jobPosting == null)
            {
                return NotFound();
            }

            return jobPosting;
        }
    }
}
