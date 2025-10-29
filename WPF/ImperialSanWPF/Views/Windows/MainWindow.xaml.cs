using System.Text;
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
using ImperialSanWPF.Views.Pages;

namespace ImperialSanWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool IsLoggedIn { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            mainFrame.Navigate(new CatalogPage());
            UpdateHeaderState();

            MainWindowClass.mainWindow = this;
        }

        public void SetLoginState(bool isLoggedIn)
        {
            IsLoggedIn = isLoggedIn;
            UpdateHeaderState();
        }

        private void UpdateHeaderState()
        {
            if (IsLoggedIn)
            {
                guestStatePanel.Visibility = Visibility.Collapsed;
                userStatePanel.Visibility = Visibility.Visible;
            }
            else
            {
                guestStatePanel.Visibility = Visibility.Visible;
                userStatePanel.Visibility = Visibility.Collapsed;
            }
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(new LoginPage());
        }

        private void registrationButton_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(new RegistrationPage());
        }

        private void logoutButton_Click(object sender, RoutedEventArgs e)
        {
            SetLoginState(false);
            mainFrame.Navigate(new CatalogPage());
        }

        private void profileButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void basketButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void catalogButton_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(new CatalogPage());
        }
    }
}