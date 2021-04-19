using AJobBoard.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jobtransparency.Models.Entity
{
    public class Sentiment
    {
        public int Id { get; set; }
        public float pos { get; set; }
        public float compound { get; set; }
        public float neu { get; set; }
        public float neg { get; set; }
        public JobPosting JobPosting { get; set; }
        public int JobPostingId { get; set; }
    }
}
