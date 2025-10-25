using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImperialSanWPF.Models
{
    public class PaginationItem
    {
        public string DisplayText { get; set; }
        public int? PageNumber {  get; set; }
        public bool IsCurrent {  get; set; }
        public bool IsEllipsis => PageNumber == null && DisplayText == "...";
    }
}
