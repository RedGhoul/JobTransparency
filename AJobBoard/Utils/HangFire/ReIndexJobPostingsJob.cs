using AJobBoard.Data;
using AJobBoard.Models.DTO;
using AJobBoard.Services;
using AutoMapper;
using Hangfire;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AJobBoard.Utils.HangFire
{
    public class ReIndexJobPostingsJob : ICustomJob
    {
        private readonly ILogger<ReIndexJobPostingsJob> _logger;
        private readonly IJobPostingRepository _jobPostingRepository;
        private readonly IKeyPharseRepository _keyPharseRepository;
        private readonly ElasticService _es;
        private readonly ApplicationDbContext _ctx;
        private readonly IMapper _mapper;
        public ReIndexJobPostingsJob(IKeyPharseRepository keyPharseRepository, IMapper mapper, ILogger<ReIndexJobPostingsJob> logger,
            IJobPostingRepository jobPostingRepository,
            ApplicationDbContext ctx,
            ElasticService elasticService)
        {
            _jobPostingRepository = jobPostingRepository;
            _keyPharseRepository = keyPharseRepository;
            _logger = logger;
            _es = elasticService;
            _ctx = ctx;
            _mapper = mapper;
        }

        public async Task Run(IJobCancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            await RunAtTimeOf(DateTime.Now);
        }

        public async Task RunAtTimeOf(DateTime now)
        {
            _logger.LogInformation("ReIndexJobPostingsJob Starts... ");
            if (await _es.DeleteJobPostingIndexAsync())
            {
                List<Models.JobPosting> jobs = await _jobPostingRepository.GetAll();
                foreach (Models.JobPosting item in jobs)
                {
                    JobPostingDTO DTO = _mapper.Map<JobPostingDTO>(item);
                    List<Models.Data.KeyPhrase> KP = _keyPharseRepository.GetKeyPhrasesAsync(item.Id);
                    List<KeyPhraseDTO> KPDTOs = _mapper.Map<List<KeyPhraseDTO>>(KP);
                    DTO.KeyPhrases = KPDTOs;
                    await _es.CreateJobPostingAsync(DTO);
                }
            }

            _logger.LogInformation("ReIndexJobPostingsJob Ends... ");
        }
    }
}
