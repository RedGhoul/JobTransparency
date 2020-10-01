using System;
using System.Collections.Generic;

namespace AJobBoard.Models.Entity
{
    public class JobPosting
    {
        public JobPosting()
        {
            DateAdded = DateTime.UtcNow;
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string URL { get; set; }
        public string Company { get; set; }
        public string Location { get; set; }
        public string PostDate { get; set; }
        public string Salary { get; set; }
        public string Posters { get; set; }
        public string JobSource { get; set; }
        public int NumberOfApplies { get; set; }
        public int NumberOfViews { get; set; }
        public List<Apply> Applies { get; set; }
        public ApplicationUser Poster { get; set; }
        public string? PosterId { get; set; }
        public DateTime DateAdded { get; set; }
        public List<KeyPhrase> KeyPhrases { get; set; }
        public string Summary { get; set; }
    }
}
