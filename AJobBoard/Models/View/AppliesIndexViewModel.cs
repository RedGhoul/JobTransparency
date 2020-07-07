using AJobBoard.Models.DTO;
using System.Collections.Generic;

namespace AJobBoard.Models.View
{
    public class AppliesIndexViewModel
    {
        public AppliesIndexViewModel()
        {
            Applies = new List<AppliesIndexDTO>();
        }
        public List<AppliesIndexDTO> Applies { get; set; }
    }
}
