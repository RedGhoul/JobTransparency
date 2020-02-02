using AJobBoard.Data;
using AJobBoard.Models;
using AJobBoard.Models.Data;
using AJobBoard.Models.DTO;
using AJobBoard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AJobBoard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobPostingsAPIController : ControllerBase
    {
        private readonly IJobPostingRepository _JobPostingRepository;
        private readonly INLTKService _NLTKService;
        public JobPostingsAPIController(IJobPostingRepository JobPostingRepository, INLTKService NLTKService)
        {
            _JobPostingRepository = JobPostingRepository;
            _NLTKService = NLTKService;
        }


        // GET: api/JobPostingsAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobPosting>>> GetJobPostings()
        {
            var jobPostings = await _JobPostingRepository.GetJobPostingsAsync(60);
            return jobPostings.ToList();
        }

        [HttpPost("GetAllNoneKeywords")]
        public async Task<ActionResult<IEnumerable<JobPosting>>> GetAllNoneKeywordsJobPostings()
        {
            var jobPostings = await _JobPostingRepository.GetAllNoneKeywordsJobPostings();
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
                bool jobPostingCount = await _JobPostingRepository.JobPostingExists(tcDTO);
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
            var newPosting = await _JobPostingRepository.CreateJobPostingAsync(jobPosting);
            try
            {
                var wrapper = await _NLTKService.GetNLTKKeyPhrases(jobPosting.Description);
                if (wrapper != null)
                {
                    var ListKeyPhrase = new List<KeyPhrase>();

                    foreach (var item in wrapper.rank_list)
                    {
                        ListKeyPhrase.Add(new KeyPhrase
                        {
                            Affinty = item.Affinty,
                            Text = item.Text,
                            JobPosting = newPosting
                        });
                    }
                    newPosting.KeyPhrases = ListKeyPhrase;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //return BadRequest(ex);
            }

            try
            {
                Console.WriteLine("6");
                var NLTKSummary = await _NLTKService.GetNLTKSummary(jobPosting.Description);
                if (NLTKSummary != null)
                {
                    newPosting.Summary = NLTKSummary.SummaryText;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //return BadRequest(ex);
            }
            Console.WriteLine("7");
            await _JobPostingRepository.PutJobPostingAsync(newPosting.Id, newPosting);

            return Ok();
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
