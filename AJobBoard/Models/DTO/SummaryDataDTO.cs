using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AJobBoard.Models.DTO
{
    public class SummaryDataWrapperDTO
    {
        public List<SummaryDataDTO> rank_list { get; set; }
    }

    public class SummaryDataDTO
    {
        public string Affinty { get; set; }
        public string Text { get; set; }
    }
}
