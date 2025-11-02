using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImperialSanWPF.Models
{
    public class OrderForProfile
    {
        public int OrderId { get; set; }
        public DateOnly? DateOfCreate { get; set; }

        public string? OrderStatus { get; set; }

        public string DiliveryAddres { get; set; } = null!;

        public string PaymentMethod { get; set; } = null!;

        public float Price { get; set; }

        public string? UserComment { get; set; }
        public List<OrderPosition> Positions { get; set; } = new();
    }
}
