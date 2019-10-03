using System;

namespace AJobBoard.Models
{
    public class Document
    {
        public Document()
        {
            this.DateCreated = DateTime.UtcNow;
        }
        public int DocumentId { get; set; }
        public string URL { get; set; }
        public bool IsResume { get; set; }
        public bool IsOtherDoc { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
