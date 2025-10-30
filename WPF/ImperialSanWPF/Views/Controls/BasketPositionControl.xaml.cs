using ImperialSanWPF.Models;
using ImperialSanWPF.Utils;
using ImperialSanWPF.Views.Windows;
using System;
using System.Collections.Generic;
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

namespace ImperialSanWPF.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для BasketPositionControl.xaml
    /// </summary>
    public partial class BasketPositionControl : UserControl
    {
        public BasketPositionControl()
        {
            InitializeComponent();
        }

        private async void reduceBasketPosition_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is ProductBasketPosition)
            {

                ProductBasketPosition pbp = (ProductBasketPosition)DataContext;

                if (pbp.ProductQuantity == 1)
                    return;

                try
                {
                    var addToBasketModel = new
                    {
                        productId = pbp.ProductId,
                        quantity = -1,
                    };

                    using HttpResponseMessage response = await BaseHttpClient.httpClient.PostAsJsonAsync($"Basket/{SessionContext.UserId}/add_position", addToBasketModel);

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadFromJsonAsync<Basket>();
                        SessionContext.UserBasket = jsonResponse;

                        int count = 0;
                        foreach (BasketPosition bp in SessionContext.UserBasket.Positions)
                            count += bp.ProductQuantity;

                        MainWindowClass.mainWindow.BasketCount = count;

                        pbp.ProductQuantity -= 1;
                    }
                    else
                    {
                        string error = await ResponseErrorHandler.ProcessErrors(response);
                        MessageBox.Show(error, "Ошибка");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Возникла непредвиденная ошибка: {ex.Message}");
                }
            }

            else
            {
                MessageBox.Show("Ошибка: DataContext не является BasketPosition");
            }
        }

        private async void increaseBasketPosition_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is ProductBasketPosition)
            {

                ProductBasketPosition pbp = (ProductBasketPosition)DataContext;

                try
                {
                    var addToBasketModel = new
                    {
                        productId = pbp.ProductId,
                        quantity = 1,
                    };

                    using HttpResponseMessage response = await BaseHttpClient.httpClient.PostAsJsonAsync($"Basket/{SessionContext.UserId}/add_position", addToBasketModel);

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadFromJsonAsync<Basket>();
                        SessionContext.UserBasket = jsonResponse;

                        int count = 0;
                        foreach (BasketPosition bp in SessionContext.UserBasket.Positions)
                            count += bp.ProductQuantity;

                        MainWindowClass.mainWindow.BasketCount = count;

                        pbp.ProductQuantity += 1;
                    }
                    else
                    {
                        string error = await ResponseErrorHandler.ProcessErrors(response);
                        MessageBox.Show(error, "Ошибка");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Возникла непредвиденная ошибка: {ex.Message}");
                }
            }

            else
            {
                MessageBox.Show("Ошибка: DataContext не является BasketPosition");
            }
        }
    }
}
