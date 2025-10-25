using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImperialSanWPF.Models
{
    internal class PaginationProduct
    {
        public ObservableCollection<Product> Products { get; set; }
        public int TotalProductsCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
