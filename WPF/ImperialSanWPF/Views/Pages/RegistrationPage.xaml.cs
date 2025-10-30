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

                using HttpResponseMessage response = await BaseHttpClient.httpClient.PostAsJsonAsync("User/register", registerModel);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    
                    MessageBox.Show($"Вы зарегестрированы: {jsonResponse}");
                    NavigationService.Navigate(new LoginPage());
                }
                else
                {
                    List<string> errorOrder = ["Email", "Password", "RepeatPassword", "Surname", "Name", "Patronymic", "Phone"];

                    string error = await ResponseErrorHandler.ProcessErrors(response, errorOrder);
                    MessageBox.Show(error, "Ошибка");
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
