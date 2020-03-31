using System;
using System.Collections.Generic;
using AJobBoard.Models.DTO;

namespace AJobBoard.Models.View
{
    public class HomeIndexViewModel
    {
        public HomeIndexViewModel()
        {
            
        }
        public HomeIndexViewModel(IEnumerable<JobPostingDTO> jobPostings, FindModel findModel, int timeToCache)
        {
          this.JobPostings = jobPostings;
          this.FindModel = findModel;
          this.TimeToCache = timeToCache;
          this.ImageName = GenerateRandomFrontImage();
        }
        public IEnumerable<JobPostingDTO> JobPostings { get; set; }
        public FindModel FindModel { get; set; }

        public string ImageName { get; set; }
        public int TimeToCache { get; set; }

        private static string GenerateRandomFrontImage()
        {
            return "https://staticassetsforsites.s3-us-west-2.amazonaws.com/tech" + new Random().Next(1, 10) +
                   "-min.jpg";
        }
    }
}
