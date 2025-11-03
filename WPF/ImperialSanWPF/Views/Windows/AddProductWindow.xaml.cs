using ImperialSanWPF.Models;
using ImperialSanWPF.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace ImperialSanWPF.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddProductWindow.xaml
    /// </summary>
    public partial class AddProductWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<Category> _availableCategories;
        private ObservableCollection<string> _availableBrands;

        public AddProduct Data { get; private set; }
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

        public ObservableCollection<string> AvailableBrands
        {
            get => _availableBrands;

            set
            {
                if (value != _availableBrands)
                {
                    _availableBrands = value;
                    OnPropertyChanged();
                }
            }

        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public AddProductWindow()
        {
            InitializeComponent();

            Data = new AddProduct();
            LoadDataAsync();
            DataContext = this;
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var categoriesResponse = await BaseHttpClient.httpClient.GetAsync("Category/get_all_categories_with_path");
                if (categoriesResponse.IsSuccessStatusCode)
                {
                    var categories = await categoriesResponse.Content.ReadFromJsonAsync<ObservableCollection<Category>>();
                    AvailableCategories = categories ?? new ObservableCollection<Category>();
                }

                var brandsResponse = await BaseHttpClient.httpClient.GetAsync("Product/get_all_brands");
                if (brandsResponse.IsSuccessStatusCode)
                {
                    var brands = await brandsResponse.Content.ReadFromJsonAsync<ObservableCollection<string>>();
                    AvailableBrands = brands ?? new ObservableCollection<string>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки справочников: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using var response = await BaseHttpClient.httpClient.PostAsJsonAsync("Product/add_product", Data);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Товар успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    DialogResult = true;
                    Close();
                }
                else
                {
                    List<string> errorOrder = new ()
                    {
                        "ProducTitle",
                        "ProductDescription",
                        "Price",
                        "QuantityInStock",
                        "ImageUrl",
                        "CategoryId",
                        "BrandTitle",
                        "IsActive"
                    };

                    string error = await ResponseErrorHandler.ProcessErrors(response);
                    MessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
