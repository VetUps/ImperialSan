using ImperialSanWPF.Models;
using ImperialSanWPF.Utils;
using ImperialSanWPF.Views.Pages;
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
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        private void addToBasketButton_Click(object sender, RoutedEventArgs e)
        {
            if (SessionContext.UserId == -1)
            {
                if (MessageBox.Show("Для начала авторизуйтесь", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    MainWindowClass.mainWindow.mainFrame.NavigationService.Navigate(new LoginPage());
            }

            else
            {
                Button clickedButton = sender as Button;
                Product product = clickedButton.DataContext as Product;
                AddProductToBasket(product);
            }

            Close();
        }

        private async void AddProductToBasket(Product product)
        {
            try
            {
                var addToBasketModel = new
                {
                    productId = product.ProductId,
                    quantity = 1,
                };

                using HttpResponseMessage response = await BaseHttpClient.httpClient.PostAsJsonAsync($"Basket/{SessionContext.UserId}/add_position", addToBasketModel);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadFromJsonAsync<Basket>();
                    SessionContext.UserBasket = jsonResponse;

                    int count = 0;
                    foreach (BasketPosition bp in SessionContext.UserBasket.Positions)
                        count += bp.ProductQuantity;

                    MainWindowClass.mainWindow.BasketCount = count;

                    MessageBox.Show("Товар добавлен в корзину", "Успешно");
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
    }
}
