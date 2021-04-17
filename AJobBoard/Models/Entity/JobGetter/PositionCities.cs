using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jobtransparency.Models.Entity.JobGetter
{
    public class PositionCities
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int JobGettingConfigId { get; set; }
        public JobGettingConfig JobGettingConfig { get; set; }
    }
}
