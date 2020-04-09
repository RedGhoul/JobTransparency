using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AJobBoard.Models.DTO
{
    public class ESDTOs
    {
    }
    public class Shards
    {
        public string total { get; set; }
        public string successful { get; set; }
        public string skipped { get; set; }
        public string failed { get; set; }
    }
    public class Total
    {
        public int value { get; set; }
        public string relation { get; set; }
    }


    public class KeyPhras
    {
        public int Id { get; set; }
        public string Affinty { get; set; }
        public string Text { get; set; }
    }

    //public class Source
    //{
    //    public int Id { get; set; }
    //    public string Title { get; set; }
    //    public string Description { get; set; }
    //    public string URL { get; set; }
    //    public string Company { get; set; }
    //    public string Location { get; set; }
    //    public string PostDate { get; set; }
    //    public string Salary { get; set; }
    //    public object Posters { get; set; }
    //    public string JobSource { get; set; }
    //    public int NumberOfApplies { get; set; }
    //    public int NumberOfViews { get; set; }
    //    public object Applies { get; set; }
    //    public object Poster { get; set; }
    //    public DateTime DateAdded { get; set; }
    //    public List<KeyPhras> KeyPhrases { get; set; }
    //    public string Summary { get; set; }
    //}

    public class Hit
    {
        public string _index { get; set; }
        public string _type { get; set; }
        public string _id { get; set; }
        public string _score { get; set; }
        public JobPostingDTO _source { get; set; }
        public List<object> sort { get; set; }
    }



    public class Hits
    {
        public Total total { get; set; }
        public string max_score { get; set; }
        public List<Hit> hits { get; set; }
    }

    public class RootObject
    {
        public int took { get; set; }
        public bool timed_out { get; set; }
        public Shards _shards { get; set; }
        public Hits hits { get; set; }
    }
}
