using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
using ImperialSanWPF.Models;

namespace ImperialSanWPF.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для MakeOrderPage.xaml
    /// </summary>
    public partial class MakeOrderPage : Page
    {
        public float TotalPirce {  get; private set; }

        private List<ProductBasketPosition> _basketPositionsForOrder;

        public List<ProductBasketPosition> BasketpositionsForOrder
        {
            get => _basketPositionsForOrder;

            set
            {
                if (value != _basketPositionsForOrder)
                    _basketPositionsForOrder = value;
            }
        }

        public MakeOrderPage(float price, List<ProductBasketPosition> positions)
        {
            InitializeComponent();

            TotalPirce = price;
            BasketpositionsForOrder = positions;

            DataContext = this;
        }

        private void backToBasketButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new UserBasket());
        }

        private void positionsCarousel_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
        }

        private void scrollUpButton_Click(object sender, RoutedEventArgs e)
        {
            double res = positionsCarousel.Height * BasketpositionsForOrder.Count;
            if (positionsCarousel.VerticalOffset == positionsCarousel.Height * (BasketpositionsForOrder.Count - 1))
            {
                positionsCarousel.ScrollToVerticalOffset(0);
                carouselIndicator.Value = 1;
            }
            else
            {

                positionsCarousel.ScrollToVerticalOffset(
                    positionsCarousel.VerticalOffset + positionsCarousel.Height
                );
                carouselIndicator.Value += 1;
            }
        }

        private void scrollDownButton_Click(object sender, RoutedEventArgs e)
        {
            if (positionsCarousel.VerticalOffset == 0)
            {
                positionsCarousel.ScrollToVerticalOffset(positionsCarousel.Height * BasketpositionsForOrder.Count);
                carouselIndicator.Value = 9;
            }
            else
            {
                positionsCarousel.ScrollToVerticalOffset(
                    positionsCarousel.VerticalOffset - positionsCarousel.Height
                );
                carouselIndicator.Value -= 1;
            }
        }
    }
}
