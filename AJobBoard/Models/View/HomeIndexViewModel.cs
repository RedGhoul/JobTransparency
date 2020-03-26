using System.Collections.Generic;
using AJobBoard.Models.DTO;

namespace AJobBoard.Models
{
    public class HomeIndexViewModel
    {
        public IEnumerable<JobPostingDTO> jobPostings { get; set; }
        public FindModel FindModel { get; set; }

        public string ImageName { get; set; }
        public int TimeToCache { get; set; }
    }
}
