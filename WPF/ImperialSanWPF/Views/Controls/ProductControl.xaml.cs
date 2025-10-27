using System.Windows.Controls;
using System.Windows.Input;
using ImperialSanWPF.Models;
using ImperialSanWPF.Views.Windows;

namespace ImperialSanWPF.Views.Controllers
{
    /// <summary>
    /// Логика взаимодействия для ProductControl.xaml
    /// </summary>
    public partial class ProductControl : UserControl
    { 
        public ProductControl()
        {
            InitializeComponent();
        }

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is Product product)
            {
                new ProductDetailsWindow(product).ShowDialog();
            }
        }
    }
}
