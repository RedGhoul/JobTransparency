using AJobBoard.Models.Dto;
using System;
using System.Collections.Generic;

namespace AJobBoard.Models.View
{
    public class HomeIndexViewModel
    {
        public HomeIndexViewModel()
        {

        }
        public HomeIndexViewModel(IEnumerable<JobPostingDTO> jobPostings, FindModel findModel, int timeToCache)
        {
            JobPostings = jobPostings;
            FindModel = findModel;
            TimeToCache = timeToCache;
            ImageName = GenerateRandomFrontImage();
        }
        public IEnumerable<JobPostingDTO> JobPostings { get; set; }
        public FindModel FindModel { get; set; }

        public string ImageName { get; set; }
        public int TimeToCache { get; set; }

        private static string GenerateRandomFrontImage()
        {
            return "/images/tech/" + new Random().Next(1, 10) +".jpg";
        }
    }
}
