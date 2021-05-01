using AJobBoard.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jobtransparency.Models.Entity
{
    public class Tag
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public List<JobPosting> JobPostings { get; set; }
        public List<JobPostingTag> JobPostingTags { get; set; }
    }
}
