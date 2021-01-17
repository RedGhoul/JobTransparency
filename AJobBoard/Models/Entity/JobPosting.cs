using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AJobBoard.Models.Entity
{
    public class JobPosting
    {
        public JobPosting()
        {
            DateAdded = DateTime.UtcNow;
        }

        public int Id { get; set; }

        [Column(TypeName = "varchar(255)")]
        public string Title { get; set; }
        [Column(TypeName = "longtext")]
        public string Description { get; set; }
        [Column(TypeName = "longtext")]
        public string URL { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string Company { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string Location { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string PostDate { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string Salary { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string Posters { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string JobSource { get; set; }
        public int NumberOfApplies { get; set; }
        public int NumberOfViews { get; set; }
        public List<Apply> Applies { get; set; }
        public ApplicationUser Poster { get; set; }
        public string? PosterId { get; set; }
        public DateTime DateAdded { get; set; }
        public List<KeyPhrase> KeyPhrases { get; set; }
        [Column(TypeName = "longtext")]
        public string Summary { get; set; }
    }
}
