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
        #region Поля
        private ObservableCollection<OrderForProfile> _userOrders;
        private ObservableCollection<OrderForProfile> _allOrders;
        private UserData _user;
        private string _currentTab = "Мои заказы";
        #endregion

        #region Свойства
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

        public ObservableCollection<OrderForProfile> AllOrders
        {
            get => _allOrders;

            set
            {
                if (value != _allOrders)
                {
                    _allOrders = value;
                    OnPropertyChanged();
                }
            }
        }

        public string CurrentTab
        {
            get => _currentTab;

            set
            {
                if (value != _currentTab)
                {
                    _currentTab = value;
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
        #endregion

        #region Реализация интерфейса INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public UserProfilePage()
        {
            _userOrders = new ObservableCollection<OrderForProfile>();
            _allOrders = new ObservableCollection<OrderForProfile>();
            _user = new UserData();

            InitializeComponent();

            _ = GetUserDataAsync();
            _ = GetUserOrdersAsync();

            DataContext = this;
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
                    ObservableCollection<OrderForProfile> newUserOrders = jsonResponse;

                    foreach (var order in newUserOrders)
                    {
                        foreach (var position in order.Positions)
                        {
                            position.ImageUrl = await GetProductImageUrlAsync((int)position.ProductId);
                        }
                    }

                    UserOrders = newUserOrders;
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

        private async Task GetAllOrdersAsync()
        {
            try
            {
                using HttpResponseMessage response = await BaseHttpClient.httpClient.GetAsync($"Order/get_all_orders");

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadFromJsonAsync<ObservableCollection<OrderForProfile>>();
                    ObservableCollection<OrderForProfile> newUserOrders = jsonResponse;

                    foreach (var order in newUserOrders)
                    {
                        foreach (var position in order.Positions)
                        {
                            position.ImageUrl = await GetProductImageUrlAsync((int)position.ProductId);
                        }
                    }

                    AllOrders = newUserOrders;
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

        private void myOrdersTabButton_Click(object sender, RoutedEventArgs e)
        {
            string content = (sender as Button).Content.ToString();
            CurrentTab = content;

            _ = GetUserOrdersAsync();
        }

        private void allOrdersTabButton_Click(object sender, RoutedEventArgs e)
        {
            string content = (sender as Button).Content.ToString();
            CurrentTab = content;

            _ = GetAllOrdersAsync();
        }
    }
}
