using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImperialSanWPF.Models
{
    public class BasketPosition
    {
        public int BasketPositionId { get; set; }
        public int ProductId {  get; set; }
        public int ProductQuantity { get; set; }
    }
}
