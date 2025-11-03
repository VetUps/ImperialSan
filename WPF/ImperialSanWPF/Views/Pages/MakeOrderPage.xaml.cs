using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ImperialSanWPF.Models;
using ImperialSanWPF.Utils;

namespace ImperialSanWPF.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для MakeOrderPage.xaml
    /// </summary>
    public partial class MakeOrderPage : Page
    {
        public float TotalPirce {  get; private set; }
        public Order CurrentOrder { get; private set; } = new Order();

        private List<ProductBasketPosition> _basketPositionsForOrder;

        public List<ProductBasketPosition> BasketpositionsForOrder
        {
            get => _basketPositionsForOrder;

            set
            {
                if (value != _basketPositionsForOrder)
                    _basketPositionsForOrder = value;
            }
        }

        public MakeOrderPage(float price, List<ProductBasketPosition> positions)
        {
            InitializeComponent();

            TotalPirce = price;
            BasketpositionsForOrder = positions;
            GetDefaultUserDeliveryAddressAsync();

            DataContext = this;


            string exeDir = AppDomain.CurrentDomain.BaseDirectory;
            string mapPath = System.IO.Path.Combine(exeDir, "Resources", "Maps", "maps.html");

            if (File.Exists(mapPath))
                MapWebView.Source = new Uri(mapPath);
            else
                MessageBox.Show("Карта не найдена!");
        }

        private void backToBasketButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new UserBasket());
        }

        private void positionsCarousel_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
        }

        private void scrollUpButton_Click(object sender, RoutedEventArgs e)
        {
            double res = positionsCarousel.Height * BasketpositionsForOrder.Count;
            if (positionsCarousel.VerticalOffset == positionsCarousel.Height * (BasketpositionsForOrder.Count - 1))
            {
                positionsCarousel.ScrollToVerticalOffset(0);
                carouselIndicator.Value = 1;
            }
            else
            {

                positionsCarousel.ScrollToVerticalOffset(
                    positionsCarousel.VerticalOffset + positionsCarousel.Height
                );
                carouselIndicator.Value += 1;
            }
        }

        private void scrollDownButton_Click(object sender, RoutedEventArgs e)
        {
            if (positionsCarousel.VerticalOffset == 0)
            {
                positionsCarousel.ScrollToVerticalOffset(positionsCarousel.Height * BasketpositionsForOrder.Count);
                carouselIndicator.Value = 9;
            }
            else
            {
                positionsCarousel.ScrollToVerticalOffset(
                    positionsCarousel.VerticalOffset - positionsCarousel.Height
                );
                carouselIndicator.Value -= 1;
            }
        }

        private void MapWebView_WebMessageReceived(object sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            var message = JsonSerializer.Deserialize<WebMessage>(e.WebMessageAsJson);
            if (message?.type == "mapClick")
            {
                CurrentOrder.DiliveryAddress = message.address;
            }
            else
            {
                MessageBox.Show($"Событие другого типа: {message?.type}");
            }
        }

        private void PaymentMethoButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            string paymentMethod = button.Content.ToString();
            CurrentOrder.PaymentMethod = paymentMethod;
        }

        private async void makeOrderButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CurrentOrder.UserComment = CurrentOrder.UserComment == null ? "" : CurrentOrder.UserComment;
                using HttpResponseMessage response = await BaseHttpClient.httpClient.PostAsJsonAsync($"Order/make_order", CurrentOrder);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    MessageBox.Show(result);
                    SessionContext.UserBasket = new Basket();
                    MainWindowClass.mainWindow.BasketCount = 0;

                    NavigationService.Navigate(new SuccessOrderPage());
                }
                else
                {
                    string error = await ResponseErrorHandler.ProcessErrors(response);
                    MessageBox.Show(error, "Ошибка");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Возникла непредвиденная ошибка: {ex.Message}");
            }
        }

        private async Task GetDefaultUserDeliveryAddressAsync()
        {
            try
            {
                using HttpResponseMessage response = await BaseHttpClient.httpClient.GetAsync($"User/{SessionContext.UserId}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadFromJsonAsync<UserData>();
                    CurrentOrder.DiliveryAddress = jsonResponse.DeliveryAddress;
                }
                else
                {
                    string error = await ResponseErrorHandler.ProcessErrors(response);
                    MessageBox.Show(error, "Ошибка");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Возникла непредвиденная ошибка: {ex.Message}");
            }
        }
    }

    public class WebMessage
    {
        public string? type { get; set; }
        public string address { get; set; }
    }
}
