using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ImperialSanWPF.Models;
using ImperialSanWPF.Utils;
using ImperialSanWPF.Views.Controllers;

namespace ImperialSanWPF.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для CatalogPage.xaml
    /// </summary>
    public partial class CatalogPage : Page
    {
        private int _pageSize = 9;
        private int _pageNumber = 1;
        private int _maxPageNumber = 0;
        private Product[] currentProducts;

        public CatalogPage()
        {
            InitializeComponent();
            UpdateCatalog();
        }

        private async void UpdateCatalog()
        {
            HttpResponseMessage response = await BaseHttpClient.httpClient.GetAsync($"Product?pageNumber={_pageNumber}&pageSize={_pageSize}");

            if (response.IsSuccessStatusCode)
            {
                currentProducts = await response.Content.ReadFromJsonAsync<Product[]>();

                foreach (Product product in currentProducts)
                    InitializeProduct(product);
            }
        }

        private void InitializeProduct(Product product)
        {
            ProductControl productControl = new ProductControl(product);
            productsListView.Items.Add(productControl);
        }

        private void previousPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (_pageNumber == 1)
                return;

            _pageNumber--;
        }

        private void nextPageButton_Click(object sender, RoutedEventArgs e)
        {
            if ()
        }
    }
}
