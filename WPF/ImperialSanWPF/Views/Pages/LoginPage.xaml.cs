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

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            var url = "http://localhost:5020/api/User/login";
            using (HttpClient client = new HttpClient())
            {
                using StringContent jsonContent = new(
                    JsonSerializer.Serialize(new
                    {
                        email = emailTextBox.Text,
                        password = passwordTextBox.Text,
                    }),
                    Encoding.UTF8,
                    "application/json");

                var response = client.PostAsync(url, jsonContent);
                if (response.Result.IsSuccessStatusCode)
                {
                    MessageBox.Show("Данные успешно отправлены!");
                }
                else
                {
                    MessageBox.Show("Ошибка при отправке данных: " + response.Result.StatusCode);
                }
            }
        }
    }
}
