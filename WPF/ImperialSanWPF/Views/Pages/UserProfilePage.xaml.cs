using ImperialSanWPF.Models;
using ImperialSanWPF.Utils;
using ImperialSanWPF.Views.Controls;
using ImperialSanWPF.Views.Windows;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImperialSanWPF.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для UserProfilePage.xaml
    /// </summary>
    public partial class UserProfilePage : Page, INotifyPropertyChanged
    {
        private ObservableCollection<OrderForProfile> _userOrders;
        private UserData _user;

        public ObservableCollection<OrderForProfile> UserOrders
        {
            get => _userOrders;

            set
            {
                if (value != _userOrders)
                {
                    _userOrders = value;
                    OnPropertyChanged();
                }
            }
        }

        public UserData User 
        {
            get => _user;

            set
            {
                if (value != _user)
                {
                    _user = value;
                    OnPropertyChanged();
                }
            }
        }
        #region Реализация интерфейса INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public UserProfilePage()
        {
            InitializeComponent();

            GetUserDataAsync();
            GetUserOrdersAsync();
        }

        private async Task GetUserDataAsync()
        {
            try
            {
                using HttpResponseMessage response = await BaseHttpClient.httpClient.GetAsync($"User/{SessionContext.UserId}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadFromJsonAsync<UserData>();
                    User = jsonResponse;
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

        private async Task GetUserOrdersAsync()
        {
            try
            {
                using HttpResponseMessage response = await BaseHttpClient.httpClient.GetAsync($"Order/{SessionContext.UserId}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadFromJsonAsync<ObservableCollection<OrderForProfile>>();
                    UserOrders = jsonResponse;

                    foreach (var order in UserOrders)
                    {
                        foreach (var position in order.Positions)
                        {
                            position.ImageUrl = await GetProductImageUrlAsync((int)position.ProductId);
                        }
                    }
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

        private async Task<string> GetProductImageUrlAsync(int productId)
        {
            try
            {
                using var response = await BaseHttpClient.httpClient.GetAsync($"Product/{productId}");
                if (response.IsSuccessStatusCode)
                {
                    var product = await response.Content.ReadFromJsonAsync<Product>();
                    return product?.ImageUrl ?? "";
                }
                else
                {
                    string error = await ResponseErrorHandler.ProcessErrors(response);
                    MessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                    return "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Возникла непредвиденная ошибка: {ex.Message}");
                return "";
            }
        }

        private void redactUserProfileButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var window = new UserProfileRedactWindow(_user);

            window.Saved += () =>
            {
                window.Close();
                _ = GetUserDataAsync();
            };

            window.Canceled += window.Close;

            window.ShowDialog();
        }
    }
}
