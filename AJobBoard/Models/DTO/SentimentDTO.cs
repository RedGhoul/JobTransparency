using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jobtransparency.Models.DTO
{
    public class SentimentDTO
    {
        public float pos { get; set; }
        public float compound { get; set; }
        public float neu { get; set; }
        public float neg { get; set; }
    }
}
