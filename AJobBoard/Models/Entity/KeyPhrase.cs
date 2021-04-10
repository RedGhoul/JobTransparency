namespace AJobBoard.Models.Entity
{
    public class KeyPhrase
    {
        public int Id { get; set; }
        public float Affinty { get; set; }
        public string Text { get; set; }
        public JobPosting JobPosting { get; set; }
        public int? JobPostingId { get; set; }
    }
}
