using AJobBoard.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jobtransparency.Models.Entity
{
    public class JobPostingTag
    {
        public int Id { get; set; }

        public JobPosting JobPosting { get; set; }
        public int JobPostingId { get; set; }

        public Tag Tag { get; set; }
        public int TagId { get; set; }
    }
}
