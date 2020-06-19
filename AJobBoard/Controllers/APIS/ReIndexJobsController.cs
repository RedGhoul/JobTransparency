using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AJobBoard.Data;
using AJobBoard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jobtransparency.Controllers.APIS
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReIndexJobsController : Controller
    {
        private readonly IJobPostingRepository _jobPostingRepository;
        private readonly ElasticService _es;
        public ReIndexJobsController(IJobPostingRepository jobPostingRepository, ElasticService elasticService)
        {
            _jobPostingRepository = jobPostingRepository;
            _es = elasticService;
        }

        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            await _es.DeleteJobPostingIndexAsync();
            var jobs = await _jobPostingRepository.GetAllJobPostingsWithKeyPhrase();
            foreach (var item in jobs)
            {
                await _es.CreateJobPostingAsync(item);
            }
            return Ok();
        }
    }
}