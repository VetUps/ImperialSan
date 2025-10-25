using ImperialSanWPF.Models;
using ImperialSanWPF.Utils;
using ImperialSanWPF.Views.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
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
using Xceed.Wpf.Toolkit.Core;

namespace ImperialSanWPF.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для CatalogPage.xaml
    /// </summary>
    public partial class CatalogPage : Page, INotifyPropertyChanged
    {
        private int _pageSize = 9;
        private int _pageNumber = 1;
        private int _maxPageNumber = 0;

        public int PageNumber
        {
            get => _pageNumber;

            set
            {
                if (_pageNumber != value)
                {
                    _pageNumber = value;
                    OnPropertyChanged();
                }
            }
        }

        public int MaxPageNumber
        {
            get => _maxPageNumber;

            set
            {
                if (_maxPageNumber != value)
                {
                    _maxPageNumber = value;
                    OnPropertyChanged();
                }
            }
        }

        private List<Product> currentProducts;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public CatalogPage()
        {
            InitializeComponent();

            DataContext = this;

            UpdateCatalog();
        }

        private async void UpdateCatalog()
        {
            HttpResponseMessage response = await BaseHttpClient.httpClient.GetAsync($"Product?pageNumber={PageNumber}&pageSize={_pageSize}");

            if (response.IsSuccessStatusCode)
            {
                PaginationProduct result = await response.Content.ReadFromJsonAsync<PaginationProduct>();
                currentProducts = result.Products;
                MaxPageNumber = (int)float.Ceiling((float)result.TotalProductsCount / (float)_pageSize);

                productsListView.Items.Clear();
                foreach (Product product in currentProducts)
                    InitializeProduct(product);
            }
        }

        private void InitializeProduct(Product product)
        {
            ProductControl productControl = new ProductControl(product);
            productsListView.Items.Add(productControl);
        }

        private async void previousPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (PageNumber == 1)
                return;

            PageNumber--;
            UpdateCatalog();
        }

        private async void nextPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (PageNumber == MaxPageNumber)
                return;

            PageNumber++;
            UpdateCatalog();
        }
    }
}
