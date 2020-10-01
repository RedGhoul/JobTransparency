using System;

namespace AJobBoard.Models.Entity
{
    public class Apply
    {
        public Apply()
        {
            DateAddedToApplies = DateTime.UtcNow;
        }
        public int Id { get; set; }
        public JobPosting JobPosting { get; set; }
        public ApplicationUser Applier { get; set; }
        public DateTime DateAddedToApplies { get; set; }
    }
}
