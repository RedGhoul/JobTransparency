using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AJobBoard.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsJobSeeker { get; set; }
        public bool IsRecruiter { get; set; }
        public List<Document> Documents { get; set; }
        public List<JobPosting> JobPostings { get; set; }
        public List<Apply> Applies { get; set; }
    }
}
