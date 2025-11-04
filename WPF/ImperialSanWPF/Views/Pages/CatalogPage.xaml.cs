using ImperialSanWPF.Models;
using ImperialSanWPF.Utils;
using ImperialSanWPF.Views.Controllers;
using ImperialSanWPF.Views.Windows;
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
        private Category _currentCategory = new Category { CategoryId = null, CategoryTitle = "Все товары"};
        private ObservableCollection<Product> _currentProducts;
        private ObservableCollection<PaginationItem> _paginationItems;
        private ObservableCollection<Category> _availableCategories;
        private ObservableCollection<Category> _categoriesHistory;

        private SortItem _currentSortItem;
        private List<SortItem> _sortItems;
        #endregion

        #region Свойства
        public string UserRole { get; set; } = SessionContext.Role;

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

        public Category CurrentCategory
        {
            get => _currentCategory;

            set
            {
                if (_currentCategory != value)
                {
                    _currentCategory = value;
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
        public ObservableCollection<Category> AvailableCategories
        {
            get => _availableCategories;

            set
            {
                if (value != _availableCategories)
                {
                    _availableCategories = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<Category> CategoriesHistory
        {
            get => _categoriesHistory;

            set
            {
                if (value != _categoriesHistory)
                {
                    _categoriesHistory = value;
                    OnPropertyChanged();
                }
            }
        }

        public List<SortItem> SortItems
        {
            get => _sortItems;

            set
            {
                if (value != _sortItems)
                    _sortItems = value;
            }
        }

        public SortItem CurrentSortItem
        {
            get => _currentSortItem;

            set
            {
                if (value != _currentSortItem)
                {
                    _currentSortItem = value;
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
            _paginationItems = new ObservableCollection<PaginationItem>();
            _availableCategories = new ObservableCollection<Category>();
            _categoriesHistory = new ObservableCollection<Category>() { new Category {
                CategoryId = CurrentCategory.CategoryId,
                CategoryTitle = CurrentCategory.CategoryTitle,
                } 
            };
            _sortItems = AvailiableSortItems.sortItems;

            InitializeComponent();
            DataContext = this;

            Loaded += CatalogPage_Loaded;
        }

        #region Методы страницы
        private async void CatalogPage_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= CatalogPage_Loaded;

            await UpdateCategories();
            await UpdateCatalog();
        }

        private async Task UpdateCatalog()
        {
            bool onlyActiveProuducts = SessionContext.Role == "Admin" ? false : true;
            string url = $"Product?pageNumber={PageNumber}&pageSize={PageSize}&categoryId={CurrentCategory.CategoryId}&sortBy={CurrentSortItem.SortText}&sortOrder={CurrentSortItem.SortOrder}&onlyActive={onlyActiveProuducts}";

            try
            {
                HttpResponseMessage response = await BaseHttpClient.httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    PaginationProduct result = await response.Content.ReadFromJsonAsync<PaginationProduct>();
                    CurrentProducts = result.Products;
                    MaxPageNumber = (int)float.Ceiling((float)result.TotalProductsCount / (float)_pageSize);
                    UpdatePaginationItems();
                }
                else
                {
                   string error = await ResponseErrorHandler.ProcessErrors(response);
                   MessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Возникла непредвиденная ошибка: {ex.Message}");
            }
        }

        private async Task UpdateCategories()
        {
            string url = $"Category?categoryId={CurrentCategory.CategoryId}";

            try
            {
                HttpResponseMessage response = await BaseHttpClient.httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    List<Category> newCategories = await response.Content.ReadFromJsonAsync<List<Category>>();

                    AvailableCategories.Clear();
                    foreach (Category categoty in newCategories)
                        AvailableCategories.Add(categoty);

                    PageNumber = 1;
                    await UpdateCatalog();
                }
                else
                {
                    string error = await ResponseErrorHandler.ProcessErrors(response);
                    MessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Возникла непредвиденная ошибка: {ex.Message}");
            }
        }

        private void UpdateCategoriesHistory()
        {
            int currentCategoryHistoryIndex = CategoriesHistory.ToList().FindIndex(c => c.CategoryId == CurrentCategory.CategoryId);

            if (currentCategoryHistoryIndex != -1)
            {
                CategoriesHistory = new ObservableCollection<Category>(CategoriesHistory.Take(currentCategoryHistoryIndex + 1).ToList());
            }
            else
            {
                CategoriesHistory.Add(new Category
                {
                    CategoryId = CurrentCategory.CategoryId,
                    CategoryTitle = CurrentCategory.CategoryTitle,
                });
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

        private async void CategoryButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            CurrentCategory.CategoryId = (int)button.CommandParameter;
            CurrentCategory.CategoryTitle = (string?)button.Content;

            await UpdateCategories();
            UpdateCategoriesHistory();
        }

        private async void CategoryHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            CurrentCategory.CategoryId = button.CommandParameter == null ? null:(int)button.CommandParameter;
            CurrentCategory.CategoryTitle= (string?)button.Content;

            await UpdateCategories();
            UpdateCategoriesHistory();
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

                await UpdateCatalog();
            }
        }
        #endregion

        private async void sortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SortItem sortItem = sortComboBox.SelectedItem as SortItem;
            CurrentSortItem = sortItem;

            UpdateCatalog();
        }

        private void addProductButton_Click(object sender, RoutedEventArgs e)
        {
            new AddProductWindow().ShowDialog();
            NavigationService.Navigate(new CatalogPage());
        }
    }
}
