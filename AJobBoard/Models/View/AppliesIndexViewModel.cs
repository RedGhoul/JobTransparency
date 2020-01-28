using AJobBoard.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
