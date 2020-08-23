using Microsoft.AspNetCore.Http;
using System;
namespace AJobBoard.Models.View
{
    public class DocumentViewModel
    {
        public DocumentViewModel()
        {
            DateCreated = DateTime.UtcNow;
        }
        public string DocumentName { get; set; }
        public bool IsResume { get; set; }
        public bool IsOtherDoc { get; set; }
        public DateTime DateCreated { get; set; }
        public IFormFile Resume { get; set; }
    }
}
