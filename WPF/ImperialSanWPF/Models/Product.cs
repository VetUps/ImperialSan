using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImperialSanWPF.Models
{
    public class Product
    {
        public int ProductId { get; set; }

        public string ProductTitle { get; set; } = null!;

        public string? ProductDescription { get; set; }

        public float Price { get; set; }

        public int QuantityInStock { get; set; }

        public string? ImageUrl { get; set; }

        public int? CategoryId { get; set; }

        public string? BrandTitle { get; set; }

        public DateOnly? DateOfCreate { get; set; }

        public bool? IsActive { get; set; }

        public string? CategoryName { get; set; }
        public int? ParentCategoryId { get; set; }
    }
}
