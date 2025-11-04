using ImperialSanWPF.Models;
using ImperialSanWPF.Utils;
using ImperialSanWPF.Views.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
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
using System.Windows.Shapes;

namespace ImperialSanWPF.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для UserProfileRedactWindow.xaml
    /// </summary>
    public partial class UserProfileRedactWindow : Window, INotifyPropertyChanged
    {
        #region Поля
        private string _password = "";
        private bool _isPasswordVisible = false;
        #endregion

        #region Свойства
        public UpdateUserData Data { get; private set; }
        public event Action? Saved;
        public event Action? Canceled;

        public string Password
        {
            get => _password;
            set
            {
                _password = value; OnPropertyChanged();
            }
        }

        public bool IsPasswordVisible
        {
            get => _isPasswordVisible;
            set
            {
                _isPasswordVisible = value; OnPropertyChanged();
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

        public UserProfileRedactWindow(UserData userData)
        {
            InitializeComponent();

            Data = new UpdateUserData
            {
                UserId = userData.UserId,
                Surname = userData.Surname,
                Name = userData.Name,
                Patronymic = userData.Patronymic,
                Phone = userData.Phone,
                DeliveryAddress = userData.DeliveryAddress
            };

            string exeDir = AppDomain.CurrentDomain.BaseDirectory;
            string mapPath = System.IO.Path.Combine(exeDir, "Resources", "Maps", "maps.html");

            if (File.Exists(mapPath))
                MapWebView.Source = new Uri(mapPath);
            else
                MessageBox.Show("Карта не найдена!");

            DataContext = this;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Data.NewPassword = Password;

            try
            {
                var json = System.Text.Json.JsonSerializer.Serialize(Data);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                using var response = await BaseHttpClient.httpClient.PutAsync("User", content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Профиль успешно обновлён!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    Saved?.Invoke();
                }
                else
                {
                    string error = await ResponseErrorHandler.ProcessErrors(response);
                    MessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MapWebView_WebMessageReceived(object sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            var message = JsonSerializer.Deserialize<WebMessage>(e.WebMessageAsJson);
            if (message?.type == "mapClick")
            {
                Data.DeliveryAddress = message.address;
            }
            else
            {
                MessageBox.Show($"Событие другого типа: {message?.type}");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Canceled?.Invoke();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            Password = PasswordBox.Password;
        }

        private void VisiblePasswordBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            PasswordBox.Password = VisiblePasswordBox.Text;
        }

        private void TogglePasswordVisibility_Click(object sender, RoutedEventArgs e)
        {
            if (IsPasswordVisible)
            {
                PasswordBox.Password = Password;
            }

            IsPasswordVisible = !IsPasswordVisible;
        }
    }
}
