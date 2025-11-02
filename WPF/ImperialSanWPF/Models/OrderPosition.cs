using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImperialSanWPF.Models
{
    public class OrderPosition
    {
        public int OrderPositionId { get; set; }
        public int? ProductId { get; set; }
        public int? ProductQuantity { get; set; }
        public float ProductPriceInMoment { get; set; }
        public string? ImageUrl { get; set; }
    }
}
