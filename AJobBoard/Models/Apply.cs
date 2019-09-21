﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AJobBoard.Models
{
    public class Apply
    {
        public int Id { get; set; }
        public JobPosting JobPosting { get; set; }
        public ApplicationUser Applier { get; set; }
        public DateTime DateAddedToApplies { get; set; }
    }
}
