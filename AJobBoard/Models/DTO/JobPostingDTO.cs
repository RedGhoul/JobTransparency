using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AJobBoard.Models.DTO
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
        [Nested]
        public List<KeyPhraseDTO> KeyPhrases { get; set; }
        public string Summary { get; set; }
    }
}
