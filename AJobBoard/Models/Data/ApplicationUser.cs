using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace AJobBoard.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            DateCreated = DateTime.UtcNow;
            Documents = new List<Document>();
            JobPostings = new List<JobPosting>();
            Applies = new List<Apply>();
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsJobSeeker { get; set; }
        public bool IsRecruiter { get; set; }
        public List<Document> Documents { get; set; }
        public List<JobPosting> JobPostings { get; set; }
        public List<Apply> Applies { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
