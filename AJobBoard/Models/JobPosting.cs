using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AJobBoard.Models
{
    public class JobPosting
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string URL { get; set; }
        public string Company { get; set; }
        public string Location { get; set; }
        public string PostDate { get; set; }
        public string Salary { get; set; }
        public string Posters { get; set; }
        public string JobSource { get; set; }
        public int NumberOfApplies { get; set; }
        public int NumberOfViews { get; set; }
        public ApplicationUser Poster { get; set; }
        //need a created by
    }
}
