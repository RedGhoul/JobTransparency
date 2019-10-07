﻿using System.Collections.Generic;
using System.Threading.Tasks;
using AJobBoard.Models;

namespace AJobBoard.Data
{
    public interface IJobPostingRepository
    {
        Task<JobPosting> GetJobPostingById(int id);
        Task<IEnumerable<JobPosting>> GetJobPostingsAsync(int amount);
        Task<bool> JobPostingExistsByURL(string url);
        Task<JobPosting> PutJobPostingAsync(int id, JobPosting jobPosting);
        Task<JobPosting> CreateJobPostingAsync(JobPosting jobPosting);
        Task<JobPosting> DeleteJobPostingAsync(int id);
    }
}