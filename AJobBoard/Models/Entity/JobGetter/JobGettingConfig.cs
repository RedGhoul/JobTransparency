using Jobtransparency.Models.Entity.JobGetter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jobtransparency.Models.Entity
{
    public class JobGettingConfig
    {
        public int Id { get; set; }
        public int MaxAge { get; set; }
        public string MaxNumber { get; set; }
        public string Host { get; set; }
        public string LinkCheckIfJobExists { get; set; }
        public string LinkAzureFunction { get; set; }
        public string LinkJobPostingCreation { get; set; }
        public ICollection<PositionCities> PositionCities { get; set; }
        public ICollection<PositionName> PositionName { get; set; }
    }
}
