using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Логика взаимодействия для SuccessOrderPage.xaml
    /// </summary>
    public partial class SuccessOrderPage : Page
    {
        public SuccessOrderPage()
        {
            InitializeComponent();
        }

        private void toProfileRun_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //NavigationService.Navigate(new UserProfilePage());
        }

        private void continuePurchasesButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CatalogPage());
        }
    }
}
