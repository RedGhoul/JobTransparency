using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AJobBoard.Models
{
    public class HomeIndexViewModel
    {
        public IEnumerable<AJobBoard.Models.JobPosting> jobPostings { get; set; }
        public FindModel FindModel { get; set; }
    }
}
