namespace AJobBoard.Models.Dto
{
    public class AppliesIndexDTO
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public string Title { get; set; }
        public string Company { get; set; }
        public string Location { get; set; }
        public string JobSource { get; set; }
        public int Applicates { get; set; }
        public int Views { get; set; }
        public string URL { get; set; }
        public string PostDate { get; set; }
    }
}
