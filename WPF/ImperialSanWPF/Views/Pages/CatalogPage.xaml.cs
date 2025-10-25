using ImperialSanWPF.Models;
using ImperialSanWPF.Utils;
using ImperialSanWPF.Views.Controllers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
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
        #region Поля
        private int _pageSize = 9;
        private int _pageNumber = 1;
        private int _maxPageNumber = 0;
        private ObservableCollection<Product> _currentProducts;
        private ObservableCollection<ProductControl> _currentProductsControls;
        private ObservableCollection<PaginationItem> _paginationItems;
        #endregion

        #region Свойства
        public int PageSize
        {
            get => _pageSize;
        }

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

        public ObservableCollection<Product> CurrentProducts
        {
            get => _currentProducts;

            set
            {
                if (value != _currentProducts)
                {
                    _currentProducts = value;
                }
            }
        }

        public ObservableCollection<ProductControl> CurrentProductsControls
        {
            get => _currentProductsControls;

            set
            {
                if (value != _currentProductsControls)
                {
                    _currentProductsControls = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<PaginationItem> PaginationItems
        {
            get => _paginationItems;

            set
            {
                if (value != _paginationItems)
                {
                    _paginationItems = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Реализация интерфейса INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public CatalogPage()
        {
            _currentProducts = new ObservableCollection<Product>();
            _currentProductsControls = new ObservableCollection<ProductControl>();
            _paginationItems = new ObservableCollection<PaginationItem>();

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
                CurrentProducts = result.Products;
                MaxPageNumber = (int)float.Ceiling((float)result.TotalProductsCount / (float)_pageSize);
                UpdatePaginationItems();

                InitializeProducts(CurrentProducts);
            }
        }

        private void UpdatePaginationItems()
        {
            PaginationItems.Clear();

            PaginationItems.Add(new PaginationItem
            {
                DisplayText = "<",
                PageNumber = null
            });

            if (MaxPageNumber <= 3)
            {
                for (int i = 1; i <= MaxPageNumber; i++)
                {
                    PaginationItems.Add(new PaginationItem
                    {
                        DisplayText = i.ToString(),
                        PageNumber = i,
                        IsCurrent = i == PageNumber,
                    });
                }
            }

            else
            {
                var pagesToShow = new SortedSet<int>
                {
                    1,
                    MaxPageNumber,
                    PageNumber,
                    PageNumber - 1,
                    PageNumber + 1,
                    PageNumber - 2,
                    PageNumber + 2
                };

                var validPages = pagesToShow.Where(p => p >= 1 && p <= MaxPageNumber).OrderBy(p => p).ToList();

                int last = 0;
                foreach (var page in validPages)
                {
                    if (page - last > 1)
                        PaginationItems.Add(new PaginationItem
                        {
                            DisplayText = "...",
                            PageNumber = null
                        });

                    PaginationItems.Add(new PaginationItem
                    {
                        DisplayText = page.ToString(),
                        PageNumber = page,
                        IsCurrent = page == PageNumber
                    });
                    last = page;
                }
            }

            PaginationItems.Add(new PaginationItem
            {
                DisplayText = ">",
                PageNumber = null
            });
        }

        private void InitializeProducts(ObservableCollection<Product> products)
        {
            CurrentProductsControls.Clear();
            foreach (Product product in CurrentProducts)
            {
                ProductControl productControl = new ProductControl(product);
                CurrentProductsControls.Add(productControl);
            }
        }

        private async void PageButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                if (button.Content.ToString() == "<")
                {
                    if (PageNumber > 1) PageNumber--;
                }
                else if (button.Content.ToString() == ">")
                {
                    if (PageNumber < MaxPageNumber) PageNumber++;
                }
                else if (button.Tag is int pageNum)
                {
                    PageNumber = pageNum;
                }
                else
                {
                    return;
                }

                UpdateCatalog();
            }
        }

        public class CurrentPageBackgroundConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return (bool)value ? Brushes.LightBlue : Brushes.Transparent;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
                => throw new NotImplementedException();
        }

        public class CurrentPageFontWeightConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return (bool)value ? FontWeights.Bold : FontWeights.Normal;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
                => throw new NotImplementedException();
        }
    }
}
