using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AJobBoard.Models
{
    public class FindModel
    {
        public string KeyWords { get; set; }
        public string Location { get; set; }
        public int MaxResults { get; set; }
    }
}
