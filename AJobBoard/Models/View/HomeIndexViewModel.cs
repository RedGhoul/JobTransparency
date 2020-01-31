using System.Collections.Generic;

namespace AJobBoard.Models
{
    public class HomeIndexViewModel
    {
        public IEnumerable<JobPosting> jobPostings { get; set; }
        public FindModel FindModel { get; set; }

        public string ImageName { get; set; }
        public int TimeToCache { get; set; }
    }
}
