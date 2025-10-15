using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
using ImperialSanWPF.Utils;

namespace ImperialSanWPF.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для RegistrationPage.xaml
    /// </summary>
    public partial class RegistrationPage : Page
    {
        public RegistrationPage()
        {
            InitializeComponent();
        }

        private static HttpClient httpClient = new()
        {
            BaseAddress = new Uri("http://localhost:5020/api/"),
        };

        private async void registrationButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var registerModel = new
                {
                    email = emailTextBox.Text,
                    password = passwordTextBox.Password,
                    repeatpassword = repeatPasswordTextBox.Password,
                    surname = surnameTextBox.Text,
                    name = nameTextBox.Text,
                    patronymic = patronymicTextBox.Text,
                    phone = phoneTextBox.Text
                };

                using StringContent jsonContent = new(
                    JsonSerializer.Serialize(registerModel),
                    Encoding.UTF8,
                    "application/json"
                );

                using HttpResponseMessage response = await httpClient.PostAsync("User/register", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    
                    MessageBox.Show("Вы зарегестрированы");
                    NavigationService.Navigate(new LoginPage());
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var errorResponse = JsonSerializer.Deserialize<ValidationErrorResponse>(
                        errorContent,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                        );

                    if (errorResponse.Errors != null)
                    {
                        List<string> errorMessages = new List<string>();

                        foreach (var error in errorResponse.Errors)
                            errorMessages.Add(error.Value[0]);

                        MessageBox.Show(errorMessages[0]);
                    }

                    else
                    {
                        MessageBox.Show($"Неизвестная ошибка: {response.StatusCode} {errorContent}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Возникла непредвиденная ошибка: {ex.Message}");
            }
        }

        private void haveAccountButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new LoginPage());
        }
    }
}
