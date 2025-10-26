using ImperialSanWPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ImperialSanWPF.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для ProductDetailsWindow.xaml
    /// </summary>
    public partial class ProductDetailsWindow : Window
    {
        private Product _mainProduct { get; set; }

        public Product MainProduct
        {
            get => _mainProduct;

            set
            {
                if (value != _mainProduct)
                    _mainProduct = value;
            }
        }

        public ProductDetailsWindow(Product product)
        {
            InitializeComponent();
            MainProduct = product;
            DataContext = MainProduct;
        }
    }
}
