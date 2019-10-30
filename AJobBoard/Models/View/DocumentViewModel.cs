using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
namespace AJobBoard.Models.View
{
    public class DocumentViewModel
    {
        public DocumentViewModel()
        {
            this.DateCreated = DateTime.UtcNow;
        }
        public string DocumentName { get; set; }
        public bool IsResume { get; set; }
        public bool IsOtherDoc { get; set; }
        public DateTime DateCreated { get; set; }
        public IFormFile Resume { get; set; }
    }
}
