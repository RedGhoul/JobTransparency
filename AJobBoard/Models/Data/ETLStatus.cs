using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AJobBoard.Models.Data
{
    public class ETLStatus
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Finished { get; set; }
        public DateTime Started { get; set; }
        public DateTime Ended { get; set; }
    }
}
