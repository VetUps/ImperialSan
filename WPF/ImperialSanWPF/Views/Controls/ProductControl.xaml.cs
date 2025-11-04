using ImperialSanWPF.Models;
using ImperialSanWPF.Utils;
using ImperialSanWPF.Views.Windows;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace ImperialSanWPF.Views.Controllers
{
    /// <summary>
    /// Логика взаимодействия для ProductControl.xaml
    /// </summary>
    public partial class ProductControl : UserControl
    {
        public string UserRole { get; set; } = SessionContext.Role;
        public Product MainProduct { get; set; }

        public ProductControl()
        {
            InitializeComponent();
        }

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            new ProductDetailsWindow(MainProduct).ShowDialog();
        }

        private async void ProductActive_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                var deleteModel = new
                {
                    productId = MainProduct.ProductId,
                    isActive = (sender as ToggleButton).IsChecked
                };

                using HttpResponseMessage response = await BaseHttpClient.httpClient.PostAsJsonAsync("Product/change_product_status", deleteModel);

                if (response.IsSuccessStatusCode)
                {

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

            DataContext = this;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is Product product)
            {
                MainProduct = product;
            }

            DataContext = this;
        }
    }
}
