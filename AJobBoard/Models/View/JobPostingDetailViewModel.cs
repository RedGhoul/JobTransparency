﻿using AJobBoard.Models.Entity;
using System;

namespace AJobBoard.Models.View
{
    public class JobPostingDetailViewModel
    {
        public JobPostingDetailViewModel()
        {
            ImageName = GenerateRandomFrontImage();
        }
        public JobPosting CurrentJobPosting { get; set; }

        public string ImageName { get; set; }
        private static string GenerateRandomFrontImage()
        {
            return "https://staticassetsforsites.s3-us-west-2.amazonaws.com/tech" + new Random().Next(1, 10) +
                   "-min.jpg";
        }
    }
}
