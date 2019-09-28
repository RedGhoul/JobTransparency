using System.Collections.Generic;

namespace AJobBoard.Models
{
    public class HomeIndexViewModel
    {
        public IEnumerable<AJobBoard.Models.JobPosting> jobPostings { get; set; }
        public FindModel FindModel { get; set; }
    }
}
