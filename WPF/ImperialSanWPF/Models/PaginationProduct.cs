using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImperialSanWPF.Models
{
    internal class PaginationProduct
    {
        public List<Product> Products { get; set; }
        public int TotalProductsCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
