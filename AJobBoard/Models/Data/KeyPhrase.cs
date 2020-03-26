using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AJobBoard.Models.Data
{
    public class KeyPhrase // Need to Make a DTO model of this
    {
        public int Id { get; set; }
        public string Affinty { get; set; }
        public string Text { get; set; }
        public JobPosting JobPosting { get; set; }
    }
}
