namespace AJobBoard.Models
{
    public class FindModel
    {
        public string KeyWords { get; set; }
        public string Location { get; set; }
        public int MaxResults { get; set; }
        public int Page { get; set; }
    }
}
