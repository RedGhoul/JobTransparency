﻿using System;
using System.Collections.Generic;
using Jobtransparency.Models.Entity;

namespace AJobBoard.Models.Dto
{
    public class JobPostingDTO
    {
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
        public DateTime DateAdded { get; set; }
        public List<KeyPhraseDTO> KeyPhrases { get; set; }
        public List<Tag> Tags { get; set; }
        public string Summary { get; set; }
    }
}
