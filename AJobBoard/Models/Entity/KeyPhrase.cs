namespace AJobBoard.Models.Entity
{
    public class KeyPhrase
    {
        public int Id { get; set; }
        public string Affinty { get; set; }
        public string Text { get; set; }
        public JobPosting JobPosting { get; set; }
    }
}
