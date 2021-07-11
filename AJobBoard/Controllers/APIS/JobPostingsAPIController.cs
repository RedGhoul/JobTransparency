using AJobBoard.Data;
using AJobBoard.Models.Dto;
using AJobBoard.Models.Entity;
using AJobBoard.Services;
using AutoMapper;
using Jobtransparency.Models.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AJobBoard.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobPostingsAPIController : ControllerBase
    {
        private const int MinAffintyScore = 5;
        private readonly IJobPostingRepository _JobPostingRepository;
        private readonly INLTKService _nltkService;
        private readonly IKeyPharseRepository _keyPharseRepository;
        private readonly IMapper _mapper;
        ILogger<JobPostingsAPIController> _logger;
        private readonly ApplicationDbContext _ctx;

        public JobPostingsAPIController(ApplicationDbContext ctx, IMapper mapper, IJobPostingRepository JobPostingRepository,
            INLTKService NLTKService, IKeyPharseRepository KeyPharseRepository, ILogger<JobPostingsAPIController> logger)
        {
            _JobPostingRepository = JobPostingRepository;
            _nltkService = NLTKService;
            _keyPharseRepository = KeyPharseRepository;
            _mapper = mapper;
            _logger = logger;
            _ctx = ctx;
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
            if (string.IsNullOrEmpty(jobPosting.Description) || string.IsNullOrWhiteSpace(jobPosting.Description) || jobPosting.Description.Equals("NULL"))
            {
                return BadRequest();
            }
            JobPosting newPosting = await _JobPostingRepository.Create(jobPosting);
            var Description = new string(jobPosting.Description.Where(c => !char.IsPunctuation(c)).ToArray());
            //try
            //{
                
            //    KeyPhrasesWrapperDTO wrapper = await _nltkService.ExtractKeyPhrases(Description);
            //    if (wrapper?.rank_list != null)
            //    {
            //        List<KeyPhrase> listKeyPhrase = new List<KeyPhrase>();

            //        foreach (KeyPhraseDTO item in wrapper.rank_list)
            //        {
            //            if(item.Affinty > MinAffintyScore)
            //            {
            //                listKeyPhrase.Add(new KeyPhrase
            //                {
            //                    Affinty = item.Affinty,
            //                    Text = item.Text,
            //                    JobPosting = newPosting
            //                });
            //            }

            //        }
            //        _keyPharseRepository.CreateKeyPhrases(listKeyPhrase);
            //        newPosting.KeyPhrases = listKeyPhrase;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex);
            //}

            //try
            //{
            //    SummaryDTO NLTKSummary = await _nltkService.ExtractSummary(Description);
            //    if (NLTKSummary != null)
            //    {
            //        newPosting.Summary = NLTKSummary.SummaryText;
            //    }
            //    else
            //    {
            //        newPosting.Summary = "none";
            //    }
            //    await _JobPostingRepository.Put(newPosting.Id, newPosting);
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex);
            //}

            //try
            //{
            //    Sentiment sentiment = _mapper.Map<Sentiment>(await _nltkService.ExtractSentiment(Description));
            //    if (sentiment != null)
            //    {
            //        sentiment.JobPostingId = newPosting.Id;
            //        _ctx.Sentiment.Add(sentiment);
            //        await _ctx.SaveChangesAsync();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex);
            //}

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
