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

namespace AJobBoard.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobPostingsAPIController : ControllerBase
    {
        private readonly IJobPostingRepository _JobPostingRepository;
        private readonly INLTKService _nltkService;
        private readonly IKeyPharseRepository _keyPharseRepository;
        private readonly ElasticService _es;
        public JobPostingsAPIController(IJobPostingRepository JobPostingRepository,
            INLTKService NLTKService, IKeyPharseRepository KeyPharseRepository, ElasticService elasticService)
        {
            _JobPostingRepository = JobPostingRepository;
            _nltkService = NLTKService;
            _keyPharseRepository = KeyPharseRepository;
            _es = elasticService;
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
                var wrapper = await _nltkService.GetNLTKKeyPhrases(jobPosting.Description);
                if (wrapper?.rank_list != null)
                {
                    var listKeyPhrase = new List<KeyPhrase>();

                    foreach (var item in wrapper.rank_list)
                    {
                        listKeyPhrase.Add(new KeyPhrase
                        {
                            Affinty = item.Affinty,
                            Text = item.Text,
                            JobPosting = newPosting
                        });
                    }
                    newPosting.KeyPhrases = listKeyPhrase;
                }
                else
                {
                    newPosting.KeyPhrases = new List<KeyPhrase>
                    {
                        new KeyPhrase
                        {
                            Affinty = "Affinty",
                            Text = "item.Text",
                            JobPosting = newPosting
                        }
                    };
                    await _keyPharseRepository.CreateKeyPhrasesAsync(newPosting.KeyPhrases);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            try
            {
                var NLTKSummary = await _nltkService.GetNLTKSummary(jobPosting.Description);
                if (NLTKSummary != null)
                {
                    newPosting.Summary = NLTKSummary.SummaryText;
                }
                else
                {
                    newPosting.Summary = "none";
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            await _JobPostingRepository.PutJobPostingAsync(newPosting.Id, newPosting);
            await _es.CreateJobPostingAsync(newPosting);
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
