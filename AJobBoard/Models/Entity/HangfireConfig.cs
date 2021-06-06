using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jobtransparency.Models.Entity
{
    public class HangfireConfig
    {
        public int Id { get; set; }
        public int SQLCommandTimeOut { get; set; }
        public int AffinityThreshold { get; set; }
        public int MinKeyPhraseLengthThreshold { get; set; }
    }
}
