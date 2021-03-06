﻿using Jobtransparency.Models.Entity;
using NpgsqlTypes;
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
            Tags = new List<Tag>();
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
        public Sentiment Sentiment { get; set; }
        public string Summary { get; set; }
        public bool Expried { get; set; } = false;
        public List<Tag> Tags { get; set; }
        public List<JobPostingTag> JobPostingTags { get; set; }
        public string CompanyLogoUrl { get; set; }
        public string Slug { get; set; }
    }
}
