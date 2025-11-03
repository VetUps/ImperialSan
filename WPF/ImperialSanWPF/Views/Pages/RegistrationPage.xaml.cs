using ImperialSanWPF.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImperialSanWPF.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для RegistrationPage.xaml
    /// </summary>
    public partial class RegistrationPage : Page, INotifyPropertyChanged
    {
        #region Поля
        private string _password = "";
        private bool _isPasswordVisible = false;

        private string _repeatPassword = "";
        private bool _isRepeatPasswordVisible = false;
        #endregion

        #region Свойства
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public bool IsPasswordVisible
        {
            get => _isPasswordVisible;
            set
            {
                _isPasswordVisible = value;
                OnPropertyChanged();
            }
        }

        public string RepeatPassword
        {
            get => _repeatPassword;
            set
            {
                _repeatPassword = value;
                OnPropertyChanged();
            }
        }

        public bool IsRepeatPasswordVisible
        {
            get => _isRepeatPasswordVisible;
            set
            {
                _isRepeatPasswordVisible = value;
                OnPropertyChanged();
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

        public RegistrationPage()
        {
            InitializeComponent();
            DataContext = this;
        }

        private async void registrationButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var registerModel = new
                {
                    email = emailTextBox.Text,
                    password = Password,
                    repeatpassword = RepeatPassword,
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
                    MessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            Password = PasswordBox.Password;
        }

        private void TogglePasswordVisibility_Click(object sender, RoutedEventArgs e)
        {
            if (IsPasswordVisible)
            {
                PasswordBox.Password = Password;
            }

            IsPasswordVisible = !IsPasswordVisible;
        }

        private void RepeatPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            RepeatPassword = RepeatPasswordBox.Password;
        }

        private void ToggleRepeatPasswordVisibility_Click(object sender, RoutedEventArgs e)
        {
            if (IsRepeatPasswordVisible)
            {
                PasswordBox.Password = Password;
            }

            IsRepeatPasswordVisible = !IsRepeatPasswordVisible;
        }
    }
}
