using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImperialSanWPF.Models
{
    public class ProductForEdit
    {
        public int ProductId { get; set; }
        public string ProductTitle { get; set; } = null!;
        public string? ProductDescription { get; set; }
        public float Price { get; set; }
        public int QuantityInStock { get; set; }
        public string? ImageUrl { get; set; }
        public int? CategoryId { get; set; }
        public string? BrandTitle { get; set; }
    }
}
