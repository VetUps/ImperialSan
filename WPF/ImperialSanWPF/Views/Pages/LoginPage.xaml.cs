using ImperialSanWPF.Models;
using ImperialSanWPF.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
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

namespace ImperialSanWPF.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void OnLoginSuccess()
        {
            MainWindowClass.mainWindow.SetLoginState(true);
            NavigationService.Navigate(new CatalogPage());
        }

        private async void loginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var loginModel = new
                {
                    email = emailTextBox.Text,
                    password = passwordTextBox.Password,
                };

                using HttpResponseMessage response = await BaseHttpClient.httpClient.PostAsJsonAsync("User/login", loginModel);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    SessionContext.UserId = Convert.ToInt32(jsonResponse);
                    OnLoginSuccess();

                    LoadBasket();
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

        private async void LoadBasket()
        {
            try
            {
                using HttpResponseMessage response = await BaseHttpClient.httpClient.GetAsync($"Basket/{SessionContext.UserId}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadFromJsonAsync<Basket>();
                    SessionContext.UserBasket = jsonResponse;

                    int count = 0;
                    foreach (BasketPosition bp in SessionContext.UserBasket.Positions)
                        count += bp.ProductQuantity;

                    MainWindowClass.mainWindow.BasketCount = count;
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

        private void registartionButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegistrationPage());
        }
    }
}
