using AJobBoard.Data;
using AJobBoard.Models.Dto;
using AJobBoard.Models.Entity;
using AJobBoard.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly IMapper _mapper;
        private readonly ILogger<JobPostingsAPIController> _logger;

        public JobPostingsAPIController(IMapper mapper, IJobPostingRepository JobPostingRepository,
            INLTKService NLTKService, IKeyPharseRepository KeyPharseRepository, ElasticService elasticService)
        {
            _JobPostingRepository = JobPostingRepository;
            _nltkService = NLTKService;
            _keyPharseRepository = KeyPharseRepository;
            _es = elasticService;
            _mapper = mapper;
        }


        [HttpPost("GetAllNoneKeywords")]
        public async Task<ActionResult<IEnumerable<JobPosting>>> GetAllNoneKeywordsJobPostings()
        {
            IEnumerable<JobPosting> jobPostings = await _JobPostingRepository.GetAllNoneKeywords();
            return jobPostings.ToList();
        }


        // GET: api/JobPostingsAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<JobPosting>> GetJobPosting(int id)
        {
            JobPosting jobposting = await _JobPostingRepository.GetById(id);
            return jobposting;
        }

        // GET: api/JobPostingsAPI/Check
        [HttpPost("Check")]
        public async Task<ActionResult<JobPosting>> CheckJobPostingAsync(TestCheckDTO tcDTO)
        {
            if (tcDTO.url != null)
            {
                bool jobPostingCount = await _JobPostingRepository.Exists(tcDTO);
                return Ok(jobPostingCount);
            }
            else
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

            JobPosting returnedJobPosting = await _JobPostingRepository.Put(id, jobPosting);

            if (returnedJobPosting != null)
            {
                return Ok();
            }

            return BadRequest();
        }

        // POST: api/JobPostingsAPI
        [HttpPost]
        public async Task<ActionResult<JobPosting>> PostJobPosting(JobPosting jobPosting)
        {
            if (string.IsNullOrEmpty(jobPosting.Description))
            {
                return BadRequest();
            }
            JobPosting newPosting = await _JobPostingRepository.Create(jobPosting);
            try
            {
                KeyPhrasesWrapperDTO wrapper = await _nltkService.GetNLTKKeyPhrases(jobPosting.Description);
                if (wrapper?.rank_list != null)
                {
                    List<KeyPhrase> listKeyPhrase = new List<KeyPhrase>();

                    foreach (KeyPhraseDTO item in wrapper.rank_list)
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

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            try
            {
                SummaryDTO NLTKSummary = await _nltkService.GetNLTKSummary(jobPosting.Description);
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
            await _JobPostingRepository.Put(newPosting.Id, newPosting);
            await _es.CreateJobPostingAsync(_mapper.Map<JobPostingDTO>(newPosting));
            return Ok();
        }

        // DELETE: api/JobPostingsAPI/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<JobPosting>> DeleteJobPosting(int id)
        {
            JobPosting jobPosting = await _JobPostingRepository.DeleteById(id);

            if (jobPosting == null)
            {
                return NotFound();
            }

            return jobPosting;
        }
    }
}
