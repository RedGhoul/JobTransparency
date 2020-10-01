using AJobBoard.Data;
using AJobBoard.Models.Dto;
using AJobBoard.Models.Entity;
using AJobBoard.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jobtransparency.Controllers.Views
{
    [Authorize(Roles = "Admin")]
    public class DataAdminActionsController : Controller
    {
        private readonly ElasticService _elasticsService;
        private readonly IJobPostingRepository _jobPostingRepository;
        private readonly IMapper _mapper;
        public DataAdminActionsController(
            IMapper mapper,
            ElasticService elasticsService,
            IJobPostingRepository jobPostingRepository)
        {
            _elasticsService = elasticsService;
            _jobPostingRepository = jobPostingRepository;
            _mapper = mapper;
        }

        public async Task<IActionResult> IndexAsync()
        {

            List<JobPosting> JPItems = await _jobPostingRepository.GetAll();
            foreach (JobPosting item in JPItems)
            {
                JobPostingDTO jobs = _mapper.Map<JobPostingDTO>(item);
                await _elasticsService.CreateJobPostingAsync(jobs);
            }
            return Ok();
        }


    }
}