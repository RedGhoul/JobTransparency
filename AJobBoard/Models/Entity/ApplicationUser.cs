using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace AJobBoard.Models.Entity
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            DateCreated = DateTime.UtcNow;
            Documents = new List<Document>();
            CreatedJobPostings = new List<JobPosting>();
            Applies = new List<Apply>();
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsJobSeeker { get; set; }
        public bool IsRecruiter { get; set; }
        public ICollection<Document> Documents { get; set; }
        public ICollection<JobPosting> CreatedJobPostings { get; set; }
        public ICollection<Apply> Applies { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
