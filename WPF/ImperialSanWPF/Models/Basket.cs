using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImperialSanWPF.Models
{
    class Basket
    {
        public int BasketId { get; set; }
        public List<BasketPosition> Positions { get; set; } = new List<BasketPosition>();
    }
}
